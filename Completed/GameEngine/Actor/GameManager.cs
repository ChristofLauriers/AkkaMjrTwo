using System;
using Akka.Actor;
using AkkaMjrTwo.GameEngine.Domain;

namespace AkkaMjrTwo.GameEngine.Actor
{
    public class Command
    { }



    public class CreateGame : Command
    { }



    public class SendCommand : Command
    {
        public GameId GameId { get; private set; }
        public GameCommand Command { get; private set; }

        public SendCommand(GameId gameId, GameCommand command)
        {
            GameId = gameId;
            Command = command;
        }
    }



    public class GameCreated
    {
        public GameId GameId { get; private set; }

        public GameCreated(GameId gameId)
        {
            GameId = gameId;
        }
    }



    public class GameDoesNotExist
    { }



    public class GameManager : ReceiveActor
    {
        public GameManager()
        {
            Receive<CreateGame>(Handle);
            Receive<SendCommand>(Handle);
        }

        public static Props GetProps()
        {
            return Props.Create<GameManager>();
        }

        private bool Handle(CreateGame command)
        {
            var id = new GameId(Guid.NewGuid().ToString());

            Context.ActorOf(GameActor.GetProps(id), id.Value);

            Sender.Tell(new GameCreated(id));

            return true;
        }

        private bool Handle(SendCommand command)
        {
            var game = Context.Child(command.GameId.Value);
            if (game != ActorRefs.Nobody)
            {
                game.Forward(command.Command);
            }
            else
            {
                Sender.Tell(new GameDoesNotExist());
            }

            return true;
        }
    }
}
