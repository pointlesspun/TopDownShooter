/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.Pathfinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tds.DungeonGeneration;

    [Serializable]
    public class DungeonSearch : PathfinderAlgorithm<DungeonNode>
    {
        public float _pathLengthWeight = 1.0f;
        public float _distanceToGoalWeight = 1.0f;
        public float _distanceToNextNode = 1.0f;

        public DungeonSearch(int poolSize)
            : base(poolSize)
        {
        }

        public void BeginSearch(DungeonNode from, DungeonNode to)
        {
            var costFunction = new Func<DungeonNode, DungeonNode, float, float>(
                (fromNode, toNode, parentCost) => fromNode.Distance(toNode) * _distanceToNextNode
                                                + parentCost * _pathLengthWeight
                                                + to.Distance(toNode) * _distanceToGoalWeight);
            
            var expandFunction = new Func<PathNode<DungeonNode>, IEnumerable<DungeonNode>>(
                            (parent) => parent._data.Edges == null ? null : parent._data.Edges.Select((e) => e.GetOther(parent._data)));

            BeginSearch(from, to, costFunction, expandFunction);           
        }
    }
}
