using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ContactQuery
{
    public string? TargetUser { get; set; }
    public bool Display { get; set; }
    public bool Mute { get; set; }
    public bool Block { get; set; }
}

[ApiController]
[Route("[controller]")]
public class ContactController : ControllerBase
{
    CommunicatorDbContext _context { get; set; }

    public ContactController(CommunicatorDbContext context)
    {
        this._context = context;
    }


    [HttpGet]
    public IActionResult Get()
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
        var contacts = _context
            .UsersRelation
            .Where(relation => relation.SubjectUser == subjectUser)
            .Select(relation => new {
                Name = relation.TargetUser.Name,
                UnreadMessages = _context.PrivateMessage.Where(msg => msg.Sender == relation.TargetUser && msg.Recipient == subjectUser && msg.ReceiptDateTime == null).Count()
            })
            .ToList();

        return new JsonResult(contacts);
    }

    [HttpPost]
    public IActionResult Post(ContactQuery contactQuery)
    {
        User? subjectUser = null;
        User? targetUser = null;
        if (User?.Identity?.Name is string subjectName)
        {
            subjectUser = _context.User.FirstOrDefault(user => user.Name == subjectName);
        }
        if (contactQuery.TargetUser is string targetName)
        {
            targetUser = _context.User.FirstOrDefault(user => user.Name == targetName);
        }
        if (subjectUser is null)
        {
            return BadRequest($"Invalid subject user");
        }
        if (targetUser is null)
        {
            return BadRequest($"Invalid target user");
        }
        var relation = _context.UsersRelation.FirstOrDefault(rel =>
            rel.SubjectUser == subjectUser && rel.TargetUser == targetUser
        );
        if (relation is not null)
        {
            relation.Displayed = contactQuery.Display;
            relation.Muted = contactQuery.Mute;
            relation.Blocked = contactQuery.Block;
        }
        else
        {
            var newSubjectId = new Guid(subjectUser.Id.ToString());
            var newTargetId = new Guid(targetUser.Id.ToString());

            var newRelation = new UsersRelation()
            {
                SubjectId = newSubjectId,
                TargetId = newTargetId,
                Displayed = contactQuery.Display,
                Muted = contactQuery.Mute,
                Blocked = contactQuery.Block
            };
            _context.UsersRelation.Add(newRelation);
            _context.SaveChanges();
        }
        return Ok();
    }
}