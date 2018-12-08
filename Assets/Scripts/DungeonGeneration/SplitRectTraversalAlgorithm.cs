/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using System;

    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using Tds.Util;
    
    [Serializable]
    public class SplitRectTraversalAlgorithm
    {
        /// <summary>
        /// Max number of iterations, set to -1 if there no limit
        /// </summary>
        public int _maxDepth = 5;

        /// <summary>
        /// Max distance the paths can traverse
        /// </summary>
        public float _maxLength = -1;

        /// <summary>
        /// Random seed, set to -1 if
        /// </summary>
        public int _seed = 42;

        /// <summary>
        /// Node which has the longest distance to the root. When the max length exceeds
        /// the heurstic length from start to end, no value will be found. Instead return
        /// the node with the longest distance;
        /// </summary>
        private TraversalNode _longestDistanceNode;
   
        public TraversalNode TraverseSplitRects( List<SplitRect> source)
        {
            var initiailRandomState = UnityEngine.Random.state;

            if (_seed >= 0)
            {
                UnityEngine.Random.InitState(_seed);
            }

            _longestDistanceNode = null;

            var result = BuildPath(source[UnityEngine.Random.Range(0, source.Count)], new HashSet<SplitRect>());

            if (_seed >= 0)
            {
                UnityEngine.Random.state = initiailRandomState;
            }

            return result;
        }

        /// <summary>
        /// Build a path with the given split as a root, returns the root node of the path
        /// </summary>
        /// <param name="split"></param>
        /// <param name="closedList"></param>
        /// <returns></returns>
        private TraversalNode BuildPath(SplitRect split, HashSet<SplitRect> closedList)
        {
            var root = CreateNode(null, split, closedList);
            var currentNode = root;

            for (int i = 0; CanContinueIteration(i, currentNode); ++i)
            {
                var childSplit = SelectRandomNeighbour(currentNode._split, closedList);

                if (childSplit == null)
                {
                    // backtrack - the loop will break if there are no more children
                    currentNode = currentNode._parent;
                }
                else
                {
                    currentNode = CreateNode(currentNode, childSplit, closedList); ;
                }
            }

            // did we find a solution
            if (currentNode == null)
            {
                // if not take the best alternative
                currentNode = _longestDistanceNode;
            }

            // traverse up again marking the primary path through the level
            while (currentNode != null && currentNode._parent != null)
            {
                currentNode._isPrimaryPath = true;
                currentNode = currentNode._parent;
            }

            return root;
        }

        private TraversalNode CreateNode(TraversalNode parent, SplitRect split, HashSet<SplitRect> closedList)
        {
            var result = new TraversalNode()
            {
                _split = split,
                _parent = parent,
                _children = new List<TraversalNode>(),
                _parentIntersection = parent == null ?  null : RectUtil.GetIntersection(parent._split._rect, split._rect),
                _pathLength = parent == null ?  0 : parent._pathLength + Vector2.Distance(parent._split._rect.center, split._rect.center)
            };

            if (_longestDistanceNode == null || result._pathLength > _longestDistanceNode._pathLength)
            {
                _longestDistanceNode = result;
            }

            closedList.Add(split);

            if (parent != null)
            {
                parent._children.Add(result);
            }
            return result;
        }

        private SplitRect SelectRandomNeighbour(SplitRect source, HashSet<SplitRect> closedList)
        {
            var neighbours = source.Neighbours.ToList();
            var count = neighbours.Count;
            var startIndex = UnityEngine.Random.Range(0, count);

            for (int i = 0; i < neighbours.Count; ++i)
            {
                var subject = neighbours[(i + startIndex) % count];

                if (!closedList.Contains(subject) )
                {
                    var intersection = RectUtil.GetIntersection(source._rect, subject._rect);

                    // need at least 3 units to build a door without artifacts (such as unreachable doors)
                    if (Vector2Int.Distance(intersection[0], intersection[1]) >= 3)
                    {
                        return subject;
                    }
                }
            }

            return null;
        }

        private bool CanContinueIteration(int i, TraversalNode current)
        {
            return (i < _maxDepth || _maxDepth == -1)
                && (current != null)
                && (current._pathLength < _maxLength || _maxLength == -1);
        }

    }
}
