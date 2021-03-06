using System;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs;
using api.Extensions;
using api.Models;
using api.Repository.Interfaces;
using api.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace api.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _presenceTracker;

        public MessageHub(IUnitOfWork unitOfWork,
                IMapper mapper,
                IHubContext<PresenceHub> presenceHub,
                PresenceTracker presenceTracker)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _presenceHub = presenceHub;
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            // Context e Groups vem de Hub assim como User vem de ControllerBase
            // Sempre que um objeto for acessado e ele não está declarado aqui significa que ele vem do pai
            var httpContext = Context.GetHttpContext();
            
            var username = Context.User.GetUsername();       
            var otherUsername = httpContext.Request.Query["user"].ToString();
            
            var groupName = GetGroupName(username, otherUsername);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
            
            var messages = await _unitOfWork.MessageRepository.GetMessagesThread(username, otherUsername);

            if (_unitOfWork.HasChanges()) await _unitOfWork.Complete();

            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);

            // Mesmo que o método não envia nada precisa dessa linha na desconexão do hub
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(CreateMessageDto createMessageDto){

            var username = Context.User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("You cannot send messages to yourself");

            var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) 
                 throw new HubException("Recipient not found");

            var message = new MessageModel{
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content,
                SenderKnownAs = sender.KnownAs,
                RecipientKnownAs = recipient.KnownAs
            };

            _unitOfWork.MessageRepository.AddMessage(message);

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _unitOfWork.MessageRepository.GetGroupByName(groupName);

            // Se for true significa que ambos estão na tela de mensagem
            // O sender pois enviou a mensagem 
            // O recipient pois está na tela de mensagem
            if(group.Connections.Any(x => x.Username == recipient.UserName)){
                message.DateRead = DateTime.UtcNow;
            }
            // Recipient não está na tela de menssagens
            else{
                var connections = await _presenceTracker.GetConnectionsForUser(recipient.UserName);
                // Recipient está online
                if(connections != null){
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                    new {
                        username = sender.UserName,
                        knownAs = sender.KnownAs,
                        message = createMessageDto.Content,
                        // photoUrl = sender.Photos.FirstOrDefault(p => p.IsMain)
                    });
                }
            }

            if (await _unitOfWork.Complete()){
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }

        private string GetGroupName(string caller, string other){
            var stringCompare = string.Compare(caller, other) < 0; 
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
        
        private async Task<GroupDto> AddToGroup(string groupName){
            var group = await _unitOfWork.MessageRepository.GetGroupByName(groupName);
            var username = Context.User.GetUsername();
            var connection = new ConnectionModel(Context.ConnectionId, username);

            //Se grupo não existe cria um
            if (group == null){
                group = new GroupModel(groupName);
                _unitOfWork.MessageRepository.AddGroup(group);
            }
            // Adiciona uma nova conexão referente a esse grupo
            group.Connections.Add(connection);

            if(await _unitOfWork.Complete()) return _mapper.Map<GroupDto>(group);

            throw new HubException("Failed to join group");
        }

        private async Task<GroupDto> RemoveFromGroup(){
            var group = await _unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _unitOfWork.MessageRepository.RemoveConnetion(connection);

            if (await _unitOfWork.Complete()) return  _mapper.Map<GroupDto>(group);

            throw new HubException("Failed to remove group");
        }
    }
}