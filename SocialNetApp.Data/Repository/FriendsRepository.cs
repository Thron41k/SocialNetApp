using Microsoft.EntityFrameworkCore;
using SocialNetApp.Data.Models;

namespace SocialNetApp.Data.Repository;

public class FriendsRepository(ApplicationDbContext db) : Repository<Friend>(db)
{
    public void AddFriend(User target, User Friend)
    {
        var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

        if (friends == null)
        {
            var item = new Friend()
            {
                UserId = target.Id,
                User = target,
                CurrentFriend = Friend,
                CurrentFriendId = Friend.Id,
            };

            Create(item);
        }
    }

    public List<User> GetFriendsByUser(User target)
    {
        var friends = Set.Include(x => x.CurrentFriend).Include(x => x.User).AsEnumerable().Where(x => x.User.Id == target.Id).Select(x => x.CurrentFriend);

        return friends.ToList();
    }

    public void DeleteFriend(User target, User Friend)
    {
        var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

        if (friends != null)
        {
            Delete(friends);
        }
    }

}