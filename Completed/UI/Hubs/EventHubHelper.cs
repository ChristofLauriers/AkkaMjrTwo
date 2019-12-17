using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaMjrTwo.Domain;
using AkkaMjrTwo.UI.Actor;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AkkaMjrTwo.UI.Hubs
{
    public class EventHubHelper : IHostedService
    {
        private readonly IHubContext<EventHub> _hub;
        private readonly ILogger<EventHub> _logger;
        private readonly ActorSystem _actorSystem;

        public EventHubHelper(IHubContext<EventHub> hub, ILogger<EventHub> logger, ActorSystem actorSystem)
        {
            _hub = hub;
            _logger = logger;
            _actorSystem = actorSystem;
        }

        public async Task PublishEvent(string gameId, GameEvent @event)
        {
            if (@event is GameStarted started)
            {
                await _hub.Clients.Group(gameId).SendAsync("broadcastEvent", new { EventType = @event.GetType().Name, Event = started });
            }
            if (@event is DiceRolled rolled)
            {
                await _hub.Clients.Group(gameId).SendAsync("broadcastEvent", new { EventType = @event.GetType().Name, Event = rolled });
            }
            if (@event is TurnChanged turnChanged)
            {
                await _hub.Clients.Group(gameId).SendAsync("broadcastEvent", new { EventType = @event.GetType().Name, Event = turnChanged });
            }
            if (@event is TurnCountdownUpdated turnCntDwnUpdated)
            {
                await _hub.Clients.Group(gameId).SendAsync("broadcastEvent", new { EventType = @event.GetType().Name, Event = turnCntDwnUpdated });
            }
            if (@event is TurnTimedOut timeout)
            {
                await _hub.Clients.Group(gameId).SendAsync("broadcastEvent", new { EventType = @event.GetType().Name, Event = timeout });
            }
            if (@event is GameFinished finished)
            {
                await _hub.Clients.Group(gameId).SendAsync("broadcastEvent", new { EventType = @event.GetType().Name, Event = finished });
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ICanTell subscriber = _actorSystem.ActorSelection("/user/EventSubscriber");
            if (subscriber == ActorRefs.Nobody)
            {
                _logger.LogError("EventSubscriberActor not found in ActorSystem.");
            }
            subscriber.Tell(new SetHub(this), ActorRefs.NoSender);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
