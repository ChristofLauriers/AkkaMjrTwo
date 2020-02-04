using Akka.Actor;
using Akka.Event;
using AkkaMjrTwo.Domain;
using AkkaMjrTwo.StatisticsEngine.ReadModels;
using System.Linq;

namespace AkkaMjrTwo.StatisticsEngine.Projectors
{
    //Transform GameFinishedProjectorActor class into an actor
    public class GameFinishedProjectorActor
    {
        public GameFinishedProjectorActor()
        {
            Initialize();
        }

        //Add Factory method (GetProps)

        private void Initialize()
        {
            //Register Project message handler
        }

        private static void Project(GameFinished @event)
        {
            //Flag winners in statistics projection records using GameStatisticsContext
        }
    }
}
