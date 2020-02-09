# Statistics Engine

This project represents the read side of the CQRS pattern. It contains the projections and read models.

The process of projecting is executed by a set of event handlers, which essentially are just methods executed 
whenever a specific type of event comes in. These methods perform CRUD operation upon the persistent read model.

For the convenience, we will be grouping all event handlers into projector classes based on the read model that they 
are working with.

We'll be using Entity Framework Core and SQL Server as our persistence layer.

## Architecture

<p align="center">
  <img src="https://github.com/ChristofLauriers/AkkaMjrTwo/blob/master/StatisticsEngine/Akka%20Major%202%20Statisitics.png">
</p>

## Workshop Steps

These steps describe the actions to complete the code and make the game work. Completed examples are provided in the Completed folder.

It's a given that you try to figure out how to complete te code yourself before taking a look at the completed examples  :)

### StatisticsProjectionActor class
_This actor will convert events into a structural representation. A structural representation 
(which is being updated as we traverse the event stream) we call read models._
<br/>
<br/>
1. Transform this class into a ReceiveActor.

2. Add a factory method necessary to create this actor.

3. Register message handlers for GameStarted, DiceRolled & GameFinished events.

4. Implement Project method for GameStarted events
    * Create new statistics read model for each player using GameStatisticsContext.

5. Implement Project method for DiceRolled events
    * Update NumberRolled in statistics read model for current player using GameStatisticsContext.

6. Implement Project method for GameFinished events
    * Flag winners in statistics read model using GameStatisticsContext