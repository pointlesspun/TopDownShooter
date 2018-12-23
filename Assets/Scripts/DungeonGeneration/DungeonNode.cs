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

    /// <summary>
    /// Node of a dungeon. 
    /// </summary>
    public class DungeonNode : IRectangle
    {
        // debug id
        private static int _idCounter = 0;

        /// <summary>
        /// Edges connecting this node to other nodes
        /// </summary>
        private List<DungeonEdge> _edges;

        /// <summary>
        /// Debug id of this node
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Human readable name of this node
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Dimensions of this node
        /// </summary>
        public Rect Bounds
        {
            get;
            private set;
        }

        /// <summary>
        /// Neighbours around this rect
        /// </summary>
        public IEnumerable<DungeonEdge> Edges
        {
            get { return _edges;  }
        } 

        /// <summary>
        /// Width of this node (convenience method for Rect.width)
        /// </summary>
        public float Width
        {
            get { return Bounds.width; }
        }

        /// <summary>
        /// Height of this node (convenience method for Rect.height)
        /// </summary>
        public float Height
        {
            get { return Bounds.height; }
        }

        /// <summary>
        /// Min of this node (convenience method for Rect.min)
        /// </summary>
        public Vector2 Min
        {
            get { return Bounds.min; }
        }

        /// <summary>
        /// Max of this node (convenience method for Rect.max)
        /// </summary>
        public Vector2 Max
        {
            get { return Bounds.max; }
        }

        /// <summary>
        /// Convenience mthod to iterate over all neighbours - use for debug purposes only
        /// </summary>
        public IEnumerable<DungeonNode> Neighbours
        {
            get
            {
                return Edges == null ? null : Edges.Select(e => e.GetOther(this));
            }
        }

        public DungeonNode(Rect dimensions )
        {
            Bounds = dimensions;
            Id = _idCounter++;
        }

        public DungeonNode(int x, int y, int w, int h)
        {
            Bounds = new Rect(x,y,w,h);
            Id = _idCounter++;
        }

        public DungeonNode[] DivideOverXAxis(int height)
        {
            var result = new DungeonNode[2];

            result[0] = new DungeonNode( new Rect(Bounds.position, new Vector2(Bounds.width, height)));
            result[1] = new DungeonNode(new Rect(Bounds.position + Vector2.up * height,
                                                        new Vector2(Bounds.width, Bounds.height - height)));

            return result;
        }

        public DungeonNode[] DivideOverYAxis(int width)
        {
            var result = new DungeonNode[2];

            result[0] = new DungeonNode(new Rect(Bounds.position, new Vector2(width, Bounds.height)));
            result[1] = new DungeonNode(new Rect(Bounds.position + Vector2.right * width,
                                                        new Vector2(Bounds.width - width, Bounds.height)));


            return result;
        }

        public void Disconnect()
        {
            if (_edges != null)
            {
                foreach( var edge in _edges )
                {
                    var other = edge.GetOther(this);
                    other.RemoveEdge(edge);
                }

                _edges.Clear();
            }
        }

        public void AddEdge(DungeonEdge edge)
        {
            if (_edges == null)
            {
                _edges = new List<DungeonEdge>();
            }

            _edges.Add(edge);
        }

        public void RemoveEdge(DungeonEdge edge)
        {
            _edges.Remove(edge);
        }

        public void ClearEdges()
        {
            _edges.Clear();
        }

        public DungeonEdge GetEdgeTo(DungeonNode other)
        {
            if (Edges != null)
            {
                return Edges.FirstOrDefault(e => e.GetOther(this) == other);
            }

            return null;
        }

        public float Distance(Vector2 point)
        {
            return (point - RectUtil.Clamp(Bounds, point)).sqrMagnitude;
        
        } 

        public bool ContainsEdge(DungeonEdge edge)
        {
            return _edges.Contains(edge);
        }

        public bool ContainsPoint(Vector2 point)
        {
            return point.x >= Bounds.min.x && point.x < Bounds.max.x
                && point.y >= Bounds.min.y && point.y < Bounds.max.y;
        }

        public float Distance(DungeonNode other)
        {
            return (Bounds.center - other.Bounds.center).magnitude;
        }

        public DungeonNode SelectRandomNeighbour( Func<DungeonEdge, DungeonNode, bool> predicate)
        {
            if (_edges != null)
            {
                var count = _edges.Count;
                var startIndex = UnityEngine.Random.Range(0, count);

                for (int i = 0; i < _edges.Count; ++i)
                {
                    var edge = _edges[(i + startIndex) % count];
                    var subject = edge.GetOther(this);

                    if (predicate(edge, subject))
                    {
                        return subject;
                    }
                }
            }

            return null;
        }

        public static DungeonEdge Connect(DungeonNode nodeA, DungeonNode nodeB)
        {
            var edge = new DungeonEdge(nodeA, nodeB, EdgeDirection.BiDirectional);

            nodeA.AddEdge(edge);
            nodeB.AddEdge(edge);

            return edge;
        }
    }
}
