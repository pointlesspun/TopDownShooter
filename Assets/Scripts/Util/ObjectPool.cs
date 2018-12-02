/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.Util
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Object containing an object which belongs to an object pool and its id in that pool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PooledObject<T>
    {
        public T   _obj;
        public int _poolId;
    }

    public class ObjectPool<T> 
    {
        private List<PooledObject<T>> _availableObjects = new List<PooledObject<T>>();
        private List<PooledObject<T>> _objectsInUse = new List<PooledObject<T>>();

        public ObjectPool()
        {

        }

        public ObjectPool(Func<T> factoryMethod, int count)
        {
            Generate(factoryMethod, count);
        }

        /// <summary>
        /// Generates 'objects according to the given factory method.
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <param name="count"></param>
        public void Generate( Func<T> factoryMethod, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                _availableObjects.Add(new PooledObject<T>()
                {
                    _poolId = _availableObjects.Count,
                    _obj = factoryMethod()
                });
            }
        }

        /// <summary>
        /// Obtain a pool object if available, or null otherwise
        /// </summary>
        /// <returns></returns>
        public PooledObject<T> Obtain()
        {
            if ( _availableObjects.Count > 0)
            {
                var result = _availableObjects[0];
                result._poolId = _objectsInUse.Count;
                _objectsInUse.Add(result);
                _availableObjects.RemoveAt(0);
                return result;
            }
            return null;
        }
        /// <summary>
        /// Obtain an object, if no one is available creates another one via the factorymethod
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns></returns>
        public PooledObject<T> Obtain(Func<T> factoryMethod)
        {
            var result = Obtain();

            if (result == null)
            {
                result = new PooledObject<T>()
                {
                    _poolId = _objectsInUse.Count,
                    _obj = factoryMethod()
                };

                _objectsInUse.Add(result);
            }
            return result;
        }

        /// <summary>
        /// Release the object back into the pool
        /// </summary>
        /// <param name="obj"></param>
        public void Release(PooledObject<T> obj)
        {
            _objectsInUse.RemoveAt(obj._poolId);
            obj._poolId = _availableObjects.Count;
            _availableObjects.Add(obj);
        }
    }
}