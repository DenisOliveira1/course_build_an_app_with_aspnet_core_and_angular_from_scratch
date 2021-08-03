using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class RoleModel : IdentityRole<int>
    {
        // N-N
        // A e B (Role e User)
        // Z (UserRole)
        public ICollection<UserRoleModel> UserRoles { get; set; }
    }
}