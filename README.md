# Minotaur
A simple Entity Component System for C#. The goal isn't to be as performant or memory efficient as possible, but to be "good enough" for use in some Godot projects that don't lend themselves to that engine's inheritance-heavy approach.

Minotaur.Core contains the ECS library. It follows the Artemis model of Components being pure data and Systems doing all the work.

Minotaur.Test has some very basic tests I built while rewriting the library to make sure things are generally working, they are not intended to provide complete test coverage.

## TGame
An arbitrary global object that is attached to and accessible by all Scenes and Systems. All of the template arguments given to any of the below classes on initialization should be of this type.

This is where you can store global game state variables, event busses, and is a good place to interact with a game engine or rendering library that's dealing with actually putting pixels on the screen.

## Component
Holds some data relating to an entity. A component is just data and has no game logic and no update / draw function.

## Entity
Represents a game object consisting of some number of Components. Any functions that modify its structure by either adding / removing components or deleting the entity won't take effect until the end of the current update call.

## Signature
A list of required and restricted component types that can be used to filter the list of all entities down to just the entities a unit of game logic compares about.

## System
A unit of game logic. Contains a signature that determines what entities are passed to its update and draw functions.

## World
Groups together entities, systems, and the game object. The entry point of the ECS that your engine's update loop will call into.