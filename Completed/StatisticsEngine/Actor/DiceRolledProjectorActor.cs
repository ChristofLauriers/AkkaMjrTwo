using Akka.Actor;
using Akka.Event;
using AkkaMjrTwo.Domain;
using System.Linq;
using AkkaMjrTwo.StatisticsEngine.ReadModels;

namespace AkkaMjrTwo.StatisticsEngine.Projectors
{
    public class DiceRolledProjectorActor : ReceiveActor
    {
        public DiceRolledProjectorActor()
        {
            Initialize();
        }

        public static Props GetProps()
        {
            return Props.Create<DiceRolledProjectorActor>();
        }

        private void Initialize()
        {
            Receive<DiceRolled>(Project);
        }

        private static void Project(DiceRolled @event)
        {
            var gameId = @event.Id.Value;
            var player = @event.Player.Value;

            using (var db = new GameStatisticsContext())
            {
                var statistic = db.Statistics.FirstOrDefault(s => s.GameId.Equals(gameId) && s.PlayerId.Equals(player));
                if (statistic != null)
                {
                    statistic.NumberRolled = @event.RolledNumber;
                    db.SaveChanges();
                }
                else
                {
                    Context.GetLogger().Warning("Unable to find GameStatistic readmodel for game id {0} and player id {1}", gameId, player);
                }
            }
        }
    }
}
