using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Application.Features.Common.DTOs;
using FieldStaff.Application.Features.TaskStatusHistories.DTOs;
using FieldStaff.Application.Features.TaskAssignmentHistories.DTOs;


namespace FieldStaff.Application.Features.ServiceTasks.Interfaces
{
    public interface IServiceTaskService
    {
        Task<ServiceTaskResponseDto> CreateServiceTaskAsync(
            CreateServiceTaskRequestDto request,
            int createdByUserId);

        Task<ServiceTaskResponseDto?> GetServiceTaskByIdAsync(int serviceTaskId);
        Task<ServiceTaskHistoryDetailResponseDto?> GetServiceTaskHistoryDetailsAsync(int serviceTaskId);
        Task<PaginatedResponse<ServiceTaskResponseDto>> GetMyTasksAsync(
            int staffId,
            ServiceTaskQueryParameters parameters);

        Task<ServiceTaskHistoryDetailResponseDto?> GetMyTaskDetailsAsync(
            int serviceTaskId,
            int staffId);

        Task<ServiceTaskHistoryDetailResponseDto?> UpdateMyTaskStatusAsync(
            int serviceTaskId,
            UpdateTaskStatusHistoryRequestDto request,
            int staffId);


        Task<PaginatedResponse<ServiceTaskResponseDto>> GetPagedServiceTasksAsync(
            ServiceTaskQueryParameters parameters);

        Task<int> ProcessOverdueTasksAsync();

        Task<ServiceTaskResponseDto?> UpdateServiceTaskAsync(
            int serviceTaskId,
            UpdateServiceTaskRequestDto request,
            int? updatedByUserId);

        Task<ServiceTaskResponseDto?> ReassignServiceTaskAsync(
            int serviceTaskId,
            ReassignServiceTaskRequestDto request,
            int reassignedByUserId);

        Task<bool> DeleteServiceTaskAsync(
            int serviceTaskId,
            int? deletedByUserId);
    }
}
