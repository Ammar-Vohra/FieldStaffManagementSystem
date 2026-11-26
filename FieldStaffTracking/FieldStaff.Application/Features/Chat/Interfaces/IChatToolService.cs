using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.Chat.Interfaces
{
    public interface IChatToolService
    {
        Task<object?> GetDashboardSummaryAsync(int roleId, int userId);
        Task<object?> GetOverdueTasksAsync(int roleId, int userId);
    }
}
