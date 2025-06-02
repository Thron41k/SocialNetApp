using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SocialNetApp.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using SocialNetApp.Extensions;
using SocialNetApp.Data.Repository;
using SocialNetApp.Data.UoW.Interfaces;
using SocialNetApp.Data.UoW;

namespace SocialNetApp.Controllers.Account
{
    public class AccountManagerController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IMapper mapper,
        IUnitOfWork unitOfWork)
        : Controller
    {
        private async Task<SearchViewModel> CreateSearch(string search)
        {
            var currentuser = User;

            var result = await userManager.GetUserAsync(currentuser);

            var list = userManager.Users.AsEnumerable().Where(x => x.GetFullName().Contains(search,StringComparison.OrdinalIgnoreCase)).ToList();
            var withfriend = await GetAllFriend();

            var data = new List<UserWithFriendExt>();
            list.ForEach(x =>
            {
                var t = mapper.Map<UserWithFriendExt>(x);
                t.IsFriendWithCurrent = withfriend.Count(y => y.Id == x.Id || x.Id == result?.Id) != 0;
                data.Add(t);
            });

            var model = new SearchViewModel()
            {
                UserList = data
            };

            return model;
        }

        private async Task<List<User>> GetAllFriend()
        {
            var user = User;

            var result = await userManager.GetUserAsync(user);

            var repository = unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(result);
        }
        private async Task<List<User>> GetAllFriend(User user)
        {
            var repository = unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(user);
        }
        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Home/Login");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [Route("MyPage")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyPage()
        {
            var user = User;

            var result = await userManager.GetUserAsync(user);

            var model = new UserViewModel(result);

            model.Friends = await GetAllFriend(model.User);

            return View("User", model);
        }

        [Route("Edit")]
        [HttpGet]
        [Authorize]
        public IActionResult Edit()
        {
            var user = User;

            var result = userManager.GetUserAsync(user);

            return View("UserEdit", new UserEditViewModel(result.Result));
        }

        [Authorize]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.Id);

                user.Convert(model);

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("MyPage", "AccountManager");
                }

                return RedirectToAction("Edit", "AccountManager");
            }

            ModelState.AddModelError("", "Некорректные данные");
            return View("UserEdit", model);
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = mapper.Map<User>(model);
                if (user.Email != null)
                {
                    var signedUser = await userManager.FindByEmailAsync(user.Email);
                    if (signedUser is { UserName: not null })
                    {
                        var result = await signInManager.PasswordSignInAsync(signedUser.UserName, model.Password, model.RememberMe, false);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("MyPage", "AccountManager");
                        }
                    }
                }

                ModelState.AddModelError("", "Неправильный логин и (или) пароль");
            }
            return View("~/Views/Home/Index.cshtml",new MainViewModel());
        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Route("UserList")]
        [HttpPost]
        public async Task<IActionResult> UserList(string search)
        {
            var model = await CreateSearch(search);
            return View("UserList", model);
        }

        [Route("AddFriend")]
        [HttpPost]
        public async Task<IActionResult> AddFriend(string id)
        {
            var currentuser = User;

            var result = await userManager.GetUserAsync(currentuser);

            var friend = await userManager.FindByIdAsync(id);

            var repository = unitOfWork.GetRepository<Friend>() as FriendsRepository;

            repository.AddFriend(result, friend);

            return RedirectToAction("MyPage", "AccountManager");

        }

        [Route("DeleteFriend")]
        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            var currentuser = User;

            var result = await userManager.GetUserAsync(currentuser);

            var friend = await userManager.FindByIdAsync(id);

            var repository = unitOfWork.GetRepository<Friend>() as FriendsRepository;

            repository.DeleteFriend(result, friend);

            return RedirectToAction("MyPage", "AccountManager");

        }
    }
}
