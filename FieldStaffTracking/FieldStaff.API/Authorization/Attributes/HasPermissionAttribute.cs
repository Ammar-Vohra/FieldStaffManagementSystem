using Microsoft.AspNetCore.Authorization;

namespace FieldStaff.API.Authorization.Attributes
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permissionCode)
        {
            Policy = $"Permission:{permissionCode}";
        }
    }
}