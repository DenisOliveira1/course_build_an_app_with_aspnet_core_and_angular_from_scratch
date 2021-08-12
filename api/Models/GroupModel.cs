using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class GroupModel
    {
        [Key]
        public string Name { get; set; }
        public ICollection<ConnectionModel> Connections { get; set;}  = new List<ConnectionModel>();

        public GroupModel()
        {
            
        }
        public GroupModel(string name)
        {
            Name = name;
        }
    }
}