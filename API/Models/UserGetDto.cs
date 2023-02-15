namespace API.Models;

#nullable disable
public class UserGetDto
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public int CoinBalance { get; set; }
    public string Description { get; set; }
}