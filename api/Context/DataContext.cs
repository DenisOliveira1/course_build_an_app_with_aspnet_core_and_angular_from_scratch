using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Context
{
    public class DataContext : DbContext
    {

        // Refazer tabelas:
        // dotnet ef database drop
        // dotnet ef database update

        public DbSet<UserModel> Users { get; set; }
        public DataContext(DbContextOptions options): base(options)
        {

        }
        

    }
}