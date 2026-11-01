using System.Collections.Generic;
using System.IO;
using PrivateChat.Models;

namespace PrivateChat.Repositories;

// Data Access Layer: reads and writes conversations.txt and messages.txt.
public class ConversationRepository
{
    private readonly string _conversationsPath = "conversations.txt";
    private readonly string _messagesPath = "messages.txt";

    // Writes ALL conversations and ALL their messages (both files are replaced).
    public void SaveAll(List<Conversation> conversations)
    {
        using (StreamWriter writer = new StreamWriter(_conversationsPath, false))
        {
            foreach (Conversation conversation in conversations)
            {
                writer.WriteLine(conversation.ToString());
            }
        }

        using (StreamWriter writer = new StreamWriter(_messagesPath, false))
        {
            foreach (Conversation conversation in conversations)
            {
                for (int i = 0; i < conversation.getMessageCount(); i++)
                {
                    Message message = conversation.getMessageAt(i);
                    // Every message line starts with the id of the conversation it belongs to.
                    writer.WriteLine(conversation.getId() + "|" + message.ToString());
                }
            }
        }
    }

    // Loads the conversations. The list of users is needed because a Conversation holds User objects.
    public List<Conversation> GetAll(List<User> users)
    {
        List<Conversation> conversations = new List<Conversation>();
        if (!File.Exists(_conversationsPath))
        {
            return conversations;
        }

        string[] lines = File.ReadAllLines(_conversationsPath);
        foreach (string line in lines)
        {
            if (line.Trim().Length == 0)
            {
                continue;
            }
            string[] parts = line.Split('|');
            if (parts.Length != 5)
            {
                continue;
            }

            int id = int.Parse(parts[0]);
            User firstUser = FindUser(users, parts[1]);
            User secondUser = FindUser(users, parts[2]);
            if (firstUser == null || secondUser == null)
            {
                continue;
            }

            Conversation conversation = new Conversation(id, firstUser, secondUser);
            conversation.setHiddenFor(parts[1], bool.Parse(parts[3]));
            conversation.setHiddenFor(parts[2], bool.Parse(parts[4]));
            conversations.Add(conversation);
        }

        LoadMessages(conversations);
        return conversations;
    }

    // Reads messages.txt and puts every message into its conversation.
    private void LoadMessages(List<Conversation> conversations)
    {
        if (!File.Exists(_messagesPath))
        {
            return;
        }

        string[] lines = File.ReadAllLines(_messagesPath);
        foreach (string line in lines)
        {
            if (line.Trim().Length == 0)
            {
                continue;
            }
            // Split into at most 5 parts, so a "|" inside the message text stays in the text.
            string[] parts = line.Split(new char[] { '|' }, 5);
            if (parts.Length != 5)
            {
                continue;
            }

            int conversationId = int.Parse(parts[0]);
            Conversation conversation = FindConversation(conversations, conversationId);
            if (conversation == null)
            {
                continue;
            }

            Message message = new Message(parts[1], parts[4], long.Parse(parts[2]), bool.Parse(parts[3]));
            conversation.addMessage(message);
        }
    }

    private User FindUser(List<User> users, string username)
    {
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].getUsername() == username)
            {
                return users[i];
            }
        }
        return null;
    }

    private Conversation FindConversation(List<Conversation> conversations, int id)
    {
        for (int i = 0; i < conversations.Count; i++)
        {
            if (conversations[i].getId() == id)
            {
                return conversations[i];
            }
        }
        return null;
    }
}
