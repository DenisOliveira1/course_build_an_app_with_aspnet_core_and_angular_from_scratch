using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTOs;
using api.Helpers;
using api.Models;

namespace api.Repository.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(MessageModel message);
        void DeleteMessage(MessageModel message);
        Task<MessageModel> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string recipientusername);
        void AddGroup(GroupModel group);
        Task<GroupModel> GetGroupByName(string name);
        Task<GroupModel> GetGroupForConnection(string connectionId);
        void RemoveConnetion(ConnectionModel connection);
        Task<ConnectionModel> GetConnetion(string id);
    }
}