using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTOs;
using api.Models;

namespace api.Repository.Interfaces
{
    public interface IUserRepository
    {
        // UserModel
        void Update(UserModel user);
        void Delete(UserModel user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<UserModel>> GetUsersAsync();
        Task<UserModel> GetUserByIdAsync(int id);
        Task<UserModel> GetUserByUsernameAsync(string username);

        // MemberDto        
        Task<IEnumerable<MemberDto>> GetMembersAsync();
        Task<MemberDto> GetMemberByIdAsync(int id);
        Task<MemberDto> GetMembersByUsernameAsync(string username);
    }
}