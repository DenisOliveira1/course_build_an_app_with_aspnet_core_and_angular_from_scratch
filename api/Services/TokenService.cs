using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace api.Services
{
    public class TokenService : ITokenService
    {

        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<UserModel> _userManager;

        public TokenService(IConfiguration config, UserManager<UserModel> userManager)
        {
            // Gera chave de segurança
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _userManager = userManager;
        }
        public async Task<string> CreateToken(UserModel user)
        {
            var claims  = new List<Claim>{
                 new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()), // Id do usuário a ser logado
                 new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName) // Nome do usuário a ser logado
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds        
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor); // Cria token que é exigido pelo [Authorize], que foi configurado no IdentityServiceExtensions.cs
            return tokenHandler.WriteToken(token);
         }
    }
}