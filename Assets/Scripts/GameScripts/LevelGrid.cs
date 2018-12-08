/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using System.Collections.Generic;

    using UnityEngine;

    using Tds.DungeonGeneration;
    using Tds.Util;

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

        /// <summary>
        /// Each level makes the length of the dungeon this much longer
        /// </summary>
        public float _dungeonLengthLevelScale = 0.25f;

        /// <summary>
        /// Length of a door connecting one room to another
        /// </summary>
        public int _maxDoorLength = 4;

        /// <summary>
        /// Maxium length of a dungeon
        /// </summary>
        public float _maxDungeonLength = 500;

        /// <summary>
        /// Location where the player will start
        /// </summary>
        private Vector3 _playerStartPosition;

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
        private Grid2D<LevelElement> _levelGrid;

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
            _spriteProviders = SetupSpritePools(_levelDefinitions);

            _offset = new Vector3((_width * _tileWidth) * -0.5f, (_height * _tileHeight) * -0.5f, transform.position.z);

            var pathRoot = BuildLevel();

            InitializePlayerPosition(pathRoot._split._rect.center);
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

        private SpriteProvider[] SetupSpritePools(LevelSpritePoolDefinition[] definitions)
        {
            var providers = new SpriteProvider[_levelDefinitions.Length];

            for (int i = 0; i < providers.Length; ++i)
            {
                providers[i] = definitions[i].CreateProvider();
            }

            return providers;
        }

        private void InitializePlayerPosition(Vector3 startPosition)
        {
            _player = GameObject.FindGameObjectWithTag(GameTags.Player);

            if (_player != null)
            {
                _playerStartPosition = startPosition;
                _player.transform.position = _playerStartPosition + _offset;
                _previousPlayerPosition = new Vector3(float.MaxValue, float.MaxValue, 0);
            }
        }

        private TraversalNode BuildLevel()
        {
            // scale the length of the dungeon with the level
            var gameStateObject = GameObject.FindGameObjectWithTag(GameTags.GameState);

            if (gameStateObject != null && _traversalAlgorithm._maxLength != -1)
            {
                var levelScale = gameStateObject.GetComponent<GameStateBehaviour>()._levelScale;
                _traversalAlgorithm._maxLength += _traversalAlgorithm._maxLength
                                                * levelScale
                                                * _dungeonLengthLevelScale;

                _traversalAlgorithm._maxLength = Mathf.Min(_traversalAlgorithm._maxLength, _maxDungeonLength);
            }

            var pathRoot = BuildLevelLayout(_width, _height, _traversalAlgorithm, _divisionAlgorithm);

            _levelGrid = BuildLevelGrid(_width, _height, pathRoot);
            
            return pathRoot;
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

                        if (_levelGrid.IsOnGrid(gridX, gridY))
                        {
                            var element = _levelGrid[gridX, gridY];

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
       
        /// <summary>
        /// Build a layout of a given width and height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="traverselAlgoirithm"></param>
        /// <param name="divisionAlgorithm"></param>
        /// <returns>The root of a path through the level rectangle </returns>
        public TraversalNode BuildLevelLayout(int width, int height, SplitRectTraversalAlgorithm traverselAlgoirithm,
                                                SubdivisionAlgorithm divisionAlgorithm)
        {
            var levelRect = new SplitRect(0, 0, width, height);
            var levelSubdivisions = _divisionAlgorithm.Subdivide(levelRect);
                
            return _traversalAlgorithm.TraverseSplitRects(levelSubdivisions);
        }

        /// <summary>
        /// Build a 2d grid of level elements of the given dimensions and the traversal path
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pathRoot"></param>
        /// <returns></returns>
        public Grid2D<LevelElement> BuildLevelGrid(int width, int height, TraversalNode pathRoot)
        {
            // fill the grid with tiles
            var grid = new Grid2D<LevelElement>(width, height,
                () => new LevelElement()
                {
                    _id = LevelElementDefinitions.None,
                    // pre-select a randomized value so the sprite variations can easily pick a random sprite / color 
                    _randomRoll = UnityEngine.Random.Range(0, 256 * 256)
                });

            // recursively build out the level starting from the root
            CreateLevelElement(pathRoot, grid);

            // draw an outline around the "rooms" (rects) in the level that do not have an outline
            TraceRoomBorders(pathRoot, grid);

            return grid;
        }

        /// <summary>
        /// Draw the outline of the level
        /// </summary>
        public void OnDrawGizmos()
        {
            // draw the outline of the level   
            Gizmos.DrawWireCube(transform.position + Vector3.forward * 10, new Vector3(_width, _height, 0));
        }

        /// <summary>
        /// Create a level element and recursively create elements for its children
        /// </summary>
        /// <param name="node"></param>
        /// <param name="grid"></param>
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

            // Draw some borders around the bottom and left part of the rect. We draw a wall on
            // two sides only so we draw a wall one thick in every room. A separate step
            // is required to draw the other walls iff necessary because children may surround this border
            var p1 = split._rect.position;
            var p2 = split._rect.position + new Vector2Int(split._rect.width, 0);
            var p3 = split._rect.position + new Vector2Int(0, split._rect.height);

            grid.TraceLine(p1, p2, (x, y, g) => grid[x, y]._id = LevelElementDefinitions.HorizontalWallIndex);
            grid.TraceLine(p1 + Vector2Int.up, p3, (x, y, g) => grid[x, y]._id = LevelElementDefinitions.VerticalWallIndex);

            if (node._parent != null)
            {
                // draw a doorway to the parent's room
                DrawDoorWay(node, node._parent, node._parentIntersection, grid);
            }

            // recursively draw the level element for each child
            node._children.ForEach((child) =>
            {
                DrawDoorWay(node, child, child._parentIntersection, grid);
                CreateLevelElement(child, grid);
            });

            // draw an exit if this is the last room on the primary path 
            if (node._children.Count == 0 && node._isPrimaryPath)
            {
                var center = node._split._rect.center;
                grid[(int)center.x, (int)center.y]._id = LevelElementDefinitions.ExitIndex;
            }
        }

        /// <summary>
        /// Draw a doorway between the parent and child
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="intersection"></param>
        /// <param name="grid"></param>
        private void DrawDoorWay(TraversalNode parent, TraversalNode child, Vector2Int[] intersection, Grid2D<LevelElement> grid)
        {
            var x1 = intersection[0].x;
            var x2 = intersection[1].x;

            var y1 = intersection[0].y;
            var y2 = intersection[1].y;

            // vertical intersection with child on the left side ?
            if (x1 == x2 && x1 <= parent._split._rect.min.x)
            {
                var wallLength = y2 - y1;
                var doorLength = Mathf.Min(_maxDoorLength, Random.Range(1, wallLength - 2));
                var doorStart = Random.Range(1, wallLength - (doorLength+1));

                for (var i = 0; i < doorLength; ++i)
                {
                    grid[x1, y1 + doorStart + i]._id = LevelElementDefinitions.FloorTileIndex;
                }
            }
            // horizontal intersection with child the bottom side ?
            else if (y1 == y2 && y1 <= parent._split._rect.min.y)
            {
                var wallLength = x2 - x1;
                var doorLength = Mathf.Min(_maxDoorLength, Random.Range(1, wallLength - 2));
                var doorStart = Random.Range(1, wallLength - (doorLength + 1));

                for (var i = 0; i < doorLength; ++i)
                {
                    grid[x1 + doorStart + i, y1]._id = LevelElementDefinitions.FloorTileIndex;
                }
            }
        }



        /// <summary>
        /// Last step of creating a grid, fill out the 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="grid"></param>
        private void TraceRoomBorders(TraversalNode node, Grid2D<LevelElement> grid)
        {
            var x1 = node._split._rect.min.x;
            var x2 = node._split._rect.max.x;

            var y1 = node._split._rect.min.y;
            var y2 = node._split._rect.max.y;

            // if we're at the top of the grid, slightly tweak the offset and adjust the condition for
            // drawing a wall
            var isAboveGrid = (y2 >= grid.Height);
            var offset = isAboveGrid  ? -1 : 0;
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
            
            node._children.ForEach((c) => TraceRoomBorders(c, grid));
        }
    }
}
