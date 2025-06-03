
using Microsoft.EntityFrameworkCore;
using SocialNetApp.Data.Models;

namespace SocialNetApp.Data.Repository;

public class MessageRepository(ApplicationDbContext db) : Repository<Message>(db)
{
    public async Task<List<Message>> GetMessages(User sender, User recipient)
    {
        return await Set
            .Include(x => x.Recipient)
            .Include(x => x.Sender)
            .Where(x => (x.SenderId == sender.Id && x.RecipientId == recipient.Id) ||
                        (x.SenderId == recipient.Id && x.RecipientId == sender.Id))
            .OrderBy(x => x.Id)
            .ToListAsync();
    }
}