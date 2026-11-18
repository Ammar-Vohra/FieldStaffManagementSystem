
using FieldStaff.Application.Features.Common.DTOs;
using FieldStaff.Application.Features.Customers.Interfaces;
using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Application.Features.ServiceTasks.Interfaces;
using FieldStaff.Application.Features.TaskPriorities.Interfaces;
using FieldStaff.Application.Features.TaskStatuses.Interfaces;
using FieldStaff.Application.Features.Users.Interfaces;
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

        public ServiceTaskService(
            IServiceTaskRepository serviceTaskRepository,
            ICustomerRepository customerRepository,
            IUserRepository userRepository,
            ITaskStatusRepository taskStatusRepository,
            ITaskPriorityRepository taskPriorityRepository)
        {
            _serviceTaskRepository = serviceTaskRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _taskStatusRepository = taskStatusRepository;
            _taskPriorityRepository = taskPriorityRepository;
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
