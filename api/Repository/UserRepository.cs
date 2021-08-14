using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Context;
using api.DTOs;
using api.Helpers;
using api.Helpers.Params;
using api.Models;
using api.Repository.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void Update(UserModel user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public void Delete(UserModel user)
        {
            _context.Remove(user);
        }

        // UserModel

        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
             return await _context.Users
                .Include(u => u.Photos)
                .ToListAsync();
        }
        public async Task<string> GetUserGenderAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .Select(x => x.Gender)
                .FirstOrDefaultAsync();
        }
        
        // MemberDto
        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsNoTracking();
                
            query = query.Where(x => x.Username != userParams.Username);
            query = query.Where(x => x.Gender == userParams.Gender);

            var minDate = DateTime.Today.AddYears(-userParams.MaxAge -1); // hoje - 150 - um dia
            var maxDate = DateTime.Today.AddYears(-userParams.MinAge); // hoje - 18
        
            query = query.Where(x => x.DateOfBirth >= minDate && x.DateOfBirth <= maxDate);

            query = userParams.OrderBy switch{
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive),
            };
                
            return await PagedList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<MemberDto> GetMembersByUsernameAsync(string username)
        {
            return await _context.Users
                .Where(u => u.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<MemberDto> GetMemberByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

    }
}