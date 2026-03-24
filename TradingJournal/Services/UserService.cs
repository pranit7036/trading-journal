using TradingJournal.Interfaces.Repository;
using TradingJournal.Interfaces.Services;
using TradingJournal.Mappers;
using TradingJournal.Models;
using TradingJournal.Models.Dto;
using TradingJournal.Models.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace TradingJournal.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly UserMapper _userMapper = new UserMapper();
        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<Response> RegisterUser(UserDto userDto)
        {
            if(userDto.UserName == null)
            {
                return (new Response
                {
                    Success = false,
                    Message = "Please enter a valid UserName",
                    Data = null
                });
            }

            if (userDto.Email == null)
            {
                return (new Response
                {
                    Success = false,
                    Message = "Please enter a valid Email",
                    Data = null
                });
            }

            if(userDto.Password == null)
            {
                return (new Response
                {
                    Success = false,
                    Message = "Please enter a valid Password",
                    Data = null
                });
            }

            if (await _userRepository.UserNameExist(userDto.UserName))
            {
                return (new Response
                {
                    Success = false,
                    Message = "UserName already exist",
                    Data = null
                });
            }

            if (await _userRepository.EmailExist(userDto.Email))
            {
                return (new Response
                {
                    Success = false,
                    Message = "Email already exist",
                    Data = null
                });
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            UserEntity userEntity = _userMapper.ConverUserDtoToUserEntity(userDto, hashedPassword);

            var result =await _userRepository.SaveUser(userEntity);

            if (result == null) {
                return (new Response
                {
                    Success = false,
                    Message = "Error while saving the data",
                    Data = null
                });
            }
            else {
                return (new Response {
                    Success = true,
                    Message = "User Resgistered",
                    Data = null
                });
            }
        }

        public async Task<Response> LoginUser(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmail(loginDto.Email);

            if (user == null)
            {
                return (new Response
                {
                    Success = false,
                    Message = "Invalid Email",
                    Data = null
                });
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);

            if (!isPasswordValid)
            {
                return (new Response
                {
                    Success = false,
                    Message = "Invalid Password",
                    Data = null
                });
            }

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            var result = await _userRepository.UpdateUser(user);

            if (!result)
            {
                return (new Response
                {
                    Success = false,
                    Message = "Error while saving the refresh token",
                    Data = null
                });
            }

            return (new Response
            {
                Success = true,
                Message = "Login Successful",
                Data = new { AccessToken = accessToken, RefreshToken = refreshToken} 
            });
        }

        public async Task<Response> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var email = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;

            if (email == null)
            {
                return new Response { Success = false, Message = "Invalid access token", Data = null };
            }

            var user = await _userRepository.GetUserByEmail(email);

            if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                return new Response { Success = false, Message = "Invalid refresh token", Data = null };
            }

            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Update the refresh token in the database
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateUser(user);

            return new Response
            {
                Success = true,
                Message = "Token refreshed successfully",
                Data = new { AccessToken = newAccessToken, RefreshToken = newRefreshToken }
            };
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private string GenerateAccessToken(UserEntity user)
        {
            var secret = _configuration["Jwt:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException("JWT secret is not configured.");
            }

            var issuer = _configuration["Jwt:Issuer"] ?? "TradingJournal";
            var audience = _configuration["Jwt:Audience"] ?? "TradingJournal";
            var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var minutes) ? minutes : 15;

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.UserName)
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var secret = _configuration["Jwt:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException("JWT secret is not configured.");
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateLifetime = false // only difference vs normal pipeline
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
