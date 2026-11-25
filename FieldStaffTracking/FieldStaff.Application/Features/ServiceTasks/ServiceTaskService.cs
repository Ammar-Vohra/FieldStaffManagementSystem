
using FieldStaff.Application.Features.Common.DTOs;
using FieldStaff.Application.Features.Customers.Interfaces;
using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Application.Features.ServiceTasks.Interfaces;
using FieldStaff.Application.Features.TaskPriorities.Interfaces;
using FieldStaff.Application.Features.TaskStatuses.Interfaces;
using FieldStaff.Application.Features.Users.Interfaces;
using FieldStaff.Application.Features.TaskStatusHistories.DTOs;
using FieldStaff.Application.Features.TaskStatusHistories.Interfaces;
using FieldStaff.Application.Features.TaskAssignmentHistories.Interfaces;
using FieldStaff.Application.Features.TaskAssignmentHistories.DTOs;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.ServiceTasks
{
    public class ServiceTaskService : IServiceTaskService
    {
        private readonly IServiceTaskRepository _serviceTaskRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITaskStatusRepository _taskStatusRepository;
        private readonly ITaskPriorityRepository _taskPriorityRepository;
        private readonly ITaskStatusHistoryService _taskStatusHistoryService;
        private readonly ITaskAssignmentHistoryService _taskAssignmentHistoryService;

        public ServiceTaskService(
            IServiceTaskRepository serviceTaskRepository,
            ICustomerRepository customerRepository,
            IUserRepository userRepository,
            ITaskStatusRepository taskStatusRepository,
            ITaskPriorityRepository taskPriorityRepository,
            ITaskStatusHistoryService taskStatusHistoryService,
            ITaskAssignmentHistoryService taskAssignmentHistoryService)
        {
            _serviceTaskRepository = serviceTaskRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _taskStatusRepository = taskStatusRepository;
            _taskPriorityRepository = taskPriorityRepository;
            _taskStatusHistoryService = taskStatusHistoryService;
            _taskAssignmentHistoryService = taskAssignmentHistoryService;
        }


        public async Task<ServiceTaskResponseDto> CreateServiceTaskAsync(
            CreateServiceTaskRequestDto request,
            int createdByUserId)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);

            if (customer == null || !customer.IsActive || customer.IsDeleted)
                throw new InvalidOperationException("Customer not found or inactive.");

            var assignedStaff = await _userRepository.GetByIdAsync(request.AssignedStaffId);

            if (assignedStaff == null || !assignedStaff.IsActive || assignedStaff.IsDeleted)
                throw new InvalidOperationException("Assigned staff member not found or inactive.");

            if (assignedStaff.Role.RoleName != "FieldStaff")
                throw new InvalidOperationException("Selected user is not a Field Staff member.");

            var priority = await _taskPriorityRepository.GetByIdAsync(request.TaskPriorityId);

            if (priority == null || !priority.IsActive || priority.IsDeleted)
                throw new InvalidOperationException("Invalid or inactive task priority.");

            var pendingStatus = await _taskStatusRepository.GetByCodeAsync("PENDING");

            if (pendingStatus == null)
                throw new InvalidOperationException("Pending task status is not configured.");

            DateTime now = DateTime.UtcNow;

            
            var serviceTask = new ServiceTask
            {
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                CustomerId = request.CustomerId,
                AssignedStaffId = request.AssignedStaffId,
                TaskPriorityId = request.TaskPriorityId,
                TaskStatusId = pendingStatus.TaskStatusId,
                ScheduledDate = request.ScheduledDate,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = createdByUserId,
                CreatedAt = now
            };

            
            var statusHistory = new TaskStatusHistory
            {
                ServiceTask = serviceTask,
                OldStatusId = null,
                NewStatusId = pendingStatus.TaskStatusId,
                UserId = createdByUserId,
                Remarks = "Task created with Pending status.",
                CreatedBy = createdByUserId,
                CreatedAt = now
            };

          
            var assignmentHistory = new TaskAssignmentHistory
            {
                ServiceTask = serviceTask,
                OldAssignedStaffId = null,
                NewAssignedStaffId = assignedStaff.UserId,
                Remarks = "Initial assignment during task creation.",
                CreatedBy = createdByUserId,
                CreatedAt = now
            };

            
            serviceTask.TaskStatusHistories.Add(statusHistory);
            serviceTask.TaskAssignmentHistories.Add(assignmentHistory);

            
            await _serviceTaskRepository.CreateAsync(serviceTask);
            await _serviceTaskRepository.SaveChangesAsync();

            return MapToResponseDto(
                serviceTask,
                customer,
                assignedStaff,
                pendingStatus,
                priority);
        }


        public async Task<ServiceTaskResponseDto?> GetServiceTaskByIdAsync(int serviceTaskId)
        {
            var serviceTask = await _serviceTaskRepository.GetByIdAsync(serviceTaskId);

            if (serviceTask == null)
                return null;

            return MapToResponseDto(serviceTask);
        }

        public async Task<ServiceTaskHistoryDetailResponseDto?> GetServiceTaskHistoryDetailsAsync(int serviceTaskId)
        {
            var task = await _serviceTaskRepository.GetByIdAsync(serviceTaskId);
            if (task == null)
                return null;

            var taskDto = MapToResponseDto(task);
            return new ServiceTaskHistoryDetailResponseDto
            {
                ServiceTaskId = taskDto.ServiceTaskId,
                Title = taskDto.Title,
                Description = taskDto.Description,
                CustomerId = taskDto.CustomerId,
                CustomerName = taskDto.CustomerName,
                AssignedStaffId = taskDto.AssignedStaffId,
                AssignedStaffName = taskDto.AssignedStaffName,
                TaskStatusId = taskDto.TaskStatusId,
                TaskStatusName = taskDto.TaskStatusName,
                TaskPriorityId = taskDto.TaskPriorityId,
                TaskPriorityName = taskDto.TaskPriorityName,
                ScheduledDate = taskDto.ScheduledDate,
                CompletionNote = taskDto.CompletionNote,
                IsActive = taskDto.IsActive,
                CreatedAt = taskDto.CreatedAt,
                StatusHistory = await _taskStatusHistoryService.GetByServiceTaskIdAsync(serviceTaskId),
                AssignmentHistory = await _taskAssignmentHistoryService.GetByServiceTaskIdAsync(serviceTaskId)
            };
        }

        public async Task<PaginatedResponse<ServiceTaskResponseDto>> GetPagedServiceTasksAsync(
            ServiceTaskQueryParameters parameters)
        {
            if (parameters.PageNumber < 1)
                throw new InvalidOperationException("Page number must be greater than zero.");

            if (parameters.PageSize < 1 || parameters.PageSize > 100)
                throw new InvalidOperationException("Page size must be between 1 and 100.");

            if (string.IsNullOrWhiteSpace(parameters.SortBy))
                parameters.SortBy = "createdat";

            if (string.IsNullOrWhiteSpace(parameters.SortDirection))
                parameters.SortDirection = "desc";

            if (!parameters.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) &&
                !parameters.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid sort direction.");
            }

            var (tasks, totalRecords) =
                await _serviceTaskRepository.GetPagedAsync(parameters);

            var data = tasks.Select(MapToResponseDto).ToList();

            return new PaginatedResponse<ServiceTaskResponseDto>
            {
                Data = data,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / parameters.PageSize),
                HasPreviousPage = parameters.PageNumber > 1,
                HasNextPage = parameters.PageNumber <
                    (int)Math.Ceiling((double)totalRecords / parameters.PageSize)
            };
        }

        public async Task<PaginatedResponse<ServiceTaskResponseDto>> GetMyTasksAsync(
            int staffId,
            ServiceTaskQueryParameters parameters)
        {
            if (staffId <= 0)
                throw new InvalidOperationException("Invalid staff ID.");

           
            parameters.AssignedStaffId = staffId;

            return await GetPagedServiceTasksAsync(parameters);
        }


        public async Task<ServiceTaskHistoryDetailResponseDto?> GetMyTaskDetailsAsync(
    int serviceTaskId,
    int staffId)
        {
            if (staffId <= 0)
                throw new InvalidOperationException("Invalid staff ID.");

            var serviceTask = await _serviceTaskRepository.GetByIdAsync(serviceTaskId);

            if (serviceTask == null || serviceTask.AssignedStaffId != staffId)
                return null;

            var statusHistory =
                await _taskStatusHistoryService.GetByServiceTaskIdAsync(serviceTaskId);

            var assignmentHistory =
                await _taskAssignmentHistoryService.GetByServiceTaskIdAsync(serviceTaskId);

            var response = new ServiceTaskHistoryDetailResponseDto();

            var taskDto = MapToResponseDto(serviceTask);

            // Copy the base ServiceTaskResponseDto properties.
            response.ServiceTaskId = taskDto.ServiceTaskId;
            response.Title = taskDto.Title;
            response.Description = taskDto.Description;
            response.CustomerId = taskDto.CustomerId;
            response.CustomerName = taskDto.CustomerName;
            response.AssignedStaffId = taskDto.AssignedStaffId;
            response.AssignedStaffName = taskDto.AssignedStaffName;
            response.TaskStatusId = taskDto.TaskStatusId;
            response.TaskStatusName = taskDto.TaskStatusName;
            response.TaskPriorityId = taskDto.TaskPriorityId;
            response.TaskPriorityName = taskDto.TaskPriorityName;
            response.ScheduledDate = taskDto.ScheduledDate;
            response.CompletionNote = taskDto.CompletionNote;
            response.IsActive = taskDto.IsActive;
            response.CreatedAt = taskDto.CreatedAt;

            response.StatusHistory = statusHistory;
            response.AssignmentHistory = assignmentHistory;

            return response;
        }


        public async Task<ServiceTaskHistoryDetailResponseDto?> UpdateMyTaskStatusAsync(
    int serviceTaskId,
    UpdateTaskStatusHistoryRequestDto request,
    int staffId)
        {
            if (staffId <= 0)
                throw new InvalidOperationException("Invalid staff ID.");

            var serviceTask = await _serviceTaskRepository.GetByIdAsync(serviceTaskId);

            if (serviceTask == null || serviceTask.AssignedStaffId != staffId)
                return null;

            if (!serviceTask.IsActive || serviceTask.IsDeleted)
                throw new InvalidOperationException("This task is inactive.");

            // Get the current status.
            var oldStatus = await _taskStatusRepository.GetByIdAsync(serviceTask.TaskStatusId);

            if (oldStatus == null || !oldStatus.IsActive || oldStatus.IsDeleted)
                throw new InvalidOperationException("Current task status is invalid.");

            // Get the requested new status.
            var newStatus = await _taskStatusRepository.GetByIdAsync(request.NewStatusId);

            if (newStatus == null || !newStatus.IsActive || newStatus.IsDeleted)
                throw new InvalidOperationException("Selected task status is invalid or inactive.");

            string currentStatusCode = oldStatus.TaskStatusCode.ToUpperInvariant();
            string newStatusCode = newStatus.TaskStatusCode.ToUpperInvariant();

            // Enforce allowed status transitions.
            bool isValidTransition =
                (currentStatusCode == "PENDING" && newStatusCode == "IN_PROGRESS") ||
                (currentStatusCode == "OVERDUE" && newStatusCode == "IN_PROGRESS") ||
                (currentStatusCode == "IN_PROGRESS" && newStatusCode == "COMPLETED");

            if (!isValidTransition)
                throw new InvalidOperationException(
                    $"Invalid status transition: {oldStatus.TaskStatusName} to {newStatus.TaskStatusName}.");

            // Completion note is mandatory when completing a task.
            if (newStatusCode == "COMPLETED" &&
                string.IsNullOrWhiteSpace(request.CompletionNote))
            {
                throw new InvalidOperationException(
                    "Completion note is required when completing a task.");
            }

            var now = DateTime.UtcNow;

            serviceTask.TaskStatusId = newStatus.TaskStatusId;

            if (newStatusCode == "COMPLETED")
                serviceTask.CompletionNote = request.CompletionNote!.Trim();

            serviceTask.UpdatedBy = staffId;
            serviceTask.UpdatedAt = now;

            // Record status change history.
            var history = new TaskStatusHistory
            {
                ServiceTaskId = serviceTask.ServiceTaskId,
                OldStatusId = oldStatus.TaskStatusId,
                NewStatusId = newStatus.TaskStatusId,
                UserId = staffId,
                Remarks = request.Remarks?.Trim(),
                CreatedBy = staffId,
                CreatedAt = now
            };

            await _taskStatusHistoryService.AddAsync(history);

            await _serviceTaskRepository.UpdateAsync(serviceTask);
            await _serviceTaskRepository.SaveChangesAsync();

            return await GetMyTaskDetailsAsync(serviceTaskId, staffId);
        }


        public async Task<int> ProcessOverdueTasksAsync()
        {
            // Get Overdue status
            var overdueStatus = await _taskStatusRepository
                .GetByCodeAsync("OVERDUE");

            if (overdueStatus == null)
            {
                throw new InvalidOperationException(
                    "Overdue task status is not configured or inactive.");
            }

            // Get an active Admin user for audit tracking
            var adminUser = await _userRepository.GetActiveAdminAsync();

            if (adminUser == null)
            {
                throw new InvalidOperationException(
                    "No active Admin user was found.");
            }

            var now = DateTime.UtcNow;

            // Get Pending tasks whose scheduled date has passed
            var overdueTasks = await _serviceTaskRepository
                .GetPendingTasksPastScheduledDateAsync(now);

            if (!overdueTasks.Any())
                return 0;

            foreach (var task in overdueTasks)
            {
                var oldStatusId = task.TaskStatusId;

                // Update task status
                task.TaskStatusId = overdueStatus.TaskStatusId;
                task.UpdatedBy = adminUser.UserId;
                task.UpdatedAt = now;

                // Create status history
                var history = new TaskStatusHistory
                {
                    ServiceTaskId = task.ServiceTaskId,
                    OldStatusId = oldStatusId,
                    NewStatusId = overdueStatus.TaskStatusId,
                    UserId = adminUser.UserId,
                    Remarks = "Automatically marked as Overdue because the task was not started by its scheduled date.",
                    CreatedBy = adminUser.UserId,
                    CreatedAt = now
                };

                await _taskStatusHistoryService.AddAsync(history);
            }

            // Save all changes
            await _serviceTaskRepository.SaveChangesAsync();

            return overdueTasks.Count;
        }

        public async Task<ServiceTaskResponseDto?> UpdateServiceTaskAsync(
            int serviceTaskId,
            UpdateServiceTaskRequestDto request,
            int? updatedByUserId)
        {
            var serviceTask = await _serviceTaskRepository.GetByIdAsync(serviceTaskId);

            if (serviceTask == null)
                return null;

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);

            if (customer == null || !customer.IsActive || customer.IsDeleted)
                throw new InvalidOperationException("Customer not found or inactive.");

            var priority = await _taskPriorityRepository.GetByIdAsync(request.TaskPriorityId);

            if (priority == null || !priority.IsActive || priority.IsDeleted)
                throw new InvalidOperationException("Invalid or inactive task priority.");

            serviceTask.Title = request.Title.Trim();
            serviceTask.Description = request.Description?.Trim();
            serviceTask.CustomerId = request.CustomerId;
            serviceTask.TaskPriorityId = request.TaskPriorityId;
            serviceTask.ScheduledDate = request.ScheduledDate;
            serviceTask.UpdatedBy = updatedByUserId;
            serviceTask.UpdatedAt = DateTime.UtcNow;

            await _serviceTaskRepository.UpdateAsync(serviceTask);
            await _serviceTaskRepository.SaveChangesAsync();

            return MapToResponseDto(serviceTask);
        }

        public async Task<ServiceTaskResponseDto?> ReassignServiceTaskAsync(
            int serviceTaskId,
            ReassignServiceTaskRequestDto request,
            int reassignedByUserId)
        {
            if (request.NewAssignedStaffId <= 0)
                throw new InvalidOperationException("Select a valid field staff member.");

            var serviceTask = await _serviceTaskRepository.GetByIdAsync(serviceTaskId);
            if (serviceTask == null)
                return null;

            if (!serviceTask.IsActive)
                throw new InvalidOperationException("Inactive tasks cannot be reassigned.");

            var newStaff = await _userRepository.GetByIdAsync(request.NewAssignedStaffId);
            if (newStaff == null || !newStaff.IsActive || newStaff.IsDeleted)
                throw new InvalidOperationException("Selected field staff member was not found or is inactive.");

            if (newStaff.Role == null || !newStaff.Role.RoleName.Equals("FieldStaff", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Selected user is not a Field Staff member.");

            if (serviceTask.AssignedStaffId == newStaff.UserId)
                throw new InvalidOperationException("This task is already assigned to that staff member.");

            var now = DateTime.UtcNow;
            var history = new TaskAssignmentHistory
            {
                ServiceTaskId = serviceTask.ServiceTaskId,
                OldAssignedStaffId = serviceTask.AssignedStaffId,
                NewAssignedStaffId = newStaff.UserId,
                Remarks = string.IsNullOrWhiteSpace(request.Remarks) ? null : request.Remarks.Trim(),
                CreatedBy = reassignedByUserId,
                CreatedAt = now
            };

            serviceTask.AssignedStaffId = newStaff.UserId;
            serviceTask.UpdatedBy = reassignedByUserId;
            serviceTask.UpdatedAt = now;

            await _taskAssignmentHistoryService.AddAsync(history);
            await _serviceTaskRepository.UpdateAsync(serviceTask);
            await _serviceTaskRepository.SaveChangesAsync();

            return MapToResponseDto(serviceTask);
        }

        public async Task<bool> DeleteServiceTaskAsync(
            int serviceTaskId,
            int? deletedByUserId)
        {
            var serviceTask = await _serviceTaskRepository.GetByIdAsync(serviceTaskId);

            if (serviceTask == null)
                return false;

            serviceTask.IsDeleted = true;
            serviceTask.IsActive = false;
            serviceTask.UpdatedBy = deletedByUserId;
            serviceTask.UpdatedAt = DateTime.UtcNow;

            await _serviceTaskRepository.UpdateAsync(serviceTask);
            await _serviceTaskRepository.SaveChangesAsync();

            return true;
        }

        private static ServiceTaskResponseDto MapToResponseDto(ServiceTask serviceTask)
        {
            return new ServiceTaskResponseDto
            {
                ServiceTaskId = serviceTask.ServiceTaskId,
                Title = serviceTask.Title,
                Description = serviceTask.Description,

                CustomerId = serviceTask.CustomerId,
                CustomerName = serviceTask.Customer.CustomerName,

                AssignedStaffId = serviceTask.AssignedStaffId,
                AssignedStaffName = serviceTask.AssignedStaff.UserName,

                TaskStatusId = serviceTask.TaskStatusId,
                TaskStatusName = serviceTask.TaskStatus.TaskStatusName,

                TaskPriorityId = serviceTask.TaskPriorityId,
                TaskPriorityName = serviceTask.TaskPriority.TaskPriorityName,

                ScheduledDate = serviceTask.ScheduledDate,
                CompletionNote = serviceTask.CompletionNote,
                IsActive = serviceTask.IsActive,
                CreatedAt = serviceTask.CreatedAt
            };
        }

        private static ServiceTaskResponseDto MapToResponseDto(
            ServiceTask serviceTask,
            Customer customer,
            User assignedStaff,
            StatusTask status,
            TaskPriority priority)
        {
            return new ServiceTaskResponseDto
            {
                ServiceTaskId = serviceTask.ServiceTaskId,
                Title = serviceTask.Title,
                Description = serviceTask.Description,

                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,

                AssignedStaffId = assignedStaff.UserId,
                AssignedStaffName = assignedStaff.UserName,

                TaskStatusId = status.TaskStatusId,
                TaskStatusName = status.TaskStatusName,

                TaskPriorityId = priority.TaskPriorityId,
                TaskPriorityName = priority.TaskPriorityName,

                ScheduledDate = serviceTask.ScheduledDate,
                CompletionNote = serviceTask.CompletionNote,
                IsActive = serviceTask.IsActive,
                CreatedAt = serviceTask.CreatedAt
            };
        }
    }
}
