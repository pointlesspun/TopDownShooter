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
    /// Nodes which captures a path through a collection of subdivisions
    /// </summary>
    public class TraversalNode
    {
        /// <summary>
        /// Parent of this node
        /// </summary>
        public TraversalNode _parent;

        /// <summary>
        /// Intersection with the parent node
        /// </summary>
        public Vector2Int[] _parentIntersection;

        /// <summary>
        /// List of all branches from this node
        /// </summary>
        public List<TraversalNode> _children = new List<TraversalNode>();

        /// <summary>
        /// Split reference to the node defining the geometry and neighbours
        /// </summary>
        public SplitRect _split;
      
        /// <summary>
        /// Current length from the root
        /// </summary>
        public float _pathLength;

        /// <summary>
        /// Flag indicating if this element is the main path
        /// </summary>
        public bool _isPrimaryPath = false;

        /// <summary>
        /// Element providing a visual for this node
        /// </summary>
        public GameObject DebugElement
        {
            get;
            set;
        }

        public TraversalNode()
        {
        }

        public TraversalNode(int x, int y, int w, int h)
        {
            _split = new SplitRect()
            {
                _rect = new RectInt(x, y, w, h)
            };

        }

        public TraversalNode AddChild(TraversalNode child)
        {
            _children.Add(child);
            child._parent = this;
            return child;
        }
    }
}
