using Akka;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using AkkaMjrTwo.Domain;
using AkkaMjrTwo.UI.Hubs;

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
        private const string TopicName = "game_event";

        private EventHubHelper _hub;

        public IStash Stash { get; set; }

        public EventSubscriberActor()
        {
            WaitingForHub();
        }

        private void WaitingForHub()
        {
            ReceiveAny(message =>
            {
                message.Match()
                       .With<SetHub>(h =>
                       {
                           _hub = h.Hub;

                           Become(InitializePubSub);
                       })
                       .Default(o =>
                       {
                           Stash.Stash();
                       });
            });
        }

        private void InitializePubSub()
        {
            ReceiveAny(message =>
            {
                message.Match()
                       .With<SubscribeAck>(subscribeAck =>
                       {
                           if (subscribeAck.Subscribe.Topic.Equals(TopicName)
                               && subscribeAck.Subscribe.Ref.Equals(Self)
                               && subscribeAck.Subscribe.Group == null)
                           {
                               Context.System.Log.Info($"Subscribed to '{TopicName}'");

                               Become(Initialized);

                               Stash.UnstashAll();
                           }
                       })
                       .Default(o =>
                       {
                           Stash.Stash();
                       });
            });

            var mediator = DistributedPubSub.Get(Context.System).Mediator;

            // subscribe to the topic named "game_event"
            mediator.Tell(new Subscribe(TopicName, Self));
        }

        private void Initialized()
        {
            ReceiveAsync<GameEvent>(async @event =>
            {
                await _hub.PublishEvent(@event.Id.Value, @event);
            });
        }
    }
}