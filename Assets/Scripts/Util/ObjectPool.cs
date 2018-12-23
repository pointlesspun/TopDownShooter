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
        public int _indexId;
        public int _poolId;

        public override string ToString()
        {
            return _obj.ToString() + "[" + _indexId + "@" + _poolId + "]";
        }
    }

    /// <summary>
    /// Data structure storing pre-created objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> 
    {
        private List<PooledObject<T>> _availableObjects = new List<PooledObject<T>>();
        private HashSet<PooledObject<T>> _objectsInUse = new HashSet<PooledObject<T>>();

        private readonly int _poolId;

        public ObjectPool(int id)
        {
            _poolId = id;
        }

        public ObjectPool(int id, Func<T> factoryMethod, int count) : this(id)
        {
            Generate(factoryMethod, count);
        }

        public void Clear()
        {
            foreach ( var poolObject in _objectsInUse )
            {
                _availableObjects.Add(poolObject);
            }

            _objectsInUse.Clear();
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
                    _indexId = _availableObjects.Count,
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
                result._indexId = _objectsInUse.Count;
                result._poolId = _poolId;
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
                    _indexId = _objectsInUse.Count,
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
            _objectsInUse.Remove(obj);
            obj._indexId = _availableObjects.Count;
            _availableObjects.Add(obj);
        }
    }
}