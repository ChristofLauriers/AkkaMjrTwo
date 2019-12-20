using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaMjrTwo.Domain;

namespace AkkaMjrTwo.StatisticsEngine.Actor
{
    public sealed class ProjectorSubscription
    {
        public Type EventType { get; private set; }

        public ProjectorSubscription(Type eventType)
        {
            EventType = eventType;
        }
    }


    public sealed class ReadyToProject
    { }


    public sealed class ProjectEvent
    {
        public Guid Id { get; private set; }
        public GameEvent Event { get; private set; }

        public ProjectEvent(Guid id, GameEvent @event)
        {
            Id = id;
            Event = @event;
        }
    }


    public sealed class ProjectionsCompleted
    {
        public bool Successfully { get; private set; }

        public ProjectionsCompleted(bool successfully)
        {
            Successfully = successfully;
        }
    }

    public class ProjectionCoordinator: ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }

        private ConcurrentDictionary<Type, List<IActorRef>> _subscriptions;
        private Guid _correlationId;
        private IActorRef _projectRequestor;
        private List<IActorRef> _subscribersForEvent;
        private bool _projectionsOk;
        private ICancelable _scheduler;


        public ProjectionCoordinator()
        {
            
        }
    }
}
