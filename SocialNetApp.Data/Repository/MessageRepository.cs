
using SocialNetApp.Data.Models;

namespace SocialNetApp.Data.Repository;

public class MessageRepository(ApplicationDbContext db) : Repository<Message>(db);