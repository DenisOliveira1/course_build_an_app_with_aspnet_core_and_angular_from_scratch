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
                            .AsQueryable();

            query = messageParams.Container switch{
                "inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username && u.RecipientDeleted == false), // mensagens recebidas
                "outbox" => query.Where(u => u.Sender.UserName == messageParams.Username && u.SenderDeleted == false), // mensagens enviadas
                _ => query.Where(u => u.Recipient.UserName == messageParams.Username && u.DateRead == null) // mensagens recebidas e não lidas
                
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber,messageParams.PageSize);


        }

        public async Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                                .Include(m => m.Sender).ThenInclude(u => u.Photos)
                                .Include(m => m.Recipient).ThenInclude(u => u.Photos)
                                .Where(m => (m.Recipient.UserName == currentUsername && m.Sender.UserName == recipientUsername && m.RecipientDeleted == false) ||
                                            (m.Sender.UserName == currentUsername && m.Recipient.UserName == recipientUsername && m.SenderDeleted == false))
                                .OrderBy(m => m.MessageSent)
                                .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername).ToList();
            if (unreadMessages.Any()){
                foreach (var message in unreadMessages){
                    // Não precisa adicionar via _context.Update(x) porque são instancias que foram recuperadas do banco
                    // e como não tem AsNoTracking já estão sendo monitoradas e serão salvas no  _context.SaveChangesAsync()
                    message.DateRead = DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}