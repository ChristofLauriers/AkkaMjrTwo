using AkkaMjrTwo.GameEngine.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AkkaMjrTwo.GameEngine.Domain
{
    public abstract class Game : AggregateRoot<Game, GameEvent>
    {
        public bool IsFinished { get { return this is FinishedGame; } }
        public bool IsRunning { get { return this is RunningGame; } }

        protected GameId GameId => Id as GameId;

        protected Game(GameId id)
            : base(id)
        {
        }

        public static UninitializedGame Create(GameId id)
        {
            return new UninitializedGame(id);
        }

        public Game HandleCommand(GameCommand command)
        {
            if (command is StartGame game)
            {
                if (this is UninitializedGame)
                {
                    return (this as UninitializedGame).Start(game.Players);
                }
                else throw new GameAlreadyStartedViolation();
            }

            if (command is RollDice dice)
            {
                if (this is RunningGame)
                {
                    return (this as RunningGame).Roll(dice.Player);
                }
                else throw new GameNotRunningViolation();
            }
            return this;
        }

        public override Game MarkCommitted()
        {
            UncommitedEvents = new List<GameEvent>();
            return this;
        }
    }



    public class UninitializedGame : Game
    {
        public UninitializedGame(GameId id)
            : base(id)
        {
        }

        public Game Start(List<PlayerId> players)
        {
            if (players.Count < 2)
            {
                throw new NotEnoughPlayersViolation();
            }

            var firstPlayer = players.First();
            ApplyEvents(new GameStarted(GameId, players, new Turn(firstPlayer, GlobalSettings.TurnTimeoutSeconds)));
            return this;
        }

        public override Game ApplyEvent(GameEvent arg)
        {
            if (arg is GameStarted)
            {
                var evnt = arg as GameStarted;

                UncommitedEvents.Add(evnt);
                return new RunningGame(GameId, evnt.Players, evnt.InitialTurn, UncommitedEvents);
            }
            return this;
        }
    }



    public class RunningGame : Game
    {
        private readonly Random _random;
        private readonly List<KeyValuePair<PlayerId, int>> _rolledNumbers;

        public List<PlayerId> Players { get; private set; }
        public Turn Turn { get; private set; }

        public RunningGame(GameId id, List<PlayerId> players, Turn turn, List<GameEvent> uncommitedEvents)
            : base(id)
        {
            _random = new Random();
            _rolledNumbers = new List<KeyValuePair<PlayerId, int>>();

            Players = players;
            Turn = turn;
            UncommitedEvents = uncommitedEvents;
        }

        public Game Roll(PlayerId player)
        {
            if (Turn.CurrentPlayer.Equals(player))
            {
                var rolledNumber = _random.Next(1, 7);
                var diceRolled = new DiceRolled(GameId, rolledNumber);

                var nextPlayer = GetNextPlayer();
                if (nextPlayer != null)
                {
                    ApplyEvents(diceRolled, new TurnChanged(GameId, new Turn(nextPlayer, GlobalSettings.TurnTimeoutSeconds)));
                }
                else
                {
                    var game = ApplyEvent(diceRolled);
                    if (game is RunningGame)
                    {
                        ApplyEvent(new GameFinished(GameId, BestPlayers()));
                    }
                }
                return this;
            }
            else throw new NotCurrentPlayerViolation();
        }

        public List<PlayerId> BestPlayers()
        {
            var highest = HighestRolledNumber();
            return _rolledNumbers.Where(x => x.Value == highest).Select(x => x.Key).ToList();
        }

        public int HighestRolledNumber()
        {
            return _rolledNumbers.Select(x => x.Value).Max();
        }

        public Game TickCountDown()
        {
            var countdownUpdated = new TurnCountdownUpdated(GameId, Turn.SecondsLeft - 1);
            if (Turn.SecondsLeft <= 1)
            {
                var timedOut = new TurnTimedOut(GameId);
                var nextPlayer = GetNextPlayer();
                if (nextPlayer != null)
                {
                    ApplyEvents(countdownUpdated, timedOut, new TurnChanged(GameId, new Turn(nextPlayer, GlobalSettings.TurnTimeoutSeconds)));
                }
                else
                {
                    ApplyEvents(countdownUpdated, timedOut, new GameFinished(GameId, BestPlayers()));
                }
            }
            else
            {
                ApplyEvent(countdownUpdated);
            }
            return this;
        }

        public override Game ApplyEvent(GameEvent arg)
        {
            Game game = this;
            if (arg is TurnChanged)
            {
                var evnt = arg as TurnChanged;
                Turn = evnt.Turn;
                UncommitedEvents.Add(arg);
            }
            if (arg is DiceRolled)
            {
                var evnt = arg as DiceRolled;
                _rolledNumbers.Add(new KeyValuePair<PlayerId, int>(Turn.CurrentPlayer, evnt.Rollednumber));
                UncommitedEvents.Add(arg);
            }
            if (arg is TurnCountdownUpdated)
            {
                var evnt = arg as TurnCountdownUpdated;
                Turn.SecondsLeft = evnt.SecondsLeft;
                UncommitedEvents.Add(arg);
            }
            if (arg is GameFinished)
            {
                var evnt = arg as GameFinished;
                UncommitedEvents.Add(arg);
                return new FinishedGame(GameId, Players, evnt.Winners, UncommitedEvents);
            }
            if (arg is TurnTimedOut)
            {
                UncommitedEvents.Add(arg);
            }
            return game;
        }

        private PlayerId GetNextPlayer()
        {
            var currentPlayerIndex = Players.IndexOf(Turn.CurrentPlayer);
            var nextPlayerIndex = currentPlayerIndex + 1;

            return Players.ElementAtOrDefault(nextPlayerIndex);
        }
    }



    public class FinishedGame : Game
    {
        public List<PlayerId> Players { get; private set; }
        public List<PlayerId> Winners { get; private set; }

        public FinishedGame(GameId id, List<PlayerId> players, List<PlayerId> winners, List<GameEvent> uncommitedEvents)
            : base(id)
        {
            Players = players;
            Winners = winners;
            UncommitedEvents = uncommitedEvents;
        }

        public override Game ApplyEvent(GameEvent arg)
        {
            return this;
        }
    }



    public class Turn
    {
        public PlayerId CurrentPlayer { get; set; }
        public int SecondsLeft { get; set; }

        public Turn(PlayerId currentPlayer, int secondsLeft)
        {
            CurrentPlayer = currentPlayer;
            SecondsLeft = secondsLeft;
        }
    }
}
