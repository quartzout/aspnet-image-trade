using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UserManager;

public class LoginManager
{
    private readonly UserDbContext _db;
    private readonly IPasswordHasher _hasher;

    //A DbContext instance is designed to be used for a single unit-of-work.
    //This means that the lifetime of a DbContext instance is usually very short.

    public LoginManager(UserDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public LoginManagerResult Register(UserRegisterInfo newUser)
    {
        var usersWithThisEmail = _db.Users.Where(user => user.Email == newUser.Email);

        if (usersWithThisEmail.Count() != 0)
            return new(error: "User with this email already exists");

        if (newUser.Password.Length < 6)
            return new(error: "Password must contain at least 6 characters");

        UserRecord userRecordToAdd = new()
        {
            Email = newUser.Email,
            Name = newUser.Name,
            HashedPassword = _hasher.HashPassword(newUser.Password)
        };

        UserRecord addedUser = _db.Users.Add(userRecordToAdd).Entity;

        _db.SaveChanges();

        UserLoginInfo userToLogin = new()
        {
            Email = addedUser.Email,
            Password = addedUser.HashedPassword
        };

        Login(userToLogin);

        return new(addedUser.Id);
    }

    public LoginManagerResult Login(UserLoginInfo user)
    {
        var usersWithThisEmail = _db.Users.Where(user => user.Email == user.Email);

        if (usersWithThisEmail.Count() == 0)
            return new(error: "User with this email doesnt exist");

        if (usersWithThisEmail.Count() > 1)
            return new(error: "There are multiple users with this email (something went wrong on our database");

        var UserRecordToCheck = usersWithThisEmail.First();

        var result = _hasher.VerifyHashedPassword(UserRecordToCheck.HashedPassword, user.Password);

        if (result is Microsoft.AspNet.Identity.PasswordVerificationResult.Failed)
            return new(error: "Wrong password");

        //result could be SuccessRehashNeeded

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Email, UserRecordToCheck.Email),
            new Claim(ClaimTypes.Name, UserRecordToCheck.Name)
        };

        ClaimsIdentity identity = new(claims, "Cookies");

        HttpContext.

        
            

    }
}
