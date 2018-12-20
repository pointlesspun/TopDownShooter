/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonPathfinding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Tds.PathFinder;
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
        
        private BestFistSearch<DungeonNode> _pathfinder;
        
        public DungeonSearch(int poolSize)
        {
            _pathfinder = new BestFistSearch<DungeonNode>(poolSize);
        }

        public BestFistSearch<DungeonNode> BeginSearch(DungeonNode from, DungeonNode to)
        {
            return _pathfinder.BeginSearch(from, to, CostFunction, ExpansionFunction, DistanceFunction);
        }

        public static BestFistSearch<DungeonNode> CreatePathfinder(int poolSize, float lengthWeight, float goalDistanceWeight, float nextNodeDistanceWeight)
        {
            var distanceFunction = new Func<DungeonNode, DungeonNode, float>((n1, n2) => n1.Distance(n2));
           
            var neighbourFunction = new Action<PathNode<DungeonNode>, Action<DungeonNode>>((pathNode, action) => 
            {
                if ( pathNode._data.Edges != null)
                {
                    foreach ( var edge in pathNode._data.Edges)
                    {
                        action(edge.GetOther(pathNode._data));
                    }
                }
            });


            return BestFistSearch<DungeonNode>.Instantiate(poolSize, lengthWeight, goalDistanceWeight, nextNodeDistanceWeight, distanceFunction, neighbourFunction);
          
        }

        private float CostFunction(DungeonNode from, DungeonNode to, DungeonNode goal, float pathLength)
        {
            return from.Distance(to) * _distanceToNextNodeWeight
                                    + pathLength * _pathLengthWeight
                                    + goal.Distance(to) * _distanceToGoalWeight;
        }

        private void ExpansionFunction( PathNode<DungeonNode> parent, Action<DungeonNode> actionOnNeighbour)
        {
            if (parent._data.Edges != null)
            {
                foreach ( var edge in parent._data.Edges)
                {
                    actionOnNeighbour(edge.GetOther(parent._data));
                }
            }
        }

        private float DistanceFunction( DungeonNode from, DungeonNode to)
        {
            return from.Distance(to);
        }
    }
}
