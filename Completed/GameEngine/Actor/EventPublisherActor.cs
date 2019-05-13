using Akka.Actor;
using Akka.Streams.Actors;
using AkkaMjrTwo.GameEngine.Domain;
using System.Collections.Generic;
using System.Linq;

namespace AkkaMjrTwo.GameEngine.Actor
{
    public class EventPublisherActor : ActorPublisher<GameEvent>
    {
        private List<GameEvent> _eventCache;

        public EventPublisherActor()
        {
            _eventCache = new List<GameEvent>();

            Context.System.EventStream.Subscribe(Context.Self, typeof(GameEvent));
        }

        public static Props GetProps()
        {
            return Props.Create<EventPublisherActor>();
        }

        protected override bool Receive(object message)
        {
            if(message is Request)
            {
                while (IsActive && TotalDemand > 0 && _eventCache.Count > 0)
                {
                    var head = _eventCache.First();
                    var tail = _eventCache.Skip(1);

                    OnNext(head);

                    _eventCache = tail.ToList();
                }
            }
            else if (message is GameEvent @event)
            {
                if (IsActive && TotalDemand > 0)
                {
                    OnNext(@event);
                }
                else
                {
                    _eventCache.Add(@event);
                }
            }
            return true;
        }
    }
}
