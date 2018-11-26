/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    /// <summary>
    /// Class which generates a grid which is the basis of a level
    /// </summary>
    public class LevelGrid : MonoBehaviour
    {
        /// <summary>
        /// GameObject prefab which contains the behaviour of a floor element
        /// </summary>
        public GameObject _floorPrefab;

        /// <summary>
        /// GameObject prefab which contains the behaviour of a horizontal wall element
        /// </summary>
        public GameObject _horizontalWallPrefab;

        /// <summary>
        /// GameObject prefab which contains the behaviour of a vertical wall element
        /// </summary>
        public GameObject _verticalWallPrefab;

        /// <summary>
        /// Prefab which contains the exit prefab
        /// </summary>
        public GameObject _exitLevelPrefab;

        /// <summary>
        /// Sprites from which the floor generation can choose to create a level from
        /// </summary>
        public Sprite[] _floorSprites;

        /// <summary>
        /// Colors from which the floor generation can choose to assign to the floor sprite
        /// </summary>
        public Color[] _floorColors;

        /// <summary>
        /// Sorting order of the floor
        /// </summary>
        public int _floorSortingOrder = 0;

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

        private Vector3 _offset;

        void Start()
        {
            var offsetX = _buildAroundCenter ? (_width * _tileWidth) * -0.5f : 0;
            var offsetY = _buildAroundCenter ? (_height * _tileHeight) * -0.5f : 0;
            _offset = new Vector3(offsetX, offsetY, transform.position.z);

            var exitX = Random.Range(1, _width - 2);
            var exitY = Random.Range(1, _height - 2);

            DeterminePlayerStartPosition(exitX, exitY);

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var element = DeterminePrefab(x, y, exitX, exitY);
                    element.transform.parent = transform;
                    element.transform.position = _offset + new Vector3(x * _tileWidth, y * _tileHeight, 0);
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

            var x = Random.Range(1 + xQuadrant * halfWidth, (xQuadrant + 1) * halfWidth - 1);
            var y = Random.Range(1+ yQuadrant * halfHeight, (yQuadrant + 1) * halfHeight - 1);

            var position = GlobalGameState._playerObject.transform.position;

            position.x = x + 0.5f;
            position.y = y + 0.5f;

            GlobalGameState._playerObject.transform.position = position + _offset;
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

            if (_exitLevelPrefab != null && x == exitX && y == exitY)
            {
                result = Instantiate(_exitLevelPrefab);
                result.name = "exit " + x + ", " + y;
                return result;
            }

            if (_addWallBorder && _horizontalWallPrefab != null && (x == 0 || x == _width - 1))
            {
                result = Instantiate(_verticalWallPrefab);
                result.name = "h-wall " + x + ", " + y;
            }
            else if (_addWallBorder && _verticalWallPrefab != null && (y == 0 || y == _height - 1))
            {
                result = Instantiate(_horizontalWallPrefab);
                result.name = "v-wall " + x + ", " + y;
            }
            else
            {
                result = Instantiate(_floorPrefab);

                var spriteRenderer = result.GetComponent<SpriteRenderer>();

                spriteRenderer.sprite = _floorSprites[Random.Range(0, _floorSprites.Length)];
                spriteRenderer.color = _floorColors[Random.Range(0, _floorColors.Length)];
                spriteRenderer.sortingOrder = _floorSortingOrder;
                result.name = "floor " + x + ", " + y;
            }

            return result;
        }

    }
}
