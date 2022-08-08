# Minotaur
A simple, bare bones Entity Component System for C#. The goal isn't to be the most performant or memory efficient ecs in the world. It's just to make one performant and expandable enough for my relatively simple Monogame projects, and to help me learn how best to utilize an ecs by building one up myself.

Minotaur contains the ECS library. It follows the Artemis model of Components being pure data and Systems doing all the work.

The sample folder contains a sample Android and Windows project using the Minotaur framework, with the bulk of the logic shared in SampleLogic.

## GameObject
An arbitrary global object that is attached to and accessible by all Scenes and Systems. All of the template arguments given to any of the below classes on initialization should be of your GameObject's type, and the object passed in to World on initialization should be your global GameObject.

This is where you can store global game state variables, event busses, as well as engine-specific components that systems need to access (for Monogame, this may include a SpriteBatch, Viewport, etc).

## Component
Holds some data relating to an entity. A component has no game logic and no update / draw function. It's just data.

## Entity
Represents a game object consisting of some number of Components. In reality, the entity doesn't actually contain components, it just has a unique Id and a reference to its EntityComponentManager that all calls are passed through to.

## System
A unit of game logic. On initialization we define a "component signature" consisting of required and restricted components to select which entities we want the logic to process. Whenever update and draw are called on the containing World object, overridable functions for updating and drawing will be called on the system with entities that match the signature.

Also has an OnActivate and OnDeactivate function, to be called when the containing Scene is made active or inactive. This is the best place to register and unregister for any events that GameObject contains.

Can also call functions that pass through to the containing EntityComponentManager to create new entities and to manually fetch all entities matching the system's signature.

#### EntitySystem
Requires a component signature. Update and Draw functions are called with matching entities as arguments

#### GameSystem
Optional component signature. Update and Draw functions are called without entities. Used for logic that doesn't operate on entities, or where entities are only needed in response to an event.

## Scene
Initialization logic for a series of entities and / or systems. Has overridable functions for initialization and loading content.

## EntityComponentManager
The real meat of the project. Though conceptually components live in entities, in reality they all live in here in a mapping from an entity id to a listing of components. This lets us more easily optimize our matchers when modifications are made to entities.

Along with that component mapping, we have a mapping from component signatures to all matching components. These matcher lists are initialized the first time a system queries for entities matching a signature, and are updated whenever entities are modified in a way that changes whether the entity qualifies to be in the matcher.

All of this is designed so calls to get all entities that match a signature (which will be called every frame of the update loop at least for every EntitySystem) are super fast at the price of a bit more work whenever we modify what components an Entity has.

## SystemManager
Holds all the systems, attaches the GameObject and EntityComponentManager they're initialized with to them, and passes relevant calls (update, draw, activate, etc) through to each system.

## World
Holds all scenes, attaches them to the GameObject they're initialized with, and allows easy switching and resetting of scenes.

## ComponentSignatureManager
Generates easily comparble signatures from a list of required and a list of restricted components. These signatures are a BitSet where each component type is given two indices. Even indices being 1 represents that component being required, odd indices being 1 represents that component being restricted.

## EntitySet
A List of entities that also allows us to do subqueries on them. For instance, when given an EntitySet containing all entities with position components, we could query for all that also have player flag components. This is optionally given to an EntitySystem's Draw and Update functions as an argument instead of each individual entity.

These queries work by ORing the signature that created the initial set with a passed in signature to get a composite, then fetching entities from the attached EntityComponentManager the same way a system would when it updates. Thus we benefit from the caching described in the EntityComponentManager section.

## EventBus
A utility class that doesn't directly connect to anything else, but will likely be useful to add to a GameObject as a global event bus.