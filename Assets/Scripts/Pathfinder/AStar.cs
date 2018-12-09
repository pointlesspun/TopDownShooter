/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.Pathfinder
{
    using System;
    using System.Collections.Generic;
    using Tds.DungeonGeneration;
    using Tds.Util;
    using UnityEngine;

    public class AStarNode
    {
        public AStarNode _parent;
        public List<AStarNode> _children = new List<AStarNode>();
        public float _cost = 0;
        public float _pathLength = 0;
        public TraversalNode _traversalNode;

        public void AddChild(AStarNode node)
        {
            _children.Add(node);
            node._parent = this;
        }
    }

    public class AStar
    {
        private struct CostResults
        {
            public float _cost;
            public float _pathLength;
        }

        public TraversalNode Start { get; private set; }
        public TraversalNode End { get; private set; }

        public Func<TraversalNode, TraversalNode, float> CostFunction { get; private set; }

        private List<AStarNode> _openList = new List<AStarNode>();
        private HashSet<TraversalNode> _closedTraversalNodes = new HashSet<TraversalNode>();

        private AStarNode _bestNode;
        private AStarNode _root;

        private ObjectPool<AStarNode> _nodePool;

        public AStar(int poolSize)
        {
            _nodePool = new ObjectPool<AStarNode>(0, () => new AStarNode(), 256);
        }

        public void BeginSearch( TraversalNode origin, TraversalNode destination, Func<TraversalNode, TraversalNode, float> cost )
        {
            Start = origin;
            End = destination;
            CostFunction = cost;

            _nodePool.Clear();
            _openList.Clear();
            _closedTraversalNodes.Clear();

            _root = _nodePool.Obtain()._obj;

            _root._traversalNode = Start;
            _root._cost = (destination._split._rect.center - origin._split._rect.center).magnitude;

            _bestNode = _root;
            _openList.Add(_root);
            _closedTraversalNodes.Add(_root._traversalNode);
        }

        public List<TraversalNode> GetOpenList()
        {
            var result = new List<TraversalNode>();

            _openList.ForEach(n => result.Add(n._traversalNode));

            return result;
        }

        public List<TraversalNode> GetClosedList()
        {
            var result = new List<TraversalNode>();

            foreach ( var n in _closedTraversalNodes)
            {
                result.Add(n);
            }

            return result;
        }

        public float Iterate()
        {
            // estimated distance to goal
            var estimatedDistance = -1.0f;

            while (_openList.Count > 0)
            {
                // did we find a / the goal node ?
                if (_openList[0]._traversalNode == End)
                {
                    estimatedDistance = 0;
                    _bestNode = _openList[0];
                    _openList.Clear();
                    break;
                }
                else
                {
                    var  current = _openList[0];
                    _openList.RemoveAt(0);

                    // update the current best position
                    if (DistanceToTarget(current) < DistanceToTarget(_bestNode))
                    {
                        _bestNode = current;
                    }

                    _closedTraversalNodes.Add(current._traversalNode);

                    // check for dead ends
                    if (current._traversalNode._children.Count > 0)
                    {
                        // found an expansion... iteration is now complete
                        current._traversalNode._children.ForEach((c) => Explore(current, c));
                        estimatedDistance = _openList.Count > 0 ? _openList[0]._cost : -1.0f;
                        break;
                    }
                }
            }

            return estimatedDistance;
        }

        public List<TraversalNode> GetBestPath()
        {
            var  result = new List<TraversalNode>();

            if (_bestNode != null)
            {
                var current = _bestNode;

                while (current != null)
                {
                    result.Insert(0, current._traversalNode);
                    current = current._parent;
                }
            }

            return result;
        }

        private float DistanceToTarget(AStarNode node)
        {
            return (node._traversalNode._split._rect.center - End._split._rect.center).sqrMagnitude;
        }

        private void Explore(AStarNode current, TraversalNode target)
        {
            // is the node not yet explored ?
            if (!_closedTraversalNodes.Contains( target))
            {
                var cost = CostFunction(current._traversalNode, target);

                // if cost is negative it means access to the targetPosition is blocked so
                // we can't expand in this direction
                if (cost >= 0)
                {
                    Expand(current, target, cost);
                }
            }
        }

        private void Expand(AStarNode current, TraversalNode target, float accessCost)
        {
            var nodeCost = CalculateCost(current, target, accessCost);

            AStarNode newChild = null;

            for (var i = 0; i < _openList.Count; ++i)
            {
                var openNode = _openList[i];

                if (openNode._traversalNode == target)
                {
                    // there is already a better path to the current node, drop this expansion
                    newChild = openNode;
                    break;
                }
                else if (openNode._cost > nodeCost._cost)
                {
                    newChild = _nodePool.Obtain(() => new AStarNode())._obj;

                    newChild._children.Clear();
                    newChild._traversalNode = target;
                    newChild._cost = nodeCost._cost;
                    newChild._pathLength = nodeCost._pathLength;

                    _openList.Insert(i, newChild);

                    current.AddChild(newChild);

                    break;
                }
            }

            // no node inserted yet ?
            if (newChild == null)
            {
                // add a node to the open list
                newChild = _nodePool.Obtain(() => new AStarNode())._obj;

                newChild._children.Clear();
                newChild._traversalNode = target;
                newChild._cost = nodeCost._cost;
                newChild._pathLength = nodeCost._pathLength;

                _openList.Add(newChild);

                current.AddChild(newChild);
            }
        }

        private CostResults CalculateCost(AStarNode current, TraversalNode target, float accessCost)
        {
            var currentPosition = current._traversalNode._split._rect.center;

            Vector2 targetPosition = target._split._rect.center;
            Vector2 goalPosition = End._split._rect.center;
       
            var pathLength = (targetPosition - currentPosition).magnitude + current._pathLength;

            return new CostResults()
            {
                _cost = (goalPosition - targetPosition).magnitude + pathLength + accessCost,
                _pathLength = pathLength
            };
        }    
    }
}
