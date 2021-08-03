using System.Threading.Tasks;
using api.Models;

namespace api.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(UserModel user);
    }
}