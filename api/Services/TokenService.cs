using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Models;
using api.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace api.Services
{
    public class TokenService : ITokenService
    {

        public readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            // Gera chave de segurança
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(UserModel user)
        {
            var claims  = new List<Claim>{
                 new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()), // Id do usuário a ser logado
                 new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName) // Nome do usuário a ser logado
            };
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds        
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor); // Cria token que é exigido pelo [Authorize], que foi configurado no IdentityServiceExtensions.cs
            return tokenHandler.WriteToken(token);
         }
    }
}