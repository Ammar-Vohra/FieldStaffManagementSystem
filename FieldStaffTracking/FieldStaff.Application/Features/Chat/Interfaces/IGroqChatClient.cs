using FieldStaff.Application.Features.Chat.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.Chat.Interfaces
{
    public interface IGroqChatClient
    {
        Task<GroqChatCompletionResponseDto> GetCompletionAsync(
            List<GroqChatMessageDto> messages, object[] tools); 
    }
}
