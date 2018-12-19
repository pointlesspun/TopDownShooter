/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.PathFinder
{
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
        public static void UpdateState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class
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
        public static void UpdateIdleState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class
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
        public static void InitializePathfindingState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class
        {
            state.stateStartTime = context.time;
            state.state = PathingState.FindingPath;
            state.pathfindingTicket = -1;
            state.agentNode = null;
            state.targetNode = null;
        }

        /// <summary>
        /// Cancels any outstanding searches started by the given state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="service"></param>
        /// <param name="time"></param>
        public static void CancelPathfinding<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class
        {
            if ( state.pathfindingTicket != -1 )
            {
                context.service.CancelSearch(state.pathfindingTicket);
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
        public static void UpdatePathfindingState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class
        {
            // does the state have an outstanding ticket to find a path
            if (state.pathfindingTicket < 0)
            {
                // prevent re-spamming the search service. If no ticket was found, wait a random amount of time
                if ((context.time - state.lastTicketRequest) > 0.25f + 0.25f * Random.value)
                {
                    // has a path been defined ?
                    if (state.agentNode == null)
                    {
                        state.agentNode = context.searchSpace.FindNearestSolution(state.agentLocation - context.settings.worldOffset, -1);
                        state.targetNode = context.searchSpace.FindNearestSolution(state.targetLocation - context.settings.worldOffset, -1);
                        state.targetStartLocation = state.targetLocation;
                    }

                    state.pathfindingTicket = context.service.BeginSearch(state.agentNode, state.targetNode);
                    state.lastTicketRequest = context.time;
                }
            }

            // completed pathfinding ?
            if (state.pathfindingTicket >= 0)
            {
                // check if a result is available
                if (context.service.RetrieveResult(state.pathfindingTicket, state.agentNode, state.targetNode, state.pathNodes) != null)
                {
                    InitializePathFollowingState(state, context);
                    state.lastTicketRequest = context.time;
                }
            }
        }

        /// <summary>
        /// Initializes the state such that it represents an pathfollowing state state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="time"></param>
        public static void InitializePathFollowingState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class
        {
            state.stateStartTime = context.time;
            state.waypointIndex = 0;
            state.pathfindingTicket = -1;
            state.waypoint = GetNextWaypointPosition(0, state.pathNodes, state.targetStartLocation - context.settings.worldOffset, context.searchSpace);
            state.state = PathingState.FollowingPath;
        }

        public static void UpdateFollowingPath<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class
        {
            // has target moved beyond a certain range ?
            if ((context.time - state.stateStartTime) > context.settings.pathValidatyCheckTimeout
                && !IsDistanceInRange(state.targetLocation, state.targetStartLocation, context.settings.targetDistanceThreshold))
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
                    if (IsDistanceInRange(state.waypoint + context.settings.worldOffset, state.agentLocation, context.settings.waypointDistance))
                    {
                        T lastNode = state.pathNodes[state.pathNodes.Length - 1];
                        // go to the next waypoint
                        state.waypointIndex = Mathf.Min(state.waypointIndex + 1, state.pathNodes.Length);

                        // check if the previous node in the list was the 'closest' node (targetNode)
                        if ((state.waypointIndex == state.pathNodes.Length -1
                            || state.pathNodes[state.waypointIndex] ==  null) && lastNode != state.targetNode)
                        {
                            // agent's buffer was too small to contain the full path, continue pathfinding from the current position
                            InitializePathfindingState(state, context);
                        }
                        else
                        {
                            state.waypoint = GetNextWaypointPosition(state.waypointIndex, 
                                                                        state.pathNodes,
                                                                            state.targetStartLocation - context.settings.worldOffset,
                                                                            context.searchSpace,
                                                                            true);
                        }
                    }

                    state.movementDirection = (state.waypoint + context.settings.worldOffset) - state.agentLocation;
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
        public static Vector2 GetNextWaypointPosition<T>(int index, 
                                                        T[] nodes, 
                                                        Vector2 endPoint,
                                                        ISearchSpace<T, Vector2> searchSpace,
                                                        bool randomize = false) where T : class
        {
            if (index < nodes.Length - 1)
            {
                var node = nodes[index];

                // node may be null - in which case we've reached the end of the path
                if (node != null)
                {
                    return searchSpace.GetInterpolatedLocation(node, nodes[index + 1], randomize ? Random.value : 0.5f, endPoint);
                }
            }

            return endPoint;
        }
    }
}
