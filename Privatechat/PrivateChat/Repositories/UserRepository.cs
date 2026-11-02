using System;
using System.Collections.Generic;
using System.IO;
using PrivateChat.Models;

namespace PrivateChat.Repositories;


public class UserRepository
{
    private readonly string _filePath = "users.txt";

   
    public void Save(User user)
    {
        using (StreamWriter writer = new StreamWriter(_filePath, true))
        {
            writer.WriteLine(user.ToString());
        }
    }

   
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
               
                users.Add(new User(parts[0], parts[1], "", User.roleUser, false, DateTime.Now.Ticks));
            }
            else if (parts.Length == 6)
            {
                
                users.Add(new User(parts[0], parts[1], parts[2], parts[3], bool.Parse(parts[4]), long.Parse(parts[5])));
            }
           
        }
        return users;
    }
}
