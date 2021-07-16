using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Context;
using api.DTOs;
using api.Extentions;
using api.Helpers;
using api.Models;
using api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLikeModel> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<UserModel> GetUserWithLikes(int userId)
        {
            return await _context.Users
                                .Include(x => x.LikedUsers)
                                .FirstOrDefaultAsync(x => x.Id == userId);

        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var likes = _context.Likes.AsQueryable();
            var users = _context.Users.OrderBy(x => x.UserName).AsQueryable();

            if (likesParams.predicate == "liked"){
                likes = likes.Where(like => like.SourceUserId == likesParams.userId); // Pega todos os likes onde o source é o user informado
                users = likes.Select(like => like.LikedUser); // Então, desses likes, pega os users que receberam o like
            }
            else if (likesParams.predicate == "likedBy"){
                likes = likes.Where(like => like.LikedUserId == likesParams.userId); // Pega todos os likes recebidos pelo user informado
                users = likes.Select(like => like.SourceUser); // Então, desses likes, pega os users que deram o like
            }

            var usersQuery =  users.Select(user => new LikeDto{
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                UserId = user.Id,
            });

            return await PagedList<LikeDto>.CreateAsync(usersQuery, likesParams.PageNumber, likesParams.PageSize);
        }
    }
}