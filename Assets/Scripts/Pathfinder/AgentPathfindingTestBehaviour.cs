/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.PathFinder
{
    using UnityEngine;

    using Tds.DungeonGeneration;
    using UnityEditor;
    using Tds.Util;

    /// <summary>
    /// Class with a state which allows for tin-editor testing of pathfinding. 
    /// </summary>
    [ExecuteInEditMode]
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
        private AgentPathingContext[] _agents;

        /// <summary>
        /// Reference to the service capable of pathfinding
        /// </summary>
        private PathfindingService<DungeonNode> _service;

        /// <summary>
        /// Current dungeon layout
        /// </summary>
        private DungeonLayout _layout;

        /// <summary>
        /// Reference to the pathfinding settings
        /// </summary>
        private AgentPathfindingSettings _settings;

        /// <summary>
        /// Cached position of the agents
        /// </summary>
        private Vector2[] _currentPosition;

        /// <summary>
        /// Randomized speed of the agents
        /// </summary>
        private float[] _agentSpeeds;

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
                _agents = new AgentPathingContext[_agentCount];
                _currentPosition = new Vector2[_agentCount];
                _agentSpeeds = new float[_agentCount];

                for ( int i = 0; i < _agentCount; ++i )
                {
                    _agents[i] = new AgentPathingContext(_agentSearchBuffer);
                    _currentPosition[i] = Vector2.zero;
                    _agentSpeeds[i] = Random.Range(_tracerUpdateSpeed - _tracerUpdateSpeed * 0.5f, _tracerUpdateSpeed +_tracerUpdateSpeed * 0.5f);
                }

                _layout = GetComponent<DungeonLayoutDebugBehaviour>().Layout;
                _service = GetComponent<PathfindingServiceBehaviour>().PathfindingService;
                _settings = GetComponent<PathfindingServiceBehaviour>()._pathfindingSettings;

                if (_layout != null && _service != null && _settings != null )
                {
                    for (int i = 0; i < _agentCount; ++i)
                    {
                        AgentPathingService.InitializeIdleState(_agents[i], Time.realtimeSinceStartup);
                        _currentPosition[i] = _layout.GetRandomNode().Rect.center;
                    }
                }

                EditorApplication.update += OnUpdate;
            }
            else
            {
                EditorApplication.update -= OnUpdate;
            }
        }

        public void OnUpdate()
        {
            if (_target == null )
            {
                // target is gone, cancel all searches (and put the agents into idle mode)
                for (int i = 0; i < _agentCount; ++i)
                {
                    if (_agents[i].state != PathingState.Idle)
                    {
                        AgentPathingService.CancelPathfinding(_agents[i], _service, Time.realtimeSinceStartup);
                    }
                }
            }
            else if (_layout != null && _settings != null && _service != null && _target != null)
            {
                for (int i = 0; i < _agentCount; ++i)
                {
                    // update the current agent location and target location
                    _agents[i].agentLocation = _currentPosition[i];
                    _agents[i].targetLocation = _target.transform.position;

                    AgentPathingService.UpdateState(_agents[i], _settings, _service, _layout, Time.realtimeSinceStartup);

                    if (_agents[i].state == PathingState.FollowingPath)
                    {
                        // move the agent
                        var distanceToWaypoint = (_currentPosition[i] - _agents[i].waypoint).magnitude;
                        _currentPosition[i] += (_agents[i].waypoint - _currentPosition[i]).normalized
                                                        * Mathf.Min(distanceToWaypoint, _agentSpeeds[i]);

                        // if the current waypoint is the end point, clamp the position of the agent to the 
                        // last dungeon node to prevent it from clipping throuhg the wall
                        if ((_agents[i].waypoint - _agents[i].targetStartLocation).sqrMagnitude < 0.1f)
                        {
                            var lastNode = _agents[i].pathNodes[_agents[i].waypointIndex];

                            if (lastNode != null)
                            {
                                _currentPosition[i] = RectUtil.Clamp(lastNode.Rect, _currentPosition[i], 0.1f, 0.1f);
                            }
                        }
                    }
                }

                _service.Update(_serviceIterations);
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
                Gizmos.DrawSphere(_currentPosition[i], _gizmoSize);
            }
        }
    }
}
