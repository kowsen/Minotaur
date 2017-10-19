# Minotaur
A simple, bare bones Entity Component System for C#. The goal isn't to be the most performant or memory efficient ecs in the world. It's just to make one performant and expandable enough for my relatively simple Monogame projects, and to help me learn how best to utilize an ecs by building one up myself.

Minotaur contains the ECS library. It follows the Artemis model of Components being pure data and Systems doing all the work.

The sample folder contains a sample Android and Windows project using the Minotaur framework, with the bulk of the logic shared in SampleLogic.

## GameObject
An arbitrary global object that is accessible in all Scenes and Systems. All of the template arguments given to any of the below classes on initialization should be the same arbitrary type, which is also passed into the World object on initialization.

This is where you can store global game state variables, event busses, as well as engine-specific components that systems need to access (for Monogame, this may include a SpriteBatch, Viewport, etc).

## IComponent
Holds some data relating to an entity. A component has no game logic and no update / draw function. It's just data.

## Entity
Represents a game object consisting of some number of IComponents. In reality, the entity doesn't actually contain components, it just has a unique Id and a reference to its EntityComponentManager.

## System
A unit of game logic. On initialization we define a "component signature" consisting of required and restricted components to select which entities we want the logic to process. Then there are overridable functions for updating and drawing that will be called with entities that match the signature whenever update and draw are called on the containing World.

Also has an OnActivate and OnDeactivate function, to be called when the containing Scene is made active or inactive. This is the best place to register and unregister for any events that GameObject contains.

### EntitySystem
Requires a component signature. Update and Draw functions are called with matching entities as arguments

### GameSystem
Optional component signature. Update and Draw functions are called without entities. Used for logic that doesn't operate on entities, or where entities are only needed in response to an event.

## Scene
Initialization logic for a series of entities and / or systems. Has overridable functions for initialization and loading content.

## EntityComponentManager
The real meat of the project

## SystemManager
asdf

## World
asdf

## ComponentSignatureManager
asdf

## EntitySet
asdf

## EventBus
asdf