Overview & contents of the directory structure (Build007)
=========================================================

Util
----
Collection of miscellaneous code used to various circumstances, such as Gizmo utilities, generic datastructures (Grid2D, Objectpool) 
or correctness tools (Contract).

CommonBehaviours
----------------
Several MonoBehaviours which are relatively context-free, ie they can be added to gameobjects without any dependencies any thing else
(both conceptual and in terms of coupling).

Pathfinder
----------
Code used to provide pathfinding functionality. Pathfinding consists of
* a standard best first search algorithm, 
* a search service which pools shared searches between agents against a predictable performance cost and
* a pathing state management approach for agents build in a fashion which I hope will make it easier to transist to the entity system

DungeonGeneration
-----------------
As the name implies, classes and behaviours aimed at creating randomized dungeons.

DungeonPathing
--------------
The pathfinder classes provide a generic approach to pathfinding without committing to a specific implementation (or at least, that is 
the intention). The DungeonPathing takes the pathfinder functionality and applies it to the dungeons created by the Dungeon Generation 
code.

GameScripts
-----------
All code related to the actual game logic. Currently a mixed back: animations, ai, game-logic, level elements can all be found in here.

Scenescripts
------------
The scripts driving the individual scenes. These are the overarching objects which resolve tthe various game object dependencies without
coupling / hard coding these dependencies in the actual code. ie the InGameStateBehaviour creates all dynamic objects such as the dungeon / level
and provides these to for instance the ai (Director).

UI
---
As the name implies : UI elements for the game.



