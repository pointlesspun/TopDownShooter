/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using UnityEngine;

    using Tds.Util;

    /// <summary>
    /// Direction of the edge. Mainly for completeness purposes as all edges in the current application are bidirectional.
    /// </summary>
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
        public Line2D NodeIntersection
        {
            get;
            set;
        }

        /// <summary>
        /// From node
        /// </summary>
        public DungeonNode From
        {
            get;
            private set;
        }

        /// <summary>
        /// The other node
        /// </summary>
        public DungeonNode To
        {
            get;
            private set;
        }

        /// <summary>
        /// Direction of the edge
        /// </summary>
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
            NodeIntersection = RectUtil.GetIntersection(from.Bounds, to.Bounds);
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
                return NodeIntersection.Length;
            }
        }

        /// <summary>
        /// Returns the center point on  the intersection between the from and to nodes   
        /// </summary>
        public Vector2 IntersectionCenter
        {
            get
            {
                return NodeIntersection.Interpolation(0.5f);
            }
        }

        /// <summary>
        /// Returns a random point on the intersection between the from and to nodes   
        /// </summary>
        public Vector2 RandomIntersectionPoint
        {
            get
            {
                return NodeIntersection.Interpolation(Random.value);
            }
        }        
    }
}