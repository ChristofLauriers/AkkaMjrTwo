using System;

namespace AkkaMjrTwo.GameEngine.Domain
{
    public class GameId : Id<Game>
    {
        public GameId(string value)
            : base(value)
        {
        }
    }
}
