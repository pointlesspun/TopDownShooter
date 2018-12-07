/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using System.Collections.Generic;
    using Tds.DungeonGeneration;
    using Tds.Util;
    using UnityEngine;

    public static class LevelBuilder
    {
        public static Grid2D<LevelElement> BuildLevel(SubdivisionAlgorithm rectSplitAlgorithm, 
                                                    SplitRectTraversalAlgorithm  traversalAlgorithm, int width, int height)
        {
            List<SplitRect> splitRectArea = rectSplitAlgorithm.Subdivide(new SplitRect(0, 0, width, height));

            // fill the grid with tiles
            var target = new Grid2D<LevelElement>(width, height,
                () => new LevelElement()
                {
                    _id = LevelElementDefinitions.None,
                    // pre-select a randomized value so the sprite variations can easily pick a random sprite / color 
                    _randomRoll = Random.Range(0, 256 * 256)
                });

            splitRectArea.ForEach((split) =>
            {
                var xOffset = split._rect.position.x;
                var yOffset = split._rect.position.y;

                for ( var x = 1; x < split._rect.width; ++x)
                {
                    for (var y = 1; y < split._rect.height; ++y)
                    {
                        target[x + xOffset, y + yOffset]._id = LevelElementDefinitions.FloorTileIndex;
                    }
                }

                // draw some borders around the bottom and left part of the rect
                var p1 = split._rect.position;
                var p2 = split._rect.position + new Vector2Int(split._rect.width, 0);
                var p3 = split._rect.position + new Vector2Int(0, split._rect.height);

                target.TraceLine(p1, p2, (x, y, grid) => grid[x, y]._id = LevelElementDefinitions.HorizontalWallIndex);
                target.TraceLine(p1 + Vector2Int.up, p3, (x, y, grid) => grid[x, y]._id = LevelElementDefinitions.VerticalWallIndex);
            });

            // add doorways
            splitRectArea.ForEach((split) =>
            {
                foreach ( var neighbour in split.Neighbours )
                {
                    var touchpoints = RectUtil.GetTouchPoints(split._rect, neighbour._rect);
                    var direction = (touchpoints[1] - touchpoints[0]);
                    direction.Clamp(Vector2Int.one * -1, Vector2Int.one);
                    direction =  new Vector2Int(-direction.y, direction.x);
                    var halfWayX = (touchpoints[1].x - touchpoints[0].x) / 2;
                    var halfWayY = (touchpoints[1].y - touchpoints[0].y) / 2;
                    var doorPosition = new Vector2Int(touchpoints[0].x + halfWayX, touchpoints[0].y + halfWayY);

                    target[doorPosition.x, doorPosition.y]._id = LevelElementDefinitions.FloorTileIndex;
                    target[doorPosition.x + direction.x, doorPosition.y + direction.y]._id = LevelElementDefinitions.FloorTileIndex;
                }
            });
        
            // draw a border around
            var levelTopLeft  = new Vector2Int(0, target.Height - 1);
            var levelTopRight = new Vector2Int(target.Width - 1, target.Height - 1);
            var levelBottomRight = new Vector2Int(target.Width - 1, 0);

            target.TraceLine(levelTopLeft, levelTopRight, (x, y, grid) => grid[x, y]._id = LevelElementDefinitions.HorizontalWallIndex);
            target.TraceLine(levelTopRight, levelBottomRight, (x, y, grid) => grid[x, y]._id = LevelElementDefinitions.VerticalWallIndex);

            return target;
        }
    }
}
