using System;
using Akka.Persistence;

namespace AkkaMjrTwo.GameEngine.Actor
{
    public class GameActor : PersistentActor
    {
        public override string PersistenceId => throw new NotImplementedException();

        protected override bool ReceiveCommand(object message)
        {
            throw new NotImplementedException();
        }

        protected override bool ReceiveRecover(object message)
        {
            throw new NotImplementedException();
        }
    }
}
