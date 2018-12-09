/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.Pathfinder
{
    using System;
    using System.Collections.Generic;
    using Tds.DungeonGeneration;
    using Tds.Util;
    using UnityEngine;

    public class AStarTestBehaviour : MonoBehaviour
    {
        public GameObject _fromNode;
        public GameObject _toNode;

        private AStar _pathfinder = new AStar(256);

        public void BeginSearch()
        {
            _pathfinder.BeginSearch(_fromNode.GetComponent<TraversalNodeDebugBehaviour>()._node,
                                        _toNode.GetComponent<TraversalNodeDebugBehaviour>()._node,
                                            (node1, node2) => (node2._split._rect.center - node1._split._rect.center).sqrMagnitude);
        }

        public void Iterate()
        {
            _pathfinder.Iterate();
        }

        public void OnDrawGizmos()
        {
            Vector3 offset = Vector3.zero;

            if (_fromNode != null)
            {
                var fromDebugBehaviour = _fromNode.GetComponent<TraversalNodeDebugBehaviour>();
                
                Vector3 position = fromDebugBehaviour._node._split._rect.center;
                position += fromDebugBehaviour._offset;

                Gizmos.DrawIcon(position, "start.png", true);

                offset = fromDebugBehaviour._offset;
            }

            if (_toNode != null)
            {
                var toDebugBehaviour = _toNode.GetComponent<TraversalNodeDebugBehaviour>();

                if (toDebugBehaviour != null && toDebugBehaviour._node != null)
                {
                    Vector3 position = toDebugBehaviour._node._split._rect.center;
                    position += toDebugBehaviour._offset;

                    Gizmos.DrawIcon(position, "flag.png", true);
                }
            }

            if (_fromNode != null && _toNode != null )
            {
                _pathfinder.GetOpenList().ForEach(n =>
                {
                    Vector3 position = n._split._rect.center;
                    position += offset;

                    Gizmos.DrawIcon(position, "question mark.png", true);
                });

                _pathfinder.GetClosedList().ForEach(n =>
                {
                    Vector3 position = n._split._rect.center;
                    position += offset;

                    Gizmos.DrawIcon(position, "footsteps.png", true);
                });

            }

        }

    }
}
