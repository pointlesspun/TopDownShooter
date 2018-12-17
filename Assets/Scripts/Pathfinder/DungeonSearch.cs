/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.PathFinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Tds.DungeonGeneration;

    /// <summary>
    /// Factory-ish class to instantiate a pahtfinder through dungeon nodes
    /// xxx refactor at some point
    /// </summary>
    [Serializable]
    public class DungeonSearch 
    {
        public float _pathLengthWeight = 1.0f;
        public float _distanceToGoalWeight = 1.0f;
        public float _distanceToNextNodeWeight = 1.0f;
        
        private PathfinderAlgorithm<DungeonNode> _pathfinder;
        
        public DungeonSearch(int poolSize)
        {
            _pathfinder = new PathfinderAlgorithm<DungeonNode>(poolSize);
        }

        public PathfinderAlgorithm<DungeonNode> BeginSearch(DungeonNode from, DungeonNode to)
        {
            return _pathfinder.BeginSearch(from, to, CostFunction, ExpansionFunction, DistanceFunction);
        }

        public static PathfinderAlgorithm<DungeonNode> CreatePathfinder(int poolSize, float lengthWeight, float goalDistanceWeight, float nextNodeDistanceWeight)
        {
            return new PathfinderAlgorithm<DungeonNode>(poolSize,
                    (from, to, goal, length) => 
                        from.Distance(to) * goalDistanceWeight
                                   + length * lengthWeight
                                   + goal.Distance(to) * nextNodeDistanceWeight,
                        (parent) => parent._data.Edges == null
                                                        ? null
                                                        : parent._data.Edges.Select((e) => e.GetOther(parent._data)), 
                        (from, to) => from.Distance(to));
        }

        private float CostFunction(DungeonNode from, DungeonNode to, DungeonNode goal, float pathLength)
        {
            return from.Distance(to) * _distanceToNextNodeWeight
                                    + pathLength * _pathLengthWeight
                                    + goal.Distance(to) * _distanceToGoalWeight;
        }

        private IEnumerable<DungeonNode> ExpansionFunction( PathNode<DungeonNode> parent)
        {
            return parent._data.Edges == null 
                ? null 
                : parent._data.Edges.Select((e) => e.GetOther(parent._data));

        }

        private float DistanceFunction( DungeonNode from, DungeonNode to)
        {
            return from.Distance(to);
        }
    }
}
