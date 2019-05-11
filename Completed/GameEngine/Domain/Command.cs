using System.Collections.Generic;

namespace AkkaMjrTwo.GameEngine.Domain
{
    public abstract class GameCommand
    {
    }

    public class StartGame : GameCommand
    {
        public List<PlayerId> Players { get; private set; }

        public StartGame(List<PlayerId> players)
        {
            Players = players;
        }
    }

    public class RollDice : GameCommand
    {
        public string PlayerId { get; private set; }

        public RollDice(string playerId)
        {
            PlayerId = playerId;
        }
    }
}
