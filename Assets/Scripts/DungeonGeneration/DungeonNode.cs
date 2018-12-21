﻿/*
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
    public class DungeonNode 
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
        public RectInt Rect
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
        public int Width
        {
            get { return Rect.width; }
        }

        /// <summary>
        /// Height of this node (convenience method for Rect.height)
        /// </summary>
        public int Height
        {
            get { return Rect.height; }
        }

        /// <summary>
        /// Min of this node (convenience method for Rect.min)
        /// </summary>
        public Vector2Int Min
        {
            get { return Rect.min; }
        }

        /// <summary>
        /// Max of this node (convenience method for Rect.max)
        /// </summary>
        public Vector2Int Max
        {
            get { return Rect.max; }
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

        public DungeonNode(RectInt dimensions )
        {
            Rect = dimensions;
            Id = _idCounter++;
        }

        public DungeonNode(int x, int y, int w, int h)
        {
            Rect = new RectInt(x,y,w,h);
            Id = _idCounter++;
        }

        public DungeonNode[] DivideOverXAxis(int height)
        {
            var result = new DungeonNode[2];

            result[0] = new DungeonNode( new RectInt(Rect.position, new Vector2Int(Rect.width, height)));
            result[1] = new DungeonNode(new RectInt(Rect.position + Vector2Int.up * height,
                                                        new Vector2Int(Rect.width, Rect.height - height)));

            return result;
        }

        public DungeonNode[] DivideOverYAxis(int width)
        {
            var result = new DungeonNode[2];

            result[0] = new DungeonNode(new RectInt(Rect.position, new Vector2Int(width, Rect.height)));
            result[1] = new DungeonNode(new RectInt(Rect.position + Vector2Int.right * width,
                                                        new Vector2Int(Rect.width - width, Rect.height)));


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
            return (point - RectUtil.Clamp(Rect, point)).sqrMagnitude;
        
        } 

        public bool ContainsEdge(DungeonEdge edge)
        {
            return _edges.Contains(edge);
        }

        public bool ContainsPoint(Vector2 point)
        {
            return point.x >= Rect.min.x && point.x < Rect.max.x
                && point.y >= Rect.min.y && point.y < Rect.max.y;
        }

        public float Distance(DungeonNode other)
        {
            return (Rect.center - other.Rect.center).magnitude;
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
