using System;
using System.Collections.Generic;
using System.IO;
using PrivateChat.Models;

namespace PrivateChat.Repositories;

// Data Access Layer: the only class that reads and writes users.txt.
public class UserRepository
{
    private readonly string _filePath = "users.txt";

    // Adds ONE user at the end of the file.
    public void Save(User user)
    {
        using (StreamWriter writer = new StreamWriter(_filePath, true))
        {
            writer.WriteLine(user.ToString());
        }
    }

    // Replaces the whole file with this list (used after a change such as a new password, ban or delete).
    public void SaveAll(List<User> users)
    {
        using (StreamWriter writer = new StreamWriter(_filePath, false))
        {
            foreach (User user in users)
            {
                writer.WriteLine(user.ToString());
            }
        }
    }

    // Reads the file and rebuilds the User objects. An empty list is returned if there is no file yet.
    public List<User> GetAll()
    {
        List<User> users = new List<User>();
        if (!File.Exists(_filePath))
        {
            return users;
        }

        string[] lines = File.ReadAllLines(_filePath);
        foreach (string line in lines)
        {
            if (line.Trim().Length == 0)
            {
                continue;
            }
            string[] parts = line.Split('|');
            if (parts.Length == 2)
            {
                // Old file format (before roles existed): username|passwordHash. Treat it as a normal, active user.
                users.Add(new User(parts[0], parts[1], "", User.roleUser, false, DateTime.Now.Ticks));
            }
            else if (parts.Length == 6)
            {
                // New format: username|passwordHash|displayName|role|isBanned|registeredTicks
                users.Add(new User(parts[0], parts[1], parts[2], parts[3], bool.Parse(parts[4]), long.Parse(parts[5])));
            }
            // Any other line is damaged and is ignored.
        }
        return users;
    }
}
