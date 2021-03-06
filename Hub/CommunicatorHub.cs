using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class CommunicatorHub : Hub
{
    CommunicatorDbContext context { get; set; }
    UserRelationRepository userRelationRepository { get; set; }
    public CommunicatorHub(CommunicatorDbContext context, UserRelationRepository userRelationRepository)
    {
        this.context = context;
        this.userRelationRepository = userRelationRepository;
    }

    private async Task TryGetSubjectUser(Func<User, Task> callback)
    {
        User? subjectUser = null;
        if (Context.UserIdentifier is string subjectName)
        {
            subjectUser = context.User.FirstOrDefault(user => user.Name == subjectName);
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
            targetUser = context.User.FirstOrDefault(user => user.Name == targetUserName);
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

    public async Task SendMessage(Guid tempId, string targetUserName, string content)
    {
        await TryGetUsers(targetUserName, async (subjectUser, targetUser) =>
        {
            var targetSenderRelation = context.UsersRelation.FirstOrDefault(relation =>
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
                context.PrivateMessage.Add(message);
                context.SaveChanges();
                await Clients
                    .Users(new List<string>() { subjectUser.Name, targetUser.Name })
                    .SendAsync("ReceiveMessage", tempId, new PrivateMessageDTO(message));
            }
        });
    }

    public async Task NotifyTyping(string targetUserName)
    {
        await Clients
            .User(targetUserName)
            .SendAsync("TypingNotify", Context.UserIdentifier);
    }

    public async Task ReadMessage(List<Guid> messageIds)
    {
        var messages = context
            .PrivateMessage
            .Include(msg => msg.Recipient)
            .Include(msg => msg.Sender)
            .Where(msg => messageIds.Contains(msg.Id))
            .ToList();
        if (messages.Count > 0)
        {
            await TryGetUsers(messages[0].Sender.Name, async (subjectUser, targetUser) =>
            {
                messages.ForEach(message =>
                {
                    if (messages[0].Recipient == subjectUser)
                    {
                        message.ReceiptDateTime = DateTime.Now;
                    }
                });
                context.SaveChanges();
                await Clients
                    .Users(new List<string>() { subjectUser.Name, targetUser.Name })
                    .SendAsync("MessageRead", messages.Select(msg => new PrivateMessageDTO(msg)).ToList());
            });
        }
    }

    public async Task DeleteMessages(List<Guid> messageIds)
    {
        var messages = context
            .PrivateMessage
            .Include(msg => msg.Recipient)
            .Include(msg => msg.Sender)
            .Where(msg => messageIds.Contains(msg.Id)).ToList();
        if (messages.Count > 0)
        {
            await TryGetSubjectUser(async subjectUser =>
            {
                var deletedMessagesInfo = new List<CommunicatorDeleteInfo>();
                DeleteMessagesWithUser(subjectUser, messages, deletedMessagesInfo);
                context.SaveChanges();
                var clients = deletedMessagesInfo.SelectMany(info => new List<string>() { info.Sender, info.Recipient }).Distinct();
                await Clients
                    .Users(clients.Distinct())
                    .SendAsync("MessagesDelete", deletedMessagesInfo);
            });
        }
    }

    private static void DeleteMessagesWithUser(
        User subjectUser,
        List<PrivateMessage> messages,
        List<CommunicatorDeleteInfo> deletedMessagesInfo)
    {
        messages.ForEach(msg =>
        {
            if (msg.Recipient == subjectUser || msg.Sender == subjectUser)
            {
                msg.Deleted = true;
                deletedMessagesInfo.Add(new CommunicatorDeleteInfo() {
                    Sender = msg.Sender.Name, 
                    Recipient = msg.Recipient.Name,
                    Id = msg.Id,
                    Watched = msg.ReceiptDateTime != null
                });
            }
        });
    }
}