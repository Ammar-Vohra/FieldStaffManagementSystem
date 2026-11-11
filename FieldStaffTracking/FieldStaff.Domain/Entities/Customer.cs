using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ServiceAddress { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;
        public ICollection<ServiceTask> ServiceTasks { get; set; } = new List<ServiceTask>();

    
    }
}
