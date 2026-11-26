using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.Chat.DTOs
{
    public class GroqChatCompletionResponseDto
    {
        public string? Content { get; set; }
        public string? FinishReason { get; set; }
        public List<GroqToolCallDto> ToolCalls { get; set; } = new();
    }
}
