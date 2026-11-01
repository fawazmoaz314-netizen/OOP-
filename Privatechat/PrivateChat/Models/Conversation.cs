    using System.Collections.Generic;

namespace PrivateChat.Models;

// Entity: the private chat between exactly two users.
//  - Aggregation:  the two User objects are only REFERENCED here. A user exists without a conversation.
//  - Composition:  the Message objects are CREATED AND OWNED here. Without the conversation they have no meaning.
public class Conversation
{
    private int id;
    private User firstUser;
    private User secondUser;
    private bool hiddenForFirstUser;    // "invisible chat" setting of the first user
    private bool hiddenForSecondUser;   // "invisible chat" setting of the second user
    private List<Message> messages;

    // There is no default constructor on purpose: a conversation must always have two users.
    public Conversation(int id, User firstUser, User secondUser)
    {
        this.id = id;
        this.firstUser = firstUser;
        this.secondUser = secondUser;
        hiddenForFirstUser = false;
        hiddenForSecondUser = false;
        messages = new List<Message>();   // composition: the list is created inside the conversation
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

    // Is this person one of the two users of the conversation?
    public bool hasParticipant(string username)
    {
        return firstUser.getUsername() == username || secondUser.getUsername() == username;
    }

    // Is this the conversation between exactly these two people (in any order)?
    public bool isBetween(string nameA, string nameB)
    {
        string first = firstUser.getUsername();
        string second = secondUser.getUsername();
        return (first == nameA && second == nameB) || (first == nameB && second == nameA);
    }

    // Given one participant, return the other one (or null if the name is not a participant).
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

    // Adds a message. Only the two participants may write in this conversation.
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

    // The list itself stays private. Others read it through these two methods.
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

    // How many messages did the OTHER person send to this user that are still unread?
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

    // This user has opened the chat, so everything the other person sent is now read.
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

    // Setter: hide or show the chat for ONE user. The other user's list is not affected.
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

    // One line of text for conversations.txt: id|firstUsername|secondUsername|hiddenFirst|hiddenSecond
    public override string ToString()
    {
        return id + "|" + firstUser.getUsername() + "|" + secondUser.getUsername() + "|" + hiddenForFirstUser + "|" + hiddenForSecondUser;
    }
}
