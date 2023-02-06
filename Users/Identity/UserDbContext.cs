using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Users.Identity.Classes;

public class UserDbContext : IdentityDbContext<User>
{
	public UserDbContext(DbContextOptions<UserDbContext> opts) : base(opts) { }
}
