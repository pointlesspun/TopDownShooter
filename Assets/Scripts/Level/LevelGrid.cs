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

        void Start()
        {
            var offsetX = _buildAroundCenter ? (_width * _tileWidth) * -0.5f : 0;
            var offsetY = _buildAroundCenter ? (_height * _tileHeight) * -0.5f : 0;
            var offset = new Vector3(offsetX, offsetY, transform.position.z);

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var cloneBase = _floorPrefab;
                    var element = (GameObject)null;

                    if (_addWallBorder  && _horizontalWallPrefab != null && ( x == 0 || x == _width - 1 ))
                    {
                        element = Instantiate<GameObject>(_verticalWallPrefab);
                        element.name = "h-wall " + x + ", " + y;
                    }
                    else if (_addWallBorder && _verticalWallPrefab != null && (y == 0 || y == _height - 1))
                    {
                        element = Instantiate<GameObject>(_horizontalWallPrefab);
                        element.name = "v-wall " + x + ", " + y;
                    }
                    else
                    {
                        element = Instantiate<GameObject>(_floorPrefab);

                        var spriteRenderer = element.GetComponent<SpriteRenderer>();

                        spriteRenderer.sprite = _floorSprites[Random.Range(0, _floorSprites.Length)];
                        spriteRenderer.color = _floorColors[Random.Range(0, _floorColors.Length)];
                        spriteRenderer.sortingOrder = _floorSortingOrder;
                        element.name = "floor " + x + ", " + y;
                    }

                    element.transform.parent = transform;
                    element.transform.position = offset + new Vector3(x * _tileWidth, y * _tileHeight, 0);

                }
            }
        }
    }
}
