/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using System;
    using System.Collections.Generic;
    using Tds.DungeonGeneration;
    using Tds.Util;
    using UnityEngine;

    /// <summary>
    /// Element describing a piece of the level
    /// </summary>
    public class LevelElement
    {
        // cached unity object representing this element
        public PooledObject<GameObject> _poolObject;

        // id of the element
        public int _id;

        // random variation applied to this element
        public int _randomRoll;
    }

    /// <summary>
    /// Class which generates a grid which is the basis of a level
    /// </summary>
    public class LevelGrid : MonoBehaviour
    {
        /// <summary>
        /// Algorithm which will split the level area in smaller rectangles
        /// </summary>
        public SubdivisionAlgorithm _divisionAlgorithm = new SubdivisionAlgorithm();

        /// <summary>
        /// Algorithm which will create a path through the area create by the SubdivisionAlgorithm
        /// </summary>
        public SplitRectTraversalAlgorithm _traversalAlgorithm = new SplitRectTraversalAlgorithm();

        /// <summary>
        /// Width in units of the level
        /// </summary>
        public int _width = 1;

        /// <summary>
        /// Height in units of the level
        /// </summary>
        public int _height = 1;

        /// <summary>
        /// Width assigned in units of one tile
        /// </summary>
        public int _tileWidth = 64;

        /// <summary>
        /// Height assigned in units of one tile
        /// </summary>
        public int _tileHeight = 64;

        /// <summary>
        /// Definitions for the level elements. Work in progress
        /// </summary>
        public LevelSpritePoolDefinition[] _levelDefinitions;

        /// <summary>
        /// How far are tiles being rendered from the point of focus
        /// </summary>
        public int _viewRadius = 6;

        public Vector3 _playerStartPosition;

        /// <summary>
        /// Cached offset of the grid
        /// </summary>
        private Vector3 _offset;

        /// <summary>
        /// Objects managing the sprite pools and the variations
        /// </summary>
        private SpriteProvider[] _spriteProviders;

        /// <summary>
        /// Contains the level data
        /// </summary>
        private Grid2D<LevelElement> _grid;

        /// <summary>
        /// Cached level elements (floors, walls) currently visible
        /// </summary>
        private List<LevelElement> _elementCache = new List<LevelElement>();

        /// <summary>
        /// Cached reference to the player
        /// </summary>
        private GameObject _player;

        /// <summary>
        /// Last position of the player
        /// </summary>
        private Vector3 _previousPlayerPosition;

        public void Start()
        {
            _spriteProviders = new SpriteProvider[_levelDefinitions.Length];

            for (int i = 0; i < _spriteProviders.Length; ++i)
            {
                _spriteProviders[i] = _levelDefinitions[i].CreateProvider();
            }

            _grid = BuildLevel();

            _offset = new Vector3((_width * _tileWidth) * -0.5f, (_height * _tileHeight) * -0.5f, transform.position.z);

          
            _player = GameObject.FindGameObjectWithTag(GameTags.Player);

            if (_player != null)
            {
                _player.transform.position = _playerStartPosition + _offset;
            }

            _previousPlayerPosition = new Vector3(float.MaxValue, float.MaxValue, 0);
        }

        /// <summary>
        /// Behaviour's update call
        /// </summary>
        public void Update()
        {
            if (_player != null)
            {
                var centerPosition = _player.transform.position;

                if (hasPlayerMoved(centerPosition, _previousPlayerPosition))
                {
                    ClearLevelElementCache();

                    _previousPlayerPosition = centerPosition;

                    UpdateVisableGrid(centerPosition);
                }
            }
        }

        /// <summary>
        /// Clears the cached level elements, returning them to the pools
        /// </summary>
        private void ClearLevelElementCache()
        {
            _elementCache.ForEach(element =>
            {
                _spriteProviders[element._poolObject._poolId].Release(element._poolObject);
            });

            _elementCache.Clear();
        }

        /// <summary>
        /// Checks if the player has moved a position on the discrete 2d grid
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="oldPosition"></param>
        /// <returns></returns>
        private bool hasPlayerMoved(Vector3 newPosition, Vector3 oldPosition)
        {
            return ((int)newPosition.x) != ((int)oldPosition.x)
                || ((int)newPosition.y) != ((int)oldPosition.y);
        }

        /// <summary>
        /// updates the parts of the level which are in range of the playuer
        /// </summary>
        /// <param name="centerPosition"></param>
        private void UpdateVisableGrid(Vector3 centerPosition)
        {
            var viewRadiusSqr = _viewRadius * _viewRadius;

            for (var x = -_viewRadius; x <= _viewRadius; x++)
            {
                for (var y = -_viewRadius; y <= _viewRadius; y++)
                {
                    // check if this grid position is in the view radius
                    if (x * x + y * y < viewRadiusSqr)
                    {
                        var gridX = x + (int)((centerPosition.x - _offset.x + _tileWidth * 0.5f) / _tileWidth);
                        var gridY = y + (int)((centerPosition.y - _offset.y + _tileHeight * 0.5f) / _tileHeight);

                        if (_grid.IsOnGrid(gridX, gridY))
                        {
                            var element = _grid[gridX, gridY];

                            if (element._id != LevelElementDefinitions.None)
                            {
                                // obtain up a visual from the pool and place it in the new position
                                element._poolObject = _spriteProviders[element._id].Obtain(element._randomRoll);
                                element._poolObject._obj.transform.position = _offset + new Vector3(gridX * _tileWidth, gridY * _tileHeight, 0);

                                _elementCache.Add(element);
                            }
                        }
                    }
                }
            }
        }
       
        public Grid2D<LevelElement> BuildLevel()
        {
            var pathRoot = _traversalAlgorithm.TraverseSplitRects(_divisionAlgorithm.Subdivide(new SplitRect(0, 0, _width, _height)));

            _playerStartPosition = pathRoot._split._rect.center;

            // fill the grid with tiles
            var target = new Grid2D<LevelElement>(_width, _height,
                () => new LevelElement()
                {
                    _id = LevelElementDefinitions.None,
                    // pre-select a randomized value so the sprite variations can easily pick a random sprite / color 
                    _randomRoll = UnityEngine.Random.Range(0, 256 * 256)
                });

            CreateLevelElement(pathRoot, target);
            TraceRoomBorders(pathRoot, target);

            return target;
        }


        public void OnDrawGizmos()
        {
            // draw the outline of the level   
            Gizmos.DrawWireCube(transform.position + Vector3.forward * 10, new Vector3(_width, _height, 0));
        }

        private void CreateLevelElement(TraversalNode node, Grid2D<LevelElement> grid)
        {
            var split = node._split;
            var xOffset = split._rect.position.x;
            var yOffset = split._rect.position.y;

            for (var x = 1; x < split._rect.width; ++x)
            {
                for (var y = 1; y < split._rect.height; ++y)
                {
                    grid[x + xOffset, y + yOffset]._id = LevelElementDefinitions.FloorTileIndex;
                }
            }

            // draw some borders around the bottom and left part of the rect
            var p1 = split._rect.position;
            var p2 = split._rect.position + new Vector2Int(split._rect.width, 0);
            var p3 = split._rect.position + new Vector2Int(0, split._rect.height);

            grid.TraceLine(p1, p2, (x, y, g) => grid[x, y]._id = LevelElementDefinitions.HorizontalWallIndex);
            grid.TraceLine(p1 + Vector2Int.up, p3, (x, y, g) => grid[x, y]._id = LevelElementDefinitions.VerticalWallIndex);

            if (node._parent != null)
            {
                DrawDoorWay(node, node._parent, node._parentIntersection, grid);
            }

            node._children.ForEach((child) =>
            {
                DrawDoorWay(node, child, child._parentIntersection, grid);
                CreateLevelElement(child, grid);
            });

            if (node._children.Count == 0 && node._isPrimaryPath)
            {
                var center = node._split._rect.center;
                grid[(int)center.x, (int)center.y]._id = LevelElementDefinitions.ExitIndex;
            }
        }

        private void DrawDoorWay(TraversalNode parent, TraversalNode child, Vector2Int[] intersection, Grid2D<LevelElement> grid)
        {
            var x1 = intersection[0].x;
            var x2 = intersection[1].x;

            var y1 = intersection[0].y;
            var y2 = intersection[1].y;

            // vertical intersection with child on the left side ?
            if (x1 == x2 && x1 <= parent._split._rect.min.x)
            {
                grid[x1, y1 + (y2 - y1) / 2]._id = LevelElementDefinitions.FloorTileIndex;
            }
            // horizontal intersection with child the bottom side ?
            else if (y1 == y2 && y1 <= parent._split._rect.min.y)
            {
                grid[x1 + (x2 - x1) / 2, y1]._id = LevelElementDefinitions.FloorTileIndex;
            }
        }

        private void TraceRoomBorders(TraversalNode node, Grid2D<LevelElement> grid)
        {
            var x1 = node._split._rect.min.x;
            var x2 = node._split._rect.max.x;

            var y1 = node._split._rect.min.y;
            var y2 = node._split._rect.max.y;

            if (y2 >= grid.Height)
            {
                // trace the top left to the top right
                for (var x = x1; x <= x2; x++)
                {
                    if (grid.IsOnGrid(x, y2-1) && grid[x, y2-1]._id == LevelElementDefinitions.FloorTileIndex)
                    {
                        grid[x, y2-1]._id = LevelElementDefinitions.HorizontalWallIndex;
                    }
                }
            }
            else
            {
                // trace the top left to the top right
                for (var x = x1; x <= x2; x++)
                {
                    if (grid.IsOnGrid(x, y2) && grid[x, y2]._id == LevelElementDefinitions.None)
                    {
                        grid[x, y2]._id = LevelElementDefinitions.HorizontalWallIndex;
                    }
                }
            }

            if (x2 >= grid.Width)
            {
                // trace the top right to the bottom right
                for (var y = y1; y < y2; y++)
                {
                    if (grid.IsOnGrid(x2-1, y) && grid[x2-1, y]._id == LevelElementDefinitions.FloorTileIndex)
                    {
                        grid[x2-1, y]._id = LevelElementDefinitions.VerticalWallIndex;
                    }
                }
            }
            else
            {
                // trace the top right to the bottom right
                for (var y = y1; y < y2; y++)
                {
                    if (grid.IsOnGrid(x2, y) && grid[x2, y]._id == LevelElementDefinitions.None)
                    {
                        grid[x2, y]._id = LevelElementDefinitions.VerticalWallIndex;
                    }
                }
            }

            node._children.ForEach((c) => TraceRoomBorders(c, grid));

        }
    }
}
