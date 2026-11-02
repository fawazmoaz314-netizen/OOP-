using System;

namespace PrivateChat.Models;


public class Message
{
    public static readonly int maxLength = 500;

    private string sender;
    private string text;
    private long sentTicks;  
    private bool isRead;

   
    public Message()
    {
        sender = "";
        text = "";
        sentTicks = DateTime.Now.Ticks;
        isRead = false;
    }

  
    public Message(string sender, string text, long sentTicks, bool isRead)
    {
        this.sender = sender;
        this.text = text;
        this.sentTicks = sentTicks;
        this.isRead = isRead;
    }

    public string getSender()
    {
        return sender;
    }

    public bool setSender(string sender)
    {
        if (sender == null || sender.Trim().Length == 0)
        {
            return false;
        }
        this.sender = sender;
        return true;
    }

    public string getText()
    {
        return text;
    }

    
    public bool setText(string text)
    {
        if (text == null)
        {
            return false;
        }
        string cleaned = text.Trim();
        if (cleaned.Length == 0 || cleaned.Length > maxLength)
        {
            return false;
        }
        this.text = cleaned;
        return true;
    }

    public long getSentTicks()
    {
        return sentTicks;
    }

    public DateTime getSentTime()
    {
        return new DateTime(sentTicks);
    }

    public bool getIsRead()
    {
        return isRead;
    }

    
    public void markAsRead()
    {
        isRead = true;
    }

    public override string ToString()
    {
        return sender + "|" + sentTicks + "|" + isRead + "|" + text;
    }
}
