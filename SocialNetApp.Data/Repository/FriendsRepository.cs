using Microsoft.EntityFrameworkCore;
using SocialNetApp.Data.Models;

namespace SocialNetApp.Data.Repository;

public class FriendsRepository(ApplicationDbContext db) : Repository<Friend>(db)
{
    public async Task AddFriend(User target, User friend)
    {
        var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == friend.Id);

        if (friends == null)
        {
            var item = new Friend()
            {
                UserId = target.Id,
                User = target,
                CurrentFriend = friend,
                CurrentFriendId = friend.Id,
            };

            await Create(item);
        }
    }

    public List<User> GetFriendsByUser(User target)
    {
        var friends = Set.Include(x => x.CurrentFriend).Include(x => x.User).AsEnumerable().Where(x => x.User.Id == target.Id).Select(x => x.CurrentFriend);

        return friends.ToList();
    }

    public async Task DeleteFriend(User target, User friend)
    {
        var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == friend.Id);

        if (friends != null)
        {
            await Delete(friends);
        }
    }

}