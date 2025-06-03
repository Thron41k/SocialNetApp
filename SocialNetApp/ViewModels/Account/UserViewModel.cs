using SocialNetApp.Data.Models;

namespace SocialNetApp.ViewModels.Account;

public class UserViewModel(User user)
{
    public User User { get; set; } = user;
    public List<User> Friends { get; set; }
    public IList<string> Roles { get; set; }
    public string FormattedBirthDate => User.GetFormattedBirthDate();
    public string FullName => User.GetFullName();
}