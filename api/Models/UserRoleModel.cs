using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class UserRoleModel : IdentityUserRole<int>
    {
        // public int UserId { get; set; } 
        // Não precisa do UserId e do RoleId porque já estão implicitos em IdentityUserRole
        public UserModel User { get; set; }
        public RoleModel Role { get; set; }
    }
}