using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(
            DataContext dataContext,
            ITokenService tokenService,
            IMapper mapper
            )
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto userInput)
        {
            if (await UserExists(userInput.Username)) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(userInput);

            using var hmac = new HMACSHA512();

            user.UserName = userInput.Username;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userInput.Password));
            user.PasswordSalt = hmac.Key;

            _dataContext.Add(user);
            await _dataContext.SaveChangesAsync();

            var userDto = new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };

            return userDto;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto userInput)
        {
            var user = await _dataContext.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(user => user.UserName == userInput.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userInput.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            var userDto = new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

            return userDto;
        }

        private async Task<bool> UserExists(string userName)
        {
            return await _dataContext.Users.AnyAsync(user => user.UserName == userName.ToLower());
        }
    }
}
