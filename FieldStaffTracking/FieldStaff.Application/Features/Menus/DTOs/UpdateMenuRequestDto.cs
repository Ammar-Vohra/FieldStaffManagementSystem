using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.Menus.DTOs
{
    public class UpdateMenuRequestDto
    {
        public int? ParentMenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string? MenuPath { get; set; }
        public string? Icon { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}

