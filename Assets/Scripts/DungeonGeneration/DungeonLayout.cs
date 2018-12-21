/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using System.Collections.Generic;
    using Tds.PathFinder;
    using Tds.Util;
    using UnityEngine;

    /// <summary>
    /// Algorithm which randomly traverses through a dungeon (list of dungeon nodes),
    /// creating a smaller dungeon with guaranteed path
    /// </summary>
    public class DungeonLayout 
    {
        // to do replace with quadtree
        private List<DungeonNode> _nodes;

        /// <summary>
        /// Collection of nodes, use for debug purposes only
        /// </summary>
        public IEnumerable<DungeonNode> Nodes
        {
            get { return _nodes; }
        }

        /// <summary>
        /// Number of nodes in this layout
        /// </summary>
        public int NodeCount
        {
            get
            {
                return _nodes.Count;
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

        public DungeonLayout()
        {
            _nodes = new List<DungeonNode>();
        }

        public DungeonLayout(params DungeonNode[] nodes)
        {
            _nodes = new List<DungeonNode>(nodes);
        }

        public DungeonLayout(IEnumerable<DungeonNode> nodes)
        {
            _nodes = new List<DungeonNode>(nodes);
        }

        public DungeonLayout(IEnumerable<DungeonNode> nodes, DungeonNode start, DungeonNode end) 
            : this(nodes)
        {
            Start = start;
            End = end;
        }

        public DungeonNode GetRandomElement()
        {
            return _nodes[Random.Range(0, _nodes.Count)];
        }

        public DungeonNode AddNode(DungeonNode node)
        {
            _nodes.Add(node);
            return node;
        }
        
        public DungeonNode FindNearestSolution(Vector2 position, float maxDistance = -1)
        {
            DungeonNode result = null;
            float bestDistance = float.MaxValue;

            foreach (var node in _nodes)
            {
                if (node.ContainsPoint(position))
                {
                    return node;
                }

                var distance = node.Distance(position);

                if (distance < bestDistance && (maxDistance == -1 || distance < maxDistance))
                {
                    bestDistance = distance;
                    result = node;
                }
            }

            return result;
        }

    }
}
