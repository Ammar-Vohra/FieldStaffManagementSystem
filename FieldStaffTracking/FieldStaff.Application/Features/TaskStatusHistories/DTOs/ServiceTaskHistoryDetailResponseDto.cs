using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Application.Features.TaskAssignmentHistories.DTOs;

namespace FieldStaff.Application.Features.TaskStatusHistories.DTOs
{
    public class ServiceTaskHistoryDetailResponseDto : ServiceTaskResponseDto
    {
        public List<TaskStatusHistoryResponseDto> StatusHistory { get; set; }
    = new List<TaskStatusHistoryResponseDto>();

        public List<TaskAssignmentHistoryResponseDto> AssignmentHistory { get; set; }
            = new List<TaskAssignmentHistoryResponseDto>();
    }
}
