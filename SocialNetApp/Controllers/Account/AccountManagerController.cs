using AutoMapper;
using SocialNetApp.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using SocialNetApp.Extensions;
using SocialNetApp.Data.Repository;
using SocialNetApp.Data.UoW.Interfaces;
using SocialNetApp.Data;
using Microsoft.EntityFrameworkCore;

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
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return new SearchViewModel { UserList = [] };
            }

            var usersQuery = userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                usersQuery = usersQuery.Where(x =>
                    EF.Functions.Like(x.LastName, $"%{search}%") ||
                    EF.Functions.Like(x.FirstName, $"%{search}%") ||
                    EF.Functions.Like(x.MiddleName, $"%{search}%"));
            }

            var friends = await GetAllFriend();
            var friendIds = friends.Select(f => f.Id).ToHashSet();

            var userList = await usersQuery
                .Select(x => new UserWithFriendExt
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MiddleName = x.MiddleName,
                    Email = x.Email,
                    Image = x.Image,
                    IsFriendWithCurrent = friendIds.Contains(x.Id) || x.Id == currentUser.Id
                })
                .ToListAsync();

            return new SearchViewModel
            {
                UserList = userList
            };
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

            if (result != null)
            {
                var model = new UserViewModel(result);

                model.Friends = await GetAllFriend(model.User);
                model.Roles = await userManager.GetRolesAsync(model.User);

                return View("User", model);
            }
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
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

        [Route("Chat")]
        [HttpPost]
        public async Task<IActionResult> Chat(string id)
        {
            var currentuser = User;

            var result = await userManager.GetUserAsync(currentuser);
            var friend = await userManager.FindByIdAsync(id);

            var repository = unitOfWork.GetRepository<Message>() as MessageRepository;

            var mess = await repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };
            return View("~/Views/Chat/Chat.cshtml", model);
        }

        [Route("NewMessage")]
        [HttpPost]
        public async Task<IActionResult> NewMessage(string id, ChatViewModel chat)
        {
            var currentuser = User;

            var result = await userManager.GetUserAsync(currentuser);
            var friend = await userManager.FindByIdAsync(id);

            var repository = unitOfWork.GetRepository<Message>() as MessageRepository;

            var item = new Message()
            {
                Sender = result,
                Recipient = friend,
                Text = chat.NewMessage.Text,
            };
            repository.Create(item);

            var mess = await repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };
            return View("~/Views/Chat/Chat.cshtml", model);
        }

        private async Task<ChatViewModel> GenerateChat(string id)
        {
            var currentuser = User;

            var result = await userManager.GetUserAsync(currentuser);
            var friend = await userManager.FindByIdAsync(id);

            var repository = unitOfWork.GetRepository<Message>() as MessageRepository;

            var mess = await repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };

            return model;
        }

        [Route("Chat")]
        [HttpGet]
        public async Task<IActionResult> Chat()
        {

            var id = Request.Query["id"];

            var model = await GenerateChat(id);
            return View("~/Views/Chat/Chat.cshtml", model);
        }

        [Authorize(Roles = "Admin")]
        [Route("Generate")]
        [HttpGet]
        public async Task<IActionResult> Generate()
        {

            var userList = UserGenerator.GenerateUsers(40);

            foreach (var user in userList)
            {
                var result = await userManager.CreateAsync(user, "1234567");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "User");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
