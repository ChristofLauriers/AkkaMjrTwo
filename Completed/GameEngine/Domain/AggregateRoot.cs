using System.Collections.Generic;
using System.Linq;

namespace AkkaMjrTwo.GameEngine.Domain
{
    public abstract class AggregateRoot<T, E> where T : AggregateRoot<T, E>
    {
        protected Id<T> Id { get; set; }
        protected List<E> UncommitedEvents { get; set; }

        public AggregateRoot(Id<T> id)
        {
            Id = id;
            UncommitedEvents = new List<E>();
        }

        protected abstract T ApplyEvent(E arg);
        protected abstract T MarkCommitted();

        protected T ApplyEvents(params E[] args)
        {
            T result = null;
            foreach (E arg in args)
            {
                result = ApplyEvent(arg);
            }
            return result;
        }
    }
}
