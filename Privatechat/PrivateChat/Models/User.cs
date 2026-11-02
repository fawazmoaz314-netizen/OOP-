using System;
using PrivateChat.Common;

namespace PrivateChat.Models;


public class User
{
   
    public static readonly string roleUser = "User";
    public static readonly string roleAdmin = "Admin";

   
    private string username;
    private string passwordHash;
    private string displayName;
    private string role;
    private bool isBanned;
    private long registeredTicks;

    
    public User()
    {
        username = "";
        passwordHash = "";
        displayName = "";
        role = roleUser;
        isBanned = false;
        registeredTicks = DateTime.Now.Ticks;
    }

   
    public User(string username, string passwordHash, string displayName, string role, bool isBanned, long registeredTicks)
    {
        this.username = username;
        this.passwordHash = passwordHash;
        this.displayName = displayName;
        this.role = role;
        this.isBanned = isBanned;
        this.registeredTicks = registeredTicks;
    }

   
    public static string cleanName(string name)
    {
        if (name == null)
        {
            return "";
        }
        return name.Trim().ToLower();
    }

   
    public string getUsername()
    {
        return username;
    }

   
    public bool setUsername(string username)
    {
        string cleaned = cleanName(username);
        if (cleaned.Length < 3 || cleaned.Length > 20)
        {
            return false;
        }
        for (int i = 0; i < cleaned.Length; i++)
        {
            char c = cleaned[i];
            bool isAllowed = char.IsLetterOrDigit(c) || c == '_';
            if (!isAllowed)
            {
                return false;
            }
        }
        this.username = cleaned;
        return true;
    }

    
    public string getPasswordHash()
    {
        return passwordHash;
    }

    
    public bool setPassword(string plainPassword)
    {
        if (plainPassword == null || plainPassword.Length < 6)
        {
            return false;
        }
        passwordHash = PasswordHelper.Hash(plainPassword);
        return true;
    }

    
    public bool checkPassword(string plainPassword)
    {
        if (plainPassword == null)
        {
            return false;
        }
        return passwordHash == PasswordHelper.Hash(plainPassword);
    }

  
    public string getDisplayName()
    {
        if (displayName == "")
        {
            return username;
        }
        return displayName;
    }

   
    public bool setDisplayName(string displayName)
    {
        if (displayName == null)
        {
            return false;
        }
        string cleaned = displayName.Trim();
        if (cleaned.Length < 1 || cleaned.Length > 30)
        {
            return false;
        }
        if (cleaned.Contains("|"))
        {
            return false;
        }
        this.displayName = cleaned;
        return true;
    }

    public string getRole()
    {
        return role;
    }

   
    public bool setRole(string role)
    {
        if (role != roleUser && role != roleAdmin)
        {
            return false;
        }
        this.role = role;
        return true;
    }

 
    public bool isAdmin()
    {
        return role == roleAdmin;
    }

    public bool getIsBanned()
    {
        return isBanned;
    }

    
    public void ban()
    {
        isBanned = true;
    }

    public void unban()
    {
        isBanned = false;
    }

    public long getRegisteredTicks()
    {
        return registeredTicks;
    }

    public DateTime getRegisteredTime()
    {
        return new DateTime(registeredTicks);
    }

    public override string ToString()
    {
        return username + "|" + passwordHash + "|" + displayName + "|" + role + "|" + isBanned + "|" + registeredTicks;
    }
}
