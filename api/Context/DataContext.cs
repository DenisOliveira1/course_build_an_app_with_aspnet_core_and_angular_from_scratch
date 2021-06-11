using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Context
{
    public class DataContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DataContext(DbContextOptions options): base(options)
        {

        }

    }
}