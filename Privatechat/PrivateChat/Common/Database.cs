using Microsoft.Data.SqlClient;

namespace PrivateChat.Common;

public static class Database
{
    // Windows login to local SQL Server. Change the server name if yours is different.
    private static readonly string connectionString =
        "Server=PHANTOM\\SQLEXPRESS;Database=PrivateChatDb;Trusted_Connection=True;TrustServerCertificate=True;";

    public static SqlConnection CreateConnection()
    {
        return new SqlConnection(connectionString);
    }
}