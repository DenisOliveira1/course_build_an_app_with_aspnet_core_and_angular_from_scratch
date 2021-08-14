using System.Threading.Tasks;
using api.Repository.Interfaces;

namespace api.Services.Interfaces
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get;}
        public IMessageRepository MessageRepository { get;}
        public ILikesRepository LikesRepository { get;}

        Task<bool> Complete();
        bool HasChanges();
    }
}