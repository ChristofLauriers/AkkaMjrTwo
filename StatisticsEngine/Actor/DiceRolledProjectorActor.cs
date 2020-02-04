using Akka.Actor;
using Akka.Event;
using AkkaMjrTwo.Domain;
using System.Linq;
using AkkaMjrTwo.StatisticsEngine.ReadModels;

namespace AkkaMjrTwo.StatisticsEngine.Projectors
{
    //Transform DiceRolledProjectorActor class into an actor
    public class DiceRolledProjectorActor
    {
        public DiceRolledProjectorActor()
        {
            Initialize();
        }

        //Add Factory method (GetProps)

        private void Initialize()
        {
            //Register Project message handler
        }

        private static void Project(DiceRolled @event)
        {
            //Update NumberRolled in statistics projection record for current player using GameStatisticsContext
        }
    }
}
