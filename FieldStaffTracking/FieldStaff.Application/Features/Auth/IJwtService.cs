using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.Auth
{
    public interface IJwtService
    {
        string GenerateToken(int userId, int roleId, string userName);
    }
}
