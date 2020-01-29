using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaMjrTwo.Domain;
using AkkaMjrTwo.GameEngine.Actor;
using AkkaMjrTwo.GameEngine.Attributes;
using AkkaMjrTwo.GameEngine.Infrastructure;
using AkkaMjrTwo.GameEngine.Model;
using Microsoft.AspNetCore.Mvc;

namespace AkkaMjrTwo.GameEngine.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IActorRef _gameManagerActor;

        public GameController(GameManagerActorProvider gameManagerActorProvider)
        {
            _gameManagerActor = gameManagerActorProvider();
        }

        [RequestLoggingActionFilter]
        [Route("create")]
        [HttpPost]
        public async Task<ActionResult> Create()
        {
            //- Send a GameCreated message to the GameManagerActor
            //- Return feedback
            return Ok();
        }

        [RequestLoggingActionFilter]
        [Route("start")]
        [HttpPost]
        public async Task<ActionResult> Start(StartGameRequest request)
        {
            var playerIds = new List<PlayerId>();
            foreach (var str in request.Players)
            {
                playerIds.Add(new PlayerId(str));
            }

            //Send a SendCommand command containing a StartGame GameCommand to the GameManagerActor
            
            return Ok(new { Result = feedback.GetType().Name });
        }

        [RequestLoggingActionFilter]
        [Route("roll")]
        [HttpPost]
        public async Task<ActionResult> Roll(RollDiceRequest request)
        {
            //Send a SendCommand command containing a RollDice GameCommand to the GameManagerActor

            return Ok(new { Result = feedback.GetType().Name });
        }
    }
}
