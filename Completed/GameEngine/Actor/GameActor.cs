using Akka.Actor;
using Akka.Persistence;
using AkkaMjrTwo.GameEngine.Domain;
using System;

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

        public override string PersistenceId => throw new NotImplementedException();

        public GameActor(GameId id)
        {
            _game = Game.Create(id);
        }

        public static Props GetProps(GameId id)
        {
            return Props.Create(() => new GameActor(id));
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message is GameCommand command)
            {
                HandleResult(_game.HandleCommand, command);
            }
            else if (message is TickCountdown)
            {
                if (_game is RunningGame game)
                {
                    HandleChanges(game.TickCountDown());
                }
            }
            else
            {
                Context.System.Log.Warning("Game is not running, cannot update countdown");
            }
            return true;
        }

        protected override bool ReceiveRecover(object message)
        {
            throw new NotImplementedException();
        }


        private void HandleResult(Func<GameCommand, Game> commandHandler, GameCommand command)
        {
            try
            {
                var game = commandHandler.Invoke(command);

                Sender.Tell(new CommandAccepted());

                HandleChanges(game);
            }
            catch (GameRuleViolation violation)
            {
                Sender.Tell(new CommandRejected(violation));
            }
        }

        private void HandleChanges(Game game)
        {

        }

        private void CancelCountdownTick()
        {

        }

        private class TickCountdown
        { }
    }
}
