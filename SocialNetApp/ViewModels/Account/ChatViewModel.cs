using SocialNetApp.Data.Models;

namespace SocialNetApp.ViewModels.Account;

public class ChatViewModel
{
    public User You { get; set; }

    public User ToWhom { get; set; }

    public List<Message> History { get; set; }

    public MessageViewModel NewMessage { get; set; } = new();
}