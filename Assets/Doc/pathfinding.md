Pathfinding (Build007)
======================

Introduction
------------
The lofty end goal of the pathfinding implementation is to (eventually) address some of the more interesting challenges beyond simple 'best first search'. Pathfinding by itself is fairly straightforward if we're talking about finding a solution in a well defined, discrete searchspace from a 'starting point' to a 'goal point'. Best first search has been around for a while after all. Beyond that however, things get messy and therefore interesting. 

This is a short write up on the challenges we face when implementing pathfinding, followed by a high level description of the current implementation in build007


Challenges faced by complete pathfinding solution
-------------------------------------------------
Here are some issues which make pathfinding considering more challenging than just finding a solution through a simple search space. First of all I would argue 'pathfinding' is just one aspect to the overall problem we're trying to solve there are more related aspects such as  making agents move in an efficient, believable way through a world. For the sake of convenience we'll group all of these under the term "pathfinding". In any case let's just scope a subset of the challenges in terms of questions which need to be answered.

* Requirements: Do we need pathfinding, can we get away with simple evasion or a waypoint system ?
* Search space representation: What representation provides the 'best' approach for the world? Is the world 2D, 2.5D, 3D or 4D? Can we use a grid, do we need a graph or a mesh? What aspects do we need to capture, distance, access, ease of traversing and so on? How are transitions between nodes represented, what aspects do we need to capture (eg width / height of transition, one way access, special access via levers, swimming) ?  
* Search space dynamics: is the search space static or does it change because parts get blown up or new blockages get introduced ?
* Search space construction: does someone come in and hand-build a search space or should the construction be automated ? If it's the latter what constraints do we need to impose ? Does the search space get generated based on existing data, is the game developed with a gameplay-first or art-first approach, ie do you constraint your level design to well defined ai-friendly elements or does level / art design get free reign? 
* Performance & Scaling: how do we manage a large amount of agents performing search in a manner which makes the engine / systems people generally very unhappy (ie it requires a lot of run-time allocations)? How do we make sure a solution is found in a predictable time? How do we utilize the available threads (often via a job system)? How do we make sure trivial searches (ie where there is a direct path) are avoided to save intialization time?	How / can we reuse solutions found ?
* Correctness: How to we make sure the found solution is the best solution? What is the 'best' solution anyway? Is the shortest path, least dangerous path, paths ? What if we want to introduce some randomness for variation purposes ? 
* Fidelity & planning: We want the agents to move in a natural way. How do we avoid agents walking in very straight paths without compromising correctness ?

These are just some of the challenges I could come with from the top of my head and I still haven't found a comprehensive overview of a methodoly addressing all these (most of the time) related issues in depth. The latter will be due to my lack of in-depth research but also because it's an engineering problem that is for the most part application specific, ie there probably won't be one 'best' approach which allows for an easy to understand, well written description.

Current implementation in build007
----------------------------------

For the game in development, I'm trying to address a subset of these problems. Most of the implementation in the current version resides 'Pathfinding' directory. The implementation at build007 aims to:

* Try to prepare the implementation for an entity oriented implementation (https://www.google.com/search?q=entity+component+system+unity&oq=entity+component+system+un&aqs=chrome.0.0j69i57j0l4.4216j0j7&sourceid=chrome&ie=UTF-8)
* Offer a generic Best First Search implementation
* Create a shared service which will take care of sharing the resources required for pathfinding and sharing paths which have already been resolved
* Provide a state machine for agents to manage the pathfinding process
* Make it testable

Not all these goals are completely met. The goal to prepare for the entity component / job system specifically is only more a small step towards a data driven apporach but other other than that, it is a not too terrible of a first pass implementation. To see how this is achieved, let's briefly break down the code:

# Generic Best First Search (Tds.PathFinder.BestFistSearch) 
A simple BFS which gets its 'generic' qualifications from the fact that the application specific functionality is abstracted via lambda functions for cost calculation, node expansion and distance determination. 

I suspect this will have to be rewritten completely to work with ECS as the current system uses Lambdas, HashSets, Lists and ObjectPools.

# A search service to share resources and results (Tds.PathFinder.SearchService)
The searchservice provides the methods to start a search and and retrieve (or cancel) the results somewhere in the future. Currently there is a simple test to see if another agent has performed a similar search by checking the start and end nodes, if these match any other start-end combinations of search, the result or pending result will be returned.

# An agent pathfinding state management service (Tds.PathFinder.AgentPathingService)
This service updates an agent pathing state (Tds.PathFinder.AgentPathingState) to provide a direction an agent should move to in order to move from one location in the dungeon to another. The AgentPathingState is a class which could be easily broken down into components to fit an ECS. 


From an agent's perspective, the only way to interact with the pathfinding is via the AgentPathingService. It provides an initialized / updated AgentPathingState with an agentLocation and a targetLocation and queries the PathingState (state)  and movementDirection to figure out if it should move and in what direction. 

The AgentPathingService updates the agent pathing states (idle, awaiting ticket, finding a path, following a path and following a target). The search related states are delegated to the SearchService. The other states are about finding (way)points to move to and making sure the current path is still valid.

The SearchService has a number of resources available to perform searches, this includes a cache of searches in progress - and search results as well as some BFS algorithms to perform these searches. 

In the directory 'DungeonPathfinding' a number of application specific classes provide the application implementation for generic pathfinding classes.  'PathfindingServiceBehaviour' provides a monobehaviour wrapper for the 'pathingservice'. 'DungeonSearch' offers a factory method to create BFS searches for DungeonNodes. 'DungeonLayoutSearchSpace' defines a dungeon node specific search space. 

  


 


 



 


