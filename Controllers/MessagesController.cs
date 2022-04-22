

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class TargetUser
{
    public string? Name { get; set; }
} 

[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    CommunicatorDbContext _context { get; set; }
    public MessagesController(CommunicatorDbContext context)
    {
        this._context = context;
    }

    [HttpGet]
    public IActionResult Get(string name)
    {
        User? subjectUser = null;
        if (User?.Identity?.Name is string subjectName)
        {
            subjectUser = _context.User.FirstOrDefault(user => user.Name == subjectName);
        }
        if (subjectUser is null)
        {
            return BadRequest($"ТЫ МЕНЯ ОБМАНУТЬ ПЫТАЕШЬСЯ А ВОТ НЕ ВЫЙДЕТ");
        }
        var messages = _context
            .PrivateMessage
            .Where(message => 
                message.Deleted == false &&
                ((message.Sender == subjectUser && message.Recipient.Name == name) || 
                (message.Sender.Name == name && message.Recipient.Name == subjectUser.Name)))
            .Include(message => message.Sender)
            .Include(message => message.Recipient)
            .Select(message => new PrivateMessageDTO(message))
            .ToList();

        return new JsonResult(messages);
    }

    /*[HttpPost]
    public IActionResult Post()
    {

    }*/
}