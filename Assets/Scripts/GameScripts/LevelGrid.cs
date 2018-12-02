/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Class which generates a grid which is the basis of a level
    /// </summary>
    public class LevelGrid : MonoBehaviour
    {
        /// <summary>
        /// Definitions for the floor
        /// </summary>
        private static int FloorTileIndex = 0;

        /// <summary>
        /// GameObject prefab which contains the behaviour of a horizontal wall element
        /// </summary>
        private static int HorizontalWallIndex = 1;

        /// <summary>
        /// GameObject prefab which contains the behaviour of a vertical wall element
        /// </summary>
        private static int VerticalWallIndex = 2;

        /// <summary>
        /// Prefab which contains the exit prefab
        /// </summary>
        private static int ExitIndex = 3;

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
        /// If true the position of the game object is the center, otherwise it will
        /// the level upwards
        /// </summary>
        public bool _buildAroundCenter = true;

        /// <summary>
        /// If true adds a border around the level space
        /// </summary>
        public bool _addWallBorder = true;

        /// <summary>
        /// Definitions for the level elements. Work in progress
        /// </summary>
        public LevelSpritePoolDefinition[] _levelDefinitions;

        private Vector3 _offset;

        /// <summary>
        /// Objects managing the sprite pools and the variations
        /// </summary>
        private SpriteProvider[] _spriteProviders;

        void Start()
        {
            _spriteProviders = new SpriteProvider[_levelDefinitions.Length];

            for (int i = 0; i < _spriteProviders.Length; ++i)
            {
                _spriteProviders[i] = _levelDefinitions[i].CreateProvider();
            }

            var offsetX = _buildAroundCenter ? (_width * _tileWidth) * -0.5f : 0;
            var offsetY = _buildAroundCenter ? (_height * _tileHeight) * -0.5f : 0;
            _offset = new Vector3(offsetX, offsetY, transform.position.z);

            var exitX = UnityEngine.Random.Range(1, _width - 2);
            var exitY = UnityEngine.Random.Range(1, _height - 2);

            DeterminePlayerStartPosition(exitX, exitY);

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var element = DeterminePrefab(x, y, exitX, exitY);
                    if (element != null)
                    {
                        element.transform.parent = transform;
                        element.transform.position = _offset + new Vector3(x * _tileWidth, y * _tileHeight, 0);
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
        private void DeterminePlayerStartPosition(int exitX, int exitY)
        {
            int halfWidth = _width / 2;
            int halfHeight = _height/ 2;

            int xQuadrant = exitX / halfWidth > 0 ? 0 : 1;
            int yQuadrant = exitY / halfHeight > 0 ? 0 : 1; 

            var x = UnityEngine.Random.Range(1 + xQuadrant * halfWidth, (xQuadrant + 1) * halfWidth - 1);
            var y = UnityEngine.Random.Range(1+ yQuadrant * halfHeight, (yQuadrant + 1) * halfHeight - 1);

            var player = GameObject.FindGameObjectWithTag(GameTags.Player);
            if (player != null)
            {
                var position = player.transform.position;

                position.x = x + 0.5f;
                position.y = y + 0.5f;

                player.transform.position = position + _offset;
            }
        }

        /// <summary>
        /// Determines what tile should be created for a given x, y position 
        /// </summary>
        /// <param name="x">Current x position on the map </param>
        /// <param name="y">Current y position on the map</param>
        /// <param name="exitX">X position of the exit</param>
        /// <param name="exitY">y position of the exit</param>
        /// <returns></returns>
        private GameObject DeterminePrefab(int x, int y, int exitX, int exitY)
        {
            var result = (GameObject)null;

            if (x == exitX && y == exitY)
            {
                return Obtain(ExitIndex, "exit", x, y);
            }

            if (_addWallBorder && (y == 0 || y == _height - 1))
            {
                result = Obtain(HorizontalWallIndex, "h-wall", x, y);
            }
            else if (_addWallBorder && (x == 0 || x == _width - 1))
            {
                result = Obtain(VerticalWallIndex, "v-wall", x, y);
            }
            else
            {
                result = Obtain(FloorTileIndex, "floor", x, y);
            }

            return result;
        }

        /// <summary>
        /// Wrapper getting a pool object. Intermediate method until pools actually
        /// get their elements returned to them.
        /// </summary>
        /// <param name="elementIndex"></param>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private GameObject Obtain(int elementIndex, string name, int x, int y)
        {
            var poolObject = _spriteProviders[elementIndex].Obtain();

            if (poolObject != null)
            {
                poolObject ._obj.name = name + " " + x + ", " + y;
                return poolObject._obj;
            }

            return null;
        }
    }
}
