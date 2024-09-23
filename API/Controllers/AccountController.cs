using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<AppUser>> RegisterAsync(RegisterRequest request){
        
        if ( await UserExisteAsync(request.Username))
        {
            return BadRequest("Username already exist");
        } 

        using var hmac = new HMACSHA512();

        // using: Una vez se ejecuta, se libera la memoria 
        var user = new AppUser{
            UserName = request.Username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    private async Task<bool> UserExisteAsync(string username) =>
        await context.Users.AnyAsync(user => user.UserName.ToLower() == username.ToLower());
}
