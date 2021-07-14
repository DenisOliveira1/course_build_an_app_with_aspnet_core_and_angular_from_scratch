using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using api.Context;
using api.DTOs;
using api.Models;
using api.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        // Quando usa a anotação [ApiController] ela identifica de onde vem as informações
        // Quando não usa [ApiController] tem que indicar se a informação está no [FromBody], [FromQuery]
        // Quando a requisição tem as informações no body, a função no Controller deve esperar um objeto como parâmetro
        // Quando a requisição tem as informações na query, a função no Controller deve esperar um objeto ou variáveis como parâmetro

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto){

            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken!");

            var user = _mapper.Map<UserModel>(registerDto);

            using var hmac = new HMACSHA512();  

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto{
                username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnowAs = user.KnownAs,
                Gender = user.Gender
            };
            
            return userDto;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){

            var user = await _context.Users
                .Include(x => x.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null) return Unauthorized("Invalid username!");

            using var hmac = new HMACSHA512(user.PasswordSalt);  
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i=0; i < computedHash.Length; i++){
                 if (computedHash[i] != user.PasswordHash[i]){
                     return Unauthorized("Invalid password!");
                 }
            }

            var userDto = new UserDto{
                username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnowAs = user.KnownAs,
                Gender = user.Gender
            };
  
            return userDto;
        }

        private async Task<bool> UserExists(string username){
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}