using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.Customers.DTOs
{
    public class CustomerQueryParameters
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }

        public string SortBy { get; set; } = "customerName";

        public string SortDirection { get; set; } = "asc";

        public bool? IsActive { get; set; }
    }
}
