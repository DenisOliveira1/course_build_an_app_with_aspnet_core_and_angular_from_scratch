
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

        // https://docs.microsoft.com/pt-br/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key#other-relationship-patterns

        // Photos, N-1
        // A, entidade com N, tem um array de B
        // B, entidade com 1, tem uma instancia e um id de A
        public ICollection<PhotoModel> Photos { get; set; }    

        // User, N-N
        // A e B, entidades com N, tem um array de Z
        // Z e uma nova classe gerada apartir dessa relação e tem uma instancia e um id de A e uma instancia e um id de B
        // Quando a PK é composta precisa configurar no OnModelCreating no DataContext
        public ICollection<UserLikeModel> LikedByUsers { get; set; }   

        // User, N-N
        public ICollection<UserLikeModel> LikedUsers { get; set; }   

 
        // O nome deve ser "Get" + nome doparâmetro para o automapper poder preencher automaticamente o valor da variável após o mapping
        // Para trazer o Dto direto do repository de maneira eficiente essa função não deve estar aqui, mas sim no AutoMapperProfiles
        // public int GetAge(){
        //     return DateOfBirth.CalculateAge();
        // }
    }
}