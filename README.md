# Coders Guild Workshop - CQRS & EventSourcing with Akka.NET
This is the second Akka.NET workshop.

In this workshop we’ll create a really simple (yet complete) event-sourced game. 
It won’t be anything spectacular thus its rules are as simple as:
- game creator specifies players taking part
- each player, one after another, has an opportunity to roll the dice
- each player’s opportunity to roll is time limited, if player won’t roll within the limit, his opportunity is gone
- winners are all players who share the highest rolled number

This workshop is a .NET Core port based on https://scalac.io/event-sourced-game-implementation-example-part-1-3-getting-started/

<p align="center">
  <img src="https://github.com/ChristofLauriers/AkkaMjrTwo/blob/master/Architecture.png">
</p>
