using System.Collections.Generic;

namespace api.DTOs
{
    public class GroupDto
    {
        public string Name { get; set; }
        public ICollection<ConnectionDto> Connections { get; set;}
    }
}