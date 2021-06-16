using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Context
{
    public class DataContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        // Não será necessario ir ao banco para consultar fotos individualmente, logo não há porque ter um contexto Photos
        // public DbSet<PhotoModel> Photos { get; set; }
        public DataContext(DbContextOptions options): base(options)
        {

        }

    }
}