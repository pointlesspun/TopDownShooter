/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.Pathfinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tds.Util;

    public class PathNode<T>
    {
        public PathNode<T> _parent;
        public List<PathNode<T>> _children = new List<PathNode<T>>();
        public float _cost = 0;
        public float _pathLength = 0;
        public T _data;

        public void AddChild(PathNode<T> node)
        {
            _children.Add(node);
            node._parent = this;
        }
    }

    public class PathfinderAlgorithm<T> where T : class
    {
        public T Start { get; private set; }
        public T End { get; private set; }

        /// <summary>
        /// Cost function for moving from A to B, taking in account the cost for
        /// getting to A.
        /// </summary>
        public Func<T, T, float, float> CostFunction
        {
            get;
            private set;
        }

        public Func<PathNode<T>, IEnumerable<T>> ExpansionFunction
        {
            get;
            private set;
        }

        public Func<T, T, float> DistanceFunction
        {
            get;
            private set;
        }

        private List<PathNode<T>> _openList = new List<PathNode<T>>();
        private HashSet<T> _closedTraversalNodes = new HashSet<T>();

        private PathNode<T> _bestNode;
        private PathNode<T> _root;

        private ObjectPool<PathNode<T>> _nodePool;

        public PathfinderAlgorithm(int poolSize)
        {
            _nodePool = new ObjectPool<PathNode<T>>(0, () => new PathNode<T>(), 256);
        }

        public void BeginSearch( T origin
                               , T destination
                               , Func<T, T, float, float> costFunction
                               , Func<PathNode<T>, IEnumerable<T>> expansionFunction
                               , Func<T,T, float> distanceFunction)
        {
            Start = origin;
            End = destination;
            CostFunction = costFunction;
            ExpansionFunction = expansionFunction;
            DistanceFunction = distanceFunction;

            _nodePool.Clear();
            _openList.Clear();
            _closedTraversalNodes.Clear();

            _root = _nodePool.Obtain()._obj;

            _root._data = Start;
            _root._cost = costFunction(origin, destination, 0);

            _bestNode = _root;
            _openList.Add(_root);
            
            _closedTraversalNodes.Add(_root._data);
        }

        public List<T> GetOpenList()
        {
            var result = new List<T>();

            _openList.ForEach(n => result.Add(n._data));

            return result;
        }

        public List<T> GetClosedList()
        {
            var result = new List<T>();

            foreach (var n in _closedTraversalNodes)
            {
                result.Add(n);
            }

            return result;
        }

        public float Iterate(int maxIterations = -1)
        {
            // estimated distance to goal
            var estimatedDistance = -1.0f;
            var iteration = 0;

            while (_openList.Count > 0 && (maxIterations < 0 || iteration < maxIterations))
            {
                iteration++;

                // did we find a / the goal node ?
                if (_openList[0]._data == End)
                {
                    estimatedDistance = 0;
                    _bestNode = _openList[0];
                    _openList.Clear();
                    break;
                }
                else
                {
                    var current = _openList[0];
                    _openList.RemoveAt(0);

                    // update the current best position
                    if (current._cost < _bestNode._cost)
                    {
                        _bestNode = current;
                    }

                    estimatedDistance = current._cost;

                    _closedTraversalNodes.Add(current._data);

                    var expansions = ExpansionFunction(current);

                    // check for dead ends
                    if (expansions != null && expansions.Any())
                    {
                        // found an expansion... iteration is now complete
                        foreach( var nextElement in expansions)
                        {
                            Explore(current, nextElement);
                        }                        
                    }
                }
            }

            return estimatedDistance;
        }

        public List<T> GetBestPath()
        {
            var result = new List<T>();

            if (_bestNode != null)
            {
                var current = _bestNode;

                while (current != null)
                {
                    result.Insert(0, current._data);
                    current = current._parent;
                }
            }

            return result;
        }

        private void Explore(PathNode<T> current, T target)
        {
            // is the node not yet explored ?
            if (!_closedTraversalNodes.Contains(target))
            {
                // how much does it cost to move to target ?
                var accessCost = CostFunction(current._data, target, current._pathLength);

                // if cost is negative it means access to the targetPosition is blocked so
                // we can't expand in this direction
                if (accessCost >= 0)
                {
                    Expand(current, target, accessCost);
                }
            }
        }

        private void Expand(PathNode<T> parentNode, T target, float accessCost)
        {
            PathNode<T> newChild = null;

            for (var i = 0; i < _openList.Count; ++i)
            {
                var openNode = _openList[i];

                if (openNode._data == target)
                {
                    // there is already a better path to the current node, drop this expansion
                    newChild = openNode;
                    break;
                }
                else if (openNode._cost > accessCost)
                {
                    newChild = InitializeNode(parentNode, target, accessCost);
                    _openList.Insert(i, newChild);
                    
                    break;
                }
            }

            // no node inserted yet ?
            if (newChild == null)
            {
                // add a node to the open list
                _openList.Add(InitializeNode(parentNode, target, accessCost));
            }
        }

        private PathNode<T> InitializeNode(PathNode<T> parent, T target, float accessCost)
        {
            PathNode<T> result = _nodePool.Obtain(() => new PathNode<T>())._obj;

            result._children.Clear();
            result._data = target;
            result._cost = accessCost;
            result._pathLength = parent._pathLength + DistanceFunction(parent._data, target);

            parent.AddChild(result);

            return result;
        }
    }
}
