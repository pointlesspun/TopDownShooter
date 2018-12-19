/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.Util
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Wrapper around a 2d array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Grid2D<T> 
    {
        private T[] _data;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public T this[int x, int y]
        {
            get
            {
                return _data[x + y * Width];
            }

            set
            {
                _data[x + y * Width] = value;
            }
        }

        /// <summary>
        /// Returns all values in this grid stored in an enumerable. May contain null values.
        /// </summary>
        public IEnumerable<T> Values
        {
            get
            {
                return _data;
            }
        }

        /// <summary>
        /// Creates a grid of width x height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="factoryMethod">a method which constructs a T element </param>
        public Grid2D( int width, int height, Func<T> factoryMethod)
        {
            Contract.Requires(width > 0, "Grid2D Width needs to be greater than 0");
            Contract.Requires(height > 0, "Grid2D height needs to be greater than 0");

            Width = width;
            Height = height;
            _data = new T[width * height];
            for (int i = 0; i < width * height; ++i)
            {
                _data[i] = factoryMethod(); 
            }
        }
        
        /// <summary>
        /// Checks if the given position is on or or off the grid (x or y are smaller than 0 
        /// or x or y are greater than widht/ height)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsOnGrid(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        /// <summary>
        /// Returns the intersection of the given rect with the grid, clamped by the grid's bounding box
        /// </summary>
        /// <param name="r"></param>
        /// <returns>A discrete rect spanning the given rectangle </returns>
        public RectInt GetIntersection(Rect r)
        {
            return new RectInt()
            {
                min = new Vector2Int(Mathf.Clamp(Mathf.FloorToInt(r.min.x), 0, Width), 
                                        Mathf.Clamp(Mathf.FloorToInt(r.min.y), 0, Height)),
                max = new Vector2Int(Mathf.Clamp(Mathf.CeilToInt(r.max.x), 0, Width),
                                        Mathf.Clamp(Mathf.CeilToInt(r.max.y), 0, Height))
            };
        }

        /// <summary>
        /// Checks if any element the given area matches the predicate
        /// </summary>
        /// <param name="area"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool AnyInArea(RectInt area, Func<T, bool> predicate)
        {
            for (int x = area.min.x; x <= area.max.x; x++)
            {
                for (int y = area.min.y; y <= area.max.y; y++)
                {
                    if (predicate(this[x,y]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Traces a line from the from point to the to point. For each discrete point
        /// reached the callback is called. 
        /// Note the x / y positions may be off the grid
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="callback"></param>
        public void TraceLine(Vector2 from, Vector2 to, Action<int, int, Grid2D<T>> callback)
        {
            var dx = Mathf.Abs(to.x - from.x);
            var dy = Mathf.Abs(to.y - from.y);
            var signX = (from.x < to.x) ? 1 : -1;
            var signY = (from.y < to.y) ? 1 : -1;

            var error = dx - dy;
            var x = from.x;
            var y = from.y;

            // while haven't reached the destination taking in account an epsilon       
            while (!(Mathf.Abs(x - to.x) < 0.0001 && (Mathf.Abs(y - to.y) < 0.0001))) 
            {
                callback((int)x, (int)y, this);

                var e2 = error * 2;

                if (e2 > -dy) {
                    error -= dy;
                    x += signX;
                }

                if (e2 < dx) {
                    error += dx;
                    y += signY;
                }
            }
        }
    }
}
