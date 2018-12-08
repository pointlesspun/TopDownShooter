/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
    using UnityEngine.Animations;

    using Tds.Util;
  
    /// <summary>
    /// Algorithm which randomly sub divides a rect in binary parts
    /// </summary>
    [Serializable]
    public class SubdivisionAlgorithm
    {
        /// <summary>
        /// Smallest allowed rectangle width
        /// </summary>
        public int _minRectWidth = 5;

        /// <summary>
        /// Smallest allowed rectangle height
        /// </summary>
        public int _minRectHeight = 5;

        /// <summary>
        /// Minimum number of splits which need to be executed
        /// </summary>
        public int _minDepth = 3;

        /// <summary>
        /// Max number of splits. If -1 there is no maximum.
        /// </summary>
        public int _maxDepth = -1;

        /// <summary>
        /// Fixed random seed, if -1 no random seed is assumed
        /// </summary>
        public int _seed = 42;

        /// <summary>
        /// Value between 0 and 1 indicating the node may not be split any further
        /// </summary>
        public float _stopChanceBase = 0;

        /// <summary>
        /// Random chance between 0 and 1 related to depth
        /// </summary>
        public float _stopChanceDepthFactor = 0;

        /// <summary>
        /// Subdivides the given root node in binary pieces according to the settings in this class
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public List<SplitRect> Subdivide(SplitRect root)
        {
            var initiailRandomState = UnityEngine.Random.state;

            if (_seed >= 0)
            {
                UnityEngine.Random.InitState(_seed);
            }

            List<SplitRect> result = new List<SplitRect>();

            TrySubdivideSplitRect(root, result, 0, UnityEngine.Random.value > 0.5f ? Axis.Y : Axis.X);

            if (_seed >= 0)
            {
                UnityEngine.Random.state = initiailRandomState;
            }

            return result;
        }

        /// <summary>
        /// Heuristic testing if the subdivision can continue
        /// </summary>
        /// <param name="split"></param>
        /// <param name="depth"></param>
        /// <param name="divisionAxis"></param>
        /// <returns></returns>
        public ContinueSubdivisionDecision ShouldStopSubdividing( SplitRect split, int depth, Axis divisionAxis)
        {
            // check we if can divide over the x axis
            if  (divisionAxis == Axis.X && split._rect.height < _minRectHeight * 2)
            {
                // if we can still continue to divide over the y axis advice to do so, 
                return (split._rect.width < _minRectWidth * 2)
                    ? ContinueSubdivisionDecision.RectTooSmall
                    : ContinueSubdivisionDecision.AttemptOtherAxis;
            }

            // check we if can divide over the y axis
            if (divisionAxis == Axis.Y && split._rect.width < _minRectWidth * 2)
            {
                // if we can still continue to divide over the x axis advice to do so, 
                return (split._rect.height < _minRectHeight * 2)
                    ? ContinueSubdivisionDecision.RectTooSmall
                    : ContinueSubdivisionDecision.AttemptOtherAxis;
            }

            if (depth > _minDepth)
            {
                // check all conditons to stop dividing
                if (_maxDepth > 0 && depth >= _maxDepth)
                {
                    return ContinueSubdivisionDecision.MaxDepthReached;
                }

                // perform a random roll to see if we should continue
                return (UnityEngine.Random.value < _stopChanceBase + _stopChanceBase * depth)
                    ? ContinueSubdivisionDecision.RandomRollFailed
                    : ContinueSubdivisionDecision.ContinueSubdivision;
            }

            return ContinueSubdivisionDecision.ContinueSubdivision;
        }

        private void TrySubdivideSplitRect(SplitRect split, List<SplitRect> result, int depth, Axis divisionAxis)
        {
            var decision = ShouldStopSubdividing(split, depth, divisionAxis);
            switch (decision)
            {
                case ContinueSubdivisionDecision.ContinueSubdivision:
                    SubdivideSplitRect(split, result, depth, divisionAxis);
                    break;

                case ContinueSubdivisionDecision.AttemptOtherAxis:
                    SubdivideSplitRect(split, result, depth, divisionAxis == Axis.X ? Axis.Y : Axis.X);
                    break;

                default:
                    split._reasonForSplitStoppedAtThisPoint = decision;
                    result.Add(split);
                    break;
            }
        }

        /// <summary>
        /// Split the given rect along the given axis
        /// </summary>
        /// <param name="split"></param>
        /// <param name="divisionAxis"></param>
        /// <returns></returns>
        public RectInt[] SplitRectRandomly(SplitRect split, Axis divisionAxis)
        {
            var result = new RectInt[2];

            if ( divisionAxis == Axis.Y )
            {
                var leftWidth = UnityEngine.Random.Range(_minRectWidth, split._rect.width - _minRectWidth);
                result[0] = new RectInt(split._rect.position, new Vector2Int(leftWidth, split._rect.height));
                result[1] = new RectInt(split._rect.position + Vector2Int.right * leftWidth,
                                        new Vector2Int(split._rect.width - leftWidth, split._rect.height));
            }
            else if (divisionAxis == Axis.X)
            {
                var bottomHeight = UnityEngine.Random.Range(_minRectHeight, split._rect.height - _minRectHeight);
                result[0] = new RectInt(split._rect.position, new Vector2Int(split._rect.width, bottomHeight));
                result[1] = new RectInt(split._rect.position + Vector2Int.up * bottomHeight,
                                        new Vector2Int(split._rect.width, split._rect.height - bottomHeight));
            }

            return result;
        }

        /// <summary>
        /// Create split from the source and the split up rectangles, spreading the neighbours
        /// of the split over its children
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rectangles"></param>
        /// <returns></returns>
        public SplitRect[] CreateSplits( SplitRect source, RectInt[] rectangles)
        {
            var result = new SplitRect[rectangles.Length];

            for (var i = 0; i < rectangles.Length; ++i )
            {
                result[i] = new SplitRect()
                {
                    _rect = rectangles[i],
                    Neighbours = new HashSet<SplitRect>(source.Neighbours.Where(
                                (neighbour) => !RectUtil.AreDisconnected(rectangles[i], neighbour._rect)))
                };

                foreach ( var neighbour in result[i].Neighbours)
                {
                    neighbour.Neighbours.Add(result[i]);
                }
            }

            for (var i = 0; i < result.Length; ++i)
            {
                for (var j = i+1; j < result.Length; ++j)
                {
                    result[i].Neighbours.Add(result[j]);
                    result[j].Neighbours.Add(result[i]);
                }
            }

            return result;
        }

        private void SubdivideSplitRect(SplitRect split, List<SplitRect> result, int depth, Axis divisionAxis)
        {
            var subdivisionRectangles = SplitRectRandomly(split, divisionAxis);
            var splitRects = CreateSplits(split, subdivisionRectangles);

            split.DisconnectFromNeighbours();

            var nextAxis = divisionAxis == Axis.X ? Axis.Y : Axis.X;

            for ( int i = 0; i < splitRects.Length; ++i)
            {
                TrySubdivideSplitRect(splitRects[i], result, depth + 1, nextAxis);
            }
        }
    }
}