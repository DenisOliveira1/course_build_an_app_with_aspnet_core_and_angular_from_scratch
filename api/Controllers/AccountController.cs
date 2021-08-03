using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using api.Context;
using api.DTOs;
using api.Helpers;
using api.Models;
using api.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

            user.UserName = registerDto.Username.ToLower();

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) return BadRequest(result.Errors);

            var userDto = new UserDto{
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnowAs = user.KnownAs,
                Gender = user.Gender
            };
            
            return userDto;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){

            var user = await _userManager.Users
                .Include(x => x.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return Unauthorized();
    
            var userDto = new UserDto{
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnowAs = user.KnownAs,
                Gender = user.Gender
            };
  
            return userDto;
        }

        private async Task<bool> UserExists(string username){
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}