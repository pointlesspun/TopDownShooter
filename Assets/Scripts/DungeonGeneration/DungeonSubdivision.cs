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
    public class DungeonSubdivision
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
        public DungeonLayout Subdivide(RectInt size, Axis startingAxis = Axis.None)
        {
            var initialRandomState = UnityEngine.Random.state;

            if (_seed >= 0)
            {
                UnityEngine.Random.InitState(_seed);
            }

            List<DungeonNode> result = new List<DungeonNode>();

            TrySubdivideSplitRect(new DungeonNode(size), result, 0, 
                    startingAxis == Axis.None ?  (UnityEngine.Random.value > 0.5f ? Axis.Y : Axis.X) : startingAxis);

            if (_seed >= 0)
            {
                UnityEngine.Random.state = initialRandomState;
            }

            return new DungeonLayout(result);
        }

        private void TrySubdivideSplitRect(DungeonNode node, List<DungeonNode> result, int depth, Axis divisionAxis)
        {
            var decision = ShouldStopSubdividing(node, depth, divisionAxis);

            switch (decision)
            {
                case ContinueSubdivisionDecision.ContinueSubdivision:
                    Subdivide(node, result, depth, divisionAxis);
                    break;

                case ContinueSubdivisionDecision.AttemptOtherAxis:
                    TrySubdivideSplitRect(node, result, depth, divisionAxis == Axis.X ? Axis.Y : Axis.X);
                    break;

                default:
                    result.Add(node);
                    break;
            }
        }
        /// <summary>
        /// Heuristic testing if the subdivision can continue
        /// </summary>
        /// <param name="node"></param>
        /// <param name="depth"></param>
        /// <param name="divisionAxis"></param>
        /// <returns></returns>
        public ContinueSubdivisionDecision ShouldStopSubdividing(DungeonNode node, int depth, Axis divisionAxis)
        {
            // check we if can divide over the x axis
            if (divisionAxis == Axis.X && node.Height < _minRectHeight * 2)
            {
                // if we can still continue to divide over the y axis advice to do so, 
                return (node.Width < _minRectWidth * 2)
                    ? ContinueSubdivisionDecision.RectTooSmall
                    : ContinueSubdivisionDecision.AttemptOtherAxis;
            }

            // check we if can divide over the y axis
            if (divisionAxis == Axis.Y && node.Width < _minRectWidth * 2)
            {
                // if we can still continue to divide over the x axis advice to do so, 
                return (node.Height < _minRectHeight * 2)
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


        /// <summary>
        /// Split the given rect along the given axis
        /// </summary>
        /// <param name="node"></param>
        /// <param name="divisionAxis"></param>
        /// <returns></returns>
        private DungeonNode[] DivideNodeRandomly(DungeonNode node, Axis divisionAxis)
        {
            if (divisionAxis == Axis.Y)
            {
                return node.DivideOverYAxis(UnityEngine.Random.Range(_minRectWidth, node.Width - _minRectWidth));
            }
            else if (divisionAxis == Axis.X)
            {
                return node.DivideOverXAxis(UnityEngine.Random.Range(_minRectHeight, node.Height - _minRectHeight));
            }

            throw new InvalidProgramException("Cannot divide Dungeon node over z axis");
        }
        
        private void TryConnectNode(DungeonNode node, IEnumerable<DungeonNode> otherNodes)
        {
            foreach (var other in otherNodes)
            {
                // if the two nodes share an intersection, connect them to each other
                if (!RectUtil.AreDisconnected(node.Rect, other.Rect))
                {
                    var newEdge = new DungeonEdge(node, other, EdgeDirection.BiDirectional);

                    node.AddEdge(newEdge);
                    other.AddEdge(newEdge);
                }
            }
        }

        private void Subdivide(DungeonNode node, List<DungeonNode> result, int depth, Axis divisionAxis)
        {
            var subdividedNodes = DivideNodeRandomly(node, divisionAxis);

            if (node.Edges != null)
            {
                TryConnectNode(subdividedNodes[0], node.Edges.Select(edge => edge.GetOther(node)));
                TryConnectNode(subdividedNodes[1], node.Edges.Select(edge => edge.GetOther(node)));

                node.Disconnect();
            }

            DungeonNode.Connect(subdividedNodes[0], subdividedNodes[1]);

            var nextAxis = divisionAxis == Axis.X ? Axis.Y : Axis.X;

            for (int i = 0; i < subdividedNodes.Length; ++i)
            {
                TrySubdivideSplitRect(subdividedNodes[i], result, depth + 1, nextAxis);
            }
        }
    }
}