using FieldStaff.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FieldStaff.Application.Features.ServiceTasks.DTOs
{
    public class CreateServiceTaskRequestDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public int AssignedStaffId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int TaskStatusId { get; set; }
        [Required]
        public int TaskPriorityId { get; set; }
        [Required]
        public DateTime ScheduledDate { get; set; }
        //public string? CompletionNote { get; set; }
        //public int IsActive { get; set; }


    }
}

