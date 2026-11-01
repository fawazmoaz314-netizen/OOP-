using System;

namespace PrivateChat.Models;

// Entity: one text message written by one user.
public class Message
{
    // Static attribute: ONE value shared by all messages (not one copy per message).
    public static readonly int maxLength = 500;

    private string sender;
    private string text;
    private long sentTicks;   // the time stored as a number, so it is easy to write into a text file
    private bool isRead;

    // Default constructor: an empty message that gets the current time.
    public Message()
    {
        sender = "";
        text = "";
        sentTicks = DateTime.Now.Ticks;
        isRead = false;
    }

    // Parameterized constructor: used when we load a saved message from the file.
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

    // Setter with validation: not empty and not longer than maxLength.
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

    // Behaviour: the receiver has opened the message.
    public void markAsRead()
    {
        isRead = true;
    }

    // One line of text for the file: sender|sentTicks|isRead|text
    // The text is LAST, so even a "|" inside the text cannot break the line.
    public override string ToString()
    {
        return sender + "|" + sentTicks + "|" + isRead + "|" + text;
    }
}
