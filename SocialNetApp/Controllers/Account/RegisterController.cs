using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetApp.Data.Models;
using SocialNetApp.ViewModels.Account;

namespace SocialNetApp.Controllers.Account;

public class RegisterController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
    : Controller
{
    [Route("Register")]
    [HttpGet]
    public IActionResult Register()
    {
        return View("Register");
    }

    [Route("RegisterPart2")]
    [HttpGet]
    public IActionResult RegisterPart2(RegisterViewModel model)
    {
        return View("RegisterPart2", model);
    }



    [Route("Register")]
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = mapper.Map<User>(model);

            var result = await userManager.CreateAsync(user, model.PasswordReg);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
        return View("RegisterPart2", model);
    }
}
