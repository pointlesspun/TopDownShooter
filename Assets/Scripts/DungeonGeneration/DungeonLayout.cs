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
    using Tds.Util;
    using UnityEngine;

    /// <summary>
    /// Algorithm which randomly traverses through a dungeon (list of dungeon nodes),
    /// creating a smaller dungeon with guaranteed path
    /// </summary>
    public class DungeonLayout 
    {
        // to do replace with quadtree
        private DungeonNode[] _nodes;

        private SpatialBinaryTree<DungeonNode> _lookup;

        
        /// <summary>
        /// Collection of nodes, use for debug purposes only
        /// </summary>
        public IEnumerable<DungeonNode> Nodes
        {
            get
            {
                return _nodes;
            }
        }

        /// <summary>
        /// Number of nodes in this layout
        /// </summary>
        public int NodeCount
        {
            get
            {
                return _nodes.Length;
            }
        }

        /// <summary>
        /// Start node of the layout (player spawn node)
        /// </summary>
        public DungeonNode Start
        {
            get;
            set;
        }

        /// <summary>
        /// End node of the layout (should contain the exit)
        /// </summary>
        public DungeonNode End
        {
            get;
            set;
        }

        public DungeonLayout(params DungeonNode[] inputNodes)
        {
            _nodes = new DungeonNode[inputNodes.Length];
            Array.Copy(inputNodes, _nodes, inputNodes.Length);
            _lookup = new SpatialBinaryTree<DungeonNode>(_nodes);
        }

        public DungeonLayout(IEnumerable<DungeonNode> inputNodes)
        {
            _nodes = inputNodes.ToArray();
            _lookup = new SpatialBinaryTree<DungeonNode>(_nodes);
        }

        public DungeonLayout(IEnumerable<DungeonNode> nodes, DungeonNode start, DungeonNode end) 
            : this(nodes)
        {
            Start = start;
            End = end;
        }

        public DungeonNode GetRandomElement()
        {
            return _nodes[UnityEngine.Random.Range(0, _nodes.Length)];
        }

        /// <summary>
        /// Debug method to traverse all bounds of the node lookup
        /// </summary>
        /// <param name="operand"></param>
        /// <param name="node"></param>
        public void TraverseThroughLookup( Func<Rect, int, bool> operand, int depth, SpatialBinaryNode node = null)
        {
            if (node == null)
            {
                TraverseThroughLookup(operand, 0, _lookup.root);
            } 
            else
            {
                if (operand(node.bounds, depth))
                {
                    if (node.left != null)
                    {
                        TraverseThroughLookup(operand, depth+1, node.left);
                    }

                    if (node.right != null)
                    {
                        TraverseThroughLookup(operand, depth + 1, node.right);
                    }
                }
            }
        }

        public DungeonNode FindNearestSolution(Vector2 position, float maxDistance = -1)
        {
            var current = _lookup.root;

            while ( current.left != null )
            {
                if (current.bounds.Contains(position))
                {
                    current = current.left.bounds.Contains(position) ? current.left : current.right;
                }
                else
                {
                    var leftClosestPoint = RectUtil.Clamp(current.left.bounds, position);
                    var rightClosestPoint = RectUtil.Clamp(current.right.bounds, position);

                    current = (leftClosestPoint - position).sqrMagnitude < (rightClosestPoint - position).sqrMagnitude
                            ? current.left
                            : current.right;
                }
            }

            return _nodes[current.index];
        }
    }
}
