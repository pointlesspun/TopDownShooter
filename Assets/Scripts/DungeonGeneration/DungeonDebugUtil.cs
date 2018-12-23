/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using UnityEngine;

    /// <summary>
    /// Extension class to draw gizmos for a dungeon layout
    /// </summary>
    public static class DungeonDebugUtil
    {
        /// <summary>
        /// Extension method to draw a dungeon layout
        /// </summary>
        /// <param name="layout">Layout to draw</param>
        /// <param name="colors">Colors for the gizmos</param>
        /// <param name="drawStartAndEnd">Draw the start and end node</param>
        /// <param name="levelOffset">Offset of the level in the world</param>
        public static void DrawLayout(this DungeonLayout layout, Color[] colors, bool drawStartAndEnd, Vector2 levelOffset, 
                                    bool drawPaths = true)
        {
            var gizmoColorIndex = 0;

            foreach (var node in layout.Nodes)
            {
                Gizmos.color = colors[gizmoColorIndex % colors.Length];
                Gizmos.DrawCube(node.Bounds.center + levelOffset, new Vector3(node.Width, node.Height, 0));
                gizmoColorIndex++;

                if (drawPaths)
                {
                    foreach (var edge in node.Edges)
                    {
                        var intersectionCenter = edge.NodeIntersection.Interpolation(0.5f);

                        Gizmos.color = Color.black;
                        Gizmos.DrawLine(node.Bounds.center + levelOffset, intersectionCenter + levelOffset);

                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(edge.NodeIntersection.from + levelOffset, edge.NodeIntersection.to + levelOffset);
                    }
                }
            }

            if (drawStartAndEnd && layout.Start != null)
            {
                Gizmos.DrawIcon(layout.Start.Bounds.center + levelOffset, "start.png", true);
                Gizmos.DrawIcon(layout.End.Bounds.center + levelOffset, "flag.png", true);
            }
        }
    }
}
