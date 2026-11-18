using System.ComponentModel.DataAnnotations;


namespace FieldStaff.Application.Features.TaskStatusHistories.DTOs
{
    public class UpdateTaskStatusHistoryRequestDto
    {
        [Required]
        public int NewStatusId { get; set; }

        public string? Remarks { get; set; }

        public string? CompletionNote { get; set; }
    }
}
