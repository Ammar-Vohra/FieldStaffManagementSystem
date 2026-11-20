using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Application.Features.TaskStatusHistories.Interfaces;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.TaskStatusHistories
{
    public class TaskStatusHistoryService : ITaskStatusHistoryService
    {
        private readonly ITaskStatusHistoryRepository _repository;

        public TaskStatusHistoryService(
            ITaskStatusHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TaskStatusHistoryResponseDto>>
            GetByServiceTaskIdAsync(int serviceTaskId)
        {
            if (serviceTaskId <= 0)
                throw new InvalidOperationException("Invalid service task ID.");

            var histories =
                await _repository.GetByServiceTaskIdAsync(serviceTaskId);

            return histories.Select(h => new TaskStatusHistoryResponseDto
            {
                TaskStatusHistoryId = h.TaskStatusHistoryId,

                OldStatusId = h.OldStatusId,
                OldStatusName = h.OldStatus?.TaskStatusName,

                NewStatusId = h.NewStatusId,
                NewStatusName = h.NewStatus.TaskStatusName,

                UserId = h.UserId,
                UserName = h.User.UserName,

                Remarks = h.Remarks,
                CreatedAt = h.CreatedAt
            }).ToList();
        }

        public async Task AddAsync(TaskStatusHistory history)
        {
            await _repository.AddAsync(history);
        }
    }
}