/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.DungeonGeneration
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Utility class capturing all possible sprite variations.
    /// </summary>
    [Serializable]
    public class SpriteVariation
    {
        public Sprite[] _spriteVariations;
        public Color[] _colorVariations;

        public Sprite GetRandomSprite()
        {
            return _spriteVariations[UnityEngine.Random.Range(0, _spriteVariations.Length)];
        }

        public Sprite GetRandomSprite(int randomRoll)
        {
            return _spriteVariations[randomRoll % _spriteVariations.Length];
        }

        public Color GetRandomColor()
        {
            return  _colorVariations[UnityEngine.Random.Range(0, _colorVariations.Length)];
        }

        public Color GetRandomColor(int randomRoll)
        {
            return _colorVariations[randomRoll % _colorVariations.Length];
        }
    } 
}
