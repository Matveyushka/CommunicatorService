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

    [HttpPost]
    public IActionResult PostAsync(ContactQuery contactQuery)
    {
        User? subjectUser = null;
        User? targetUser = null;
        if (User?.Identity?.Name is string subjectName)
        {
            subjectUser = _context.Users.FirstOrDefault(user => user.Name == subjectName);
        }
        if (contactQuery.TargetUser is string targetName)
        {
            targetUser = _context.Users.FirstOrDefault(user => user.Name == targetName);
        }
        if (subjectUser is null)
        {
            return BadRequest($"Invalid subject user");
        }
        if (targetUser is null)
        {
            return BadRequest($"Invalid target user");
        }
        var relation = _context.UsersRelations.FirstOrDefault(rel =>
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

            var newRelation = new UsersRelation() {
                SubjectId = newSubjectId,
                TargetId = newTargetId,
                Displayed = contactQuery.Display,
                Muted = contactQuery.Mute,
                Blocked = contactQuery.Block
            };
            _context.UsersRelations.Add(newRelation);
            _context.SaveChanges();
        }
        return Ok();
    }
}