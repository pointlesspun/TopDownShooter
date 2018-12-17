/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */
using Tds.DungeonGeneration;
using UnityEngine;

namespace Tds.PathFinder
{
    public static class AgentPathing
    {
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

        public static void UpdateIdleState(AgentPathingContext context, AgentPathfindingSettings settings, float time)
        {
            if ((context.targetLocation - context.targetStartLocation).sqrMagnitude > settings.targetDistanceThreshold * settings.targetDistanceThreshold)
            {
                InitializePathfindingState(context, time);
            }
        }

        public static void InitializePathfindingState(AgentPathingContext context, float time)
        {
            context.stateStartTime = time;
            context.state = PathingState.FindingPath;
            context.pathfindingTicket = -1;
            context.agentNode = null;
            context.targetNode = null;
        }

        public static void CancelPathfinding(AgentPathingContext context, PathfindingService<DungeonNode> service, float time)
        {
            if ( context.pathfindingTicket != -1 )
            {
                service.CancelSearch(context.pathfindingTicket);
            }

            InitializeIdleState(context, time);
        }

        public static void UpdatePathfindingState(AgentPathingContext context,
                                                        AgentPathfindingSettings settings,
                                                        PathfindingService<DungeonNode> service,
                                                        DungeonLayout layout,
                                                        float time)
        {
            if (context.pathfindingTicket < 0)
            {
                context.agentNode = layout.FindClosestNode(context.agentLocation);
                context.targetNode = layout.FindClosestNode(context.targetLocation);
                context.targetStartLocation = context.targetLocation;
                
                context.pathfindingTicket = service.BeginSearch(context.agentNode, context.targetNode);
            }

            // completed pathfinding ?
            if (context.pathfindingTicket >= 0
                 && service.RetrieveResult(context.pathfindingTicket, context.agentNode, context.targetNode, context.pathNodes) != null)
            {
                InitializePathFollowingState(context, time);
            }
        }

        public static void InitializePathFollowingState(AgentPathingContext context, float time)
        {
            context.stateStartTime = time;
            context.waypointIndex = 0;
            context.pathfindingTicket = -1;
            context.waypoint = GetNextWaypointPosition(0, context.pathNodes, context.targetStartLocation);
            context.state = PathingState.FollowingPath;
        }

        public static void UpdateFollowingPath(AgentPathingContext context,
                                                AgentPathfindingSettings settings,
                                                float time)
        {
            // has target moved beyond a certain range ?
            if ((time - context.stateStartTime) > settings.pathValidatyCheckTimeout
                && ((context.targetLocation - context.targetStartLocation).sqrMagnitude > settings.targetDistanceThreshold * settings.targetDistanceThreshold))
            {
                InitializePathfindingState(context, time);
            }
            else
            {
                if (HasReachedTarget(context.agentLocation, context.targetStartLocation, settings.waypointDistance))
                {
                    InitializeIdleState(context, time);
                }
                else
                { 
                    if (HasReachedTarget(context.waypoint, context.agentLocation, settings.waypointDistance))
                    {
                        context.waypointIndex = Mathf.Min(context.waypointIndex + 1, context.pathNodes.Length);
                        context.waypoint = GetNextWaypointPosition(context.waypointIndex, context.pathNodes, context.targetStartLocation);
                    }
                }
            }
        }

        public static bool HasReachedTarget(Vector2 target, Vector2 position, float threshold)
        {
            return (target - position).sqrMagnitude < threshold * threshold;
        }

        public static Vector2 GetNextWaypointPosition(int index, DungeonNode[] nodes, Vector2 endPoint)
        {
            if (index < nodes.Length - 1)
            {
                var node = nodes[index];

                // node may be null - in which case we've reached the end of the path
                if (node != null)
                {
                    var edge = nodes[index].GetEdgeTo(nodes[index + 1]);
                    return edge == null ? endPoint : edge.IntersectionCenter;
                }
            }

            return endPoint;
        }
    }
}
