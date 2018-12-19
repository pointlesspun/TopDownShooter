/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.DungeonPathfinding
{
    using UnityEngine;

    using Tds.PathFinder;
    using Tds.DungeonGeneration;

    /// <summary>
    /// Test behaviour which can be used to visualize the pathfinder algorithm
    /// </summary>
    public class DungeonSearchTestBehaviour : MonoBehaviour
    { 
        public BestFistSearch<DungeonNode> _pathfinder = DungeonSearch.CreatePathfinder(256, 1, 1, 1);
        
        public void BeginSearch()
        {
            var layoutTraversal = GetComponent<DungeonLayoutDebugBehaviour>();

            if (layoutTraversal != null)
            {
                if (layoutTraversal.Layout != null)
                {
                    if (layoutTraversal.Layout.Start != null && layoutTraversal.Layout.End != null)
                    {
                        _pathfinder.BeginSearch(layoutTraversal.Layout.Start, layoutTraversal.Layout.End);
                    }
                }
            }
        }

        public void Iterate()
        {
            _pathfinder.Iterate(1);
        }
       
        public void OnDrawGizmos()
        {
            Vector3 offset = Vector3.zero;

            if (_pathfinder != null)
            {
                _pathfinder.GetOpenList().ForEach(n =>
                {
                    Vector3 position = n.Rect.center;

                    Gizmos.DrawIcon(position, "question mark.png", true);
                });

                _pathfinder.GetClosedList().ForEach(n =>
                {
                    Gizmos.DrawIcon(n.Rect.center, "footsteps.png", true);
                });
            }
        }
    }
}
