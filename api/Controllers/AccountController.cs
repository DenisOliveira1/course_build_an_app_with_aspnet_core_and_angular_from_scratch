using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using api.Context;
using api.DTOs;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // Quando usa a anotação [ApiController] ela identifica de onde vem as informações
        // Quando não usa [ApiController] tem que indicar se a informação está no [FromBody], [FromQuery]
        // Quando a requisição tem as informações no body, a função no Controller deve esperar um objeto como parâmetro
        // Quando a requisição tem as informações na query, a função no Controller deve esperar um objeto ou variáveis como parâmetro

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto request){

            if (await UserExists(request.Username)) return BadRequest("Username is taken!");

            using var hmac = new HMACSHA512();  
            var user = new UserModel{
                UserName = request.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto{
                username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
            
            return userDto;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto request){

            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == request.Username);

            if (user == null) return Unauthorized("Invalid username!");

            using var hmac = new HMACSHA512(user.PasswordSalt);  
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

            for(int i=0; i < computedHash.Length; i++){
                 if (computedHash[i] != user.PasswordHash[i]){
                     return Unauthorized("Invalid password!");
                 }
            }

            var userDto = new UserDto{
                username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
  
            return userDto;
        }

        private async Task<bool> UserExists(string username){
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}