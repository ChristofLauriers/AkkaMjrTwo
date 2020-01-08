using Akka;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Event;
using AkkaMjrTwo.Domain;
using AkkaMjrTwo.StatisticsEngine.Projectors;

namespace AkkaMjrTwo.StatisticsEngine.Actor
{
    public class EventSubscriberActor : ReceiveActor, IWithUnboundedStash
    {
        private const string TopicName = "game_event";

        public IStash Stash { get; set; }

        public EventSubscriberActor()
        {
            InitializePubSub();
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
            Receive<GameEvent>(ProjectEvent);
        }

        private void ProjectEvent(GameEvent @event)
        {
            IActorRef projector = ActorRefs.Nobody;
            @event.Match()
                  .With<GameStarted>(_ =>
                  {
                      projector = GetProjectorActor(GameStartedProjectorActor.GetProps(), nameof(GameStartedProjectorActor));
                  })
                  .With<DiceRolled>(_ =>
                  {
                      projector = GetProjectorActor(DiceRolledProjectorActor.GetProps(), nameof(DiceRolledProjectorActor));
                  })
                  .With<GameFinished>(_ =>
                  {
                      projector = GetProjectorActor(GameFinishedProjectorActor.GetProps(), nameof(GameFinishedProjectorActor));
                  })
                  .Default(_ =>
                  {
                      Context.GetLogger().Info("Event {0} not projected. No projector for this event.", @event.GetType().Name);
                  });

            if (!projector.Equals(ActorRefs.Nobody))
            {
                projector.Tell(@event);
            }
            else
            {
                Context.GetLogger().Warning("Unable to create or find child projector actor for event {0}", @event.GetType().Name);
            }
        }

        private static IActorRef GetProjectorActor(Props props, string name)
        {
            //Find existing child actor
            var projector = Context.Child(name);
            if (projector.Equals(ActorRefs.Nobody))
            {
                //Create new child actor if not found
                projector = Context.ActorOf(props, name);
            }
            return projector;
        }
    }
}