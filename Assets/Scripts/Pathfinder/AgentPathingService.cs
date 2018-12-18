/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.PathFinder
{
    using UnityEngine;

    using Tds.DungeonGeneration;

    /// <summary>
    /// Service guiding the agent's pathing across a dungeon
    /// </summary>
    public static class AgentPathingService
    {
        /// <summary>
        /// Updates the current state of the context.
        /// </summary>
        /// <param name="context">Context containing all information to find a path</param>
        /// <param name="settings">Settings for executing the pathfinding</param>
        /// <param name="service">Service which finds a path through a dungeon</param>
        /// <param name="layout">Layout of the dungeon (searchspace)</param>
        /// <param name="time">Current time</param>
        public static void UpdateState(AgentPathingContext context,
                                        AgentPathfindingSettings settings,
                                        PathfindingService<DungeonNode> service,
                                        DungeonLayout layout,
                                        float time)
        {
            switch (context.state)
            {
                case PathingState.Idle:
                    UpdateIdleState(context, settings, time);

                    break;

                case PathingState.FindingPath:
                    UpdatePathfindingState(context, settings, service, layout, time);

                    break;

                case PathingState.FollowingPath:
                    UpdateFollowingPath(context, settings, time);

                    break;
            }
        }

        /// <summary>
        /// Initializes the context such that it represents an idle state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="time"></param>
        public static void InitializeIdleState(AgentPathingContext context, float time)
        {
            context.stateStartTime = time;
            context.state = PathingState.Idle;
            context.pathfindingTicket = -1;
            context.agentNode = null;
            context.targetNode = null;
            context.targetLocation = context.agentLocation;
            context.targetStartLocation = context.agentLocation;
        }

        /// <summary>
        /// If the target is outside the target distance threshold as defined in settings, proceed to finding a path
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        /// <param name="time"></param>
        public static void UpdateIdleState(AgentPathingContext context, AgentPathfindingSettings settings, float time)
        {
            if (!IsDistanceInRange(context.targetLocation, context.agentLocation, settings.waypointDistance))
            {
                InitializePathfindingState(context, time);
            }
        }

        /// <summary>
        /// Initializes the context such that it represents an pathfinding state state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="time"></param>
        public static void InitializePathfindingState(AgentPathingContext context, float time)
        {
            context.stateStartTime = time;
            context.state = PathingState.FindingPath;
            context.pathfindingTicket = -1;
            context.agentNode = null;
            context.targetNode = null;
        }

        /// <summary>
        /// Cancels any outstanding searches started by the given context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="service"></param>
        /// <param name="time"></param>
        public static void CancelPathfinding(AgentPathingContext context, PathfindingService<DungeonNode> service, float time)
        {
            if ( context.pathfindingTicket != -1 )
            {
                service.CancelSearch(context.pathfindingTicket);
            }

            InitializeIdleState(context, time);
        }

        /// <summary>
        /// Attempts to find a path to a given target. When found proceeds to the pathfollowing state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        /// <param name="service"></param>
        /// <param name="layout"></param>
        /// <param name="time"></param>
        public static void UpdatePathfindingState(AgentPathingContext context,
                                                        AgentPathfindingSettings settings,
                                                        PathfindingService<DungeonNode> service,
                                                        DungeonLayout layout,
                                                        float time)
        {
            // does the context have an outstanding ticket to find a path
            if (context.pathfindingTicket < 0)
            {
                // prevent re-spamming the search service. If no ticket was found, wait a random amount of time
                if ((time - context.lastTicketRequest) > 0.25f + 0.25f * Random.value)
                {
                    // has a path been defined ?
                    if (context.agentNode == null)
                    {
                        context.agentNode = layout.FindClosestNode(context.agentLocation - settings.worldOffset);
                        context.targetNode = layout.FindClosestNode(context.targetLocation - settings.worldOffset);
                        context.targetStartLocation = context.targetLocation;
                    }

                    context.pathfindingTicket = service.BeginSearch(context.agentNode, context.targetNode);
                    context.lastTicketRequest = time;
                }
            }

            // completed pathfinding ?
            if (context.pathfindingTicket >= 0)
            {
                // check if a result is available
                if (service.RetrieveResult(context.pathfindingTicket, context.agentNode, context.targetNode, context.pathNodes) != null)
                {
                    InitializePathFollowingState(context, settings, time);
                    context.lastTicketRequest = time;
                }
            }
        }

        /// <summary>
        /// Initializes the context such that it represents an pathfollowing state state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="time"></param>
        public static void InitializePathFollowingState(AgentPathingContext context, AgentPathfindingSettings settings, float time)
        {
            context.stateStartTime = time;
            context.waypointIndex = 0;
            context.pathfindingTicket = -1;
            context.waypoint = GetNextWaypointPosition(0, context.pathNodes, context.targetStartLocation - settings.worldOffset);
            context.state = PathingState.FollowingPath;
        }

        public static void UpdateFollowingPath(AgentPathingContext context,
                                                AgentPathfindingSettings settings,
                                                float time)
        {
            // has target moved beyond a certain range ?
            if ((time - context.stateStartTime) > settings.pathValidatyCheckTimeout
                && !IsDistanceInRange(context.targetLocation, context.targetStartLocation, settings.targetDistanceThreshold))
            {
                InitializePathfindingState(context, time);
            }
            else
            {
                // is the agent within range of the target ?
                if (IsDistanceInRange(context.agentLocation, context.targetStartLocation, settings.waypointDistance))
                {
                    InitializeIdleState(context, time);
                }
                else
                { 
                    // is the agent within range of the current waypoint
                    if (IsDistanceInRange(context.waypoint + settings.worldOffset, context.agentLocation, settings.waypointDistance))
                    {
                        DungeonNode lastNode = context.pathNodes[context.pathNodes.Length - 1];
                        // go to the next waypoint
                        context.waypointIndex = Mathf.Min(context.waypointIndex + 1, context.pathNodes.Length);

                        // check if the previous node in the list was the 'closest' node (targetNode)
                        if ((context.waypointIndex == context.pathNodes.Length -1
                            || context.pathNodes[context.waypointIndex] ==  null) && lastNode != context.targetNode)
                        {
                            // agent's buffer was too small to contain the full path, continue pathfinding from the current position
                            InitializePathfindingState(context, time);
                        }
                        else
                        {
                            context.waypoint = GetNextWaypointPosition(context.waypointIndex, context.pathNodes,
                                                                            context.targetStartLocation - settings.worldOffset, true);
                        }
                    }

                    context.movementDirection = (context.waypoint + settings.worldOffset) - context.agentLocation;
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
        public static Vector2 GetNextWaypointPosition(int index, DungeonNode[] nodes, Vector2 endPoint, bool randomize = false)
        {
            if (index < nodes.Length - 1)
            {
                var node = nodes[index];

                // node may be null - in which case we've reached the end of the path
                if (node != null)
                {
                    var edge = nodes[index].GetEdgeTo(nodes[index + 1]);

                    if (edge != null)
                    {
                        return randomize ? edge.RandomIntersectionPoint : edge.IntersectionCenter;
                    }
                }
            }

            return endPoint;
        }
    }
}
