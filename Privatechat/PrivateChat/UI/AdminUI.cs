using System;
using System.Collections.Generic;
using PrivateChat.Models;
using PrivateChat.Services;

namespace PrivateChat.UI;

// Presentation Layer: the Admin Panel screens. It contains NO rules: AdminService decides who may do what.
public class AdminUI
{
    private readonly AdminService _adminService;

    public AdminUI()
    {
        _adminService = new AdminService();
    }

    // The Admin Panel menu. Runs until the admin chooses Back.
    public void ShowAdminMenu(string adminName)
    {
        bool keepRunning = true;
        while (keepRunning)
        {
            Console.Clear();
            Console.WriteLine("=== Admin Panel === (" + adminName + ")");
            Console.WriteLine("1. List, search and sort users");
            Console.WriteLine("2. Create a user");
            Console.WriteLine("3. Delete a user");
            Console.WriteLine("4. Ban a user");
            Console.WriteLine("5. Unban a user");
            Console.WriteLine("6. Back");
            Console.Write("Select option: ");
            string choice = ReadText();

            if (choice == "1")
            {
                ListUsersUI(adminName);
            }
            else if (choice == "2")
            {
                CreateUserUI(adminName);
            }
            else if (choice == "3")
            {
                DeleteUserUI(adminName);
            }
            else if (choice == "4")
            {
                BanUI(adminName, true);
            }
            else if (choice == "5")
            {
                BanUI(adminName, false);
            }
            else if (choice == "6")
            {
                keepRunning = false;
            }
            else
            {
                Console.WriteLine("Invalid option!");
                Pause();
            }
        }
    }

    private void ListUsersUI(string adminName)
    {
        Console.Clear();
        Console.WriteLine("=== Users ===");
        Console.Write("Search text (press Enter to show everybody): ");
        string searchText = ReadText();
        Console.WriteLine("Sort by: 1. Username   2. Name   3. Registration date");
        Console.Write("Select (Enter = username): ");
        string sortChoice = ReadText();

        string sortBy = "username";
        if (sortChoice == "2")
        {
            sortBy = "name";
        }
        else if (sortChoice == "3")
        {
            sortBy = "date";
        }

        List<User> users = _adminService.SearchUsers(adminName, searchText, sortBy);
        if (users == null)
        {
            Console.WriteLine("Access denied: only an administrator can see this list.");
        }
        else if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
        }
        else
        {
            Console.WriteLine(users.Count + " user(s):");
            foreach (User user in users)
            {
                string status = "Active";
                if (user.getIsBanned())
                {
                    status = "BANNED";
                }
                Console.WriteLine("- " + user.getUsername() + " | " + user.getDisplayName() + " | " + user.getRole()
                    + " | " + status + " | registered " + user.getRegisteredTime().ToString("dd MMM yyyy"));
            }
        }
        Pause();
    }

    private void CreateUserUI(string adminName)
    {
        Console.Clear();
        Console.WriteLine("=== Create a user ===");
        Console.Write("Username: ");
        string username = ReadText();
        Console.Write("Display name (Enter = same as username): ");
        string displayName = ReadText();
        Console.Write("Password: ");
        string password = ReadText();
        Console.Write("Role (User or Admin, Enter = User): ");
        string role = ReadText().Trim();
        if (role == "")
        {
            role = User.roleUser;
        }
        else if (role.ToLower() == "admin")
        {
            role = User.roleAdmin;
        }
        else if (role.ToLower() == "user")
        {
            role = User.roleUser;
        }

        string error = _adminService.CreateUser(adminName, username, displayName, password, role);
        if (error == "")
        {
            Console.WriteLine("User created.");
        }
        else
        {
            Console.WriteLine("Error: " + error);
        }
        Pause();
    }

    private void DeleteUserUI(string adminName)
    {
        Console.Clear();
        Console.WriteLine("=== Delete a user ===");
        Console.Write("Username to delete: ");
        string target = ReadText();
        Console.WriteLine("WARNING: this deletes the account and ALL its chats and messages (the other people lose those chats too).");
        Console.Write("Type YES to confirm: ");
        string answer = ReadText();
        if (answer != "YES")
        {
            Console.WriteLine("Cancelled. Nothing was deleted.");
            Pause();
            return;
        }

        string error = _adminService.DeleteUser(adminName, target);
        if (error == "")
        {
            Console.WriteLine("User deleted.");
        }
        else
        {
            Console.WriteLine("Error: " + error);
        }
        Pause();
    }

    // ban = true bans the user, ban = false removes the ban.
    private void BanUI(string adminName, bool ban)
    {
        Console.Clear();
        if (ban)
        {
            Console.WriteLine("=== Ban a user ===");
        }
        else
        {
            Console.WriteLine("=== Unban a user ===");
        }
        Console.Write("Username: ");
        string target = ReadText();

        string error;
        if (ban)
        {
            error = _adminService.BanUser(adminName, target);
        }
        else
        {
            error = _adminService.UnbanUser(adminName, target);
        }

        if (error == "")
        {
            if (ban)
            {
                Console.WriteLine("User banned.");
            }
            else
            {
                Console.WriteLine("User unbanned.");
            }
        }
        else
        {
            Console.WriteLine("Error: " + error);
        }
        Pause();
    }

    // Reads one line from the keyboard. ReadLine gives null if the input is closed, so we return "" then.
    private string ReadText()
    {
        string input = Console.ReadLine();
        if (input == null)
        {
            return "";
        }
        return input;
    }

    // Waits so the admin can read the result before the screen is cleared.
    private void Pause()
    {
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}
