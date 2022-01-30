using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class CommunicatorHub : Hub
{
    CommunicatorDbContext _context { get; set; }
    UserRelationRepository userRelationRepository { get; set; }
    public CommunicatorHub(CommunicatorDbContext context, UserRelationRepository userRelationRepository)
    {
        this._context = context;
        this.userRelationRepository = userRelationRepository;
    }

    private async Task TryGetSubjectUser(Func<User, Task> callback)
    {
        User? subjectUser = null;
        if (Context.UserIdentifier is string subjectName)
        {
            subjectUser = _context.User.FirstOrDefault(user => user.Name == subjectName);
        }
        if (subjectUser is not null)
        {
            await callback(subjectUser);
        }
    }

    private async Task TryGetTargetUser(string targetUserName, Func<User, Task> callback)
    {
        User? targetUser = null;
        if (targetUserName is string)
        {
            targetUser = _context.User.FirstOrDefault(user => user.Name == targetUserName);
        }
        if (targetUser is not null)
        {
            await callback(targetUser);
        }
    }

    private async Task TryGetUsers(string targetUserName, Func<User, User, Task> callback)
    {
        await TryGetSubjectUser(async subjectUser =>
        {
            await TryGetTargetUser(targetUserName, async targetUser =>
            {
                await callback(subjectUser, targetUser);
            });
        });
    }

    public async Task SendMessage(string targetUserName, string content)
    {
        await TryGetUsers(targetUserName, async (subjectUser, targetUser) =>
        {
            var targetSenderRelation = _context.UsersRelation.FirstOrDefault(relation =>
                relation.SubjectUser == targetUser && relation.TargetUser == subjectUser);
            if (targetSenderRelation is null)
            {
                var newSubjectId = new Guid(targetUser.Id.ToString());
                var newTargetId = new Guid(subjectUser.Id.ToString());
                targetSenderRelation = userRelationRepository.Add(newSubjectId, newTargetId);
            }

            if (targetSenderRelation.Blocked == false)
            {
                var message = new PrivateMessage()
                {
                    Content = content,
                    SendingDateTime = DateTime.Now,
                    ReceiptDateTime = null,
                    SenderId = subjectUser.Id,
                    RecipientId = targetUser.Id,
                };
                _context.PrivateMessage.Add(message);
                _context.SaveChanges();
                await Clients
                    .Users(new List<string>() { subjectUser.Name, targetUser.Name })
                    .SendAsync("ReceiveMessage", new PrivateMessageDTO(message));
            }
        });
    }

    public async Task NotifyTyping(string targetUserName)
    {
        await TryGetUsers(targetUserName, async (subjectUser, targetUser) =>
        {
            await Clients
                .User(targetUser.Name)
                .SendAsync("TypingNotify", subjectUser.Name);
        });
    }

    public async Task ReadMessage(Guid messageId)
    {
        var message = _context
            .PrivateMessage
            .Include(message => message.Recipient)
            .Include(message => message.Sender)
            .FirstOrDefault(message => message.Id == messageId);
        if (message is not null)
        {
            await TryGetUsers(message.Sender.Name, async (subjectUser, targetUser) =>
            {
                if (message.Recipient == subjectUser)
                {
                    message.ReceiptDateTime = DateTime.Now;
                    _context.SaveChanges();
                    await Clients
                        .Users(new List<string>() { subjectUser.Name, targetUser.Name })
                        .SendAsync("MessageRead", new PrivateMessageDTO(message));
                }
            });
        }
    }

    public async Task DeleteMessage(Guid messageId)
    {
        var message = _context.PrivateMessage.FirstOrDefault(message => message.Id == messageId);
        if (message is not null)
        {
            await TryGetUsers(message.Sender.Name, async (subjectUser, targetUser) =>
            {
                if (message.Recipient == subjectUser || message.Sender == subjectUser)
                {
                    message.Deleted = true;
                    _context.SaveChanges();
                    await Clients
                        .User(message.Sender.Name)
                        .SendAsync("MessageDelete", messageId);
                }
            });
        }
    }
}