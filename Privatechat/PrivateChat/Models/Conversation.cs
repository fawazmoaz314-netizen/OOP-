    using System.Collections.Generic;

namespace PrivateChat.Models;


public class Conversation
{
    private int id;
    private User firstUser;
    private User secondUser;
    private bool hiddenForFirstUser;   
    private bool hiddenForSecondUser;  
    private List<Message> messages;

    
    public Conversation(int id, User firstUser, User secondUser)
    {
        this.id = id;
        this.firstUser = firstUser;
        this.secondUser = secondUser;
        hiddenForFirstUser = false;
        hiddenForSecondUser = false;
        messages = new List<Message>();   
    }

    public int getId()
    {
        return id;
    }

    public User getFirstUser()
    {
        return firstUser;
    }

    public User getSecondUser()
    {
        return secondUser;
    }

 
    public bool hasParticipant(string username)
    {
        return firstUser.getUsername() == username || secondUser.getUsername() == username;
    }

   
    public bool isBetween(string nameA, string nameB)
    {
        string first = firstUser.getUsername();
        string second = secondUser.getUsername();
        return (first == nameA && second == nameB) || (first == nameB && second == nameA);
    }

    
    public User getOtherUser(string username)
    {
        if (firstUser.getUsername() == username)
        {
            return secondUser;
        }
        if (secondUser.getUsername() == username)
        {
            return firstUser;
        }
        return null;
    }


    public bool addMessage(Message message)
    {
        if (message == null)
        {
            return false;
        }
        if (!hasParticipant(message.getSender()))
        {
            return false;
        }
        messages.Add(message);
        return true;
    }
 
  

   
    public int getMessageCount()
    {
        return messages.Count;
    }

    public Message getMessageAt(int index)
    {
        if (index < 0 || index >= messages.Count)
        {
            return null;
        }
        return messages[index];
    }

    public Message getLastMessage()
    {
        if (messages.Count == 0)
        {
            return null;
        }
        return messages[messages.Count - 1];
    }

   
    public int countUnreadFor(string username)
    {
        int count = 0;
        for (int i = 0; i < messages.Count; i++)
        {
            Message message = messages[i];
            if (message.getSender() != username && !message.getIsRead())
            {
                count++;
            }
        }
        return count;
    }

   
    public void markAllReadFor(string username)
    {
        for (int i = 0; i < messages.Count; i++)
        {
            Message message = messages[i];
            if (message.getSender() != username)
            {
                message.markAsRead();
            }
        }
    }

    public bool isHiddenFor(string username)
    {
        if (firstUser.getUsername() == username)
        {
            return hiddenForFirstUser;
        }
        if (secondUser.getUsername() == username)
        {
            return hiddenForSecondUser;
        }
        return false;
    }

   
    public bool setHiddenFor(string username, bool hidden)
    {
        if (firstUser.getUsername() == username)
        {
            hiddenForFirstUser = hidden;
            return true;
        }
        if (secondUser.getUsername() == username)
        {
            hiddenForSecondUser = hidden;
            return true;
        }
        return false;
    }

    
    public override string ToString()
    {
        return id + "|" + firstUser.getUsername() + "|" + secondUser.getUsername() + "|" + hiddenForFirstUser + "|" + hiddenForSecondUser;
    }
}
