using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.Chat.DTOs
{
    public class GroqToolCallDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "function";
        public GroqFunctionCallDto Function { get; set; } = new();
    }
}
