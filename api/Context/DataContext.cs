using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Context
{
    public class DataContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserLikeModel> Likes { get; set; }
        // Não será necessario ir ao banco para consultar fotos individualmente, logo não há porque ter um contexto Photos
        // public DbSet<PhotoModel> Photos { get; set; }

        public DataContext(DbContextOptions options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder){
            base.OnModelCreating(builder);

            builder.Entity<UserLikeModel>()
                .HasKey(k => new {k.SourceUserId, k.LikedUserId});

            builder.Entity<UserLikeModel>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLikeModel>()
                .HasOne(s => s.LikedUser)
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}