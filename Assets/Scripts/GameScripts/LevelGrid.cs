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
        public SubdivisionAlgorithm _algorithm = new SubdivisionAlgorithm();

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

            _grid = LevelBuilder.BuildLevel(_algorithm, _width, _height);
        
            
            _offset = new Vector3((_width * _tileWidth) * -0.5f, (_height * _tileHeight) * -0.5f, transform.position.z);

            var exitX = UnityEngine.Random.Range(1, _width - 2);
            var exitY = UnityEngine.Random.Range(1, _height - 2);

            _grid[exitX, exitY]._id = LevelElementDefinitions.ExitIndex;

            _player = GameObject.FindGameObjectWithTag(GameTags.Player);

            if (_player != null)
            {
                _player.transform.position = DeterminePlayerStartPosition(exitX, exitY) + _offset;
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
                    if (x * x + y * y <viewRadiusSqr)
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

        /// <summary>
        /// Position the player in a different quadrant than the exit. This is just a temporary function to
        /// capture the need to put the exit away from the player.
        /// </summary>
        /// <param name="exitX"></param>
        /// <param name="exitY"></param>
        private Vector3 DeterminePlayerStartPosition(int exitX, int exitY)
        {            
            int halfWidth = _width / 2;
            int halfHeight = _height/ 2;

            int xQuadrant = exitX / halfWidth > 0 ? 0 : 1;
            int yQuadrant = exitY / halfHeight > 0 ? 0 : 1; 

            var x = UnityEngine.Random.Range(1 + xQuadrant * halfWidth, (xQuadrant + 1) * halfWidth - 1);
            var y = UnityEngine.Random.Range(1+ yQuadrant * halfHeight, (yQuadrant + 1) * halfHeight - 1);
            
            return new Vector3(x + 0.5f, y + 0.5f, 0);
        }
    }
}
