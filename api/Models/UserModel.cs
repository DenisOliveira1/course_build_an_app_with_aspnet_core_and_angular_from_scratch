
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class UserModel : IdentityUser<int>
    {
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

        // 1-N
        // A, entidade com 1, tem um array de B (User)
        // B, entidade com N, tem uma instancia e um id de A (Photos)
        public ICollection<PhotoModel> Photos { get; set; }    

        // N-N (A 1:N Z N:1 B) 
        // A e B, entidades com N, tem um array de Z (Nesse caso A e B são a mesma classe, logo ela recebe 2 arrays) (User e User)
        // Z e uma nova classe gerada apartir dessa relação e tem uma instancia e um id de A e uma instancia e um id de B (UserLike)
        // Quando a PK é composta precisa configurar no OnModelCreating no DataContext
        public ICollection<UserLikeModel> LikedByUsers { get; set; }   
        public ICollection<UserLikeModel> LikedUsers { get; set; }   

        // N-N
        // A e B (User e User)
        // Z (Message)
        public ICollection<MessageModel> MessagesSent { get; set; }
        public ICollection<MessageModel> MessagesReceived { get; set; }

        // N-N
        // A e B (User e Role)
        // Z (UserRole)
        public ICollection<UserRoleModel> UserRoles { get; set; }


 
        // O nome deve ser "Get" + nome doparâmetro para o automapper poder preencher automaticamente o valor da variável após o mapping
        // Para trazer o Dto direto do repository de maneira eficiente essa função não deve estar aqui, mas sim no AutoMapperProfiles
        // public int GetAge(){
        //     return DateOfBirth.CalculateAge();
        // }
    }
}