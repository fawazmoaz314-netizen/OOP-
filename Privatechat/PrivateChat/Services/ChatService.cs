using System.Collections.Generic;
using PrivateChat.Models;
using PrivateChat.Repositories;

namespace PrivateChat.Services;

// Business Logic Layer: the rules for chatting (send, open, list, hide).
public class ChatService
{
    private readonly UserRepository _userRepository;
    private readonly ConversationRepository _conversationRepository;

    public ChatService()
    {
        _userRepository = new UserRepository();
        _conversationRepository = new ConversationRepository();
    }

    // Sends a message. Returns "" when fine, otherwise the reason it failed.
    public string SendMessage(string senderName, string receiverName, string text)
    {
        senderName = User.cleanName(senderName);
        receiverName = User.cleanName(receiverName);

        if (senderName == receiverName)
        {
            return "You cannot send a message to yourself.";
        }

        List<User> users = _userRepository.GetAll();
        User sender = FindUser(users, senderName);
        User receiver = FindUser(users, receiverName);
        if (sender == null)
        {
            return "Your account was not found.";
        }
        if (sender.getIsBanned())
        {
            return "Your account is banned. You cannot send messages.";
        }
        if (receiver == null)
        {
            return "User '" + receiverName + "' was not found.";
        }

        Message message = new Message();
        message.setSender(senderName);
        if (!message.setText(text))
        {
            return "A message must have 1 to " + Message.maxLength + " characters.";
        }

        List<Conversation> conversations = _conversationRepository.GetAll(users);
        Conversation conversation = FindConversation(conversations, senderName, receiverName);
        if (conversation == null)
        {
            // First message between these two: create the conversation.
            conversation = new Conversation(NextConversationId(conversations), sender, receiver);
            conversations.Add(conversation);
        }

        conversation.addMessage(message);
        _conversationRepository.SaveAll(conversations);
        return "";
    }

    // Opens the chat with another person and marks the messages they sent as read.
    // Returns null when there is no conversation yet.
    public Conversation OpenConversation(string myName, string otherName)
    {
        myName = User.cleanName(myName);
        otherName = User.cleanName(otherName);

        List<User> users = _userRepository.GetAll();
        if (!IsActiveUser(users, myName))
        {
            return null;
        }
        List<Conversation> conversations = _conversationRepository.GetAll(users);
        Conversation conversation = FindConversation(conversations, myName, otherName);
        if (conversation == null)
        {
            return null;
        }

        conversation.markAllReadFor(myName);
        _conversationRepository.SaveAll(conversations);
        return conversation;
    }

    // My chats, newest first. hidden = false gives the normal list, hidden = true gives my invisible chats.
    public List<Conversation> GetMyConversations(string myName, bool hidden)
    {
        myName = User.cleanName(myName);

        List<User> users = _userRepository.GetAll();
        List<Conversation> result = new List<Conversation>();
        if (!IsActiveUser(users, myName))
        {
            return result;
        }
        List<Conversation> all = _conversationRepository.GetAll(users);

        for (int i = 0; i < all.Count; i++)
        {
            Conversation conversation = all[i];
            if (conversation.hasParticipant(myName) && conversation.isHiddenFor(myName) == hidden)
            {
                result.Add(conversation);
            }
        }

        SortNewestFirst(result);
        return result;
    }

    // Hides (hidden = true) or shows (hidden = false) my chat with one person.
    // Returns "" when fine, otherwise the reason it failed.
    public string SetChatHidden(string myName, string otherName, bool hidden)
    {
        myName = User.cleanName(myName);
        otherName = User.cleanName(otherName);

        List<User> users = _userRepository.GetAll();
        if (!IsActiveUser(users, myName))
        {
            return "Your account is banned or does not exist.";
        }
        List<Conversation> conversations = _conversationRepository.GetAll(users);
        Conversation conversation = FindConversation(conversations, myName, otherName);
        if (conversation == null)
        {
            return "You have no chat with that user yet.";
        }

        conversation.setHiddenFor(myName, hidden);
        _conversationRepository.SaveAll(conversations);
        return "";
    }

    // Searches the text of my messages. Only conversations that I take part in and that are NOT hidden for me are
    // searched, so nobody can read other people's private chats and hidden chats stay hidden.
    // The search is not case-sensitive. An empty search text, or a banned user, gives an empty list.
    public List<SearchResult> SearchMessages(string myName, string searchText)
    {
        myName = User.cleanName(myName);
        List<SearchResult> results = new List<SearchResult>();
        if (searchText == null || searchText.Trim().Length == 0)
        {
            return results;
        }
        string search = searchText.Trim().ToLower();

        List<User> users = _userRepository.GetAll();
        if (!IsActiveUser(users, myName))
        {
            return results;
        }
        List<Conversation> conversations = _conversationRepository.GetAll(users);

        for (int i = 0; i < conversations.Count; i++)
        {
            Conversation conversation = conversations[i];
            if (!conversation.hasParticipant(myName) || conversation.isHiddenFor(myName))
            {
                continue;
            }
            string otherName = conversation.getOtherUser(myName).getUsername();
            for (int j = 0; j < conversation.getMessageCount(); j++)
            {
                Message message = conversation.getMessageAt(j);
                if (message.getText().ToLower().Contains(search))
                {
                    results.Add(new SearchResult(otherName, message));
                }
            }
        }
        return results;
    }

    // Is this person an existing account that is not banned?
    private bool IsActiveUser(List<User> users, string username)
    {
        User user = FindUser(users, username);
        return user != null && !user.getIsBanned();
    }

    // The new conversation gets (biggest existing id + 1). The first one gets 1.
    private int NextConversationId(List<Conversation> conversations)
    {
        int biggest = 0;
        for (int i = 0; i < conversations.Count; i++)
        {
            if (conversations[i].getId() > biggest)
            {
                biggest = conversations[i].getId();
            }
        }
        return biggest + 1;
    }

    // Simple "bubble sort": the chat with the newest message moves to the front.
    private void SortNewestFirst(List<Conversation> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = 0; j < list.Count - 1 - i; j++)
            {
                if (LastTicks(list[j]) < LastTicks(list[j + 1]))
                {
                    Conversation temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }
    }

    private long LastTicks(Conversation conversation)
    {
        Message last = conversation.getLastMessage();
        if (last == null)
        {
            return 0;
        }
        return last.getSentTicks();
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

    private Conversation FindConversation(List<Conversation> conversations, string nameA, string nameB)
    {
        for (int i = 0; i < conversations.Count; i++)
        {
            if (conversations[i].isBetween(nameA, nameB))
            {
                return conversations[i];
            }
        }
        return null;
    }
}
