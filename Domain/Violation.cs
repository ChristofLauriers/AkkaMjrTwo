using System;

namespace AkkaMjrTwo.Domain
{
    public abstract class GameRuleViolation : Exception
    { }

    public class NotEnoughPlayersViolation : GameRuleViolation
    { }

    public class NotCurrentPlayerViolation : GameRuleViolation
    { }

    public class GameAlreadyStartedViolation : GameRuleViolation
    { }

    public class GameNotRunningViolation : GameRuleViolation
    { }
}
