using System;
using System.Collections.Generic;
using Akka;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Persistence;
using AkkaMjrTwo.Domain;

namespace AkkaMjrTwo.GameEngine.Actor
{
    #region Messages

    public abstract class CommandResult
    { }



    public class CommandAccepted : CommandResult
    { }



    public class CommandRejected : CommandResult
    {
        public GameRuleViolation Violation { get; private set; }

        public CommandRejected(GameRuleViolation violation)
        {
            Violation = violation;
        }
    } 

    #endregion

    //Transform GameActor class into a persistent actor
    public class GameActor
    {
        private Game _game;

        private readonly GameId _id;
        private readonly List<ICancelable> _cancelable;

        public GameActor(GameId id)
        {
            _id = id;
            _game = Game.Create(id);
            _cancelable = new List<ICancelable>();
        }

        //Implement Factory method using Props.Create
        public static Props GetProps(GameId id)
        {
        }

        //Implement ReceiveCommand.
        //This method has to react to GameCommands as well as TickCountdown messages
        //(tip: use pattern match to differentiate between event types)
        //  * Delegate GameCommand execution to the domain using the existing HandleResult method.
        //  * Apply a tick countdown on the domain and persist the message using the HandleChanges method.
        //    (Only apply on a running game)
        //  * Unkown message? Log using Akka and stop the countdown ticker.




        //Implement ReceiveRecover to handle recovery (state re-build) 
        //This handler must not have side-effects other than changing persistent actor state i.e.
        //it should not perform actions that may fail, such as interacting with external services
        //  * Apply all GameEvents on the domain.
        //  * Schedule a countdown tick if recovery is completed to continue the game in it's current state.
        //    This may only occur if the game was already running!!
        //(tip: use pattern match to differentiate between event types)





        private void HandleResult(Func<GameCommand, Game> commandHandler, GameCommand command)
        {
            try
            {
                _game = commandHandler.Invoke(command);

                //Reply to the Sender that command is accepted

                HandleChanges();
            }
            catch (GameRuleViolation violation)
            {
                //Reply to the Sender that command is rejected
            }
        }

        private void HandleChanges()
        {
            //Steps:
            //- Persist all uncommitted events (tip: Use base class method PersistAll)
            //- Apply event to game and mark the event committed
            //- publish the event
            //- Manage the countdown ticker (tip: use pattern match to differentiate between event types)
            //  1 event GameStarted -> schedule the countdown ticker
            //  2 event TurnChanged -> Cancel the countdown tick and schedule a new one
            //  3 event GameFinished -> Cancel the coundown tick and stop the actor
        }

        private void PublishEvent(GameEvent @event)
        {
            // Using Akka DistributedPubSub to publish the event
            var mediator = DistributedPubSub.Get(Context.System).Mediator;
            if (mediator.Equals(ActorRefs.Nobody))
            {
                Log.Error($"Unable to publish event { @event.GetType().Name }. Distributed pub/sub mediator not found.");
                return;
            }
            mediator.Tell(new Publish($"game_event", @event));
        }

        private void ScheduleCountdownTick()
        {
            // Using Akka Scheduler as CountDownTicker. 
            var cancelable = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1), Self, new TickCountdown(), ActorRefs.NoSender);

            _cancelable.Add(cancelable);
        }

        private void CancelCountdownTick()
        {
            foreach (var cancelable in _cancelable)
            {
                cancelable.Cancel();
            }
            _cancelable.Clear();
        }

        private class TickCountdown
        { }
    }
}
