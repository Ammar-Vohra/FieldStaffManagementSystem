
using FieldStaff.Application.Features.TaskPriorities.DTOs;
using FieldStaff.Application.Features.TaskPriorities.Interfaces;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Services
{
    public class TaskPriorityService : ITaskPriorityService
    {
        private readonly ITaskPriorityRepository _repository;

        public TaskPriorityService(ITaskPriorityRepository repository)
        {
            _repository = repository;
        }

        public async Task<TaskPriorityResponseDto> CreateTaskPriorityAsync(
            CreateTaskPriorityRequestDto request,
            int? addedByUserId)
        {
            string name = request.TaskPriorityName.Trim();
            string code = request.TaskPriorityCode.Trim().ToUpperInvariant();

           
            if (await _repository.ExistsByNameAsync(name))
            {
                throw new InvalidOperationException(
                    "Task priority name already exists.");
            }

            
            if (await _repository.ExistsByCodeAsync(code))
            {
                throw new InvalidOperationException(
                    "Task priority code already exists.");
            }

            var taskPriority = new TaskPriority
            {
                TaskPriorityName = name,
                TaskPriorityCode = code,
                IsActive = request.IsActive,
                IsDeleted = false,
                CreatedBy = addedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(taskPriority);

            return MapToResponseDto(taskPriority);
        }


        public async Task<TaskPriorityResponseDto?> GetTaskPriorityByIdAsync(
            int taskPriorityId)
        {
            var taskPriority = await _repository.GetByIdAsync(taskPriorityId);

            if (taskPriority == null)
                return null;

            return MapToResponseDto(taskPriority);
        }

        public async Task<List<TaskPriorityResponseDto>>
            GetAllTaskPrioritiesAsync()
        {
            var priorities = await _repository.GetAllAsync();

            return priorities
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<TaskPriorityResponseDto?> UpdateTaskPriorityAsync(
            int taskPriorityId,
            UpdateTaskPriorityRequestDto request,
            int? updatedByUserId)
        {
            var taskPriority = await _repository.GetByIdAsync(taskPriorityId);

            if (taskPriority == null)
                return null;

            string name = request.TaskPriorityName.Trim();
            string code = request.TaskPriorityCode.Trim().ToUpperInvariant();

         
            if (await _repository.ExistsByNameAsync(name, taskPriorityId))
            {
                throw new InvalidOperationException(
                    "Another task priority with this name already exists.");
            }

        
            if (await _repository.ExistsByCodeAsync(code, taskPriorityId))
            {
                throw new InvalidOperationException(
                    "Another task priority with this code already exists.");
            }

            taskPriority.TaskPriorityName = name;
            taskPriority.TaskPriorityCode = code;
            taskPriority.IsActive = request.IsActive;
            taskPriority.UpdatedBy = updatedByUserId;
            taskPriority.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(taskPriority);

            return MapToResponseDto(taskPriority);
        }

        public async Task<bool> DeleteTaskPriorityAsync(
            int taskPriorityId,
            int? deletedByUserId)
        {
            var taskPriority = await _repository.GetByIdAsync(taskPriorityId);

            if (taskPriority == null)
                return false;

            taskPriority.IsDeleted = true;
            taskPriority.IsActive = false;
            taskPriority.UpdatedBy = deletedByUserId;
            taskPriority.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(taskPriority);

            return true;
        }


        private static TaskPriorityResponseDto MapToResponseDto(
            TaskPriority taskPriority)
        {
            return new TaskPriorityResponseDto
            {
                TaskPriorityId = taskPriority.TaskPriorityId,
                TaskPriorityName = taskPriority.TaskPriorityName,
                TaskPriorityCode = taskPriority.TaskPriorityCode,
                IsActive = taskPriority.IsActive,
                CreatedAt = taskPriority.CreatedAt,
                CreatedBy = taskPriority.CreatedBy,
                UpdatedAt = taskPriority.UpdatedAt,
                UpdatedBy = taskPriority.UpdatedBy
            };
        }
    }
}
