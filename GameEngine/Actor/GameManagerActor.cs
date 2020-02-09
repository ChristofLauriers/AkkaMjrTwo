using System;
using Akka.Actor;
using AkkaMjrTwo.Domain;

namespace AkkaMjrTwo.GameEngine.Actor
{
    #region Messages

    public class CreateGame
    { }



    public class SendCommand
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


    public class GameAlreadyExists
    { }

    #endregion

    //Transform GameManagerActor class into an actor
    public class GameManagerActor
    {
        public GameManagerActor()
        {
            //Register handlers for:
            //  - CreateGame messages
            //  - SendCommand messages
        }

        //Add Factory method (GetProps)

        private bool Handle(CreateGame message)
        {
            var id = new GameId($"Game_{Guid.NewGuid().ToString()}");

            //Retrieve child GameActor and check if it already exists.
            //If GameActor already exists:
            //  - Respond with GameAlreadyExists message
            //If GameActor does not exist:
            //  - Create new GameActor as a child
            //  - Respond with GameCreated message

            return true;
        }

        private bool Handle(SendCommand message)
        {
            //Retrieve child GameActor and check if it already exists.
            //If GameActor exists:
            //  - Forward the command
            //If GameActor does not exist:
            //  - Respond with GameDoesNotExist message

            return true;
        }
    }
}
