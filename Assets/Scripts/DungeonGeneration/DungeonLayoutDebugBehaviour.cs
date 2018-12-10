/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using UnityEngine;

    /// <summary>
    /// Algorithm which randomly traverses through a dungeon (list of dungeon nodes),
    /// creating a smaller dungeon with guaranteed path
    /// </summary>
    public class DungeonLayoutDebugBehaviour : MonoBehaviour
    {
        private DungeonLayout _layout;

        public int _width = 40;
        public int _height = 40;

        public DungeonSubdivision _subdivisionAlgorithm = new DungeonSubdivision();
        public bool _useTraversal = false;
        public DungeonTraversal _traversalAlgorithm = new DungeonTraversal(); 

        /// <summary>
        /// Colors used to color the dungeon nodes
        /// </summary>
        public Color[] _gizmoColors = new Color[] { Color.white, Color.yellow, Color.cyan, Color.blue, Color.green, Color.red };

        public void BuildLayout()
        {
            _layout = _subdivisionAlgorithm.Subdivide(new RectInt(0, 0, _width, _height));

            if (_useTraversal)
            {
                _layout = _traversalAlgorithm.Traverse(_layout);
            } 
        }

        public void OnDrawGizmos()
        {
            if (_layout != null)
            {
                var gizmoColorIndex = 0;

                foreach( var node in _layout.Nodes)
                {
                    Gizmos.color = _gizmoColors[gizmoColorIndex % _gizmoColors.Length];
                    Gizmos.DrawCube(node.Rect.center, new Vector3(node.Width, node.Height, 0));
                    gizmoColorIndex++;

                    foreach ( var edge in node.Edges )
                    {
                        var centerOffset = edge.NodeIntersection[1] - edge.NodeIntersection[0];

                        centerOffset.x /= 2;
                        centerOffset.y /= 2;

                        var intersectionCenter = new Vector2( edge.NodeIntersection[0].x + centerOffset.x, 
                                                                        edge.NodeIntersection[0].y + centerOffset.y);
                        Gizmos.color = Color.black;

                        Gizmos.DrawLine(node.Rect.center, intersectionCenter);
                    }
                }

                if (_useTraversal && _layout.Start != null)
                {
                    Gizmos.DrawIcon(_layout.Start.Rect.center, "start.png", true);
                    Gizmos.DrawIcon(_layout.End.Rect.center, "flag.png", true);
                }
            }
        }
    }
}
