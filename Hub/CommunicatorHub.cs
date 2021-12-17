using Microsoft.AspNetCore.SignalR;

public class CommunicatorHub: Hub
{
    public async Task DoSignal(string content)
    {
        await Clients.All.SendAsync("ReceiveSignal", Context.UserIdentifier + " : " + content);
    }
}