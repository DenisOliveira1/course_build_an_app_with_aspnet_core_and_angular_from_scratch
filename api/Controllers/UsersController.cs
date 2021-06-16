using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Context;
using api.DTOs;
using api.Models;
using api.Repository.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly  IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();
            var usersDto = _mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(usersDto);
        }

        // [Authorize] exige autenticação por token para acessar o endpoint
        //[Authorize]
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            var userDto = _mapper.Map<MemberDto>(user);
            return Ok(userDto);
        }
    }
}