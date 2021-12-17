using Microsoft.AspNetCore.SignalR;

public class CommunicatorHub : Hub
{
    CommunicatorDbContext _context { get; set; }
    public CommunicatorHub(CommunicatorDbContext context)
    {
        this._context = context;
    }

    public async Task SendMessage(string targetUserName, string content)
    {
        User? subjectUser = null;
        User? targetUser = null;
        if (Context.UserIdentifier is string subjectName)
        {
            subjectUser = _context.User.FirstOrDefault(user => user.Name == subjectName);
        }
        if (targetUserName is string targetName)
        {
            targetUser = _context.User.FirstOrDefault(user => user.Name == targetName);
        }

        if (subjectUser is not null && targetUser is not null)
        {
            string message = Context.UserIdentifier + " : " + content;
            _context.PrivateMessage.Add(new PrivateMessage()
            {
                Content = message,
                SendingDateTime = new DateTime(),
                ReceiptDateTime = new DateTime(),
                SenderId = subjectUser.Id,
                RecipientId = targetUser.Id,
            });
            _context.SaveChanges();
            await Clients.Users(new List<string>() { subjectUser.Name, targetUser.Name }).SendAsync("ReceiveSignal", message);
        }      
    }
}