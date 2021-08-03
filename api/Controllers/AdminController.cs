using System.Linq;
using System.Threading.Tasks;
using api.DTOs;
using api.Extensions;
using api.Helpers;
using api.Helpers.Params;
using api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<UserModel> _userManager;
        public AdminController(UserManager<UserModel> userManager)
        {
            _userManager = userManager;
        }
        
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles([FromQuery] RolesParams rolesParams){
            var query =  _userManager.Users
                            .Include(r => r.UserRoles)
                            .ThenInclude(r => r.Role)
                            .OrderBy(u => u.UserName)
                            .Select(u => new RolesDto{
                                Id = u.Id,
                                Username = u.UserName,
                                KnownAs = u.KnownAs,
                                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                            })
                            .AsQueryable();

            var users = await PagedList<RolesDto>.CreateAsync(query, rolesParams.PageNumber, rolesParams.PageSize);
            Response.AddPaginationHeader(users.CurrentPage, users.TotalPages, users.PageSize, users.TotalCount);
            
            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles){

            var salectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return BadRequest("Could not find user");

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, salectedRoles.Except(userRoles));
            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(salectedRoles));
            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration(){             
            return Ok("Only admins and moderators can read it");

        }
        
    }
}