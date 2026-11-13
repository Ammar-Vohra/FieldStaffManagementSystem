using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Domain.Entities
{
    public class MenuPermission : BaseEntity
    {
        public int MenuPermissionId { get; set; }
        public int MenuId { get; set; }
        public int PermissionId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public Menu Menu { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}
