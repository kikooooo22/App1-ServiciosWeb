using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        // '??' Si no encuentra el key, tira el error 
        var tokenKey = config["TokenKey"] ?? throw new Exception("Token not found");

        // Caso contrario perimero verificamos la longitud del token 
        if (tokenKey.Length < 64) throw new Exception("Token key too short");

        // Si todo sale bien, creamos la key con el TokenKey
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, user.UserName),
        };

        // Creamos credenciales con (y para) la llave 
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature); 

        var tokenDescrpitor = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescrpitor);

        return tokenHandler.WriteToken(token);
    }

}
