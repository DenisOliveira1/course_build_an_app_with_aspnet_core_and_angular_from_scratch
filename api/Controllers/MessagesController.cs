using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTOs;
using api.Extensions;
using api.Helpers;
using api.Models;
using api.Repository.Interfaces;
using api.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams){

            messageParams.Username = User.GetUsername();

            var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return Ok(messages);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id){

            var username = User.GetUsername();
            var message = await _unitOfWork.MessageRepository.GetMessage(id);

            if (message.Sender.UserName != username && message.Recipient.UserName != username)
                return Unauthorized();

            if (message.Sender.UserName == username)
                message.SenderDeleted = true;

            if (message.Recipient.UserName == username)
                message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
                // Não tem await porque não tem retorno
                _unitOfWork.MessageRepository.DeleteMessage(message);

            if(await _unitOfWork.Complete())
                return Ok();
            
            return BadRequest("There was a problem deleting the message");
        }
        
    }
}