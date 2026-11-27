using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FieldStaff.Application.Features.Chat.DTOs;

namespace FieldStaff.Application.Features.Chat.DTOs
{
    public class ChatRequestDto
    {
        [Required]
        [MaxLength(2000)]
        public string Message { get; set; } = string.Empty;

        public List<ChatMessageDto> History { get; set; } = new();
    }
}
