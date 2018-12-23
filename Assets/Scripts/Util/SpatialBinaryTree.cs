/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Tds.Util
{
    public interface IRectangle
    {
        Rect Bounds { get; }
    }

    public class CompareMinXBounds<T> : IComparer<T> where T : IRectangle
    {
        public int Compare(T x, T y)
        {
            return x.Bounds.xMin.CompareTo(y.Bounds.xMin);
        }
    }

    public class CompareMinYBounds<T> : IComparer<T> where T : IRectangle
    {
        public int Compare(T x, T y)
        {
            return x.Bounds.yMin.CompareTo(y.Bounds.yMin);
        }
    }

    public class SpatialBinaryTree<T> where T : IRectangle
    {
        public SpatialBinaryNode root;
        public T[] data;

        public SpatialBinaryTree( T[] treeData )
        {
            data = treeData;
            root = SpatialTreeConstruction<T>.Split(data, 0, data.Length);
        }
    }

    public class SpatialBinaryNode
    {
        public int index;
        public int length;
        public SpatialBinaryNode left;
        public SpatialBinaryNode right;
        public Rect bounds = Rect.zero;
    }

    public static class SpatialTreeConstruction<T> where T : IRectangle
    {
        public static CompareMinXBounds<T> minXComparer = new CompareMinXBounds<T>();
        public static CompareMinYBounds<T> minYComparer = new CompareMinYBounds<T>();

        public delegate int SplitFunction(T[] dataSet, int index, int length, ref Rect union);

        public static SpatialBinaryNode Split(T[] dataSet, int index, int length)
        {            
            var bounds = Rect.zero;

            if (FindHorizontalPivot(dataSet, index, length, ref bounds) >= 0)
            {
                return SplitNode(dataSet, index, length, FindHorizontalPivot);
            }

            bounds = Rect.zero;

            if (FindVerticalPivot(dataSet, index, length, ref bounds) >= 0)
            {
                return SplitNode(dataSet, index, length, FindVerticalPivot);
            }

            return new SpatialBinaryNode()
            {
                bounds = bounds,
                index = index,
                length = length
            };
        }

        public static SpatialBinaryNode SplitNode(T[] dataSet, int index, int length, SplitFunction function)
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
                SplitFunction nextFunction = function == FindHorizontalPivot ?
                                                (SplitFunction)FindVerticalPivot :
                                                (SplitFunction)FindHorizontalPivot;

                node.left = SplitNode(dataSet, index, pivot, nextFunction);
                node.right = SplitNode(dataSet, pivot + index, length - pivot, nextFunction);
            }          
            else
            {
                function = function == FindHorizontalPivot ?
                                                 (SplitFunction)FindVerticalPivot :
                                                 (SplitFunction)FindHorizontalPivot;

                pivot = function(dataSet, index, length, ref bounds);

                if (pivot > 0 && pivot < length)
                {
                    SplitFunction nextFunction = function == FindHorizontalPivot ?
                                                    (SplitFunction)FindVerticalPivot :
                                                    (SplitFunction)FindHorizontalPivot;

                    node.left = SplitNode(dataSet, index, pivot, nextFunction);
                    node.right = SplitNode(dataSet, pivot + index, length - pivot, nextFunction);
                }
            }

            node.bounds = bounds;

            return node;
        }

        public static int FindHorizontalPivot(T[] dataSet, int index, int length, ref Rect union) 
        {
            union = dataSet[index].Bounds;

            // guard for trivial cases
            if ( length <= 1)
            {
                return index;
            }

            Array.Sort<T>(dataSet, index, length, minXComparer);
            
            float maxX = dataSet[index].Bounds.xMax;            
            int bestIndex = -1;
            float bestValue = -1;
            float midPoint = length / 2.0f;

            for ( int i = 0; i < length; ++i)
            {
                var t = dataSet[i + index];

                union = t.Bounds.Union(union);

                if (t.Bounds.xMin >= maxX)
                {
                    var value = midPoint - Math.Abs(((float)i) - midPoint);

                    if ( value > bestValue)
                    {
                        bestValue = value;
                        bestIndex = i;
                    }
                }

                if (t.Bounds.xMax > maxX )
                {
                    maxX = t.Bounds.xMax;
                }
            }

            return bestIndex;
        }

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
    }
}
