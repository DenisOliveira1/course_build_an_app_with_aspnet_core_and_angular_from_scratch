using System.Collections.Generic;

namespace api.DTOs
{
    public class RolesDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string KnownAs { get; set; }
        public List<string> Roles { get; set; }
    }
}