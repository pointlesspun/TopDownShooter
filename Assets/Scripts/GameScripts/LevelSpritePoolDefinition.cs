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
    /// Unity Editor data structure used to define a sprite provider
    /// </summary>
    [Serializable]
    public class LevelSpritePoolDefinition
    {
        public SpriteVariation _variations;
        public GameObject _prefab;
        public int _sortOrder = 0;
        public int _poolSize = 1;
        public int _poolId = 0;

        public SpriteProvider CreateProvider()
        {
            return new SpriteProvider().Initialize(_poolId, _poolSize, () => GameObject.Instantiate(_prefab), _variations, _sortOrder);
        }
    }
}