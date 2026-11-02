using System;
using System.Collections.Generic;
using PrivateChat.Models;
using PrivateChat.Services;

namespace PrivateChat.UI;


public class ChatUI
{
    private readonly AuthService _authService;
    private readonly ChatService _chatService;
    private readonly AdminUI _adminUI;
    private string _currentUsername;   
    private bool _currentIsAdmin;     

    public ChatUI()
    {
        _authService = new AuthService();
        _chatService = new ChatService();
        _adminUI = new AdminUI();
        _currentUsername = "";
        _currentIsAdmin = false;
    }


    public void ShowMainMenu()
    {
        SetupFirstAdminUI();

        bool keepRunning = true;
        while (keepRunning)
        {
            if (_currentUsername == "")
            {
                keepRunning = ShowGuestMenu();
            }
            else
            {
                keepRunning = ShowUserMenu();
            }
        }
        Console.WriteLine("Goodbye!");
    }

  
    private bool ShowGuestMenu()
    {
        Console.Clear();
        Console.WriteLine("=== Private Chat ===");
        Console.WriteLine("1. Register");
        Console.WriteLine("2. Login");
        Console.WriteLine("3. Exit");
        Console.Write("Select option: ");
        string choice = ReadText();

        if (choice == "1")
        {
            RegisterUI();
        }
        else if (choice == "2")
        {
            LoginUI();
        }
        else if (choice == "3")
        {
            return false;
        }
        else
        {
            Console.WriteLine("Invalid option!");
            Pause();
        }
        return true;
    }

    
    private bool ShowUserMenu()
    {
        Console.Clear();
        Console.WriteLine("=== Private Chat === (logged in as " + _currentUsername + ")");
        Console.WriteLine("1. Send a message");
        Console.WriteLine("2. Open a chat");
        Console.WriteLine("3. My chats");
        Console.WriteLine("4. Search my messages");
        Console.WriteLine("5. Hide a chat");
        Console.WriteLine("6. Hidden chats (needs your password)");
        Console.WriteLine("7. Change password");
        Console.WriteLine("8. Logout");
        Console.WriteLine("9. Exit");
        if (_currentIsAdmin)
        {
            Console.WriteLine("0. Admin Panel");
        }
        Console.Write("Select option: ");
        string choice = ReadText();

        if (choice == "1")
        {
            SendMessageUI();
        }
        else if (choice == "2")
        {
            OpenChatUI();
        }
        else if (choice == "3")
        {
            MyChatsUI();
        }
        else if (choice == "4")
        {
            SearchMessagesUI();
        }
        else if (choice == "5")
        {
            HideChatUI();
        }
        else if (choice == "6")
        {
            HiddenChatsUI();
        }
        else if (choice == "7")
        {
            ChangePasswordUI();
        }
        else if (choice == "8")
        {
            LogoutUI();
        }
        else if (choice == "9")
        {
            return false;
        }
        else if (choice == "0" && _currentIsAdmin)
        {
            _adminUI.ShowAdminMenu(_currentUsername);
        }
        else
        {
            Console.WriteLine("Invalid option!");
            Pause();
        }
        return true;
    }

   
    private void SetupFirstAdminUI()
    {
        if (_authService.AdminExists())
        {
            return;
        }

        Console.Clear();
        Console.WriteLine("=== First-time setup ===");
        Console.WriteLine("There is no administrator yet. Create the administrator account now.");
        Console.WriteLine("(Press Enter without a username to skip. The setup is then shown again at the next start.)");
        string error = "x";
        while (error != "")
        {
            Console.Write("Admin username: ");
            string username = ReadText();
            if (username.Trim().Length == 0)
            {
                return;
            }
            Console.Write("Admin password: ");
            string password = ReadText();
            Console.Write("Repeat the password: ");
            string confirmPassword = ReadText();

            error = _authService.CreateFirstAdmin(username, password, confirmPassword);
            if (error != "")
            {
                Console.WriteLine("Error: " + error);
            }
        }
        Console.WriteLine("Administrator account created.");
        Pause();
    }

    private void RegisterUI()
    {
        Console.Clear();
        Console.WriteLine("=== Register ===");
        Console.Write("Choose a username: ");
        string username = ReadText();
        Console.Write("Choose a password: ");
        string password = ReadText();
        Console.Write("Repeat the password: ");
        string confirmPassword = ReadText();

        string error = _authService.Register(username, password, confirmPassword);
        if (error == "")
        {
            Console.WriteLine("Account created. Please log in.");
        }
        else
        {
            Console.WriteLine("Error: " + error);
        }
        Pause();
    }

    private void LoginUI()
    {
        Console.Clear();
        Console.WriteLine("=== Login ===");
        Console.Write("Username: ");
        string username = ReadText();
        Console.Write("Password: ");
        string password = ReadText();

        User user = _authService.Login(username, password);
        if (user == null)
        {
            Console.WriteLine(_authService.getLastError());
        }
        else
        {
            _currentUsername = user.getUsername();
            _currentIsAdmin = user.isAdmin();
            Console.WriteLine("Welcome, " + _currentUsername + "!");
        }
        Pause();
    }

    private void SendMessageUI()
    {
        Console.Clear();
        Console.WriteLine("=== Send a message ===");
        Console.Write("Send to (username): ");
        string receiver = ReadText();
        Console.Write("Message: ");
        string text = ReadText();

        string error = _chatService.SendMessage(_currentUsername, receiver, text);
        if (error == "")
        {
            Console.WriteLine("Message sent.");
        }
        else
        {
            Console.WriteLine("Error: " + error);
        }
        Pause();
    }

    private void OpenChatUI()
    {
        Console.Clear();
        Console.WriteLine("=== Open a chat ===");
        Console.Write("Chat with (username): ");
        string other = ReadText();

        Conversation conversation = _chatService.OpenConversation(_currentUsername, other);
        if (conversation == null)
        {
            Console.WriteLine("You have no messages with that user yet.");
            Pause();
            return;
        }

        for (int i = 0; i < conversation.getMessageCount(); i++)
        {
            Message message = conversation.getMessageAt(i);
            string who = message.getSender();
            if (who == _currentUsername)
            {
                who = "You";
            }
            string time = message.getSentTime().ToString("dd MMM HH:mm");
            Console.WriteLine("[" + time + "] " + who + ": " + message.getText());
        }
        Pause();
    }

    private void MyChatsUI()
    {
        Console.Clear();
        Console.WriteLine("=== My chats ===");
        List<Conversation> conversations = _chatService.GetMyConversations(_currentUsername, false);
        PrintChatList(conversations);
        Pause();
    }

    private void SearchMessagesUI()
    {
        Console.Clear();
        Console.WriteLine("=== Search my messages ===");
        Console.Write("Search for: ");
        string searchText = ReadText();
        if (searchText.Trim().Length == 0)
        {
            Console.WriteLine("Please type something to search for.");
            Pause();
            return;
        }

        List<SearchResult> results = _chatService.SearchMessages(_currentUsername, searchText);
        if (results.Count == 0)
        {
            Console.WriteLine("No messages found for '" + searchText.Trim() + "'.");
        }
        else
        {
            Console.WriteLine(results.Count + " message(s) found:");
            foreach (SearchResult result in results)
            {
                Message message = result.getMessage();
                string who = message.getSender();
                if (who == _currentUsername)
                {
                    who = "You";
                }
                string time = message.getSentTime().ToString("dd MMM HH:mm");
                Console.WriteLine("[chat with " + result.getOtherUsername() + "] [" + time + "] " + who + ": " + message.getText());
            }
        }
        Pause();
    }

    private void HideChatUI()
    {
        Console.Clear();
        Console.WriteLine("=== Hide a chat ===");
        Console.Write("Hide the chat with (username): ");
        string other = ReadText();

        string error = _chatService.SetChatHidden(_currentUsername, other, true);
        if (error == "")
        {
            Console.WriteLine("Chat hidden. You can find it under 'Hidden chats'.");
        }
        else
        {
            Console.WriteLine("Error: " + error);
        }
        Pause();
    }

    private void HiddenChatsUI()
    {
        Console.Clear();
        Console.WriteLine("=== Hidden chats ===");
        Console.Write("Enter your password to continue: ");
        string password = ReadText();
        if (_authService.Login(_currentUsername, password) == null)
        {
            Console.WriteLine("Wrong password.");
            Pause();
            return;
        }

        List<Conversation> conversations = _chatService.GetMyConversations(_currentUsername, true);
        PrintChatList(conversations);

        Console.Write("Type a username to unhide, or press Enter to go back: ");
        string other = ReadText();
        if (other.Trim().Length > 0)
        {
            string error = _chatService.SetChatHidden(_currentUsername, other, false);
            if (error == "")
            {
                Console.WriteLine("The chat is visible again.");
            }
            else
            {
                Console.WriteLine("Error: " + error);
            }
            Pause();
        }
    }

    private void ChangePasswordUI()
    {
        Console.Clear();
        Console.WriteLine("=== Change password ===");
        Console.Write("Old password: ");
        string oldPassword = ReadText();
        Console.Write("New password: ");
        string newPassword = ReadText();
        Console.Write("Repeat the new password: ");
        string confirmPassword = ReadText();

        string error = _authService.ChangePassword(_currentUsername, oldPassword, newPassword, confirmPassword);
        if (error == "")
        {
            Console.WriteLine("Password changed.");
        }
        else
        {
            Console.WriteLine("Error: " + error);
        }
        Pause();
    }

    private void LogoutUI()
    {
        _currentUsername = "";
        _currentIsAdmin = false;
        Console.WriteLine("You are logged out.");
        Pause();
    }

  
    private void PrintChatList(List<Conversation> conversations)
    {
        if (conversations.Count == 0)
        {
            Console.WriteLine("No chats here yet.");
            return;
        }

        foreach (Conversation conversation in conversations)
        {
            User other = conversation.getOtherUser(_currentUsername);
            int unread = conversation.countUnreadFor(_currentUsername);
            Message last = conversation.getLastMessage();

            string line = "- " + other.getUsername();
            if (unread > 0)
            {
                line = line + " (" + unread + " new)";
            }
            if (last != null)
            {
                line = line + ": " + Shorten(last.getText(), 30);
            }
            Console.WriteLine(line);
        }
    }

   
    private string Shorten(string text, int maxLength)
    {
        if (text.Length <= maxLength)
        {
            return text;
        }
        return text.Substring(0, maxLength) + "...";
    }

   
    private string ReadText()
    {
        string input = Console.ReadLine();
        if (input == null)
        {
            return "";
        }
        return input;
    }

  
    private void Pause()
    {
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}
