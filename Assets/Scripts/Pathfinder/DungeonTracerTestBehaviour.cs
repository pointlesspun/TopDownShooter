/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.PathFinder
{
    using System.Collections.Generic;

    using UnityEditor;
    using UnityEngine;

    using Tds.DungeonGeneration;
    using Tds.Util;

    /// <summary>
    /// Behaviour which follows a target through a dungeon layout via a pathfinder.
    /// Works only in editor
    /// </summary>
    [ExecuteInEditMode]
    public class DungeonTracerTestBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Distance to which a waypoint is reached
        /// </summary>
        public float _waypointDistance = 0.25f;

        /// <summary>
        /// Speed of the movement of the tracer
        /// </summary>
        public float _tracerUpdateSpeed = 0.1f;

        /// <summary>
        /// Size of the gizmo representing the tracer
        /// </summary>
        public float _gizmoSize = 0.25f;

        /// <summary>
        /// Target object to follow
        /// </summary>
        public GameObject _targetObject;

        /// <summary>
        /// Threshold which when the target moves this much units away, will cause an update 
        /// of the pathfinder
        /// </summary>
        public float _endPointThreshold = 5.5f;

        private PathfinderAlgorithm<DungeonNode> _pathfinder = DungeonSearch.CreatePathfinder(256, 1, 1, 1);
        private DungeonPathTracer _tracer = new DungeonPathTracer();

        // current position of the tracer
        private Vector2 _tracerPosition;

        // current path to follow
        private List<DungeonNode> _path = null;

        /// <summary>
        /// Flag indicating whether or not the tracer is following the target
        /// </summary>
        public bool IsFollowingTarget
        {
            get;
            set;
        }

        /// <summary>
        /// Toggle following the target
        /// </summary>
        public void ToggleFollowTarget()
        {
            if (!IsFollowingTarget)
            {
                BeginTrace();
            }

            IsFollowingTarget = !IsFollowingTarget;

            if (IsFollowingTarget)
            {
                EditorApplication.update += OnUpdate;
            }
            else
            {
                EditorApplication.update -= OnUpdate;
            }
        }

        public void OnUpdate()
        {
            if (IsFollowingTarget)
            {
                UpdateTrace();

                Vector2 endPoint = _targetObject == null ? InputUtil.GetCursorPosition() : _targetObject.transform.position;
                
                // if the target has moved away, reinitialize the path
                if ( (endPoint - _tracer.EndPoint).magnitude > _endPointThreshold)
                {
                    InitializeTrace(_tracerPosition);
                }
            }
        }

        /// <summary>
        /// Begin the path tracing
        /// </summary>
        private void BeginTrace()
        {
            var layout = GetComponent<DungeonLayoutDebugBehaviour>().Layout;

            InitializeTrace(layout.Start.Rect.center);

            IsFollowingTarget = false;
        }

        private void UpdateTrace()
        {
            if (!_tracer.HasReachedEnd(_tracerPosition))
            {
                var distanceToEndPoint = (_tracerPosition - _tracer.Waypoint).magnitude;
                _tracerPosition += _tracer.Direction.normalized * Mathf.Min(distanceToEndPoint , _tracerUpdateSpeed);

                if (_tracer.Waypoint == _tracer.EndPoint)
                {
                    _tracerPosition = RectUtil.Clamp(_path[_path.Count - 1].Rect, _tracerPosition, 0.1f, 0.1f);
                }

                _tracer.Update();
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 offset = Vector3.zero;

            if (_pathfinder != null)
            {
                Gizmos.DrawIcon(_tracer.EndPoint, "flag.png", true);

                Gizmos.color = Color.black;
                Gizmos.DrawSphere(_tracerPosition, _gizmoSize);
            }
        }

        private void InitializeTrace(Vector2 startPosition)
        {
            var layout = GetComponent<DungeonLayoutDebugBehaviour>().Layout;
            var endPoint = _targetObject == null ? InputUtil.GetCursorPosition() : _targetObject.transform.position;
            var beginNode = layout.FindClosestNode(startPosition);
            var endNode = layout.FindClosestNode(endPoint);
            var breakCounter = 0;

            _tracerPosition = startPosition;

            _pathfinder.BeginSearch(beginNode, endNode);

            while (_pathfinder.Iterate() > 0)
            {
                breakCounter++;

                if (breakCounter >= 1000)
                {
                    throw new System.InvalidProgramException("pathfinding exceeded count");
                }
            }

            _path = _pathfinder.GetBestPath();
            _tracer.Initialize(() => _tracerPosition, _waypointDistance);
            _tracer.BeginTrace(endPoint, _path);
        }
    }
}
