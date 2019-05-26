using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using AkkaMjrTwo.Domain;
using AkkaMjrTwo.UI.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AkkaMjrTwo.UI.Actor
{
    public class SetHub : INoSerializationVerificationNeeded
    {
        public SetHub(EventHubHelper hub)
        {
            Hub = hub;
        }
        public EventHubHelper Hub { get; }
    }



    public class EventSubscriberActor : ReceiveActor, IWithUnboundedStash
    {
        private EventHubHelper _hub;

        public IStash Stash { get; set; }

        public EventSubscriberActor()
        {
            WaitingForHub();
        }

        private void WaitingForHub()
        {
            Receive<SetHub>(h =>
            {
                _hub = h.Hub;
                Become(HubAvailable);
                Stash.UnstashAll();
            });

            ReceiveAny(_ => Stash.Stash());
        } 

        private void HubAvailable()
        {
            var mediator = DistributedPubSub.Get(Context.System).Mediator;

            // subscribe to the topic named "game_event"
            mediator.Tell(new Subscribe($"game_event", Self));

            ReceiveAsync<GameEvent>(async @event =>
            {
                await _hub.PublishEvent(@event.Id.Value, @event);
            });

            Receive<SubscribeAck>(subscribeAck =>
            {
                if (subscribeAck.Subscribe.Topic.Equals("game_event")
                    && subscribeAck.Subscribe.Ref.Equals(Self)
                    && subscribeAck.Subscribe.Group == null)
                {
                    Context.System.Log.Info("subscribing");
                }
            });
        }
    }
}