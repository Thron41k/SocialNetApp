using SocialNetApp.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace SocialNetApp.ViewModels.Account;

public class UserEditViewModel
{
    public string Id { get; set; }
    [Required(ErrorMessage = "Поле Имя обязательно для заполнения")]
    [DataType(DataType.Text)]
    [Display(Name = "Имя", Prompt = "Введите имя")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Поле Фамилия обязательно для заполнения")]
    [DataType(DataType.Text)]
    [Display(Name = "Фамилия", Prompt = "Введите фамилию")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Поле Отчество обязательно для заполнения")]
    [DataType(DataType.Text)]
    [Display(Name = "Отчество", Prompt = "Введите Отчество")]
    public string MiddleName { get; set; }
    public DateTime BirthDate { get; set; }
    [Required(ErrorMessage = "Поле Фото профиля обязательно для заполнения")]
    [DataType(DataType.ImageUrl)]
    [Display(Name = "Фото профиля", Prompt = "URL фотографии")]
    public string Image { get; set; }
    [Required(ErrorMessage = "Поле Статус обязательно для заполнения")]
    [DataType(DataType.Text)]
    [Display(Name = "Статус", Prompt = "Введите Статус")]
    public string Status { get; set; }
    [Required(ErrorMessage = "Поле О себе обязательно для заполнения")]
    [DataType(DataType.Text)]
    [Display(Name = "О себе", Prompt = "Введите информацию о себе")]
    public string About { get; set; }
    [Required(ErrorMessage = "Поле Email обязательно для заполнения")]
    [EmailAddress]
    [Display(Name = "Email", Prompt = "example.com")]
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