using System;
using System.Threading.Tasks;
using api.Extensions;
using api.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace api.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly IUserRepository _userRepository;
        private readonly PresenceTracker _presenceTracker;

        public PresenceHub(IUserRepository userRepository, PresenceTracker presenceTracker)
        {
            _userRepository = userRepository;
            _presenceTracker = presenceTracker;
        }

        // No front, todo usuario que abrir uma conexao com o hub PresenceHub vai ser um cliente desse hub
        // No back, quando um usuario loga ele dispara o OnConnectedAsync e vai informar a todos os clientes que estiverem inscritos em "UserIsOnline" o knownAs
        // Todos os clientes que estiverem inscritos em await Clients.All.SendAsync("GetOnlineUsers",currentUsers) receberão a lista de usuários online

        // Conectando ao hub (quando loga)
        public override async Task OnConnectedAsync()
        {

            var isOnline = await _presenceTracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            if (isOnline){
                var username = Context.User.GetUsername();
                var user = await _userRepository.GetUserByUsernameAsync(username);
                await Clients.Others.SendAsync("UserIsOnline", user.KnownAs);
            }

            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers",currentUsers);
            
        }

        // Desconectando do hub (quando da logout, refresh na página ou fecha página)
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var isOffline =  await _presenceTracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
            if (isOffline){
                var username = Context.User.GetUsername();
                var user = await _userRepository.GetUserByUsernameAsync(username);
                await Clients.Others.SendAsync("UserIsOffline", user.KnownAs);
            }

            // Tem que ser a última coisa do OnDisconnectedAsync
            await base.OnDisconnectedAsync(exception);
        }
    }
}