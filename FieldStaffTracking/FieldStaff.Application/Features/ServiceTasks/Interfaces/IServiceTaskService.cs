using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Application.Features.Common.DTOs;
using FieldStaff.Application.Features.TaskStatusHistories.DTOs;


namespace FieldStaff.Application.Features.ServiceTasks.Interfaces
{
    public interface IServiceTaskService
    {
        Task<ServiceTaskResponseDto> CreateServiceTaskAsync(
            CreateServiceTaskRequestDto request,
            int createdByUserId);

        Task<ServiceTaskResponseDto?> GetServiceTaskByIdAsync(int serviceTaskId);
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

        Task<ServiceTaskResponseDto?> UpdateServiceTaskAsync(
            int serviceTaskId,
            UpdateServiceTaskRequestDto request,
            int? updatedByUserId);

        Task<bool> DeleteServiceTaskAsync(
            int serviceTaskId,
            int? deletedByUserId);
    }
}
