using Microsoft.AspNetCore.SignalR;

public class CommunicatorHub : Hub
{
    CommunicatorDbContext _context { get; set; }
    public CommunicatorHub(CommunicatorDbContext context)
    {
        this._context = context;
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

    public async Task SendMessage(string targetUserName, string content)
    {
        await TryGetSubjectUser(async subjectUser =>
        {
            await TryGetTargetUser(targetUserName, async targetUser =>
            {
                _context.PrivateMessage.Add(new PrivateMessage()
                {
                    Content = content,
                    SendingDateTime = new DateTime(),
                    ReceiptDateTime = new DateTime(),
                    SenderId = subjectUser.Id,
                    RecipientId = targetUser.Id,
                });
                _context.SaveChanges();
                await Clients
                    .Users(new List<string>() { subjectUser.Name, targetUser.Name })
                    .SendAsync("ReceiveSignal", new
                    {
                        Sender = subjectUser.Name,
                        Recepient = targetUser.Name,
                        Content = content
                    });
            });
        });
    }

    public async Task NotifyTyping(string targetUserName)
    {
        await TryGetSubjectUser(async subjectUser =>
        {
            await TryGetTargetUser(targetUserName, async targetUser =>
            {
                await Clients
                    .Users(new List<string>() { targetUser.Name })
                    .SendAsync("TypingNotify", subjectUser.Name);
            });
        });
    }
}