using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TradingJournal.Interfaces.Services;
using TradingJournal.Models;
using TradingJournal.Models.Dto;
using TradingJournal.Services;

namespace TradingJournal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserService UserService;

        public UserController(IUserService _userService)
        {
            UserService = _userService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser(UserDto userDto)
        { 
            if(userDto == null) 
            {
                return BadRequest(new Response { Success = false, Message = "Error while registering the user", Data = null });
            }

            var result = await  UserService.RegisterUser(userDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginUser(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest(new Response { Success = false, Message = "Error while logging in the user", Data = null });
            }

            var result = await UserService.LoginUser(loginDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenDto tokenDto)
        {
            if (tokenDto == null)
            {
                return BadRequest(new Response { Success = false, Message = "Error while refreshing the token", Data = null });
            }

            var result = await UserService.RefreshToken(tokenDto);

            return Ok(result);
        }
    }
}