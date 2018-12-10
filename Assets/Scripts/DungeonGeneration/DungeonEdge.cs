﻿/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using System;
    using System.Collections.Generic;
    using Tds.Util;
    using UnityEngine;

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

        public DungeonNode GetOther(DungeonNode node)
        {
            return node == From ? To : From;
        }

        public float IntersectionLength
        {
            get
            {
                return Vector2Int.Distance(NodeIntersection[0], NodeIntersection[1]);
            }
        }
    }
}
