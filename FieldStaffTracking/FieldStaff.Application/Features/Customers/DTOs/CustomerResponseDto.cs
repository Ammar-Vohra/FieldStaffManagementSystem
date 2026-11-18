using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.Customers.DTOs
{
    public class CustomerResponseDto
    {
      
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        
        public string Email { get; set; } = string.Empty;

        
        public string ServiceAddress { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }


    }
}
