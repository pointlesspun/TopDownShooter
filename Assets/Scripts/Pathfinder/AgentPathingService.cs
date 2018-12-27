/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.PathFinder
{
    using Tds.Util;
    using UnityEngine;

    /// <summary>
    /// Service guiding the agent's pathing across a dungeon
    /// </summary>
    public static class AgentPathingService
    {
        /// <summary>
        /// Updates the current state of the state.
        /// </summary>
        /// <param name="state">State containing all information to find a path</param>
        /// <param name="settings">Settings for executing the pathfinding</param>
        /// <param name="service">Service which finds a path through a dungeon</param>
        /// <param name="searchSpace">Layout of the dungeon (searchspace)</param>
        /// <param name="time">Current time</param>
        public static void UpdateState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            switch (state.state)
            {
                case PathingState.Idle:
                    UpdateIdleState(state, context);

                    break;

                case PathingState.FindingPath:
                    UpdatePathfindingState(state, context);

                    break;

                case PathingState.FollowingPath:
                    UpdateFollowingPath(state, context);

                    break;
            }
        }

        /// <summary>
        /// Initializes the state such that it represents an idle state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="time"></param>
        public static void InitializeIdleState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class
        {
            state.stateStartTime = context.time;
            state.state = PathingState.Idle;
            state.pathfindingTicket = -1;
            state.agentNode = null;
            state.targetNode = null;
            state.targetLocation = state.agentLocation;
            state.targetStartLocation = state.agentLocation;
        }

        /// <summary>
        /// If the target is outside the target distance threshold as defined in settings, proceed to finding a path
        /// </summary>
        /// <param name="state"></param>
        /// <param name="settings"></param>
        /// <param name="time"></param>
        public static void UpdateIdleState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            if (!IsDistanceInRange(state.targetLocation, state.agentLocation, context.settings.waypointDistance))
            {
                InitializePathfindingState(state, context);
            }
        }

        /// <summary>
        /// Initializes the state such that it represents an pathfinding state state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="time"></param>
        public static void InitializePathfindingState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            state.stateStartTime = context.time;
            state.state = PathingState.FindingPath;
            state.pathfindingTicket = -1;
            state.agentNode = null;
            state.targetNode = null;
            state.waypointIndex = 0;
        }

        /// <summary>
        /// Cancels any outstanding searches started by the given state and sets the state up for idle
        /// </summary>
        /// <param name="state"></param>
        /// <param name="service"></param>
        /// <param name="time"></param>
        public static void CancelPathfinding<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            if ( state.pathfindingTicket != -1 )
            {
                //Contract.Requires(context.service.ValidateTicket(state.pathfindingTicket, state.agentNode, state.targetNode), "Cannot cancel a ticket which is not owned");
                context.service.CancelSearch(state.pathfindingTicket);
                state.pathfindingTicket = -1;
            }

            InitializeIdleState(state, context);
        }

        /// <summary>
        /// Attempts to find a path to a given target. When found proceeds to the pathfollowing state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="settings"></param>
        /// <param name="service"></param>
        /// <param name="searchSpace"></param>
        /// <param name="time"></param>
        public static void UpdatePathfindingState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            // does the state have an outstanding ticket to find a path
            if (state.pathfindingTicket < 0)
            {
                // prevent re-spamming the search service. If no ticket was found, wait a random amount of time
                if ((context.time - state.lastTimeoutCheck) > 0.1f + 0.1f * Random.value)
                {
                    // has a path been defined ?
                    if (state.agentNode == null)
                    {
                        state.agentNode = context.searchSpace.FindNearestSolution(state.agentLocation - context.settings.worldOffset, -1);
                        state.targetNode = context.searchSpace.FindNearestSolution(state.targetLocation - context.settings.worldOffset, -1);

                        state.isTargetNodeApproximation =  !state.targetNode.Bounds.Contains(state.targetStartLocation);

                        state.targetStartLocation = state.targetLocation;
                    }

                    state.pathfindingTicket = context.service.BeginSearch(state.agentNode, state.targetNode, state.GetHashCode());
                    state.lastTimeoutCheck = context.time;
                }
            }

            // started pathfinding ?
            if (state.pathfindingTicket >= 0)
            {
                // has target moved beyond a certain range and do we need to reset the pathfinding?
                if (ShouldRestartPathfinding(state, context))
                {
                    CancelPathfinding(state, context);
                    InitializePathfindingState(state, context);
                }
                else
                {
                    // Contract.Requires(context.service.ValidateTicket(state.pathfindingTicket, state.agentNode, state.targetNode), "" + state.GetHashCode());
                    
                    // check if a result is available
                    if (context.service.RetrieveResult(state.pathfindingTicket, state.agentNode,
                                                            state.targetNode, state.pathNodes, context.searchSpace) != null)
                    {
                        // clear the ticket as the agent no longer has ownership over the associated search
                        // (and avoid canceling other agent's search setup)                   
                        state.pathfindingTicket = -1;
                        InitializePathFollowingState(state, context);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the state such that it represents an pathfollowing state state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="time"></param>
        public static void InitializePathFollowingState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            state.stateStartTime = context.time;
            state.pathNodeIndex = 0;
            state.state = PathingState.FollowingPath;
            state.lastTimeoutCheck = context.time;

            Contract.RequiresForAllSequential(state.pathNodes,
                (index, n1, n2) => n1 == null || n2 == null || context.searchSpace.AreNeighbours(n1, n2));

            GetNextWaypoints(state, context, true);
        }

        /// <summary>
        /// Heuristic to determine whether or not the agent should restart pathfinding if the agent is in the 
        /// pathfollowing state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool ShouldRestartPathfinding<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            if ( state.state == PathingState.FollowingPath )
            {
                // only check so often
                if ((context.time - state.lastTimeoutCheck) > context.settings.pathValidatyCheckTimeout)
                {
                    state.lastTimeoutCheck = context.time;

                    if (state.isTargetNodeApproximation)
                    {
                        // make a guess if the target has moved away base on the context
                        return !IsDistanceInRange(state.targetLocation,
                                    state.targetStartLocation, context.settings.targetDistanceThreshold);
                    }
                    else
                    {
                        // need to restart if the target has left its current node
                        return !state.targetNode.Bounds.Contains(state.targetLocation);
                    }
                }
            }

            return false;
        }

        public static void UpdateFollowingPath<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            // has target moved beyond a certain range and do we need to reset the pathfinding?
            if (ShouldRestartPathfinding(state, context))
            {
                InitializePathfindingState(state, context);
            }
            else
            {
                // is the agent within range of the target ?
                if (IsDistanceInRange(state.agentLocation, state.targetStartLocation, context.settings.waypointDistance))
                {
                    InitializeIdleState(state, context);
                }
                else
                { 
                    // is the agent within range of the current waypoint
                    if (IsDistanceInRange(state.waypoints[state.waypointIndex], state.agentLocation, context.settings.waypointDistance))
                    {
                        state.waypointIndex++;

                        if (state.waypointIndex == state.waypoints.Length)
                        {
                            // proceed to the next node
                            T lastNode = state.pathNodes[state.pathNodes.Length - 1];

                            // go to the next waypoint
                            state.pathNodeIndex = Mathf.Min(state.pathNodeIndex + 1, state.pathNodes.Length);

                            // check if the previous node in the list was the 'closest' node (targetNode)
                            if ((state.pathNodeIndex == state.pathNodes.Length - 1
                                || state.pathNodes[state.pathNodeIndex] == null) && lastNode != state.targetNode)
                            {
                                // agent's buffer was too small to contain the full path, 
                                // continue pathfinding from the current position
                                InitializePathfindingState(state, context);
                            }
                            else
                            {
                                // No more waypoints retrieve the next set.
                                GetNextWaypoints(state, context, true);
                            }
                        }
                    }

                    state.movementDirection = state.waypoints[state.waypointIndex] - state.agentLocation;
                }
            }
        }
        

        /// <summary>
        /// Check if the given points are within range of each other
        /// xxx move to extension for Vector2
        /// </summary>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool IsDistanceInRange(Vector2 target, Vector2 position, float range)
        {
            return (target - position).sqrMagnitude < range * range;
        }


        /// <summary>
        /// returns the position of next waypoint if any, otherwise returns the endpoint
        /// </summary>
        /// <param name="index"></param>
        /// <param name="nodes"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static void GetNextWaypoints<T>(AgentPathingState<T> agentState, AgentPathingContext<T> context,
                                                                                        bool randomize = false) where T : class, IBounds
        {
            var index = agentState.pathNodeIndex;

            agentState.waypoints[0] = agentState.targetStartLocation;
            agentState.waypointIndex = 0;

            if (index < agentState.pathNodes.Length - 1)
            {
                var node = agentState.pathNodes[index];
                var nextNode = agentState.pathNodes[index + 1];

                // node may be null - in which case we've reached the end of the path
                if (node != null && nextNode != null && node != nextNode)
                {
                    context.searchSpace.GetWaypoints(node, nextNode, agentState.waypoints, context.settings.worldOffset, randomize);
                }
            }
        }
    }
}
