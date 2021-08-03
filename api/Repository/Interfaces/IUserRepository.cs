using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTOs;
using api.Helpers;
using api.Helpers.Params;
using api.Models;

namespace api.Repository.Interfaces
{
    public interface IUserRepository
    {

        void Update(UserModel user);
        void Delete(UserModel user);
        Task<bool> SaveAllAsync();
        
        // UserModel
        Task<IEnumerable<UserModel>> GetUsersAsync();
        Task<UserModel> GetUserByIdAsync(int id);
        Task<UserModel> GetUserByUsernameAsync(string username);

        // MemberDto        
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberByIdAsync(int id);
        Task<MemberDto> GetMembersByUsernameAsync(string username);
    }
}