/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using UnityEngine;

    using Tds.Util;

    public enum EdgeDirection
    {
        UniDirectional,
        BiDirectional
    }

    /// <summary>
    /// Edge between two dungeon nodes
    /// </summary>
    public class DungeonEdge
    {
        /// <summary>
        /// Between the nodes
        /// </summary>
        public Vector2Int[] NodeIntersection
        {
            get;
            private set;
        }

        public DungeonNode From
        {
            get;
            private set;
        }

        public DungeonNode To
        {
            get;
            private set;
        }

        public EdgeDirection Direction
        {
            get;
            private set;
        }

        public DungeonEdge(DungeonNode from, DungeonNode to, EdgeDirection direction)
        {
            From = from;
            To = to;
            Direction = direction;
            NodeIntersection = RectUtil.GetIntersection(from.Rect, to.Rect);
        }

        /// <summary>
        /// Returns the from node when the given node is to and to when the node is from
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public DungeonNode GetOther(DungeonNode node)
        {
            return node == From ? To : From;
        }

        /// <summary>
        /// Length of the intersection etween the from and to nodes   
        /// </summary>
        public float IntersectionLength
        {
            get
            {
                return Vector2Int.Distance(NodeIntersection[0], NodeIntersection[1]);
            }
        }

        /// <summary>
        /// Returns the center point on  the intersection between the from and to nodes   
        /// </summary>
        public Vector2 IntersectionCenter
        {
            get
            {
                return GetIntersectionPoint(0.5f);
            }
        }

        /// <summary>
        /// Returns a random point on the intersection between the from and to nodes   
        /// </summary>
        public Vector2 RandomIntersectionPoint
        {
            get
            {
                return GetIntersectionPoint(Random.value);
            }
        }

        public Vector2 GetIntersectionPoint(float interpolation)
        {
            Vector2 direction = NodeIntersection[1] - NodeIntersection[0];
            return NodeIntersection[0] + direction * interpolation;
        }
    }
}