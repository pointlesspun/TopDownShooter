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
    /// Class with a state which allows for following a path consisting of dungeonnodes. 
    /// </summary>
    [ExecuteInEditMode]
    public class AgentPathfindingTestBehaviour : MonoBehaviour
    {
        private PathfindingService<DungeonNode> _service;
        private DungeonLayout _layout;
        private AgentPathfindingSettings _settings;

        private AgentPathingContext _context = new AgentPathingContext(48);

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
        /// Cached position of the agent using this tracer
        /// </summary>
        private Vector2 _currentPosition;

        /// <summary>
        /// Toggle following the target
        /// </summary>
        public void ToggleFollowTarget()
        {
            IsFollowingTarget = !IsFollowingTarget;

            if (IsFollowingTarget)
            {
                _layout = GetComponent<DungeonLayoutDebugBehaviour>().Layout;
                _service = GetComponent<PathfindingServiceBehaviour>().PathfindingService;
                _settings = GetComponent<PathfindingServiceBehaviour>()._pathfindingSettings;

                if (_layout != null && _service != null && _settings != null )
                {
                    AgentPathing.InitializeIdleState(_context, Time.realtimeSinceStartup);
                    _currentPosition = _layout.Start.Rect.center;
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
                if (_context.state != PathingState.Idle)
                {
                    AgentPathing.CancelPathfinding(_context, _service, Time.realtimeSinceStartup);
                }
            }
            else if (_layout != null && _settings != null && _service != null && _target != null)
            {
                _context.agentLocation = _currentPosition;
                _context.targetLocation = _target.transform.position;

                AgentPathing.UpdateState(_context, _settings, _service, _layout, Time.realtimeSinceStartup);

                if (_context.state == PathingState.FollowingPath)
                {
                    var distanceToWaypoint = (_currentPosition - _context.waypoint).magnitude;
                    _currentPosition += (_context.waypoint - _currentPosition).normalized 
                                                    * Mathf.Min(distanceToWaypoint, _tracerUpdateSpeed);

                    if ((_context.waypoint - _context.targetStartLocation).sqrMagnitude < 0.1f)
                    {
                        var lastNode = _context.pathNodes[_context.waypointIndex];

                        if (lastNode != null)
                        {
                            _currentPosition = RectUtil.Clamp(lastNode.Rect, _currentPosition, 0.1f, 0.1f);
                        }
                    }
                }

                _service.Update(5);
            }
        }

        public void OnDrawGizmos()
        {
            Vector3 offset = Vector3.zero;

            if (_target != null)
            {
                if (_context.state == PathingState.FollowingPath)
                {
                    Gizmos.DrawIcon(_context.targetLocation, "flag.png", true);
                }
            }

            Gizmos.color = Color.black;
            Gizmos.DrawSphere(_currentPosition, _gizmoSize);
        }
    }
}
