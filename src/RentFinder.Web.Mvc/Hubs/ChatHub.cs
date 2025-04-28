using Microsoft.AspNetCore.SignalR;

namespace RentFinder.Web.Mvc.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string senderId, string receiverId, string message)
    {
        await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message);
    }

    public async Task JoinChat(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    }
}