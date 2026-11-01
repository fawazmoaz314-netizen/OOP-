using System;
using System.Security.Cryptography;
using System.Text;

namespace PrivateChat.Common;

// A static helper class: we never create an object of it, we call PasswordHelper.Hash(...) directly.
public static class PasswordHelper
{
    // Turns a password into a scrambled text (a hash) so the real password is never saved in a file.
    public static string Hash(string password)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }
}
