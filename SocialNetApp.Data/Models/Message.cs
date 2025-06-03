namespace SocialNetApp.Data.Models;

public class Message
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string SenderId { get; set; }
    public User Sender { get; set; }

    public string RecipientId { get; set; }
    public User Recipient { get; set; }
}