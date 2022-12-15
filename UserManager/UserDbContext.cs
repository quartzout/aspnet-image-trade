using Microsoft.EntityFrameworkCore;
using UserManager;

public class UserDbContext : DbContext
{
	public UserDbContext()
	{

	}

	public DbSet<UserRecord> Users;
}