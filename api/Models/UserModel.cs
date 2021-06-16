
using System;
using System.Collections.Generic;
using api.Extentions;

namespace api.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; } 
        public ICollection<PhotoModel> Photos { get; set; }    
 
        // O nome deve ser nome do "Ger" + parâmetro para o automapper poder preencher automaticamente o valor da variável após o mapping
        // Para trazer o Dto direto do repository de maneira eficiente essa função não deve estar aqui, mas sim no AutoMapperProfiles
        // public int GetAge(){
        //     return DateOfBirth.CalculateAge();
        // }
    }
}