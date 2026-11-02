using PrivateChat.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivateChat;


class Program
{
static void Main(string[] args)
    {
        // Console title
        Console.Title = "🥷PrivateChat |🔏Secure Messaging";

        // Console colours
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Cyan;

        // Clear console with selected background
        Console.Clear();

        // Enable UTF-8 for emojis and special characters
        Console.OutputEncoding = new UTF8Encoding(false);
        Console.InputEncoding = Encoding.UTF8;

        // Initialize Chat UI
        ChatUI ui = new ChatUI();
        ui.ShowMainMenu();

        // Reset colours
        Console.ResetColor();
    }
}
    

