using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FieldStaff.Application.Features.Chat.DTOs;

namespace FieldStaff.Application.Features.Chat.Interfaces
{
    public interface IChatService
    {
        Task<ChatResponseDto> SendMessageAsync(ChatRequestDto response, int roleId, int userId);
    }
}
