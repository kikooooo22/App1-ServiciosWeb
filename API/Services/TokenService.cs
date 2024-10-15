namespace API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        // '??' Si no encuentra la clave "TokenKey" en la configuración, lanza una excepción con el mensaje "Token not found"
        var tokenKey = config["TokenKey"] ?? throw new ArgumentException("Token not found");

        // Verificamos que la longitud del "TokenKey" sea al menos de 64 caracteres, si no es así, lanza una excepción
        if (tokenKey.Length < 64)
        {
            throw new ArgumentException("Token key too short");
        }

        // Convertimos el TokenKey a un arreglo de bytes usando UTF-8 y lo usamos para crear una clave de seguridad simétrica
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        // Creamos una lista de "claims" o declaraciones de identidad del usuario; en este caso, solo añadimos el nombre de usuario
        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, user.UserName),
        };

        // Creamos las credenciales de firma, indicando que usamos la clave generada y el algoritmo HMAC-SHA256 para firmar el token
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        // Definimos los detalles del token:
        // - Los claims (identidad del usuario)
        // - Tiempo de expiración (7 días desde el momento actual)
        // - Las credenciales de firma
        var tokenDescrpitor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        // Creamos un manejador de tokens JWT
        var tokenHandler = new JwtSecurityTokenHandler();

        // Creamos el token basándonos en la descripción del token (tokenDescriptor)
        var token = tokenHandler.CreateToken(tokenDescrpitor);

        // Finalmente, escribimos el token en formato legible (string) y lo devolvemos
        return tokenHandler.WriteToken(token);
    }

}
