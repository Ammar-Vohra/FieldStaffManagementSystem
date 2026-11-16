using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Domain.Entities
{
    public class RoleMenu : BaseEntity
    {
        public int RoleMenuId { get; set; }

        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public bool IsActive { get; set; } = true; 
        public bool IsDeleted { get; set; } = false;


        public Role Role { get; set; } = null!;
        public Menu Menu { get; set; } = null!;
    }
}
