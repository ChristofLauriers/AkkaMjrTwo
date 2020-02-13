using Akka.Actor;
using Akka.Event;
using AkkaMjrTwo.Domain;
using System.Linq;
using AkkaMjrTwo.StatisticsEngine.ReadModels;

namespace AkkaMjrTwo.StatisticsEngine.Projectors
{
    //Transform StatisticsProjectorActor class into an actor
    public class StatisticsProjectionActor
    {
        public StatisticsProjectionActor()
        {
            Initialize();
        }

        //Implement Factory method using Props.Create
        public static Props GetProps()
        {
        }

        private void Initialize()
        {
            //Register Project message handlers
        }

        private static void Project(GameStarted @event)
        {
            //Create new statistics read model for each player using GameStatisticsContext
        }

        private static void Project(DiceRolled @event)
        {
            //Update NumberRolled in statistics read model for current player using GameStatisticsContext
        }

        private static void Project(GameFinished @event)
        {
            //Flag winners in statistics read model using GameStatisticsContext
        }
    }
}
