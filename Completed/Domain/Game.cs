using AkkaMjrTwo.Domain.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AkkaMjrTwo.Domain
{
    public abstract class Game : AggregateRoot<Game, GameEvent>
    {
        public bool IsFinished => this is FinishedGame;
        public bool IsRunning => this is RunningGame;

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
                if (this is UninitializedGame uninitializedGame)
                {
                    return uninitializedGame.Start(game.Players);
                }
                else throw new GameAlreadyStartedViolation();
            }

            if (command is RollDice dice)
            {
                if (this is RunningGame runningGame)
                {
                    return runningGame.Roll(dice.Player);
                }
                else throw new GameNotRunningViolation();
            }
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

            RegisterUncommitedEvents(new GameStarted(GameId, players, new Turn(firstPlayer, GlobalSettings.TurnTimeoutSeconds)));

            return this;
        }

        public override Game ApplyEvent(GameEvent @event)
        {
            Game game = this;
            if (@event is GameStarted gameStarted)
            {
                game = new RunningGame(GameId, gameStarted.Players, gameStarted.InitialTurn, UncommitedEvents);
            }

            MarkCommitted(@event);

            return game;
        }
    }



    public class RunningGame : Game
    {
        private readonly Random _random;
        private readonly List<KeyValuePair<PlayerId, int>> _rolledNumbers;
        private readonly List<PlayerId> _players;

        private Turn _turn;

        public RunningGame(GameId id, List<PlayerId> players, Turn turn, List<GameEvent> uncommitedEvents)
            : base(id)
        {
            _random = new Random();
            _rolledNumbers = new List<KeyValuePair<PlayerId, int>>();
            _players = players;
            _turn = turn;

            UncommitedEvents = uncommitedEvents;
        }

        public Game Roll(PlayerId player)
        {
            if (_turn.CurrentPlayer.Equals(player))
            {
                var rolledNumber = _random.Next(1, 7);
                var diceRolled = new DiceRolled(GameId, rolledNumber, player);

                var nextPlayer = GetNextPlayer();
                if (nextPlayer != null)
                {
                    RegisterUncommitedEvents(diceRolled, new TurnChanged(GameId, new Turn(nextPlayer, GlobalSettings.TurnTimeoutSeconds)));
                }
                else
                {
                    RegisterUncommitedEvents(diceRolled, new GameFinished(GameId, BestPlayers()));
                }
                return this;
            }
            else throw new NotCurrentPlayerViolation();
        }

        public Game TickCountDown()
        {
            var countdownUpdated = new TurnCountdownUpdated(GameId, _turn.SecondsLeft - 1);
            if (_turn.SecondsLeft <= 1)
            {
                var timedOut = new TurnTimedOut(GameId);
                var nextPlayer = GetNextPlayer();
                if (nextPlayer != null)
                {
                    RegisterUncommitedEvents(timedOut, new TurnChanged(GameId, new Turn(nextPlayer, GlobalSettings.TurnTimeoutSeconds)));
                }
                else
                {
                    RegisterUncommitedEvents(timedOut, new GameFinished(GameId, BestPlayers()));
                }
            }
            else
            {
                RegisterUncommitedEvents(countdownUpdated);
            }
            return this;
        }

        public override Game ApplyEvent(GameEvent @event)
        {
            Game game = this;
            if (@event is TurnChanged turnChanged)
            {
                _turn = turnChanged.Turn;
            }
            if (@event is DiceRolled diceRolled)
            {
                if (!_rolledNumbers.Exists(x => x.Key.Equals(diceRolled.Player)))
                {
                    _rolledNumbers.Add(new KeyValuePair<PlayerId, int>(diceRolled.Player, diceRolled.RolledNumber));
                }
            }
            if (@event is TurnCountdownUpdated turnCountdownUpdated)
            {
                _turn.SecondsLeft = turnCountdownUpdated.SecondsLeft;
            }
            if (@event is GameFinished gameFinished)
            {
                game = new FinishedGame(GameId, _players, gameFinished.Winners, UncommitedEvents);
            }

            MarkCommitted(@event);

            return game;
        }

        private List<PlayerId> BestPlayers()
        {
            var highest = HighestRolledNumber();
            var best = _rolledNumbers.Where(x => x.Value == highest).Select(x => x.Key).ToList();

            return best;
        }

        private int HighestRolledNumber()
        {
            if (!_rolledNumbers.Any())
                return -1;

            return _rolledNumbers.Select(x => x.Value).Max();
        }

        private PlayerId GetNextPlayer()
        {
            var currentPlayerIndex = _players.IndexOf(_turn.CurrentPlayer);
            var nextPlayerIndex = currentPlayerIndex + 1;

            return _players.ElementAtOrDefault(nextPlayerIndex);
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
