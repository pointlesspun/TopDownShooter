/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */

 /// <summary>
 /// Set of classes around constructing a kd tree from a set of rectangles
 /// </summary>
namespace Tds.Util
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Animations;

    /// <summary>
    /// Compare function for sorting an array of IBounds objects against the y min
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CompareMinXBounds<T> : IComparer<T> where T : IBounds
    {
        public int Compare(T x, T y)
        {
            return x.Bounds.xMin.CompareTo(y.Bounds.xMin);
        }
    }

    /// <summary>
    /// Compare function for sorting an array of IBounds objects against the y min
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class CompareMinYBounds<T> : IComparer<T> where T : IBounds
    {
        public int Compare(T x, T y)
        {
            return x.Bounds.yMin.CompareTo(y.Bounds.yMin);
        }
    }

    /// <summary>
    /// Delegate for a pivot function. Normally the approach would be to use lambdas but sadly
    /// this didn't work out for the Array sorting hence the somewhat clunky delegate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataSet"></param>
    /// <param name="index"></param>
    /// <param name="length"></param>
    /// <param name="union"></param>
    /// <returns></returns>
    public delegate int PivotFunction<T>(T[] dataSet, int index, int length, ref Rect union);

    /// <summary>
    /// Node for the SpatialBinaryTree 
    /// </summary>
    public class SpatialBinaryNode
    {
        /// <summary>
        /// Index in a data set (defined in the tree) 
        /// </summary>
        public int index;
        /// <summary>
        /// Length of the data set
        /// </summary>
        public int length;

        /// <summary>
        /// Left child 
        /// </summary>
        public SpatialBinaryNode left;

        /// <summary>
        /// Right child
        /// </summary>
        public SpatialBinaryNode right;

        /// <summary>
        /// Bounds of this node which should equal the union over all the data eleents
        /// </summary>
        public Rect bounds = Rect.zero;
    }

    /// <summary>
    /// Class used to construct a spatial look up tree from a given set of IBounds objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpatialBinaryTree<T> where T : IBounds
    {
        public static CompareMinXBounds<T> minXComparer = new CompareMinXBounds<T>();
        public static CompareMinYBounds<T> minYComparer = new CompareMinYBounds<T>();

        /// <summary>
        /// Find a pivot point on the horizontal axis where the given data set gets split in two 
        /// equally sized sub sets (if possible).
        /// </summary>
        /// <param name="dataSet">A non null data set of IBounds objects</param>
        /// <param name="index">The start index in the data set</param>
        /// <param name="length">The length of the data to balance</param>
        /// <param name="union">Bounding box over the elements of the given data set</param>
        /// <returns></returns>
        public static int FindHorizontalPivot(T[] dataSet, int index, int length, ref Rect union)
        {
            union = dataSet[index].Bounds;

            // guard for trivial cases
            if (length <= 1)
            {
                return index;
            }

            // sort by min x position
            Array.Sort<T>(dataSet, index, length, minXComparer);

            float maxX = dataSet[index].Bounds.xMax;
            int bestIndex = -1;
            float bestValue = -1;
            float midPoint = length / 2.0f;

            // iterate over the data points, we find a pivot if the given position
            // splits the data set in two without overlapping elements
            for (int i = 0; i < length; ++i)
            {
                var t = dataSet[i + index];

                union = t.Bounds.Union(union);

                // all elements after this are right of the previous element
                // so we found a pivot
                if (t.Bounds.xMin >= maxX)
                {
                    // determine the value of this pivot, the closer to the midsection
                    // the better
                    var value = midPoint - Math.Abs(((float)i) - midPoint);

                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestIndex = i;
                    }
                }

                // move the right side test if needed
                if (t.Bounds.xMax > maxX)
                {
                    maxX = t.Bounds.xMax;
                }
            }

            return bestIndex;
        }

        /// <summary>
        /// See FindHorizontalPivot
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="union"></param>
        /// <returns></returns>
        public static int FindVerticalPivot(T[] dataSet, int index, int length, ref Rect union)
        {
            union = dataSet[index].Bounds;

            // guard for trivial cases
            if (length <= 1)
            {
                return index;
            }

            Array.Sort(dataSet, index, length, minYComparer);

            float maxY = dataSet[index].Bounds.yMax;
            int bestIndex = -1;
            float bestValue = -1;
            float midPoint = length / 2.0f;

            for (int i = 0; i < length; ++i)
            {
                var t = dataSet[i + index];

                union = t.Bounds.Union(union);

                if (t.Bounds.yMin >= maxY)
                {
                    var value = midPoint - Math.Abs(i - midPoint);

                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestIndex = i;
                    }
                }

                if (t.Bounds.yMax > maxY)
                {
                    maxY = t.Bounds.yMax;
                }
            }

            return bestIndex;
        }

        /// <summary>
        /// Root of this tree
        /// </summary>
        public SpatialBinaryNode root;

        /// <summary>
        /// Data set to which the nodes refer to 
        /// </summary>
        public T[] data;

        /// <summary>
        /// Create a lookup from the given data
        /// </summary>
        /// <param name="treeData"></param>
        public SpatialBinaryTree( T[] treeData, Axis preferentialStartAxis = Axis.None)
        {
            data = treeData;
            root = Split(data, 0, data.Length, preferentialStartAxis);
        }

        /// <summary>
        /// Split the given data set in the range from index to index + length along the preferred axis
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="preferentialStartAxis">If set to none a default axis will be chosen</param>
        /// <returns></returns>
        private SpatialBinaryNode Split(T[] dataSet, int index, int length, Axis preferentialStartAxis = Axis.None)
        {
            switch (preferentialStartAxis)
            {
                case Axis.None:
                    return UnityEngine.Random.value >= 0.5f
                            ? SplitNode(dataSet, index, length, FindHorizontalPivot)
                            : SplitNode(dataSet, index, length, FindVerticalPivot);
                case Axis.X:
                    return SplitNode(dataSet, index, length, FindVerticalPivot);
                case Axis.Y:
                    return SplitNode(dataSet, index, length, FindHorizontalPivot);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Split the given data set in the range from index to index + length using the given 
        /// pivot function determine the most optimal mid point
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="preferentialStartAxis">If set to none a default axis will be chosen</param>
        /// <returns></returns>
        public SpatialBinaryNode SplitNode(T[] dataSet, int index, int length, PivotFunction<T> function)
        {
            var node = new SpatialBinaryNode()
            {
                bounds = Rect.zero,
                index = index,
                length = length
            };

            var bounds = node.bounds;
            var pivot = function(dataSet, index, length, ref bounds);

            if (pivot > 0 && pivot < length)
            {
                PivotFunction<T> nextFunction = function == FindHorizontalPivot ?
                                                (PivotFunction<T>)FindVerticalPivot :
                                                (PivotFunction<T>)FindHorizontalPivot;

                node.left = SplitNode(dataSet, index, pivot, nextFunction);
                node.right = SplitNode(dataSet, pivot + index, length - pivot, nextFunction);
            }
            else
            {
                function = function == FindHorizontalPivot ?
                                                 (PivotFunction<T>)FindVerticalPivot :
                                                 (PivotFunction<T>)FindHorizontalPivot;

                bounds = node.bounds;
                pivot = function(dataSet, index, length, ref bounds);

                if (pivot > 0 && pivot < length)
                {
                    PivotFunction<T> nextFunction = function == FindHorizontalPivot ?
                                                    (PivotFunction<T>)FindVerticalPivot :
                                                    (PivotFunction<T>)FindHorizontalPivot;

                    node.left = SplitNode(dataSet, index, pivot, nextFunction);
                    node.right = SplitNode(dataSet, pivot + index, length - pivot, nextFunction);
                }
            }

            node.bounds = bounds;

            return node;
        }
    }
}
