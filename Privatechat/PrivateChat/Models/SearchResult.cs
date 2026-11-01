namespace PrivateChat.Models;

// A small helper entity: one message found by a search, together with the person I chat with.
// It only holds data. The search rules are in ChatService.
public class SearchResult
{
    private string otherUsername;
    private Message message;

    public SearchResult(string otherUsername, Message message)
    {
        this.otherUsername = otherUsername;
        this.message = message;
    }

    // The chat partner of the logged-in user (not the logged-in user).
    public string getOtherUsername()
    {
        return otherUsername;
    }

    public Message getMessage()
    {
        return message;
    }
}
