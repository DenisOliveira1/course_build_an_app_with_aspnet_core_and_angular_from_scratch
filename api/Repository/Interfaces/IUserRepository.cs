using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models;

namespace api.Repository.Interfaces
{
    public interface IUserRepository
    {
        void Update(UserModel user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<UserModel>> GetUsersAsync();
        Task<UserModel> GetUserByIdAsync(int id);
        Task<UserModel> GetUserByUsernameAsync(string username);
    }
}