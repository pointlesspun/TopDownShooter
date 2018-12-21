/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using System.Linq;

    using UnityEngine;

    using Tds.Util;

    /// <summary>
    /// Algorithm which randomly traverses through a dungeon (list of dungeon nodes),
    /// creating a smaller dungeon with guaranteed path
    /// </summary>
    public class DungeonLayoutDebugBehaviour : MonoBehaviour
    {
        public DungeonLayout Layout
        {
            get;
            private set;
        }

        public int _width = 40;
        public int _height = 40;

        [TextArea(3, 10)]
        public string _textInput;
        public bool _useTextInput = false;
        public int _textInputRectWidth = 10;
        public int _textInputRectHeight = 10;
        public Vector2Int _startCoordinate = Vector2Int.zero;
        public Vector2Int _endCoordinate = Vector2Int.zero;


        public DungeonSubdivision _subdivisionAlgorithm = new DungeonSubdivision();
        public bool _useTraversal = false;
        public DungeonTraversal _traversalAlgorithm = new DungeonTraversal(); 

        /// <summary>
        /// Colors used to color the dungeon nodes
        /// </summary>
        public Color[] _gizmoColors = new Color[] { Color.white, Color.yellow, Color.cyan, Color.blue, Color.green, Color.red };

        public void BuildLayout()
        {
            if (_useTextInput)
            {
                var input = _textInput.Split('\n');
                var grid = LevelGridFactory.CreateFrom(input, _textInputRectWidth, _textInputRectHeight);
                var nodes = grid.Values.Where(v => v != null);
                Layout = new DungeonLayout(nodes)
                {
                    Start = GetAndValidateNode(_startCoordinate, grid, "Error getting start node"),
                    End = GetAndValidateNode(_endCoordinate, grid, "Error getting end node")
                };
            }
            else
            {
                Layout = _subdivisionAlgorithm.Subdivide(new RectInt(0, 0, _width, _height));
            }

            if (_useTraversal)
            {
                Layout = _traversalAlgorithm.Traverse(Layout);
            } 
        }

        private DungeonNode GetAndValidateNode( Vector2Int coordinate, Grid2D<DungeonNode> grid, string message)
        {
            DungeonNode result = null;

            if ( grid.IsOnGrid(coordinate.x, coordinate.y))
            {
                result = grid[coordinate.x, coordinate.y];

                if (result != null )
                {
                    return result;
                }

                Debug.LogError(message + ", coordinate " + coordinate + " does not contain a dungeon node." );

            }
            else
            {
                Debug.LogError(message + ", coordinate " + coordinate + " is not with in grid bounds (" + grid.Width + ", " + grid.Height + ").");
            }

            return result;
        }

        public void OnDrawGizmos()
        {
            if (Layout != null)
            {
                Layout.DrawLayout(_gizmoColors, _useTraversal, Vector2.zero);
            }
        }
    }
}
