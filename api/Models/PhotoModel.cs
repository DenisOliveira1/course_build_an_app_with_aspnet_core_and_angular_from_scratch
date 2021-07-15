using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    // Já que essa tabela não tem um contexto é necessario essa anotação para definir o nome da tabela no banco de dados
    [Table("Photos")]
    public class PhotoModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public int MyProperty { get; set; }
        public string PublicId { get; set; }


        // User, 1-N
        public UserModel User { get; set; }
        public int UserId { get; set; }
    }
}