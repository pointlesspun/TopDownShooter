/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.DungeonPathfinding
{
    using UnityEngine;

    using Tds.Util;
    using Tds.PathFinder;
    using Tds.DungeonGeneration;

    /// <summary>
    /// Class with a state which allows for tin-editor testing of pathfinding. 
    /// </summary>
    public class AgentPathfindingTestBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Number of times a service is allowed to search
        /// </summary>
        public int _serviceIterations = 16;

        /// <summary>
        /// Number of agents created
        /// </summary>
        public int _agentCount = 4;

        /// <summary>
        /// Target object to follow
        /// </summary>
        public GameObject _target;

        /// <summary>
        /// Speed of the movement of the tracer
        /// </summary>
        public float _tracerUpdateSpeed = 0.1f;

        /// <summary>
        /// Size of the gizmo representing the tracer
        /// </summary>
        public float _gizmoSize = 0.25f;

        /// <summary>
        /// Flag indicating whether or not the tracer is following the target
        /// </summary>
        public bool IsFollowingTarget
        {
            get;
            set;
        }

        /// <summary>
        /// Length of the buffer the agent has to store path nodes in
        /// </summary>
        public int _agentSearchBuffer = 128;

        /// <summary>
        /// Agents containing the pathing information
        /// </summary>
        private AgentPathingState<DungeonNode>[] _agents;

        /// <summary>
        /// Cached position of the agents
        /// </summary>
        private Vector2[] _currentPosition;

        /// <summary>
        /// Randomized speed of the agents
        /// </summary>
        private float[] _agentSpeeds;

        private AgentPathingContext<DungeonNode> _context;

        /// <summary>
        /// Toggle following the target
        /// </summary>
        public void ToggleFollowTarget()
        {
            IsFollowingTarget = !IsFollowingTarget;

            if (IsFollowingTarget)
            {
                // recreate the agents and settings from scratch every time the button is
                // clicked as re-using them turns out to be tricky
                _agents = new AgentPathingState<DungeonNode>[_agentCount];
                _currentPosition = new Vector2[_agentCount];
                _agentSpeeds = new float[_agentCount];

                for (int i = 0; i < _agentCount; ++i)
                {
                    _agents[i] = new AgentPathingState<DungeonNode>(_agentSearchBuffer);
                    _currentPosition[i] = Vector2.zero;
                    _agentSpeeds[i] = Random.Range(_tracerUpdateSpeed - _tracerUpdateSpeed * 0.5f, _tracerUpdateSpeed + _tracerUpdateSpeed * 0.5f);
                }

                _context = CreatePathingContext();

                if (_context.IsInitialized())
                {
                    for (int i = 0; i < _agentCount; ++i)
                    {
                        AgentPathingService.InitializeIdleState(_agents[i], _context);
                        _currentPosition[i] = _context.searchSpace.GetRandomElement().Rect.center;
                    }
                }
            }
        }

        private AgentPathingContext<DungeonNode> CreatePathingContext()
        {
            return new AgentPathingContext<DungeonNode>()
            {
                searchSpace = new DungeonLayoutSearchSpace() { Layout = GetComponent<DungeonLayoutDebugBehaviour>().Layout },
                service = GetComponent<PathfindingServiceBehaviour>().PathfindingService,
                settings = GetComponent<PathfindingServiceBehaviour>()._pathfindingSettings,
                time = Time.realtimeSinceStartup
            };
        }

        public void OnUpdate()
        {
            _context.time = Time.realtimeSinceStartup;

            if (_target == null )
            {
                // target is gone, cancel all searches (and put the agents into idle mode)
                for (int i = 0; i < _agentCount; ++i)
                {
                    if (_agents[i].state != PathingState.Idle)
                    {
                        AgentPathingService.CancelPathfinding(_agents[i], _context);
                    }
                }
            }
            else if (_context.IsInitialized() && _target != null)
            {
                for (int i = 0; i < _agentCount; ++i)
                {
                    UpdateAgent(_agents[i], _agentSpeeds[i], ref _currentPosition[i]);
                }

                _context.service.Update(_serviceIterations);
            }
        }

        public void OnDrawGizmos()
        {
            Vector3 offset = Vector3.zero;

            if (_target != null)
            {
                Gizmos.DrawIcon(_target.transform.position, "flag.png", true);
            }

            Gizmos.color = Color.black;

            for (int i = 0; _currentPosition != null && i < _currentPosition.Length; ++i)
            {
                // not great for the performance
                /*Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(_agents[i].waypoints[0], _gizmoSize * 0.5f);
                Gizmos.DrawWireSphere(_agents[i].waypoints[1], _gizmoSize * 0.5f);*/

                Gizmos.DrawSphere(_currentPosition[i], _gizmoSize);
            }
        }

        private void UpdateAgent(AgentPathingState<DungeonNode> agentState, float agentSpeed, ref Vector2 agentPosition)
        {
            // update the current agent location and target location
            agentState.agentLocation = agentPosition;
            agentState.targetLocation = _target.transform.position;

            AgentPathingService.UpdateState(agentState, _context);

            if (agentState.state == PathingState.FollowingPath)
            {
                // move the agent
                var agentWaypointPosition = agentState.waypoints[agentState.waypointIndex];

                var distanceToWaypoint = (agentPosition - agentWaypointPosition).magnitude;
                agentPosition += (agentWaypointPosition - agentPosition).normalized
                                                * Mathf.Min(distanceToWaypoint, agentSpeed);

                // if the current waypoint is the end point, clamp the position of the agent to the 
                // last dungeon node to prevent it from clipping throuhg the wall
                if ((agentWaypointPosition - agentState.targetStartLocation).sqrMagnitude < 0.1f)
                {
                    var lastNode = agentState.pathNodes[agentState.pathNodeIndex];

                    if (lastNode != null)
                    {
                        agentPosition = RectUtil.Clamp(lastNode.Rect, agentPosition, 0.1f, 0.1f);
                    }
                }
            }
        }
    }
}
