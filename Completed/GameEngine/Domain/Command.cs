using System.Collections.Generic;

namespace AkkaMjrTwo.GameEngine.Domain
{
    public abstract class GameCommand
    { }



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
        public PlayerId Player { get; private set; }

        public RollDice(PlayerId player)
        {
            Player = player;
        }
    }
}
