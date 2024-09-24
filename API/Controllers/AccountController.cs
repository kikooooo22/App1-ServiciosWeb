using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(
    DataContext context,
    ITokenService tokenService
    ) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> RegisterAsync(RegisterRequest request){
        
        // Verificamos si el usuario ya esta registrado 
        if ( await UserExisteAsync(request.Username))
        {
            return BadRequest("Username already exist");
        } 

        // Se crea el salt (No olvidemos que es objeto para hashear)
        using var hmac = new HMACSHA512();

        // using: Una vez se ejecuta, se libera la memoria 
        var user = new AppUser{
            UserName = request.Username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserResponse{
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };

    }//RegisterAsync

    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> LoginAsync(LoginRequest request)
    {
        // Verificamos que el usuario este en nuestra base de datos
        var user = await context.Users.FirstOrDefaultAsync(x => 
        x.UserName.ToLower() == request.Username.ToLower());

        if (user == null)
        {
            return Unauthorized("Invalid username or password");
        }
        
        // Generamos la semilla con el PasswordSalt del usuario  
        using var hmac = new HMACSHA512(user.PasswordSalt);
        // Encriptamos el password con dicha semilla del PasswordSalt
        var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

        // Comparar cada uno de los bytes generados 
        // Password ya Hasheado - vs - Password que Hasheamos nosotros mismos (con el Salt)
        for(int i = 0; i < computeHash.Length; i++){
            if(computeHash[i] != user.PasswordHash[i]){
                return Unauthorized("Invalid username or password"); 
            }
        }

        return new UserResponse{
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };

    }

    private async Task<bool> UserExisteAsync(string username) =>
        await context.Users.AnyAsync(user => user.UserName.ToLower() == username.ToLower());
}
