/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.DungeonGeneration
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Tds.Util;

    /// <summary>
    /// Class which generates a grid which is the basis of a level
    /// </summary>
    public class LevelBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Algorithm which will split the level area in smaller rectangles
        /// </summary>
        public DungeonSubdivision _divisionAlgorithm = new DungeonSubdivision();

        /// <summary>
        /// Algorithm which will create a path through the area create by the divisionAlgorithm
        /// </summary>
        public DungeonTraversal _traversalAlgorithm = new DungeonTraversal();

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
        public int _tileWidth = 1;

        /// <summary>
        /// Height assigned in units of one tile
        /// </summary>
        public int _tileHeight = 1;

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
        /// Dungeon nodes making up this level
        /// </summary>
        public DungeonLayout _layout;

        /// <summary>
        /// Location where the player will start
        /// </summary>
        public Vector3 _playerStartPosition;

        /// <summary>
        /// Cached offset of the grid
        /// </summary>
        public Vector3 _levelOffset;

        public bool _drawGizmoLayout = false;
        public Color[] _layoutGizmoColors = {
                new Color(0.1f, 0.1f, 0.1f),
                new Color(0.3f, 0.45f, 0.45f),
                new Color(0.5f, 0.8f, 0.8f),
                new Color(0.7f, 1.0f, 1.0f),
        };

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
        /// Last position of the player
        /// </summary>
        private Vector3 _previousFocusPosition;

        public Vector3 StartPosition
        {
            get
            {
                return _layout != null 
                    ? new Vector3(_layout.Start.Rect.center.x, _layout.Start.Rect.center.y, 0) + _levelOffset 
                    : Vector3.zero;
            }
        }

        public bool AnyOnPath(Vector3 position, Vector3 movementDirection, float radius, Func<LevelElement,bool> predicate)
        {
            var p1 = position - _levelOffset;
            var p2 = p1 + movementDirection;

            var gridRect = _levelGrid.GetIntersection(RectUtil.CreateBoundingBox(p1, p2, radius));

            return _levelGrid.AnyInArea(gridRect, predicate);
        }

        public void BuildGrid(float levelScale)
        {
            _spriteProviders = SetupSpritePools(_levelDefinitions);

            _levelOffset = new Vector3(_width * -0.5f, _height * -0.5f, transform.position.z);

            // scale the length of the dungeon with the level
            if (_traversalAlgorithm._maxLength != -1)
            {
                _traversalAlgorithm._maxLength += _traversalAlgorithm._maxLength
                                                * levelScale
                                                * _dungeonLengthLevelScale;

                _traversalAlgorithm._maxLength = Mathf.Min(_traversalAlgorithm._maxLength, _maxDungeonLength);
            }

            _layout = BuildLevelLayout(_width, _height, _traversalAlgorithm, _divisionAlgorithm);
            _levelGrid = LevelGridFactory.CreateLevelElementGrid(_width, _height, _maxDoorLength, _layout);
        }

        /// <summary>
        /// Behaviour's update call
        /// </summary>
        public void UpdateFocus(Vector3 focusPosition)
        {
            if (HasFocusMoved(focusPosition, _previousFocusPosition))
            {
                ClearLevelElementCache();

                _previousFocusPosition = focusPosition;

                UpdateVisableGrid(focusPosition);
            }
        }

        private SpriteProvider[] SetupSpritePools(LevelSpritePoolDefinition[] definitions)
        {
            var providers = new SpriteProvider[_levelDefinitions.Length];

            for (int i = 0; i < providers.Length; ++i)
            {
                providers[i] = definitions[i].CreateProvider(gameObject);
            }

            return providers;
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
        private bool HasFocusMoved(Vector3 newPosition, Vector3 oldPosition)
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
                        var gridX = x + (int)((centerPosition.x - _levelOffset.x + _tileWidth * 0.5f) / _tileWidth);
                        var gridY = y + (int)((centerPosition.y - _levelOffset.y + _tileHeight * 0.5f) / _tileHeight);

                        if (_levelGrid.IsOnGrid(gridX, gridY))
                        {
                            var element = _levelGrid[gridX, gridY];

                            if (element._id != LevelElementDefinitions.None)
                            {
                                // obtain up a visual from the pool and place it in the new position
                                element._poolObject = _spriteProviders[element._id].Obtain(element._randomRoll);

                                if (element._poolObject != null)
                                {
                                    element._poolObject._obj.transform.position = _levelOffset + new Vector3(gridX * _tileWidth, gridY * _tileHeight, 0);
                                }

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
        //        public TraversalNode BuildLevelLayout(int width, int height, SplitRectTraversalAlgorithm traverselAlgoirithm,
        //                                              SubdivisionAlgorithm divisionAlgorithm)
        public DungeonLayout BuildLevelLayout(int width, int height, DungeonTraversal traverselAlgoirithm,
                                                     DungeonSubdivision divisionAlgorithm)
        {
            var levelRect = new RectInt(0, 0, width, height);
            var levelSubdivisions = _divisionAlgorithm.Subdivide(levelRect);
                
            return _traversalAlgorithm.Traverse(levelSubdivisions);
        }

        /// <summary>
        /// Draw the outline of the level
        /// </summary>
        public void OnDrawGizmos()
        {
            // draw the outline of the level   
            Gizmos.DrawWireCube(transform.position + Vector3.forward * 10, new Vector3(_width, _height, 0));

            if (_drawGizmoLayout && _layout != null)
            {
                _layout.DrawLayout(_layoutGizmoColors, true, _levelOffset);
            }    
        }

    }
}
