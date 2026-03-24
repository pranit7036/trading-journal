using Microsoft.AspNetCore.Mvc;
using TradingJournal.Models;
using TradingJournal.Models.Dto;

namespace TradingJournal.Interfaces.Services
{
    public interface IUserService
    {
        public Task<Response> RegisterUser(UserDto userDto);
        public Task<Response> LoginUser(LoginDto loginDto);
        public Task<Response> RefreshToken(TokenDto tokenDto);
    }
}
