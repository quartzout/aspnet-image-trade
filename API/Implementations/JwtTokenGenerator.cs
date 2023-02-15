using API.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Users.Identity.Classes;

namespace API.Implementations;

/// <summary>
/// Реализация <see cref="IJwtTokenGenerator"/>. Берет данные для JWT из <see cref="JwtOptions"/>
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{

    IOptions<JwtOptions> jwtOptions;

    public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
    {
        this.jwtOptions = jwtOptions;
    }

    public string GenerateToken(string email, string userId)
    {
        //Шифровка ключа, известного только серверу (по нему он определяет валидность пришедшего из запроса токена)
        var signingCredentials = new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Key)),
        SecurityAlgorithms.HmacSha256
    );

        //Набор клеймов, сохраняемых в токене. После аутентификации запроса по этому токену эти клеймы будут
        //записаны в HttpContext.User. Эти клемы никак не связаны и не взаимодействуют с Identity.
        var claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId), //Субьект клеймов (пользователь) 
            new Claim(ClaimTypes.Name, email), //Кастомный клейм, по которому будет искаться пользователь в бд через UserManager
            //Тип клейма должен быть одинаковым во всех auth схемах, чтобы потом по этому типу можно было достать username
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) //GUID токена. Уникальный для каждого токена
        };

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            expires: DateTime.Now.AddDays(jwtOptions.Value.ExpireDays),
            claims: claims,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token); //Возвращает выстроенный токен как строку
    }
}
