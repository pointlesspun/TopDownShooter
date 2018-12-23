/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.DungeonGeneration
{
    using UnityEngine;
    using Tds.Util;
    using System.Collections.Generic;

    public static class LevelGridFactory
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
                        result[x, yPosition] = new DungeonNode(new Rect(x * nodeWidth, yPosition * nodeHeight, nodeWidth, nodeHeight));
                    }
                }
            }

            for (int y = 0; y < result.Height; ++y)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    var node = result[x, y];

                    if (node != null)
                    {
                        foreach (var offset in offsetSet)
                        {
                            if (result.IsOnGrid(x + offset.x, y + offset.y))
                            {
                                var other = result[x + offset.x, y + offset.y];

                                if (other != null && other.GetEdgeTo(node) == null)
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

        /// <summary>
        /// Creates a default level element
        /// </summary>
        /// <returns></returns>
        public static LevelElement InstantiateLevelElement()
        {
            return new LevelElement()
            {
                _id = LevelElementDefinitions.None,
                // pre-select a randomized value so the sprite variations can easily pick a random sprite / color 
                _randomRoll = Random.Range(0, 256 * 256)
            };
        }

        public static Grid2D<LevelElement> CreateLevelElementGrid(int width, int height, int maxDoorLength, DungeonLayout layout)
        {
            var grid = new Grid2D<LevelElement>(width, height, () => InstantiateLevelElement());

            AddDungeonNodes(grid, layout);
            AddDungeonDoorways(grid, layout, maxDoorLength);

            // draw an outline around the "rooms" (rects) in the level that do not have an outline
            AddMisingRoomBorders(grid, layout);

            // draw the exit
            var exitPosition = layout.End.Bounds.center;
            grid[(int)exitPosition.x, (int)exitPosition.y]._id = LevelElementDefinitions.ExitIndex;

            return grid;
        }

        public static void AddDungeonNodes(Grid2D<LevelElement> grid, DungeonLayout layout)
        {
            foreach (var node in layout.Nodes)
            {
                var rect = node.Bounds.ToRectInt();
                var xOffset = rect.position.x;
                var yOffset = rect.position.y;

                for (var x = 1; x < rect.width; ++x)
                {
                    for (var y = 1; y < rect.height; ++y)
                    {
                        grid[x + xOffset, y + yOffset]._id = LevelElementDefinitions.FloorTileIndex;
                    }
                }

                // Draw some borders around the bottom and left part of the rect. We draw a wall on
                // two sides only so we draw a wall one thick in every room. A separate step
                // is required to draw the other walls iff necessary because children may surround this border
                var p1 = rect.position;
                var p2 = rect.position + new Vector2Int(rect.width, 0);
                var p3 = rect.position + new Vector2Int(0, rect.height);

                grid.TraceLine(p1, p2, (x, y, g) => grid[x, y]._id = LevelElementDefinitions.HorizontalWallIndex);
                grid.TraceLine(p1 + Vector2Int.up, p3, (x, y, g) => grid[x, y]._id = LevelElementDefinitions.VerticalWallIndex);
            }
        }


        /// <summary>
        /// Draws all intersection doorways (and changes the intersection0
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="grid"></param>
        public static void AddDungeonDoorways(Grid2D<LevelElement> grid, DungeonLayout layout, int maxDoorLength)
        {
            var edgesDrawn = new HashSet<DungeonEdge>();

            foreach (var node in layout.Nodes)
            {
                // recursively draw the level element for each child
                if (node.Edges != null)
                {
                    foreach (var edge in node.Edges)
                    {
                        if (!edgesDrawn.Contains(edge))
                        {
                            var doorwayIntersection = DrawDoorWay(grid, edge.NodeIntersection, maxDoorLength);

                            if (doorwayIntersection.from.x == doorwayIntersection.to.x)
                            {
                                edge.NodeIntersection = doorwayIntersection.Translate(0, -0.5f);
                            }
                            else
                            {
                                edge.NodeIntersection = doorwayIntersection.Translate(-0.5f, 0);
                            }

                            edgesDrawn.Add(edge);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws the doorway and modifies the intersection to fit the actual door drawn
        /// </summary>
        /// <param name="intersection"></param>
        /// <param name="grid"></param>
        public static Line2D DrawDoorWay(Grid2D<LevelElement> grid, Line2D nodeIntersection, int maxDoorLength)
        {
            var intersection = nodeIntersection
                                        .Randomized(1, maxDoorLength, 1, 1)
                                        .Snap();

            grid.TraceLine(intersection.from, intersection.to, (x, y, g) =>
            {
                grid[x, y]._id = LevelElementDefinitions.FloorTileIndex;
            });

            return intersection;
        }

        /// <summary>
        /// Last step of creating a grid, fill out the 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="grid"></param>
        public static void AddMisingRoomBorders(Grid2D<LevelElement> grid, DungeonLayout layout)
        {
            foreach (var node in layout.Nodes)
            {
                var rect = node.Bounds.ToRectInt();
                var x1 = rect.min.x;
                var x2 = rect.max.x;

                var y1 = rect.min.y;
                var y2 = rect.max.y;

                // if we're at the top of the grid, slightly tweak the offset and adjust the condition for
                // drawing a wall
                var isAboveGrid = (y2 >= grid.Height);
                var offset = isAboveGrid ? -1 : 0;
                var condition = isAboveGrid ? LevelElementDefinitions.FloorTileIndex : LevelElementDefinitions.None;

                // trace the top left to the top right
                for (var x = x1; x <= x2; x++)
                {
                    var y = y2 + offset;

                    if (grid.IsOnGrid(x, y) && grid[x, y]._id == condition)
                    {
                        grid[x, y]._id = LevelElementDefinitions.HorizontalWallIndex;
                    }
                }

                var isRightOfGrid = (x2 >= grid.Width);
                offset = isRightOfGrid ? -1 : 0;
                condition = isRightOfGrid ? LevelElementDefinitions.FloorTileIndex : LevelElementDefinitions.None;

                // trace the top right to the bottom right
                for (var y = y1; y < y2; y++)
                {
                    var x = x2 + offset;

                    if (grid.IsOnGrid(x, y) && grid[x, y]._id == condition)
                    {
                        grid[x, y]._id = LevelElementDefinitions.VerticalWallIndex;
                    }
                }
            }
        }
    }
}

