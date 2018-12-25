/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.DungeonGeneration
{
    using UnityEngine;
    using Tds.Util;
    using System.Collections.Generic;

    /// <summary>
    /// Element describing a piece of the level
    /// </summary>
    public class LevelElement : IBounds
    {
        // cached unity object representing this element
        public PooledObject<GameObject> _poolObject;

        // id of the element
        public int _id;

        // random variation applied to this element
        public int _randomRoll;

        public Vector2Int _position;

        public Rect Bounds
        {
            get
            {
                return new Rect(_position, Vector2.one);
            }
        }

        public float Distance(LevelElement other)
        {
            return (_position - other._position).sqrMagnitude;
        }
    }
}
