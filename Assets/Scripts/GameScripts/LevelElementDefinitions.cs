/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{    
    public static class LevelElementDefinitions
    {
        /// <summary>
        /// Definitions for the floor
        /// </summary>
        public readonly static int None= -1;

        /// <summary>
        /// Definitions for the floor
        /// </summary>
        public readonly static int FloorTileIndex = 0;

        /// <summary>
        /// GameObject prefab which contains the behaviour of a horizontal wall element
        /// </summary>
        public readonly static int HorizontalWallIndex = 1;

        /// <summary>
        /// GameObject prefab which contains the behaviour of a vertical wall element
        /// </summary>
        public readonly static int VerticalWallIndex = 2;

        /// <summary>
        /// Prefab which contains the exit prefab
        /// </summary>
        public readonly static int ExitIndex = 3;
    }
}
