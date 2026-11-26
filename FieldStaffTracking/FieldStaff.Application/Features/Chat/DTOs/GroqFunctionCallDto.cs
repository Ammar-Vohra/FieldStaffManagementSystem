using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.Chat.DTOs
{
    public class GroqFunctionCallDto
    {
        public string Name { get; set; } = string.Empty;
        public string Arguments { get; set; } = "{}";
    }
}
