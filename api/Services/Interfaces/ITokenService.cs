using api.Models;

namespace api.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(UserModel user);
    }
}