using System.ComponentModel.DataAnnotations;

namespace FieldStaff.Application.Features.TaskAssignmentHistories.DTOs
{
    public class ReassignServiceTaskRequestDto
    {
        [Range(1, int.MaxValue)]
        public int NewAssignedStaffId { get; set; }

        [StringLength(1000)]
        public string? Remarks { get; set; }
    }
}
