/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Enum capturing the decision whether or not to continue splitting a rect.
    /// Mostly for debug purposes
    /// </summary>
    public enum ContinueSubdivisionDecision
    {
        /// <summary>
        /// Rect can potentially still be divivde d over the other axis
        /// </summary>
        AttemptOtherAxis,

        /// <summary>
        /// Randomly decided not to split this any further
        /// </summary>
        RandomRollFailed,

        /// <summary>
        /// MAx depth has been reached, no need to further divide this
        /// </summary>
        MaxDepthReached,

        /// <summary>
        /// No more space , cannot subdivide this rect any further
        /// </summary>
        RectTooSmall,

        /// <summary>
        /// Do continue
        /// </summary>
        ContinueSubdivision
    }

    /// <summary>
    /// Object which forms a 'node' in the level graph.
    /// </summary>
    [Serializable]
    public class SplitRect
    {
        /// <summary>
        /// Debug reason why this rect wasn't split any further
        /// </summary>
        public ContinueSubdivisionDecision _reasonForSplitStoppedAtThisPoint;

        /// <summary>
        /// Size of this node
        /// </summary>
        public RectInt _rect;

        /// <summary>
        /// Element providing a visual for this rect
        /// </summary>
        public GameObject DebugElement
        {
            get;
            set;
        }

        /// <summary>
        /// Neighbours around this rect
        /// </summary>
        public HashSet<SplitRect> Neighbours
        {
            get;
            set;
        }

        public SplitRect()
        {
            Neighbours = new HashSet<SplitRect>();
        }

        public SplitRect(RectInt rect) : this()
        {
            _rect = rect;
        }

        public SplitRect(int x, int y, int width, int height) : this( new RectInt(0, 0, width, height))
        {
        }

        /// <summary>
        /// Remove all neighbour relations
        /// </summary>
        public void DisconnectFromNeighbours()
        {
            foreach (var n in Neighbours)
            {
                n.Neighbours.Remove(this);
            }

            Neighbours.Clear();
        }
    }
}
