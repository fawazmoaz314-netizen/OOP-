using System.Collections.Generic;
using PrivateChat.Models;
using PrivateChat.Repositories;

namespace PrivateChat.Services;


public class AuthService
{
    private readonly UserRepository _userRepository;
    private string _lastError;   

    public AuthService()
    {
        _userRepository = new UserRepository();
        _lastError = "";
    }

  
    public string getLastError()
    {
        return _lastError;
    }


    public string Register(string username, string password, string confirmPassword)
    {
        return CreateAccount(username, password, confirmPassword, User.roleUser);
    }

    
    public string CreateFirstAdmin(string username, string password, string confirmPassword)
    {
        if (AdminExists())
        {
            return "An admin account already exists.";
        }
        return CreateAccount(username, password, confirmPassword, User.roleAdmin);
    }

  
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
           
            _lastError = "This account is banned. Please contact an administrator.";
            return null;
        }
        return user;
    }

   
    public bool UserExists(string username)
    {
        List<User> users = _userRepository.GetAll();
        return FindUserByName(users, User.cleanName(username)) != null;
    }

    
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
