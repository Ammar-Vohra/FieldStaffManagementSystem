using System.ComponentModel.DataAnnotations; 

namespace FieldStaff.Application.Features.TaskAssignmentHistories.DTOs
{
    public class ReassignServiceTaskRequestDto
    {
        [Required]
        public int NewAssignedStaffId { get; set; }

        public string? Remarks { get; set; }
    }
}
