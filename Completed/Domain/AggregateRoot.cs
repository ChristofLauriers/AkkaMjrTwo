using System.Collections.Generic;

namespace AkkaMjrTwo.Domain
{
    public abstract class AggregateRoot<T, E> where T : AggregateRoot<T, E>
    {
        public List<E> UncommitedEvents { get; protected set; }

        protected Id<T> Id { get; }

        protected AggregateRoot(Id<T> id)
        {
            Id = id;
            UncommitedEvents = new List<E>();
        }

        public abstract T ApplyEvent(E @event);

        public void MarkCommitted(E @event)
        {
            UncommitedEvents.Remove(@event);
        }

        protected void RegisterUncommitedEvents(params E[] events)
        {
            foreach (var @event in events)
            {
                UncommitedEvents.Add(@event);
            }
        }
    }
}
