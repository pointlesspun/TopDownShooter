/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{

    using UnityEngine;

    using Tds.PathFinder;
    using Tds.Util;

    public class LevelGridSearchSpace: ISearchSpace<LevelElement, Vector2>
    {
        private DungeonLayout _layout;
        public Grid2D<LevelElement> Grid
        {
            get;
            private set;
        }
    
        public LevelGridSearchSpace(Grid2D<LevelElement> grid, DungeonLayout layout)
        {
            _layout = layout;
            Grid = grid;
        }

        public LevelElement FindNearestSolution(Vector2 location, float maxDistance)
        {
            var node = _layout.FindNearestSolution(location, maxDistance);

            if (node != null)
            {
                var clampedLocation = RectUtil.Clamp(node.Bounds, location, 0.1f, 0.1f);
                return Grid.FindNearestAround(new Vector2Int(Mathf.FloorToInt(clampedLocation.x), Mathf.FloorToInt(clampedLocation.y)),
                                               node.Bounds.ToRectInt(),
                                            (element, distance) => element._id == LevelElementDefinitions.FloorTileIndex,
                                            true);
            }

            return null;
        }

        public Vector2 GetInterpolatedLocation(LevelElement from, LevelElement to, float value, Vector2 fallbackLocation)
        {
            if (from != null && to != null)
            {
                Vector2 direction = from._position - to._position;
                return from._position + (direction * value);
            }

            return fallbackLocation;
        }

        public void GetWaypoints(LevelElement from, LevelElement to, Vector2[] waypoints, Vector2 offset, bool randomize)
        {
        }
        
        public LevelElement GetRandomElement()
        {
            var node = _layout.GetRandomElement();

            if (node != null)
            {
                var xStart = Random.Range(0, node.Bounds.width);
                var yStart = Random.Range(0, node.Bounds.height);

                for (var x = 0; x < node.Width; x++)
                {
                    for (var y = 0; y < node.Height; y++)
                    {
                        var xPrime = (x + xStart) % node.Bounds.width;
                        var yPrime = (y + yStart) % node.Bounds.height;
                        var element = Grid[Mathf.FloorToInt(xPrime), Mathf.FloorToInt(yPrime)];

                        if (element._id == LevelElementDefinitions.FloorTileIndex)
                        {
                            return element;
                        }
                    }
                }
            }

            return null;
        }

        public bool AreNeighbours(LevelElement from, LevelElement to)
        {
            throw new System.NotImplementedException();
        }
    }
}