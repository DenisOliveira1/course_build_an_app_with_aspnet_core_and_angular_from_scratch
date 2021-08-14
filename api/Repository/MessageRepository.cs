using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Context;
using api.DTOs;
using api.Helpers;
using api.Models;
using api.Repository.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddMessage(MessageModel message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(MessageModel message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<MessageModel> GetMessage(int id)
        {
            return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                            .OrderByDescending(m => m.MessageSent)
                            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                            .AsQueryable();

            query = messageParams.Container switch{
                "inbox" => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false), // mensagens recebidas
                "outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false), // mensagens enviadas
                _ => query.Where(u => u.RecipientUsername == messageParams.Username && u.DateRead == null) // mensagens recebidas e não lidas
                
            };

            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber,messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                                // .Include(m => m.Sender).ThenInclude(u => u.Photos)
                                // .Include(m => m.Recipient).ThenInclude(u => u.Photos)
                                .Where(m => (m.Recipient.UserName == currentUsername && m.Sender.UserName == recipientUsername && m.RecipientDeleted == false) ||
                                            (m.Sender.UserName == currentUsername && m.Recipient.UserName == recipientUsername && m.SenderDeleted == false))
                                .OrderBy(m => m.MessageSent)
                                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider) // Com ProjectTo não precisa mais fazer includes
                                .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();
            if (unreadMessages.Any()){
                foreach (var message in unreadMessages){
                    // Não precisa adicionar via _context.Update(x) porque são instancias que foram recuperadas do banco
                    // e como não tem AsNoTracking já estão sendo monitoradas e serão salvas no  _context.SaveChangesAsync()
                    message.DateRead = DateTime.UtcNow;
                }
                // Não é responsabilidade do repositorio, mas sim do UnitOfWork
                // await _context.SaveChangesAsync();
            }

            return messages;
        }

        // Groups
        // Tive que criar um Dto para GroupModel porque esta dando referencia circular ao recuperar um grupo
        // Cada grupo tinha uma lista de connection e cada connection tinha um grupo com uma nova lista de connections dentro, infinitamente
        public void AddGroup(GroupModel group)
        {
            _context.Groups.Add(group);
        }

        public async Task<GroupModel> GetGroupByName(string name)
        {
            return await _context.Groups
                        .Include(x => x.Connections)
                        .FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<GroupModel> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(c => c.Connections)
                .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        // Connetions

        public void RemoveConnetion(ConnectionModel connection)
        {
            _context.Connections.Remove(connection);

            var connections = _context.Connections.Where(x => x.groupName == connection.groupName).ToListAsync().Result;

            if (connections.Count() <= 1) {
                var group = _context.Groups.FirstOrDefaultAsync(x => x.Name == connection.groupName).Result;
                _context.Groups.Remove(group);
            }
        }

        public async Task<ConnectionModel> GetConnetion(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }
    }
}