using System.Collections.Generic;
using PrivateChat.Models;
using PrivateChat.Repositories;

namespace PrivateChat.Services;

// Business Logic Layer: the rules for accounts (register, login, change password, first admin).
public class AuthService
{
    private readonly UserRepository _userRepository;
    private string _lastError;   // the reason the last Login failed

    public AuthService()
    {
        _userRepository = new UserRepository();
        _lastError = "";
    }

    // The reason the last Login call failed (empty when it worked).
    public string getLastError()
    {
        return _lastError;
    }

    // Self-registration. The new account is ALWAYS a normal user: the caller cannot choose the role.
    // Returns "" when everything is fine, otherwise the reason it failed.
    public string Register(string username, string password, string confirmPassword)
    {
        return CreateAccount(username, password, confirmPassword, User.roleUser);
    }

    // Creates the very first administrator. It only works while there is no admin yet.
    // Returns "" when fine, otherwise the reason it failed.
    public string CreateFirstAdmin(string username, string password, string confirmPassword)
    {
        if (AdminExists())
        {
            return "An admin account already exists.";
        }
        return CreateAccount(username, password, confirmPassword, User.roleAdmin);
    }

    // Is there at least one administrator in the file?
    public bool AdminExists()
    {
        List<User> users = _userRepository.GetAll();
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].isAdmin())
            {
                return true;
            }
        }
        return false;
    }

    // Returns the User when the username and password are right and the account is not banned, otherwise null.
    // When null is returned, getLastError() tells the reason.
    public User Login(string username, string password)
    {
        _lastError = "";
        List<User> users = _userRepository.GetAll();
        User user = FindUserByName(users, User.cleanName(username));
        if (user == null || !user.checkPassword(password))
        {
            _lastError = "Wrong username or password.";
            return null;
        }
        if (user.getIsBanned())
        {
            // Only shown after the password was right, so strangers cannot probe which accounts exist.
            _lastError = "This account is banned. Please contact an administrator.";
            return null;
        }
        return user;
    }

    // Does an account with this username exist?
    public bool UserExists(string username)
    {
        List<User> users = _userRepository.GetAll();
        return FindUserByName(users, User.cleanName(username)) != null;
    }

    // Changes the password. Returns "" when fine, otherwise the reason it failed.
    public string ChangePassword(string username, string oldPassword, string newPassword, string confirmPassword)
    {
        List<User> users = _userRepository.GetAll();
        User user = FindUserByName(users, User.cleanName(username));
        if (user == null)
        {
            return "Account was not found.";
        }
        if (user.getIsBanned())
        {
            return "This account is banned.";
        }
        if (!user.checkPassword(oldPassword))
        {
            return "The old password is wrong.";
        }
        if (newPassword != confirmPassword)
        {
            return "The two new passwords are different.";
        }
        if (!user.setPassword(newPassword))
        {
            return "Password must be at least 6 characters.";
        }

        _userRepository.SaveAll(users);
        return "";
    }

    // Shared by Register and CreateFirstAdmin. Returns "" when fine, otherwise the reason.
    private string CreateAccount(string username, string password, string confirmPassword, string role)
    {
        User user = new User();
        if (!user.setUsername(username))
        {
            return "Username must be 3 to 20 characters: letters, numbers or underscore only.";
        }
        if (password != confirmPassword)
        {
            return "The two passwords are different.";
        }
        if (!user.setPassword(password))
        {
            return "Password must be at least 6 characters.";
        }
        user.setRole(role);

        List<User> users = _userRepository.GetAll();
        if (FindUserByName(users, user.getUsername()) != null)
        {
            return "That username is already taken.";
        }

        _userRepository.Save(user);
        return "";
    }

    private User FindUserByName(List<User> users, string username)
    {
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].getUsername() == username)
            {
                return users[i];
            }
        }
        return null;
    }
}
