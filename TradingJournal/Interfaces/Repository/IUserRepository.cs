using TradingJournal.Models;
using TradingJournal.Models.Entity;

namespace TradingJournal.Interfaces.Repository
{
    public interface IUserRepository
    {
        public Task<UserEntity?> SaveUser (UserEntity userEntity);
        public Task<bool> UserNameExist (string userName);
        public Task<bool> EmailExist (string email);
        public Task<UserEntity?> GetUserByEmail(string email);
        public Task<bool> UpdateUser(UserEntity userEntity);
    }
}
