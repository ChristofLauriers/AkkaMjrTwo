using Akka.Actor;
using Akka.Event;
using AkkaMjrTwo.Domain;
using System.Linq;
using AkkaMjrTwo.StatisticsEngine.ReadModels;

namespace AkkaMjrTwo.StatisticsEngine.Projectors
{
    //Transform StatisticsProjectorActor class into an actor
    public class StatisticsProjectorActor
    {
        public StatisticsProjectorActor()
        {
            Initialize();
        }

        //Add Factory method (GetProps)

        private void Initialize()
        {
            //Register Project message handlers
        }

        private static void Project(GameStarted @event)
        {
            //Create new statistics projection records for each player using GameStatisticsContext
        }

        private static void Project(DiceRolled @event)
        {
            //Update NumberRolled in statistics projection record for current player using GameStatisticsContext
        }

        private static void Project(GameFinished @event)
        {
            //Flag winners in statistics projection records using GameStatisticsContext
        }
    }
}
