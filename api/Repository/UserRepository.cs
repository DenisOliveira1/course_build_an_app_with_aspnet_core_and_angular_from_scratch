using System.Collections.Generic;
using System.Threading.Tasks;
using api.Context;
using api.Models;
using api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
             return await _context.Users
                .Include(u => u.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {   
            // Retorna o número de mudanças salvas no banco de dados
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(UserModel user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}