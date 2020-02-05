# Game Engine

This project contains the game engine of the dice game
and handles all game commands & messages. This is the heart of the game.

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
