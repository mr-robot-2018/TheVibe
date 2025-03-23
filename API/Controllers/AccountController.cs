using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(DataContext context,ITokenService tokenService):BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(registerDTO registerDTO){
            using var hmac = new HMACSHA512();
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
            var user = new AppUser
            {
                UserName = registerDTO.Username.ToLower(),
                PasswordHash = computeHash,
                PasswordSalt = hmac.Key
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return new UserDTO{
                Username=user.UserName,
                Token=tokenService.CreateToken(user)
            };

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(loginDTO loginDTO)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.Username.ToLower());
            if (user == null)
            {
                return Unauthorized("Invalid username");
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            for (int i = 0; i < computeHash.Length; i++)
            {
                if (computeHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid password");
                }
            }
            return new UserDTO
            {
                Username = user.UserName,
                Token = tokenService.CreateToken(user)
            };
        }
        public async Task<bool> IsUserExits(string username)
        {
            return await context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}