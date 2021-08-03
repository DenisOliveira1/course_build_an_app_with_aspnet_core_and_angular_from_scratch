using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTOs;
using api.Extensions;
using api.Helpers;
using api.Helpers.Params;
using api.Models;
using api.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username){

            var sourceUserId = User.GetUserId();
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            var likedUser = await _userRepository.GetUserByUsernameAsync(username);

            if (likedUser == null) return NotFound();
            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike =  await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userLike != null) return BadRequest("You have already liked " + likedUser.UserName.ToTitleCase());

            userLike = new UserLikeModel{
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id,     
            };

            sourceUser.LikedUsers.Add(userLike);

            // Mesmo adicionando pelo contexto likeRepository da para Salvar pelo contexto do userRepository, mas isso será atualizado
            if(await _userRepository.SaveAllAsync()){
                return Ok();
            }
            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams){

            likesParams.userId = User.GetUserId();
            // Se esquecer o await o resultado chega, porém emcapsulado em um json diferente
            var users = await _likesRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(users.CurrentPage, users.TotalPages, users.PageSize, users.TotalCount);

            return Ok(users);
        }
        
    }
}