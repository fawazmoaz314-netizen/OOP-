namespace PrivateChat.Models;

public class SearchResult
{
    private string otherUsername;
    private Message message;

    public SearchResult(string otherUsername, Message message)
    {
        this.otherUsername = otherUsername;
        this.message = message;
    }

    
    public string getOtherUsername()
    {
        return otherUsername;
    }

    public Message getMessage()
    {
        return message;
    }
}
