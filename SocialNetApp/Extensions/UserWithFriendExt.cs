using SocialNetApp.Data.Models;

namespace SocialNetApp.Extensions;

public class UserWithFriendExt : User
{
    public bool IsFriendWithCurrent { get; set; }
}