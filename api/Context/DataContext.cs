using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Context
{
    public class DataContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
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
            // (A) 1:N (B)* N:1 (C) em conjunto, ambos tem o mesmo significado
            // ** a nova tabela com as FKs
            // Ja que a tabela N:N foi criada manualmente é necessário estabelecer manualmente as relações 1:N e N:1

            // Em 1:1 a FK fica em qualquer uma das tabelas
            // Em 1:N a FK fica em N 
            // Em N:N as FKs foram uma nova tabela

            // 1.2. Define que... um gostador tem muitas pessoas de que gosta
            builder.Entity<UserLikeModel>()
                .HasOne(l => l.SourceUser) // 1 UserLikeModel.SourceUser tem ...
                .WithMany(u => u.LikedUsers) // N UserModel.LikedUsers...
                .HasForeignKey(l => l.SourceUserId) // ode UserLikeModel.SourceUserId é a FK
                .OnDelete(DeleteBehavior.Cascade); // Se deletar UserLikeModel.SourceUser deleta UserLikeModel 

            // 1.3. Define que ... um gostador tem muitas pessoas que gostam dele
            builder.Entity<UserLikeModel>()
                .HasOne(l => l.LikedUser) // 1 UserLikeModel.LikedUser tem ...
                .WithMany(u => u.LikedByUsers) // N UserModel.LikedByUsers...
                .HasForeignKey(l => l.LikedUserId) // onde UserLikeModel.LikedUserId é a FK
                .OnDelete(DeleteBehavior.Cascade); // Se deletar UserLikeModel.LikedUser deleta UserLikeModel 

            // 2. User N-N User, <MessageModel>

            // 2.1. Define que ... um recebedor tem muitas mensagens recebidas
            builder.Entity<MessageModel>()
                .HasOne(m => m.Recipient) // 1 MessageModel.Recipient tem ...
                .WithMany(u=> u.MessagesReceived) // N MessageModel.MessagesReceived...
                .HasForeignKey(m => m.RecipientId) // onde MessageModel.RecipientId é a FK
                .OnDelete(DeleteBehavior.Restrict); // Se deletar MessageModel.Recipient não deleta MessageModel 

            // 2.2. Define que ... um enviador tem muitas mensagens enviadas
            builder.Entity<MessageModel>()
                .HasOne(m => m.Sender) // 1 MessageModel.Sender tem ...
                .WithMany(u => u.MessagesSent) // N MessageModel.MessagesSent...
                .HasForeignKey(m => m.SenderId) // onde MessageModel.SenderId é a FK
                .OnDelete(DeleteBehavior.Restrict); // Se deletar MessageModel.Sender não deleta MessageModel 
        }

    }
}