using System;
using PrivateChat.Common;

namespace PrivateChat.Models;

// Entity: a person who has an account in PrivateChat (a normal user or an admin).
public class User
{
    // Static attributes: the only two valid roles. One shared copy for the whole class.
    public static readonly string roleUser = "User";
    public static readonly string roleAdmin = "Admin";

    // Data (attributes). They are private, so only this class can change them directly.
    private string username;
    private string passwordHash;
    private string displayName;
    private string role;
    private bool isBanned;
    private long registeredTicks;

    // Default constructor: creates a normal, active user with empty text. The setters fill it in later.
    public User()
    {
        username = "";
        passwordHash = "";
        displayName = "";
        role = roleUser;
        isBanned = false;
        registeredTicks = DateTime.Now.Ticks;
    }

    // Parameterized constructor: used when we load an already saved user from the file.
    public User(string username, string passwordHash, string displayName, string role, bool isBanned, long registeredTicks)
    {
        this.username = username;
        this.passwordHash = passwordHash;
        this.displayName = displayName;
        this.role = role;
        this.isBanned = isBanned;
        this.registeredTicks = registeredTicks;
    }

    // Static method: needs no User object. Makes a username tidy: "  Ali_01 " becomes "ali_01".
    public static string cleanName(string name)
    {
        if (name == null)
        {
            return "";
        }
        return name.Trim().ToLower();
    }

    // Getter
    public string getUsername()
    {
        return username;
    }

    // Setter with validation: 3 to 20 characters, only letters, digits and underscore.
    // Returns true when the username was accepted, false when it was rejected.
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

    // Getter for the saved (scrambled) password. The real password is never stored.
    public string getPasswordHash()
    {
        return passwordHash;
    }

    // Setter with validation: the password must have at least 6 characters.
    // We store only the hash of it.
    public bool setPassword(string plainPassword)
    {
        if (plainPassword == null || plainPassword.Length < 6)
        {
            return false;
        }
        passwordHash = PasswordHelper.Hash(plainPassword);
        return true;
    }

    // Behaviour: is this the right password for this user?
    public bool checkPassword(string plainPassword)
    {
        if (plainPassword == null)
        {
            return false;
        }
        return passwordHash == PasswordHelper.Hash(plainPassword);
    }

    // Getter. If no display name was saved, the username is shown instead.
    public string getDisplayName()
    {
        if (displayName == "")
        {
            return username;
        }
        return displayName;
    }

    // Setter with validation: 1 to 30 characters and no "|" (that character separates the fields in the file).
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

    // Setter with validation: only "User" or "Admin" is allowed.
    public bool setRole(string role)
    {
        if (role != roleUser && role != roleAdmin)
        {
            return false;
        }
        this.role = role;
        return true;
    }

    // Behaviour: is this user an administrator?
    public bool isAdmin()
    {
        return role == roleAdmin;
    }

    public bool getIsBanned()
    {
        return isBanned;
    }

    // Behaviours: an admin bans or unbans an account. A banned user cannot log in or chat.
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

    // One line of text for the file: username|passwordHash|displayName|role|isBanned|registeredTicks
    // The saved display name is used here (not the username fallback), so a missing name stays missing.
    public override string ToString()
    {
        return username + "|" + passwordHash + "|" + displayName + "|" + role + "|" + isBanned + "|" + registeredTicks;
    }
}
