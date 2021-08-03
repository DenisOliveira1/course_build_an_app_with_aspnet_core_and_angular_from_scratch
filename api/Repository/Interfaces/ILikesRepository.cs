using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTOs;
using api.Helpers;
using api.Helpers.Params;
using api.Models;

namespace api.Repository.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLikeModel> GetUserLike(int sourceUserId, int likedUserId);
        Task<UserModel> GetUserWithLikes(int userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}