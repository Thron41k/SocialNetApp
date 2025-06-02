using SocialNetApp.Data.Models;

namespace SocialNetApp.ViewModels.Account;

public class UserEditViewModel
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Image { get; set; }
    public string Status { get; set; }
    public string About { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public UserEditViewModel() { }
    public UserEditViewModel(User user) {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        MiddleName = user.MiddleName;
        BirthDate = user.BirthDate;
        Image = user.Image;
        Status = user.Status;
        About = user.About;
        Email = user.Email;
        UserName = user.UserName;
    }
}