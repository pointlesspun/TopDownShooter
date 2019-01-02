/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.PathFinder
{
    using Tds.Util;

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

                case PathingState.AwaitingTicket:
                    UpdateAwaitingTicketState(state, context);
                    break;

                case PathingState.FindingPath:
                    UpdatePathfindingState(state, context);
                    break;

                case PathingState.FollowingPath:
                    UpdateFollowingPath(state, context);
                    break;

                case PathingState.FollowingTarget:
                    FollowTarget(state, context);
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
            state.lastTimeoutCheck = context.time;
            state.state = PathingState.Idle;
            state.pathfindingTicket = -1;
            state.agentNode = null;
            state.targetNode = null;
            state.targetLocation = state.agentLocation;
            state.targetStartLocation = state.agentLocation;

            // // Debug.Log("Idle");
        }

        /// <summary>
        /// If the target is outside the target distance threshold as defined in settings, proceed to finding a path
        /// </summary>
        /// <param name="state"></param>
        /// <param name="settings"></param>
        /// <param name="time"></param>
        public static void UpdateIdleState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            // only periodically do something
            if (context.IsTimeout(state.lastTimeoutCheck))
            {
                state.lastTimeoutCheck = context.time;

                // is the target not in range of the agent
                if (!state.targetLocation.IsInRange(state.agentLocation, context.settings.waypointDistance))
                {
                    // find the closest solution - ie the dungeon node closest to the target
                    var solution = context.searchSpace.FindNearestSolution(state.targetLocation - context.settings.worldOffset, -1);

                    if (solution != null)
                    {
                        // convert the agent location to dungeon space
                        if (!solution.Bounds.Contains(state.agentLocation - context.settings.worldOffset))
                        {
                            // need to find a path
                            InitializeAwaitingTicketState(state, context);
                        }
                        else
                        {
                            // target is in the same node, start following the target immediately
                            state.targetNode = solution;
                            InitializeTargetFollowingState(state, context);
                        }
                    }
                }    
            }
        }

        /// <summary>
        /// Initializes the state such that it represents an pathfinding state state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="time"></param>
        public static void InitializeAwaitingTicketState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            state.stateStartTime = context.time;
            state.state = PathingState.AwaitingTicket;
            state.pathfindingTicket = -1;
            state.agentNode = null;
            state.targetNode = null;
            state.waypointIndex = 0;

            // Debug.Log(state.GetHashCode() + " Awaiting ticket");
        }

        /// <summary>
        /// Cancels any outstanding searches started by the given state and sets the state up for idle
        /// </summary>
        /// <param name="state"></param>
        /// <param name="service"></param>
        /// <param name="time"></param>
        public static void CancelPathfinding<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            // Debug.Log(state.GetHashCode() + " canceling ticket " + state.pathfindingTicket);
        
            if (state.pathfindingTicket != -1)
            {
                Contract.Requires(context.service.ValidateTicket(state.pathfindingTicket, state.agentNode, state.targetNode), "Cannot cancel a ticket which is not owned");
                context.service.ReleaseSearch(state.pathfindingTicket, state.GetHashCode());
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
        public static void UpdateAwaitingTicketState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            // prevent re-spamming the search service. If no ticket was found, wait a random amount of time
            if (context.IsTimeout(state.lastTimeoutCheck))
            {
                // has a path been defined ?
                if (state.agentNode == null)
                {
                    state.agentNode = context.searchSpace.FindNearestSolution(state.agentLocation - context.settings.worldOffset, -1);
                    state.targetNode = context.searchSpace.FindNearestSolution(state.targetLocation - context.settings.worldOffset, -1);

                    // if the target is outside the bounds of the closest node, the result is an approximation and
                    // not a perfect solution. We need to keep track of this scenario for when the agent 
                    // reaches the end, it should know it can go into idle when there is no perfect solution.
                    state.isTargetNodeApproximation = !state.targetNode.Bounds.Contains(state.targetStartLocation - context.settings.worldOffset);

                    state.targetStartLocation = state.targetLocation;
                }

                // if the end is the same as the target, there is no need to find a path, continue to pathfinding
                if (state.agentNode == state.targetNode)
                {
                    InitializeTargetFollowingState(state, context);
                }
                else
                {
                    state.pathfindingTicket = context.service.BeginSearch(state.agentNode, state.targetNode, state.GetHashCode());
                    state.lastTimeoutCheck = context.time;

                    if (state.pathfindingTicket >= 0)
                    {
                        InitializePathfindingState(state, context);
                    }
                }
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
            state.waypointIndex = 0;
            // Debug.Log(state.GetHashCode() + " Finding path");
        }

        public static void UpdatePathfindingState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            // has target moved beyond a certain range and do we need to reset the pathfinding?
            if (ShouldRestartPathfinding(state, context))
            {
                CancelPathfinding(state, context);
                InitializeAwaitingTicketState(state, context);
            }
            else
            {
                Contract.Requires(context.service.ValidateTicket(state.pathfindingTicket, state.agentNode, state.targetNode), "" + state.GetHashCode());

                // check if a result is available
                if (context.service.RetrieveResult(state.pathfindingTicket, state.agentNode,
                                                        state.targetNode, state.pathNodes, context.searchSpace) != null)
                {
                    context.service.ReleaseSearch(state.pathfindingTicket, state.GetHashCode());
                    // clear the ticket as the agent no longer has ownership over the associated search
                    // (and avoid canceling other agent's search setup)                   
                    state.pathfindingTicket = -1;
                    InitializePathFollowingState(state, context);
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

            // // // Debug.Log("Following path");
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
            // only check so often
            if (context.IsTimeout(state.lastTimeoutCheck))
            {
                state.lastTimeoutCheck = context.time;

                if (state.isTargetNodeApproximation)
                {
                    // make a guess if the target has moved away base on the context
                    return !state.targetLocation.IsInRange(state.targetStartLocation, context.settings.targetDistanceThreshold);
                }
                else
                {
                    // need to restart if the target has left its current node
                    return !state.targetNode.Bounds.Contains(state.targetLocation - context.settings.worldOffset);
                }
            }

            return false;
        }

        public static void UpdateFollowingPath<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            // has target moved beyond a certain range and do we need to reset the pathfinding?
            if (ShouldRestartPathfinding(state, context))
            {
                InitializeAwaitingTicketState(state, context);
            }
            // is the agent within range of the current waypoint ?
            else if (state.waypoints[state.waypointIndex].IsInRange(state.agentLocation, context.settings.waypointDistance))
            {
                // has the agent reached the target node ?
                if (state.targetNode.Bounds.Contains(state.agentLocation - context.settings.worldOffset)) 
                {
                    InitializeTargetFollowingState(state, context);
                }
                else
                {
                    ProgressToNextWaypoint(state, context);
                    state.movementDirection = state.DirectionToWaypoint;
                }
            }
            else
            {
                state.movementDirection = state.DirectionToWaypoint;
            }
        }

        /// <summary>
        /// Moves the state to the next waypoint. If there are no more waitpoints, the next state will be either
        /// * AwaitingTicket if the target is not in the current node and the last path node has been reached
        /// * Following target is the target is in the current node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="state"></param>
        /// <param name="context"></param>
        public static void ProgressToNextWaypoint<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            state.waypointIndex++;

            // no more waypoints to go to ?
            if (state.waypointIndex >= state.waypoints.Length)
            {
                // go to the next node
                state.pathNodeIndex = state.NextNodeIndex;

                // is the new pathnode index the last in the node list
                if (state.HasReachedLastNode())
                {
                    if (state.CurrentNode != state.targetNode)
                    {
                        // As the current node is not the same as node the agent was meant to go 
                        // to, this implies the agent's buffer was too small to contain the full path.
                        // So continue pathfinding from the current position
                        InitializeAwaitingTicketState(state, context);
                    } 
                    else
                    {
                        // target node has been reached - start moving towards the goal
                        InitializeTargetFollowingState(state, context);
                    }
                }
                else
                {
                    // No more waypoints retrieve the next set.
                    GetNextWaypoints(state, context, true);
                }
            }
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

        public static void InitializeTargetFollowingState<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            state.stateStartTime = context.time;
            state.state = PathingState.FollowingTarget;

             // Debug.Log(state.GetHashCode() + " Following target");
        }

        /// <summary>
        /// Updates the following target state and its exit conditions which are:
        ///  * Target has moved to a different node (start pathfinding)
        ///  * Target is in range (start idling)
        ///  * Target is not in any node, if the agent is close to the best possible point start idling
        /// 
        /// else set the movement direction towards the target
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="state"></param>
        /// <param name="context"></param>
        public static void FollowTarget<T>(AgentPathingState<T> state, AgentPathingContext<T> context) where T : class, IBounds
        {
            // has target moved beyond a certain range and do we need to reset the pathfinding?
            if (ShouldRestartPathfinding(state, context))
            {
                InitializeAwaitingTicketState(state, context);
            }
            // has reached the target
            else if (state.targetLocation.IsInRange(state.agentLocation, context.settings.waypointDistance))
            {
                InitializeIdleState(state, context);
            }
            else if ( state.isTargetNodeApproximation )
            {
                var bestPoint = RectUtil.Clamp(state.targetNode.Bounds, state.targetLocation, 0.1f);

                if (bestPoint.IsInRange(state.agentLocation, context.settings.waypointDistance))
                {
                    InitializeIdleState(state, context);
                }
                else
                {
                    state.movementDirection = bestPoint - state.agentLocation;
                }
            }
            else
            {
                state.movementDirection = state.targetLocation - state.agentLocation;
            }
        }
    }
}
