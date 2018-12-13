/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.PathFinder
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Tds.DungeonGeneration;

    /// <summary>
    /// Class with a state which allows for following a path consisting of dungeonnodes. 
    /// </summary>
    public class DungeonPathTracer
    {
        /// <summary>
        /// Current waypoint the tracer is moving to 
        /// </summary>
        public Vector2 Waypoint
        {
            get;
            private set;
        }

        /// <summary>
        /// End point of the trace
        /// </summary>
        public Vector2 EndPoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Current direction to the next waypoint (not normalized)
        /// </summary>
        public Vector2 Direction
        {
            get
            {
                return Waypoint - _currentPosition;
            }
        }

        /// <summary>
        /// Function returning the current position of the agent using this tracer  
        /// </summary>
        private Func<Vector2> _getPosition;

        /// <summary>
        /// List of nodes to go through
        /// </summary>
        private List<DungeonNode> _nodes;

        /// <summary>
        /// Distance to waypoint where it is considered 'reached'
        /// </summary>
        private float _waypointDistance;

        /// <summary>
        /// Current waypoint index in the nodes
        /// </summary>
        private int _waypointIndex;

        /// <summary>
        /// Cached position of the agent using this tracer
        /// </summary>
        private Vector2 _currentPosition;

        public DungeonPathTracer()
        {
        }

        /// <summary>
        /// Creates a tracer with the current function provider and waypoint criterium
        /// </summary>
        /// <param name="positionFunction"></param>
        /// <param name="waypointDistance"></param>
        public DungeonPathTracer(Func<Vector2> positionFunction, float waypointDistance)
        {
            Initialize(positionFunction, waypointDistance);
        }

        public void Initialize(Func<Vector2> positionFunction, float waypointDistance)
        {
            _getPosition = positionFunction;
            _waypointDistance = waypointDistance;
        }

        public void BeginTrace(Vector2 endPosition, List<DungeonNode> nodes)
        {
            EndPoint = endPosition;
            _nodes = new List<DungeonNode>(nodes);
            _waypointIndex = 0;
            Waypoint = GetNextWaypointPosition();
        }

        public bool Update()
        {
            _currentPosition = _getPosition();

            if (!HasReachedEnd(_currentPosition))
            {
                if ( HasReachedTarget(Waypoint, _currentPosition))
                {
                    _waypointIndex = Mathf.Min( _waypointIndex + 1, _nodes.Count);
                    Waypoint = GetNextWaypointPosition();
                }

                return false;
            }

            return true;
        }


        public bool HasReachedEnd(Vector2 position)
        {
            return HasReachedTarget(EndPoint, position);
        }

        public bool HasReachedTarget(Vector2 target, Vector2 position)
        {
            return (target -  position).sqrMagnitude < _waypointDistance * _waypointDistance;
        }

        private Vector2 GetNextWaypointPosition()
        {
            if (_waypointIndex < _nodes.Count - 1)
            {
                var edge = _nodes[_waypointIndex].GetEdgeTo(_nodes[_waypointIndex + 1]);
                return edge == null ? EndPoint : edge.IntersectionCenter;
            }

            return EndPoint;
        }

    }
}
