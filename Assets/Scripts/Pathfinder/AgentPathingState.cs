/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.PathFinder
{
    using System;

    using UnityEngine;
    
    public enum PathingState
    {
        Idle,
        FindingPath,
        FollowingPath,
    }

    /// <summary>
    /// Class containing the information for an agent to perform pathfinding.
    /// </summary>
    [Serializable]
    public class AgentPathingState<T>
    {
        /// <summary>
        /// Current state 
        /// </summary>
        public PathingState state;

        /// <summary>
        /// Start time of the current state
        /// </summary>
        public float stateStartTime;

        /// <summary>
        /// Current target location of the pathing
        /// </summary>
        public Vector2 targetLocation;

        /// <summary>
        /// Target location of the pathing when pathfinding was started
        /// </summary>
        public Vector2 targetStartLocation;

        /// <summary>
        /// Cached position of the agent using this pathing
        /// </summary>
        public Vector2 agentLocation;

        /// <summary>
        /// Current direction to the next waypoint (not normalized)
        /// </summary>
        public Vector2 movementDirection;

        /// <summary>
        /// Agent's current location
        /// </summary>
        public T agentNode;

        /// <summary>
        /// Target's node
        /// </summary>
        public T targetNode;

        public bool isTargetNodeApproximation;

        /// <summary>
        /// Ticket id in pathfinding service
        /// </summary>
        public int pathfindingTicket;

        /// <summary>
        /// Last time this agent requested a pathfinding. Used to prevent spamming
        /// the pathfinding service.
        /// </summary>
        public float lastTimeoutCheck;
       
        /// <summary>
        /// List of nodes making up the current path
        /// </summary>
        public T[] pathNodes;

        /// <summary>
        /// Current waypoint index in the nodes
        /// </summary>
        public int pathNodeIndex;

        /// <summary>
        /// Current waypoint the agent is moving to when in the tracing state 
        /// </summary>
        public Vector2[] waypoints;

        /// <summary>
        /// Current index in the waypoints
        /// </summary>
        public int waypointIndex;

        public AgentPathingState()
        {
            state = PathingState.Idle;
            pathfindingTicket = -1;
            lastTimeoutCheck = 0;
            // need two points at the moment one in front of the doorway and one after the doorway
            // this is just a crutch until detailed pathfinding is in place
            waypoints = new Vector2[2];
        }

        /// <summary>
        /// Creates a context with a given set of nodes. This nodes represents the maximum
        /// length of the path an agent can follow. 
        /// </summary>
        /// <param name="pathNodeCount"></param>
        public AgentPathingState(int pathNodeCount) : this()
        {
            pathNodes = new T[pathNodeCount];
        }
    }
}
