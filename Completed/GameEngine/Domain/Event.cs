using System.Collections.Generic;

namespace AkkaMjrTwo.GameEngine.Domain
{
    public abstract class GameEvent
    {
        protected GameId Id { get; private set; }

        protected GameEvent(GameId id)
        {
            Id = id;
        }
    }

    public class DiceRolled : GameEvent
    {
        public int Rollednumber { get; private set; }

        public DiceRolled(GameId id, int rolledNumber)
            : base(id)
        {
            Rollednumber = rolledNumber;
        }
    }

    public class GameStarted : GameEvent
    {
        public List<PlayerId> Players { get; private set; }
        public Turn InitialTurn { get; private set; }

        public GameStarted(GameId id, List<PlayerId> players, Turn initialTurn)
            : base(id)
        {
            Players = players;
            InitialTurn = initialTurn;
        }
    }

    public class TurnChanged : GameEvent
    {
        public Turn Turn { get; private set; }

        public TurnChanged(GameId id, Turn turn)
            : base(id)
        {
            Turn = turn;
        }
    }

    public class TurnCountdownUpdated : GameEvent
    {
        public int SecondsLeft { get; private set; }

        public TurnCountdownUpdated(GameId id, int secondsLeft)
            : base(id)
        {
            SecondsLeft = secondsLeft;
        }
    }

    public class TurnTimedOut : GameEvent
    {
        public TurnTimedOut(GameId id)
            : base(id)
        {
        }
    }

    public class GameFinished : GameEvent
    {
        public List<PlayerId> Winners { get; private set; }

        public GameFinished(GameId id, List<PlayerId> winners)
            : base(id)
        {
            Winners = winners;
        }
    }
}
