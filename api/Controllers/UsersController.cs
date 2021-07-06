using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Context;
using api.DTOs;
using api.Extensions;
using api.Models;
using api.Repository.Interfaces;
using api.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    // [Authorize] exige autenticação por token para acessar o endpoint
    // Pode ser colocado em cima da classe toda, abrangendo todos os métodos, ou
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _mapper = mapper;
            _photoService = photoService;
            _userRepository = userRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);
        }

        // [Authorize] exige autenticação por token para acessar o endpoint
        //Essa rota tem um nome
        //[Authorize]
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _userRepository.GetMembersByUsernameAsync(username);
            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto){

            //Pega o username baseado no claim (token) de quem enviou a requisição
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync (username);

            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);

           if (await _userRepository.SaveAllAsync()) return NoContent();
           return BadRequest("Failed to update user");
                
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file){

            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);
            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new PhotoModel{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0){
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync()){
                var photoDto =  _mapper.Map<PhotoDto>(photo);
                // Como parametro é passado o nome de uma rota
                // CreatedAtRoute é o status 201, assim como o Ok é 200
                return CreatedAtRoute("GetUser",new {username = user.UserName}, photoDto);
            }

             return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(long photoId){

            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);

            // Não é Async porque não está indo na base de dados, o user já está em momória
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null){
                currentMain.IsMain = false;
            }
            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync()){
                return NoContent();
            }

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(long photoId){

            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You can't delete your main photo");

            // Tem PublicId se a foto esta no Cloudinary
            if (photo.PublicId != null){
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)  return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if(await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Failed to delete the photo");
        }

        [HttpDelete("delete-user")]
        public async Task<ActionResult> DeleteUser(long userId){

            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);

            // Deletando fotos
            foreach (var photo in user.Photos){
                if (photo.PublicId != null){
                    var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                    if (result.Error != null)  return BadRequest(result.Error.Message);
                }
            }

            _userRepository.Delete(user);
            if(await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Failed to delete the user");
        }
    }
}