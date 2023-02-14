using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Interfaces;

/// <summary>
/// Интерфейс, позволяющий генерировать JWT-токен с Email как идентифицирующий пользователя клейм.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Генерирует JWT-токен с Email как идентифицирующий пользователя клейм.
    /// </summary>
    string GenerateToken(string email);
}
