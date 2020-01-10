using System;

namespace AkkaMjrTwo.Domain
{
    public class GameId : Id<Game>
    {
        public GameId(string value)
            : base(value)
        {
        }
    }
}
