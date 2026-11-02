using PrivateChat.Models;
using PrivateChat.Services;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PrivateChat.Common;

public static class PasswordHelper
{
    
    public static string Hash(string password)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }
}
//List<X> result = new List<X>();
//for (int i = 0; i<all.Count; i++)
//{
//    if (CONDITION ) { result.Add(all[i]); }
//}
//return result;


//private void FilterXUI(string adminName)
//    {
//        Console.Clear();
//        Console.Write("Input: ");
//        string input = ReadText();
//        List<User> users = _adminService.GetXxx(adminName, input);
//        if (users == null) { Console.WriteLine("Access denied."); }
//        else if (users.Count == 0) { Console.WriteLine("No users match."); }
//        else
//        {
//            foreach (User user in users)
//            {
//                Console.WriteLine("- " + user.getUsername() + " | " + user.getRole());
//            }
//        }
//        Pause();
//    //}