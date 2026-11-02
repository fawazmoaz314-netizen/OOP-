using PrivateChat.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivateChat;


class Program
{
static void Main(string[] args)
    {
        
        Console.Title = "🥷PrivateChat |🔏Secure Messaging";

      
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Cyan;

        
        Console.Clear();

        
        Console.OutputEncoding = new UTF8Encoding(false);
        Console.InputEncoding = Encoding.UTF8;

     
        ChatUI ui = new ChatUI();
        ui.ShowMainMenu();

       
        Console.ResetColor();
    }
}
    

