using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.Customers.DTOs
{
    public class UpdateCustomerDto
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
