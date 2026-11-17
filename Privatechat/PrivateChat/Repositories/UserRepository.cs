using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using PrivateChat.Common;
using PrivateChat.Models;

namespace PrivateChat.Repositories;

public class UserRepository
{
    public void Save(User user)
    {
        using (SqlConnection con = Database.CreateConnection())
        {
            con.Open();
            string sql = "INSERT INTO Users (Username, PasswordHash, DisplayName, Role, IsBanned, RegisteredTicks) " +
                         "VALUES (@u, @p, @d, @r, @b, @t)";
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@u", user.getUsername());
                cmd.Parameters.AddWithValue("@p", user.getPasswordHash());
                cmd.Parameters.AddWithValue("@d", user.getDisplayName());
                cmd.Parameters.AddWithValue("@r", user.getRole());
                cmd.Parameters.AddWithValue("@b", user.getIsBanned());
                cmd.Parameters.AddWithValue("@t", user.getRegisteredTicks());
                cmd.ExecuteNonQuery();
            }
        }
    }

    public List<User> GetAll()
    {
        List<User> users = new List<User>();
        using (SqlConnection con = Database.CreateConnection())
        {
            con.Open();
            string sql = "SELECT Username, PasswordHash, DisplayName, Role, IsBanned, RegisteredTicks FROM Users";
            using (SqlCommand cmd = new SqlCommand(sql, con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    User user = new User(
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetBoolean(4),
                        reader.GetInt64(5));
                    users.Add(user);
                }
            }
        }
        return users;
    }

    // Keeps the old name so AuthService/AdminService need no change.
    // Rewrites every user in one transaction.
    public void SaveAll(List<User> users)
    {
        using (SqlConnection con = Database.CreateConnection())
        {
            con.Open();
            using (SqlTransaction tx = con.BeginTransaction())
            {
                // Conversations point at Users, so a plain DELETE would fail if chats exist.
                // See the note below: for SaveAll prefer UPDATE per user.
                foreach (User user in users)
                {
                    string sql = "UPDATE Users SET PasswordHash=@p, DisplayName=@d, Role=@r, IsBanned=@b WHERE Username=@u";
                    using (SqlCommand cmd = new SqlCommand(sql, con, tx))
                    {
                        cmd.Parameters.AddWithValue("@p", user.getPasswordHash());
                        cmd.Parameters.AddWithValue("@d", user.getDisplayName());
                        cmd.Parameters.AddWithValue("@r", user.getRole());
                        cmd.Parameters.AddWithValue("@b", user.getIsBanned());
                        cmd.Parameters.AddWithValue("@u", user.getUsername());
                        cmd.ExecuteNonQuery();
                    }
                }
                tx.Commit();
            }
        }
    }
}