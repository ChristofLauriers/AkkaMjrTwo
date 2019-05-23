using Akka;
using Akka.Actor;
using Akka.Persistence;
using AkkaMjrTwo.GameEngine.Domain;
using System;
using System.Collections.Generic;

namespace AkkaMjrTwo.GameEngine.Actor
{
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



    public class GameActor : PersistentActor
    {
        private Game _game;
        private GameId _id;
        private List<ICancelable> _cancelable;

        public override string PersistenceId => _id.Value;

        public GameActor(GameId id)
        {
            _id = id;
            _game = Game.Create(id);
            _cancelable = new List<ICancelable>();
        }

        public static Props GetProps(GameId id)
        {
            return Props.Create(() => new GameActor(id));
        }

        protected override bool ReceiveCommand(object message)
        {
            return message.Match()
                .With<GameCommand>(cmd =>
                {
                    HandleResult(_game.HandleCommand, cmd);
                })
                .With<TickCountdown>(() =>
                {
                    if (_game is RunningGame game)
                    {
                        _game = game.TickCountDown();
                        HandleChanges();
                    }
                })
                .Default((o) =>
                {
                    Context.System.Log.Warning("Game is not running, cannot update countdown");
                    CancelCountdownTick();
                })
                .WasHandled;
        }

        protected override bool ReceiveRecover(object message)
        {
            return message.Match()
                .With<GameEvent>((ev) =>
                {
                    _game = _game.ApplyEvent(ev);
                })
                .With<RecoveryCompleted>(() =>
                {
                    if (_game.IsRunning)
                    {
                        ScheduleCountdownTick();
                    }
                })
                .WasHandled;
        }

        protected override void OnPersistFailure(Exception cause, object @event, long sequenceNr)
        {
            base.OnPersistFailure(cause, @event, sequenceNr);
        }

        protected override void OnPersistRejected(Exception cause, object @event, long sequenceNr)
        {
            base.OnPersistRejected(cause, @event, sequenceNr);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            base.PreRestart(reason, message);
        }

        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
        }

        private void HandleResult(Func<GameCommand, Game> commandHandler, GameCommand command)
        {
            try
            {
                _game = commandHandler.Invoke(command);

                Sender.Tell(new CommandAccepted());

                HandleChanges();
            }
            catch (GameRuleViolation violation)
            {
                Sender.Tell(new CommandRejected(violation));
            }
        }

        private void HandleChanges()
        {
            foreach (var @event in _game.UncommitedEvents)
            {
                Persist(@event, ev =>
                {
                    _game = _game.ApplyEvent(ev).MarkCommitted();

                    PublishEvent(ev);

                    ev.Match()
                      .With<GameStarted>(() =>
                      {
                          ScheduleCountdownTick();
                      })
                      .With<TurnChanged>(() =>
                      {
                          CancelCountdownTick();
                          ScheduleCountdownTick();
                      })
                      .With<GameFinished>(() =>
                      {
                          CancelCountdownTick();
                          Context.Stop(this.Self);
                      });
                });
            }
        }

        private void PublishEvent(GameEvent @event)
        {
            Context.System.EventStream.Publish(@event);
        }

        private void ScheduleCountdownTick()
        {
            var cancelable = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1), this.Self, new TickCountdown(), ActorRefs.NoSender);

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
