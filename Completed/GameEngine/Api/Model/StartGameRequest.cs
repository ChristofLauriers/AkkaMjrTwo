using System.Collections.Generic;

namespace AkkaMjrTwo.GameEngine.Api.Model
{
    public class StartGameRequest
    {
        public string GameId { get; set; }
        public List<string> Players { get; set; }
    }
}
