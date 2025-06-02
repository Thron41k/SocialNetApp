namespace SocialNetApp.ViewModels.Account;

public class MainViewModel
{
    public RegisterViewModel RegisterView { get; set; } = new();

    public LoginViewModel LoginView { get; set; } = new();
}