using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SocialNetApp.Data.Models;
using SocialNetApp.Data.Repository;
using SocialNetApp.Data.UoW.Interfaces;
using SocialNetApp.Hubs.SocialNetApp.Hubs;
using SocialNetApp.ViewModels.Account;

namespace SocialNetApp.Controllers;

[Authorize]
public class ChatController(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IHubContext<ChatHub> hubContext) : Controller
{
    [HttpPost]
    [Route("Chat/Index")]
    public async Task<IActionResult> Index(string id)
    {
        var currentUser = await userManager.GetUserAsync(User);
        var recipient = await userManager.FindByIdAsync(id);

        if (recipient == null)
        {
            return NotFound();
        }

        var repository = unitOfWork.GetRepository<Message>() as MessageRepository;
        var messages = await repository.GetMessages(currentUser, recipient);

        var model = new ChatViewModel
        {
            You = currentUser,
            ToWhom = recipient,
            History = messages.OrderBy(x => x.Id).ToList(),
        };

        return View("~/Views/Chat/Chat.cshtml",model);
    }
    [HttpPost]
    [Route("Chat/SendMessage")]
    public async Task<IActionResult> SendMessage([FromQuery] string recipientId, [FromBody] ChatViewModel chat)
    {
        if (chat == null)
        {
            return BadRequest("Chat data is required");
        }
        var sender = await userManager.GetUserAsync(User);
        var recipient = await userManager.FindByIdAsync(recipientId);

        if (recipient == null)
        {
            return NotFound();
        }

        var repository = unitOfWork.GetRepository<Message>() as MessageRepository;

        var message = new Message()
        {
            Sender = sender,
            Recipient = recipient,
            Text = chat.NewMessage.Text,
            CreatedAt = DateTime.UtcNow
        };
        await repository.Create(message);
        await hubContext.Clients.Group(ChatHub.GetGroupName(sender.Id, recipient.Id))
            .SendAsync("ReceiveMessage", sender.Id, sender.FirstName, message.Text, message.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));

        return Ok();
    }
    [HttpGet]
    [Route("Chat/GetChat")]
    public async Task<IActionResult> GetChat(string id)
    {
        var currentUser = await userManager.GetUserAsync(User);
        var recipient = await userManager.FindByIdAsync(id);

        if (recipient == null)
        {
            return NotFound();
        }

        var repository = unitOfWork.GetRepository<Message>() as MessageRepository;
        var messages = await repository.GetMessages(currentUser, recipient);

        var model = new ChatViewModel()
        {
            You = currentUser,
            ToWhom = recipient,
            History = messages.OrderBy(x => x.Id).ToList(),
        };

        return PartialView("_ChatMessages", model);
    }
}