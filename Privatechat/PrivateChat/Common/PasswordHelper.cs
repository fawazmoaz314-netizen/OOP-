using System;
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
