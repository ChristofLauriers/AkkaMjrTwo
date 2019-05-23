using Akka;
using Akka.Actor;
using Akka.Streams.Actors;
using AkkaMjrTwo.GameEngine.Domain;
using System.Collections.Generic;
using System.Linq;

namespace AkkaMjrTwo.GameEngine.Actor
{
    public class EventPublisherActor : ActorPublisher<GameEvent>
    {
        private List<GameEvent> _buffer;

        public EventPublisherActor()
        {
            _buffer = new List<GameEvent>();

            Context.System.EventStream.Subscribe(Context.Self, typeof(GameEvent));
        }

        public static Props GetProps()
        {
            return Props.Create<EventPublisherActor>();
        }

        protected override bool Receive(object message)
        {
            return message.Match()
                .With<GameEvent>(ev =>
                {
                    if (_buffer.Count == 0 && TotalDemand > 0)
                    {
                        OnNext(ev);
                    }
                    else
                    {
                        _buffer.Add(ev);
                        DeliverBuffer();
                    }
                })
                .With<Request>(DeliverBuffer)
                .With<Cancel>(() => Context.Stop(Self))
                .WasHandled;
        }

        private void DeliverBuffer()
        {
            if (TotalDemand > 0)
            {
                var use = _buffer.Take((int)TotalDemand).ToList();
                _buffer = _buffer.Skip((int)TotalDemand).ToList();
                use.ForEach(OnNext);
            }
            else
            {
                var use = _buffer.Take(int.MaxValue).ToList();
                _buffer = _buffer.Skip(int.MaxValue).ToList();
                use.ForEach(OnNext);
                DeliverBuffer();
            }
        }
    }
}
