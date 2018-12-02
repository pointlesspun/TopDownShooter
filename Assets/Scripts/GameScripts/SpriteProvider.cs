/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using System;
    using System.Collections.Generic;
    using Tds.Util;
    using UnityEngine;

    /// <summary>
    /// Class providing game objects with sprite renderers from an object pool and
    /// adds variations.
    /// </summary>
    public class SpriteProvider
    {
        private ObjectPool<GameObject> _pool;
        private SpriteVariation _variations; 

        /// <summary>
        /// Initialize the provider
        /// </summary>
        /// <param name="providerId">id of this provider</param>
        /// <param name="count">Number of elements to initialize</param>
        /// <param name="factoryMethod">Method to generate an object</param>
        /// <param name="variations">Variations on the sprite</param>
        /// <param name="sortingOrder">Sorting order of the sprite</param>
        /// <returns></returns>
        public SpriteProvider Initialize(int providerId, int count, Func<GameObject> factoryMethod, SpriteVariation variations, int sortingOrder)
        {
            _pool = new ObjectPool<GameObject>(providerId, () =>
            {
                var instance = factoryMethod();
                var renderer = instance.GetComponent<SpriteRenderer>();

                renderer.sortingOrder = sortingOrder;

                instance.SetActive(false);

                return instance;
            }, count);

            _variations = variations;

            return this;
        }

        /// <summary>
        /// Returns a pool object if available or null otherwise
        /// </summary>
        /// <returns></returns>
        public PooledObject<GameObject> Obtain(int randomRoll)
        {
            var poolObject = _pool.Obtain();

            if (poolObject != null)
            {
                var spriteRenderer = poolObject._obj.GetComponent<SpriteRenderer>();

                spriteRenderer.sprite = _variations.GetRandomSprite(randomRoll);
                spriteRenderer.color = _variations.GetRandomColor(randomRoll);

                poolObject._obj.SetActive(true);
            }


            return poolObject;
        }

        /// <summary>
        /// Releases the pool object back into this provider's pool
        /// </summary>
        /// <param name="instance"></param>
        public void Release(PooledObject<GameObject> instance)
        {
            instance._obj.SetActive(false);

            _pool.Release(instance);
        }
    }

}