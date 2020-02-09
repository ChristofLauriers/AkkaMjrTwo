# Game Engine

This project contains the game engine of the dice game
and handles all game commands & messages. This is the heart of the game.
It represents the write side of the CQRS pattern. 
This project uses Akka.NET Persistence as event sourcing provider.

## Architecture

Game engine has a REST API as entry point. This REST API is used to issue different game commands. 
Those commands are handled by Akka.NET actors (see Actor Hierarchy).

<p align="center">
  <img src="https://github.com/ChristofLauriers/AkkaMjrTwo/blob/master/GameEngine/Game%20Engine%20Architecture.png">
</p>

## Actor Hierarchy

<p align="center">
  <img src="https://github.com/ChristofLauriers/AkkaMjrTwo/blob/master/GameEngine/ActorHierarchy.png">
</p>

## Game Engine Details

<p align="center">
  <img src="https://github.com/ChristofLauriers/AkkaMjrTwo/blob/master/GameEngine/Game%20Engine%20Flow.png">
</p>

<p align="center">
  <img src="https://github.com/ChristofLauriers/AkkaMjrTwo/blob/master/GameEngine/Game%20Engine%20Timer.png">
</p>

## Workshop Steps

These steps describe the actions to complete the code and make the game work. Completed examples are provided in the Completed folder.

It's a given that you try to figure out how to complete te code yourself before taking a look at the completed examples  :)

_**General rule:** Try to use Akka Pattern Match if you need to conditionally check messages. (See Akka cheat sheet)_

### Startup class
Create a new actor system named DiceGameSystem. Use the ConfigureActorSystem method.

### GameActor class
_This actor is the heart of the game. It is responsible for the following things_
* _Handle game commands and apply the resulting events to the domain._
* _Handle the turn timer._
1. Transform this class into a PersistentActor.

2. Add a factory method necessary to create this actor.

3. Create a method to handle incoming messages by implementing ReceiveCommand. 
This method has to react to GameCommands as well as TickCountdown messages.
	* Delegate GameCommand execution to the domain using the existing HandleResult method.
    * Apply a tick countdown on the domain and persist the message using the HandleChanges method. 
    (Only apply on a running game)

4. Create a method to handle recovery (state re-build) by implement ReceiveRecover. This handler must
not have side-effects other than changing persistent actor state i.e. it should
not perform actions that may fail, such as interacting with external services, for example.
	* Apply all GameEvents on the domain.
    * Schedule a countdown tick if recovery is completed to continue the game in it's current state.

5. Complete HandleResult method by replying command execution result to the sender.

6. Complete HandleChanges method. This method will make sure all uncommitted events in the domain are persisted. 
The following actions need to happen after the events are persisted:
	* re-apply the events to make sure the domain is in the correct persisted state. 
    * Mark events commited.
    * Publish the event using the PublishEvent method.
    * Countdown ticker management
      * Schedule a new countdown ticker if the event was a GameStarted event.
      * Cancel current countdown ticker and start a new one if the event was a TurnChanged event.
      * Cancel current countdown ticker and stop the actor if the event was a GameFinished event.

### GameManagerActor class
_This actor's sole responsibility is to create game actors and forward game commands to them. 
It also acts as the supervisor for it's child actors. Supervision is out of scope for this workshop, 
but you can read up on it [here][1]. This is the single entry point into the actor system_	
<br/>
<br/>
1. Transform this class into a ReceiveActor.

2. Add a factory method necessary to create this actor.

3. Register message handlers for CreateGame & SendCommand messages.
	
4. Implement the CreateGame handler. This handler is used to create a new game and needs to handle 
the message in the following manner
	* Try to retrieve an existing child GameActor.
        * If the GameActor exists => Respond with GameAlreadyExists message.
        * If the GameActor does not exist:
			* Create new GameActor as a child.
            * Respond with GameCreated message.

5. Implement the SendCommand handler. This handler is used to forward commands to child GameActor actors.
	* Try to retrieve an existing child GameActor.
		* If the GameActor exists => Forward the command
        * If the GameActor does not exist => Respond with GameDoesNotExist message

### GameController class
_This class is used to create the REST api for the game engine. The REST api is the door into the game engine. It is used to
issue commands from the user interface into the game._
<br/>
<br/>
You'll need to implement the following endpoints:
1. Create, used to create a new game.
	* Send a CreateGame message to the GameManagerActor.
    * Return feedback. 

2. Start, used to start an uninitialized game.
	* Send a SendCommand message containing a StartGame command to the GameManagerActor.

3. Roll, used to issue a roll dice command.
    * Send a SendCommand message containing a RollDice command to the GameManagerActor.
		
- Startup
	- ConfigureActorSystem method
		- Create the ActorSystem with name "DiceGameSystem" (tip: use ConfigurationLoader to bootstap Akka configuration)

[1]: https://getakka.net/articles/concepts/supervision.html "Akka.NET Supervision"		
