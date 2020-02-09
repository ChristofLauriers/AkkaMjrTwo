# Coders Guild Workshop - CQRS & EventSourcing with Akka.NET
This is the second Akka.NET workshop.

In this workshop we’ll create a really simple (yet complete) event-sourced game. 
The rules are:
- game creator specifies players taking part
- each player, one after another, has an opportunity to roll the dice
- each player’s opportunity to roll is timeboxed, if a player didn't roll on time, his opportunity is gone
- winners are all players that rolled the highest number

This workshop is a .NET Core 3 port based on https://scalac.io/event-sourced-game-implementation-example-part-1-3-getting-started/

<p align="center">
  <img src="https://github.com/ChristofLauriers/AkkaMjrTwo/blob/master/Architecture.png">
</p>


_DISCLAIMER:_
<br/>
_The code in this repository is created to demonstrate some core concept about CQRS and Event Sourcing using Akka.NET
in the context of a workshop, this code is far from production-ready!_
