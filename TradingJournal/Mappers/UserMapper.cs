using System;
using TradingJournal.Models.Dto;
using TradingJournal.Models.Entity;

namespace TradingJournal.Mappers
{
    public class UserMapper
    {
        public UserEntity ConverUserDtoToUserEntity (UserDto userDto, string hashedPassword)
        {
            return (new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = userDto.UserName,
                Email = userDto.Email,
                Password = hashedPassword
            });
        }

        public UserDto ConvertUserEntityToUserDto (UserEntity userEntity)
        {
            return (new UserDto
            {
                UserName = userEntity.UserName,
                Email = userEntity.Email,
                Password = userEntity.Password
            });
        }
    }
}
