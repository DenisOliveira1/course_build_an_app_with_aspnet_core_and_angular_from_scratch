using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Context
{
    public class DataContext : IdentityDbContext<UserModel, RoleModel, int,
                                                IdentityUserClaim<int>, UserRoleModel,IdentityUserLogin<int>, 
                                                IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        // Aqui se define o nome das tabelas que serão criadas no banco de dados
        public DbSet<UserLikeModel> Likes { get; set; }
        public DbSet<MessageModel> Messages { get; set; }
        // Não será necessario ir ao banco para consultar fotos individualmente, logo não há porque ter um contexto Photos
        // public DbSet<PhotoModel> Photos { get; set; }

        public DataContext(DbContextOptions options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder){
            base.OnModelCreating(builder);

    // 1. User N-N User, <UserLikeModel>

            // 1.1. Define que essa tabela tem uma PK composta por essas duas colunas
            builder.Entity<UserLikeModel>()
                .HasKey(k => new {k.SourceUserId, k.LikedUserId});

            // Uma relação (A) N:N (B) pode ser expressada por:
            // (A) 1:N (Z) N:1 (B) em conjunto, ambos tem o mesmo significado
            // Onde Z é uma nova tabela com as FKs de A e B
            // Ja que a tabela N:N foi criada manualmente é necessário estabelecer manualmente as relações 1:N e N:1

            // Em 1:1 a FK fica em qualquer uma das tabelas
            // Em 1:N a FK fica em N 
            // Em N:N as FKs formam uma nova tabela

            // 1.2. Define que... um gostador tem muitas pessoas de que gosta
            builder.Entity<UserLikeModel>()
                .HasOne(l => l.SourceUser) // 1 UserLikeModel.SourceUser tem ...
                .WithMany(u => u.LikedUsers) // N SourceUser.LikedUsers... (array)
                .HasForeignKey(l => l.SourceUserId) // onde UserLikeModel.SourceUserId é a FK (Z)
                .OnDelete(DeleteBehavior.Cascade); // Se deletar o SourceUser deleta os UserLikeModel 

            // 1.3. Define que ... um gostado tem muitas pessoas que gostam dele
            builder.Entity<UserLikeModel>()
                .HasOne(l => l.LikedUser) // 1 UserLikeModel.LikedUser tem ...
                .WithMany(u => u.LikedByUsers) // N LikedUser.LikedByUsers... (array)
                .HasForeignKey(l => l.LikedUserId) // onde UserLikeModel.LikedUserId é a FK (Z)
                .OnDelete(DeleteBehavior.Cascade); // Se deletar o LikedUser deleta os UserLikeModel 

    // 2. User N-N User, <MessageModel>

            // 2.1. Define que ... um recebedor tem muitas mensagens recebidas
            builder.Entity<MessageModel>()
                .HasOne(m => m.Recipient) // 1 MessageModel.Recipient tem ...
                .WithMany(u=> u.MessagesReceived) // N Recipient.MessagesReceived... (array)
                .HasForeignKey(m => m.RecipientId) // onde MessageModel.RecipientId é a FK (Z)
                .OnDelete(DeleteBehavior.Restrict); // Se deletar o Recipient não deleta os MessageModel 

            // 2.2. Define que ... um enviador tem muitas mensagens enviadas
            builder.Entity<MessageModel>()
                .HasOne(m => m.Sender) // 1 MessageModel.Sender tem ...
                .WithMany(u => u.MessagesSent) // N Sender.MessagesSent...
                .HasForeignKey(m => m.SenderId) // onde MessageModel.SenderId é a FK (Z)
                .OnDelete(DeleteBehavior.Restrict); // Se deletar o Sender não deleta os MessageModel 

    // 3. User N-N Role, <UserRoleModel>
            // Foi implementado aqui HasMany + WithOne, porém poderia ser implementado como nos outrs também, usando HasOne + WithMany

            // 3.1. Define que ... um user tem muitas roles
            builder.Entity<UserModel>()
                .HasMany(u => u.UserRoles) // N UserModel.UserRoles tem ... (array)
                .WithOne(ur => ur.User) // 1 UserRoles.User ...
                .HasForeignKey(ur => ur.UserId) // onde UserRoles.UserId é a FK (Z)
                .IsRequired();

            // 3.2. Define que ... uma role tem muitos users
            builder.Entity<RoleModel>()
                .HasMany(r => r.UserRoles) // N RoleModel.UserRoles tem ... (array)
                .WithOne(ur => ur.Role) // 1 UserRoles.Role ...
                .HasForeignKey(ur => ur.RoleId) // onde UserRoles.RoleId é a FK (Z)
                .IsRequired();
        }

    }
}