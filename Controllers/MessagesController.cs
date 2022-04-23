

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    CommunicatorDbContext _context { get; set; }
    public MessagesController(CommunicatorDbContext context)
    {
        this._context = context;
    }

    [HttpGet]
    public IActionResult Get(string name, DateTime from, DateTime till)
    {
        var subjectUserName = User.Identity?.Name;
        var messages = _context
            .PrivateMessage
            .Where(message => 
                message.Deleted == false &&
                ((message.Sender.Name == subjectUserName && message.Recipient.Name == name) || 
                (message.Sender.Name == name && message.Recipient.Name == subjectUserName)) &&
                message.SendingDateTime > from &&
                message.SendingDateTime <= till)
            .Include(message => message.Sender)
            .Include(message => message.Recipient)
            .Select(message => new PrivateMessageDTO(message))
            .ToList();

        return new JsonResult(messages);
    }
}