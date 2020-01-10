using Akka.Actor;
using Akka.Event;
using AkkaMjrTwo.Domain;
using AkkaMjrTwo.StatisticsEngine.ReadModels;
using System.Linq;

namespace AkkaMjrTwo.StatisticsEngine.Projectors
{
    public class GameFinishedProjectorActor : ReceiveActor
    {
        public GameFinishedProjectorActor()
        {
            Initialize();
        }

        public static Props GetProps()
        {
            return Props.Create<GameFinishedProjectorActor>();
        }

        private void Initialize()
        {
            Receive<GameFinished>(Project);
        }

        private static void Project(GameFinished @event)
        {
            var gameId = @event.Id.Value;

            using (var db = new GameStatisticsContext())
            {
                foreach (var player in @event.Winners)
                {
                    var statistic = db.Statistics.FirstOrDefault(s => s.GameId.Equals(gameId) && s.PlayerId.Equals(player.Value));
                    if (statistic != null)
                    {
                        statistic.Winner = true;
                    }
                    else
                    {
                        Context.GetLogger().Warning("Unable to find GameStatistic readmodel for game id {0} and player id {1}", gameId, player);
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
