/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using UnityEngine;

    public static class DungeonDebugUtil
    {
        public static void DrawLayout(this DungeonLayout layout, Color[] colors, bool drawStartAndEnd, Vector2 levelOffset)
        {
            var gizmoColorIndex = 0;

            foreach (var node in layout.Nodes)
            {
                Gizmos.color = colors[gizmoColorIndex % colors.Length];
                Gizmos.DrawCube(node.Rect.center + levelOffset, new Vector3(node.Width, node.Height, 0));
                gizmoColorIndex++;

                foreach (var edge in node.Edges)
                {
                    var intersectionCenter = edge.NodeIntersection.Interpolation(0.5f);

                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(node.Rect.center + levelOffset, intersectionCenter + levelOffset);

                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(edge.NodeIntersection.from + levelOffset, edge.NodeIntersection.to + levelOffset);
                }
            }

            if (drawStartAndEnd && layout.Start != null)
            {
                Gizmos.DrawIcon(layout.Start.Rect.center + levelOffset, "start.png", true);
                Gizmos.DrawIcon(layout.End.Rect.center + levelOffset, "flag.png", true);
            }
        }
    }
}
