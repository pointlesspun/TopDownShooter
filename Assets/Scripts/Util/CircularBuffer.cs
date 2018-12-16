/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
using System;
using UnityEngine;

namespace Tds.Util
{
    public class CircularBuffer<T>
    {
        private int _startIndex;
        private int _endIndex;

        private T[] _elements;
        
        public T this[int index]
        {
            get
            {
                Contract.Requires(index >= 0 && index < _elements.Length, "index is out of range");
                return _elements[(_startIndex + index) % _elements.Length];
            }
        }

        public int Count
        {
            get
            {
                return _endIndex - _startIndex;
            }
        }

        public CircularBuffer(int size)
        {
            _elements = new T[size];
        }

        public CircularBuffer(int size, Func<T> factoryMethod) 
            : this(size)
        {
            for (int i = 0; i < size; ++i)
            {
                Add(factoryMethod());
            }
        }

        public T Add(T value)
        {
            var insertIndex = _endIndex % _elements.Length;
            _elements[insertIndex] = value;

            if (_endIndex < int.MaxValue)
            {
                _endIndex++;
                _startIndex = Mathf.Max(_startIndex, _endIndex - _elements.Length);
            }
            else
            {
                _endIndex = (int.MaxValue % _elements.Length) + _elements.Length + 1;
                _startIndex = _endIndex - _elements.Length;
            }

            return value;
        }

        public void Clear()
        {
            _startIndex = 0;
            _endIndex = 0;
        }

        public T Last
        {
            get
            {
                return this[_elements.Length - 1];
            }
        }

        public int Size
        {
            get
            {
                return _elements.Length;
            }
        }


        // for debug & testing purposes only
        public void __D(int e)
        {
            _endIndex = Mathf.Max(0, e);
            _startIndex = Mathf.Max(0, e - _elements.Length);
        }
    }
}
