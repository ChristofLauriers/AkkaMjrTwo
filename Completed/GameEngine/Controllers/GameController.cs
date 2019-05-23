using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaMjrTwo.GameEngine.Actor;
using AkkaMjrTwo.GameEngine.Api.Attributes;
using AkkaMjrTwo.GameEngine.Api.Model;
using AkkaMjrTwo.GameEngine.Domain;
using AkkaMjrTwo.GameEngine.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace AkkaMjrTwo.GameEngine.Api.Controllers
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
            var feedback = await _gameManagerActor.Ask<GameCreated>(new CreateGame());
            return Ok(feedback);
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

            var msg = new SendCommand(new GameId(request.GameId), new StartGame(playerIds));

            var feedback = await _gameManagerActor.Ask<object>(msg);
            return Ok(new { Result = feedback.GetType().Name });
        }

        [RequestLoggingActionFilter]
        [Route("roll")]
        [HttpPost]
        public async Task<ActionResult> Roll(RollDiceRequest request)
        {
            var msg = new SendCommand(new GameId(request.GameId), new RollDice(new PlayerId(request.PlayerId)));

            var feedback = await _gameManagerActor.Ask<object>(msg);
            return Ok(new { Result = feedback.GetType().Name });
        }
    }
}
