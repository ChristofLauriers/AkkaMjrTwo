using Akka.Actor;
using AkkaMjrTwo.Domain;
using AkkaMjrTwo.StatisticsEngine.ReadModels;

namespace AkkaMjrTwo.StatisticsEngine.Projectors
{
    public class GameStartedProjectorActor : ReceiveActor
    {
        public GameStartedProjectorActor()
        {
            Initialize();
        }

        public static Props GetProps()
        {
            return Props.Create<GameStartedProjectorActor>();
        }

        private void Initialize()
        {
            Receive<GameStarted>(Project);
        }

        private static void Project(GameStarted @event)
        {
            var gameId = @event.Id.Value;

            using (var db = new GameStatisticsContext())
            {
                foreach (var player in @event.Players)
                {
                    db.Add(new GameStatistic
                    {
                        GameId = gameId,
                        PlayerId = player.Value
                    });
                }
                db.SaveChanges();
            }
        }
    }
}
