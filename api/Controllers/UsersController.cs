using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Context;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
           _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers(){
            var users = await _context.Users.ToListAsync();
            return users;
        }

        // [Authorize] exige autenticação por token para acessar o endpoint
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(int id){
            var user = await _context.Users.FindAsync(id);
            return user;
        }
    }
}