using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(string userId);
    }
}
