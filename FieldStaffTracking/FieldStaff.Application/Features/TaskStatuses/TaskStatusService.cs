
using FieldStaff.Application.Features.TaskStatuses.DTOs;
using FieldStaff.Application.Features.TaskStatuses.Interfaces;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Services
{
    public class TaskStatusService : ITaskStatusService
    {
        private readonly ITaskStatusRepository _repository;

        public TaskStatusService(ITaskStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<TaskStatusResponseDto> CreateTaskStatusAsync(CreateTaskStatusRequestDto request, int? addedByUserId)
        {
            string name = request.TaskStatusName.Trim();
            string code = request.TaskStatusCode.Trim().ToUpperInvariant();


            if (await _repository.ExistsByNameAsync(name))
            {
                throw new InvalidOperationException(
                    "Task status name already exists.");
            }


            if (await _repository.ExistsByCodeAsync(code))
            {
                throw new InvalidOperationException(
                    "Task status code already exists.");
            }

            var taskStatus = new StatusTask
            {
                TaskStatusName = name,
                TaskStatusCode = code,
                IsActive = request.IsActive,
                IsDeleted = false,
                CreatedBy = addedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(taskStatus);

            return MapToResponseDto(taskStatus);
        }


        public async Task<TaskStatusResponseDto?> GetTaskStatusByIdAsync(int taskStatusId)
        {
            var taskStatus = await _repository.GetByIdAsync(taskStatusId);

            if (taskStatus == null)
                return null;

            return MapToResponseDto(taskStatus);
        }

        public async Task<List<TaskStatusResponseDto>> GetAllTaskStatusesAsync()
        {
            var priorities = await _repository.GetAllAsync();

            return priorities
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<TaskStatusResponseDto?> UpdateTaskStatusAsync(int taskStatusId, UpdateTaskStatusRequestDto request,
            int? updatedByUserId)
        {
            var taskStatus = await _repository.GetByIdAsync(taskStatusId);

            if (taskStatus == null)
                return null;

            string name = request.TaskStatusName.Trim();
            string code = request.TaskStatusCode.Trim().ToUpperInvariant();


            if (await _repository.ExistsByNameAsync(name, taskStatusId))
            {
                throw new InvalidOperationException(
                    "Another task status with this name already exists.");
            }


            if (await _repository.ExistsByCodeAsync(code, taskStatusId))
            {
                throw new InvalidOperationException(
                    "Another task status with this code already exists.");
            }

            taskStatus.TaskStatusName = name;
            taskStatus.TaskStatusCode = code;
            taskStatus.IsActive = request.IsActive;
            taskStatus.UpdatedBy = updatedByUserId;
            taskStatus.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(taskStatus);

            return MapToResponseDto(taskStatus);
        }

        public async Task<bool> DeleteTaskStatusAsync(int taskStatusId, int? deletedByUserId)
        {
            var taskStatus = await _repository.GetByIdAsync(taskStatusId);

            if (taskStatus == null)
                return false;

            taskStatus.IsDeleted = true;
            taskStatus.IsActive = false;
            taskStatus.UpdatedBy = deletedByUserId;
            taskStatus.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(taskStatus);

            return true;
        }


        private static TaskStatusResponseDto MapToResponseDto(
            StatusTask taskStatus)
        {
            return new TaskStatusResponseDto
            {
                TaskStatusId = taskStatus.TaskStatusId,
                TaskStatusName = taskStatus.TaskStatusName,
                TaskStatusCode = taskStatus.TaskStatusCode,
                IsActive = taskStatus.IsActive,
                CreatedAt = taskStatus.CreatedAt,
                CreatedBy = taskStatus.CreatedBy,
                UpdatedAt = taskStatus.UpdatedAt,
                UpdatedBy = taskStatus.UpdatedBy
            };
        }
    }
}
