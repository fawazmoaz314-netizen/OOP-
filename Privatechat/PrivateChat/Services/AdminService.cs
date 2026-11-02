using System.Collections.Generic;
using PrivateChat.Models;
using PrivateChat.Repositories;

namespace PrivateChat.Services;

// Business Logic Layer: the rules for the Admin Panel.
// EVERY public method first checks that the person who asks (actorName) is an active administrator.
// So even if the UI showed an admin screen by mistake, a normal user could still do nothing here.
public class AdminService
{
    private readonly UserRepository _userRepository;
    private readonly ConversationRepository _conversationRepository;

    public AdminService()
    {
        _userRepository = new UserRepository();
        _conversationRepository = new ConversationRepository();
    }

    // Returns the users that match the search text, in the chosen order.
    // searchText: empty = all users; otherwise it is looked for in username, display name and role (not case-sensitive).
    // sortBy: "username", "name" or "date" (anything else gives the default: username).
    // Returns null when the actor is not an active administrator.
    public List<User> SearchUsers(string actorName, string searchText, string sortBy)
    {
        List<User> users = _userRepository.GetAll();
        if (!IsActiveAdmin(users, actorName))
        {
            return null;
        }

        string search = "";
        if (searchText != null)
        {
            search = searchText.Trim().ToLower();
        }

        List<User> result = new List<User>();
        for (int i = 0; i < users.Count; i++)
        {
            User user = users[i];
            bool matches = search == ""
                || user.getUsername().Contains(search)
                || user.getDisplayName().ToLower().Contains(search)
                || user.getRole().ToLower().Contains(search);
            if (matches)
            {
                result.Add(user);
            }
        }

        SortUsers(result, sortBy);
        return result;
    }

    // Creates an account (normal user or admin). Returns "" when fine, otherwise the reason it failed.
    public string CreateUser(string actorName, string username, string displayName, string password, string role)
    {
        List<User> users = _userRepository.GetAll();
        if (!IsActiveAdmin(users, actorName))
        {
            return "Access denied: only an administrator can do this.";
        }

        User user = new User();
        if (!user.setUsername(username))
        {
            return "Username must be 3 to 20 characters: letters, numbers or underscore only.";
        }
        if (displayName != null && displayName.Trim().Length > 0)
        {
            if (!user.setDisplayName(displayName))
            {
                return "Display name must be 1 to 30 characters and must not contain '|'.";
            }
        }
        if (!user.setPassword(password))
        {
            return "Password must be at least 6 characters.";
        }
        if (!user.setRole(role))
        {
            return "Role must be 'User' or 'Admin'.";
        }
        if (FindUser(users, user.getUsername()) != null)
        {
            return "That username is already taken.";
        }

        _userRepository.Save(user);
        return "";
    }

    // Deletes an account AND every conversation (with all its messages) that this person took part in.
    // The other person loses those chats too. An admin cannot delete his or her own account.
    // Returns "" when fine, otherwise the reason it failed.
    public string DeleteUser(string actorName, string targetName)
    {
        List<User> users = _userRepository.GetAll();
        if (!IsActiveAdmin(users, actorName))
        {
            return "Access denied: only an administrator can do this.";
        }

        targetName = User.cleanName(targetName);
        if (targetName == User.cleanName(actorName))
        {
            return "You cannot delete your own account.";
        }
        User target = FindUser(users, targetName);
        if (target == null)
        {
            return "User '" + targetName + "' was not found.";
        }

        // Keep only the conversations that do NOT involve the deleted user.
        List<Conversation> all = _conversationRepository.GetAll(users);
        List<Conversation> kept = new List<Conversation>();
        for (int i = 0; i < all.Count; i++)
        {
            if (!all[i].hasParticipant(targetName))
            {
                kept.Add(all[i]);
            }
        }
        _conversationRepository.SaveAll(kept);

        users.Remove(target);
        _userRepository.SaveAll(users);
        return "";
    }

    // Bans an account. A banned user cannot log in or chat. An admin cannot ban himself or herself.
    // Returns "" when fine, otherwise the reason it failed.
    public string BanUser(string actorName, string targetName)
    {
        return ChangeBan(actorName, targetName, true);
    }

    // Removes a ban. Returns "" when fine, otherwise the reason it failed.
    public string UnbanUser(string actorName, string targetName)
    {
        return ChangeBan(actorName, targetName, false);
    }

    private string ChangeBan(string actorName, string targetName, bool banned)
    {
        List<User> users = _userRepository.GetAll();
        if (!IsActiveAdmin(users, actorName))
        {
            return "Access denied: only an administrator can do this.";
        }

        targetName = User.cleanName(targetName);
        if (targetName == User.cleanName(actorName))
        {
            return "You cannot ban or unban your own account.";
        }
        User target = FindUser(users, targetName);
        if (target == null)
        {
            return "User '" + targetName + "' was not found.";
        }
        if (banned && target.getIsBanned())
        {
            return "That user is already banned.";
        }
        if (!banned && !target.getIsBanned())
        {
            return "That user is not banned.";
        }

        if (banned)
        {
            target.ban();
        }
        else
        {
            target.unban();
        }
        _userRepository.SaveAll(users);
        return "";
    }

    // The authorization check used by every method above: the actor must exist, be an admin and not be banned.
    private bool IsActiveAdmin(List<User> users, string actorName)
    {
        User actor = FindUser(users, User.cleanName(actorName));
        if (actor == null)
        {
            return false;
        }
        return actor.isAdmin() && !actor.getIsBanned();
    }

    // Simple bubble sort. "name" sorts by display name, "date" by registration time (oldest first),
    // and everything else (the default) by username. Equal values keep a stable order by username.
    private void SortUsers(List<User> list, string sortBy)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = 0; j < list.Count - 1 - i; j++)
            {
                if (CompareUsers(list[j], list[j + 1], sortBy) > 0)
                {
                    User temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }
    }

    // Returns a negative number if a comes first, a positive number if b comes first, 0 if equal.
    private int CompareUsers(User a, User b, string sortBy)
    {
        int result = 0;
        if (sortBy == "name")
        {
            result = string.CompareOrdinal(a.getDisplayName().ToLower(), b.getDisplayName().ToLower());
        }
        else if (sortBy == "date")
        {
            if (a.getRegisteredTicks() < b.getRegisteredTicks())
            {
                result = -1;
            }
            else if (a.getRegisteredTicks() > b.getRegisteredTicks())
            {
                result = 1;
            }
        }
        if (result == 0)
        {
            result = string.CompareOrdinal(a.getUsername(), b.getUsername());
        }
        return result;
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
    //  now 
    // Returns all users whose username starts with the given letter(s), sorted by username.
    // Returns null when the actor is not an active administrator.
    public List<User> SearchUsersByFirstLetter(string actorName, string letter)
    {
        List<User> users = _userRepository.GetAll();
        if (!IsActiveAdmin(users, actorName))
        {
            return null;
        }

        string start = User.cleanName(letter);          // trim + lowercase: "A" and "a" give the same result
        List<User> result = new List<User>();
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].getUsername().StartsWith(start))
            {
                result.Add(users[i]);
            }
        }

        SortUsers(result, "username");
        return result;
    }
}