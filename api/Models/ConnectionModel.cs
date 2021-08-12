using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class ConnectionModel
    {
        // Caso o nome seja Id o EF vai interpretar o campo sendo a PK
        // Caso o nome seja nomeDaClassId o EF vai interpretar o campo sendo a PK
        // Caso o nome seja diferente de ambos precisa da anotação [Key]

        [Key]
        public string ConnectionId { get; set; }
        public string Username { get; set; }
        public string groupName { get; set; }
        public GroupModel group { get; set; }

        public ConnectionModel()
        {
            
        }
        public ConnectionModel(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

    }
}