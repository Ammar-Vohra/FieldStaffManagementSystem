using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Application.Features.TaskAssignmentHistories.Interfaces;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.TaskAssignmentHistories
{
    public class TaskAssignmentHistoryService
        : ITaskAssignmentHistoryService
    {
        private readonly ITaskAssignmentHistoryRepository _repository;

        public TaskAssignmentHistoryService(
            ITaskAssignmentHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TaskAssignmentHistoryResponseDto>>
            GetByServiceTaskIdAsync(int serviceTaskId)
        {
            if (serviceTaskId <= 0)
                throw new InvalidOperationException("Invalid service task ID.");

            var histories =
                await _repository.GetByServiceTaskIdAsync(serviceTaskId);

            return histories.Select(h => new TaskAssignmentHistoryResponseDto
            {
                TaskAssignmentHistoryId = h.TaskAssignmentHistoryId,

                OldAssignedStaffId = h.OldAssignedStaffId,
                OldAssignedStaffName = h.OldAssignedStaff?.UserName,

                NewAssignedStaffId = h.NewAssignedStaffId,
                NewAssignedStaffName = h.NewAssignedStaff.UserName,

                Remarks = h.Remarks,
                CreatedAt = h.CreatedAt
            }).ToList();
        }

        public async Task AddAsync(TaskAssignmentHistory history)
        {
            await _repository.AddAsync(history);
        }
    }
}