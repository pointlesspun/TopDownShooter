/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */


namespace Tds.DungeonGeneration
{
    using Tds.Util;
    using UnityEngine;

    public static class DungeonGenerationUtil
    {
        /// <summary>
        /// Create a dungeon from a string where a space will be a void 
        /// and a # a node. All nodes with top / bottom / left / right neighbour will be connected.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Grid2D<DungeonNode> CreateFrom(string[] input, int nodeWidth = 1, int nodeHeight = 1)
        {
            Vector2Int[] offsetSet = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
            var result = new Grid2D<DungeonNode>(input[0].Length, input.Length, () => null);

            for (int y = 0; y < input.Length; ++y)
            {
                for (int x = 0; x < input[y].Length; x++)
                {
                    if (input[y][x] == '#')
                    {
                        var yPosition = result.Height - (y + 1);
                        result[x, yPosition] = new DungeonNode(new RectInt(x * nodeWidth, yPosition * nodeHeight, nodeWidth, nodeHeight));
                    }
                }
            }

            for (int y = 0; y < result.Height; ++y)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    var node = result[x, y];

                    if ( node != null)
                    {
                        foreach( var offset in offsetSet)
                        {
                            if ( result.IsOnGrid(x + offset.x, y + offset.y ))
                            {
                                var other = result[x + offset.x, y + offset.y];

                                if ( other != null && other.GetEdgeTo(node) == null)
                                {
                                    DungeonNode.Connect(node, other);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
