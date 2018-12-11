/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.Pathfinder
{
    using System.Collections.Generic;
    using Tds.DungeonGeneration;
    using UnityEngine;

    public class DungeonSearchTestBehaviour : MonoBehaviour
    { 
        public DungeonSearch _pathfinder = new DungeonSearch(256);
        private List<DungeonNode> _path = null;

        public void BeginSearch()
        {
            var layoutTraversal = GetComponent<DungeonLayoutDebugBehaviour>();

            _path = null;

            if (layoutTraversal != null)
            {
                _pathfinder.BeginSearch(layoutTraversal.Layout.Start, layoutTraversal.Layout.End);
            }
        }

        public void Iterate()
        {
            if (_pathfinder.Iterate(1) == 0 )
            {
                _path = _pathfinder.GetBestPath();
            }
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
