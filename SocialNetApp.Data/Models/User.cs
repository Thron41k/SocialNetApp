using Microsoft.AspNetCore.Identity;

namespace SocialNetApp.Data.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string MiddleName { get; set; }

    public DateTime BirthDate { get; set; }
    public string Image { get; set; }

    public string Status { get; set; }

    public string About { get; set; }

    public string GetFullName()
    {
        return FirstName + " " + MiddleName + " " + LastName;
    }
    public string GetFormattedBirthDate()
    {
        return BirthDate.ToString("dd MMMM yyyy г.");
    }
    public User()
    {
        Image = "https://dummyimage.com/500x500/a6a6a6/000000.png";
        Status = "Ура! Я в соцсети!";
        About = "Информация обо мне.";
    }
    public virtual ICollection<Friend> Friends { get; set; }

}