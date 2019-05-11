using System;
namespace AkkaMjrTwo.GameEngine.Domain
{
    public class GameId : Id<Game>
    {
        public GameId()
            : base(Guid.NewGuid().ToString())
        {
        }
    }
}
