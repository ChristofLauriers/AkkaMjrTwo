using Akka.Actor;
using AkkaMjrTwo.Domain;
using AkkaMjrTwo.StatisticsEngine.ReadModels;

namespace AkkaMjrTwo.StatisticsEngine.Projectors
{
    //Transform GameStartedProjectorActor class into an actor
    public class GameStartedProjectorActor
    {
        public GameStartedProjectorActor()
        {
            Initialize();
        }

        //Add Factory method (GetProps)

        private void Initialize()
        {
            //Register Project message handler
        }

        private static void Project(GameStarted @event)
        {
            //Create new statistics projection records for each player using GameStatisticsContext
        }
    }
}
