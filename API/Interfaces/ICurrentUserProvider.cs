using Users.Identity.Classes;

namespace API.Interfaces
{
    public interface ICurrentUserProvider
    {
        Task<User?> GetCurrentUser();
    }
}
