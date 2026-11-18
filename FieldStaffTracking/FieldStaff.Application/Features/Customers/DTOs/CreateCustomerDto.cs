using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace FieldStaff.Application.Features.Customers.DTOs
{
    public class CreateCustomerDto
    {
        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string ServiceAddress { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
