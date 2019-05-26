using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AkkaMjrTwo.UI.Hubs
{
    public class EventHub : Hub
    {
        private readonly ILogger<EventHub> _logger;

        public EventHub(ILogger<EventHub> logger)
        {
            _logger = logger;
        }

        public async Task AddToGroup(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

            _logger.LogInformation($"{Context.ConnectionId} has joined the group {gameId}.");
        }

        public async Task RemoveFromGroup(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);

            _logger.LogInformation($"{Context.ConnectionId} has left the group {gameId}.");
        }
    }
}
