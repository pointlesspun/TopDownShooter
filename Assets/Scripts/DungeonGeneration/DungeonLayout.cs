/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using System.Collections.Generic;
    using Tds.PathFinder;
    using UnityEngine;

    /// <summary>
    /// Algorithm which randomly traverses through a dungeon (list of dungeon nodes),
    /// creating a smaller dungeon with guaranteed path
    /// </summary>
    public class DungeonLayout : ISearchSpace<DungeonNode, Vector2>
    {
        // to do replace with quadtree
        private List<DungeonNode> _nodes;

        public IEnumerable<DungeonNode> Nodes
        {
            get { return _nodes;  }
        }

        public DungeonNode Start
        {
            get;
            set;
        }

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
            return _nodes[UnityEngine.Random.Range(0, _nodes.Count)];
        }

        public DungeonNode AddNode(DungeonNode node)
        {
            _nodes.Add(node);
            return node;
        }

        public Vector2 GetInterpolatedLocation(DungeonNode from, DungeonNode to, float value, Vector2 fallbackLocation)
        {
            var edge = from.GetEdgeTo(to);

            if (edge != null)
            {
                return  edge.GetIntersectionPoint(value);
            }

            return fallbackLocation;
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
