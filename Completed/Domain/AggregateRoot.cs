using System.Collections.Generic;
using System.Linq;

namespace AkkaMjrTwo.Domain
{
    public abstract class AggregateRoot<T, E> where T : AggregateRoot<T, E>
    {
        public List<E> UncommitedEvents { get; set; }

        protected Id<T> Id { get; set; }

        protected AggregateRoot(Id<T> id)
        {
            Id = id;
            UncommitedEvents = new List<E>();
        }

        public abstract T ApplyEvent(E arg);
        public abstract T MarkCommitted();

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
