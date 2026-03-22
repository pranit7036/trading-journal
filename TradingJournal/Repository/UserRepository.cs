using Microsoft.EntityFrameworkCore;
using TradingJournal.Context;
using TradingJournal.Interfaces.Repository;
using TradingJournal.Models;
using TradingJournal.Models.Entity;

namespace TradingJournal.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DBContext _context;
        
        public UserRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<UserEntity?> SaveUser(UserEntity userEntity)
        {
            try
            {
                var result = await _context.Users.AddAsync(userEntity);
                await _context.SaveChangesAsync();
                return result.Entity;
                
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UserNameExist(string userName)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.UserName == userName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> EmailExist(string email)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserEntity?> GetUserByEmail(string email)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateUser(UserEntity userEntity)
        {
            try
            {
                _context.Users.Update(userEntity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
