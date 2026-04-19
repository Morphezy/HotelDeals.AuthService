using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs;

public class AuthHub : Hub
{

    private readonly ConcurrentDictionary<string, string> _users = new(); 
    public async Task JoinRegistration(string name)
    {
        if (_users.TryAdd(name, Context.ConnectionId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, name);
        }
        else
        {
            await Clients.Caller.SendAsync("Registration is already pending", name);
        }
    }

    public  override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = _users.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        if (group != null)
        {
            _users.TryRemove(group, out _);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
