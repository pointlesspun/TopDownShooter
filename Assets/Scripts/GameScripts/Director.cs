/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    using Tds.DungeonGeneration;
    using System.Collections.Generic;
    using Tds.PathFinder;

    /// <summary>
    /// Class which controls and directs the level's monster behaviour
    /// </summary>
    public class Director : MonoBehaviour
    {
        /// <summary>
        /// List of monsters which can be spawned
        /// </summary>
        public GameObject _monsterPrefab;

        /// <summary>
        /// Level offset required to position newly spawned monsters correctly
        /// </summary>
        public Vector3 _offset;

        /// <summary>
        /// Number of monsters spawned per trigger
        /// </summary>
        public int _spawnCountPerTrigger = 3;

        /// <summary>
        /// List of nodes which already have spawned monsters, in the current implementation the director
        /// only spawns a set of monsters once for each room.
        /// </summary>
        private HashSet<DungeonNode> _closedSet = new HashSet<DungeonNode>();

        private DungeonLayout _layout;
        public DungeonNode _currentPlayerNode;

        /// <summary>
        /// Set of default offsets from which the monsters are spawned
        /// </summary>
        private static readonly Vector3[] _spawnOffsets = new Vector3[]
        {
            Vector3.left, Vector3.left + Vector3.up, Vector3.up, Vector3.right + Vector3.up, Vector3.right,
            Vector3.right + Vector3.down, Vector3.down, Vector3.left + Vector3.down, Vector3.zero
        };

        public void SetDungeonLayout(DungeonLayout layout, Vector3 offset)
        {
            _offset = offset;
            _layout = layout;

            foreach (var node in layout.Nodes)
            {
                AddTriggerFor(node);
            }
        }

        // xxx note this is not efficient but good enough for the scope of build007
        // need to replace this with several things like a quadtree and a service for
        // pathfinding
        public PathfinderAlgorithm<DungeonNode> FindPath(Vector2 from, Vector2 to)
        {
            if (_layout != null)
            {
                DungeonNode fromNode = null;
                DungeonNode toNode = null;

                foreach (var node in _layout.Nodes)
                {
                    if (node.ContainsPoint(from))
                    {
                        fromNode = node;
                    }

                    if (node.ContainsPoint(to))
                    {
                        toNode = node;
                    }

                    if (fromNode != null && toNode != null)
                    {
                        return new DungeonSearch(128).BeginSearch(fromNode, toNode);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Add a trigger to the director
        /// </summary>
        /// <param name="node"></param>
        /// <param name="offset"></param>
        //public void AddTrigger(TraversalNode node, Vector3 offset)
        public void AddTriggerFor(DungeonNode node)
        {
            RectInt rect = node.Rect;
            var triggerObject = new GameObject("trigger for " + rect);

            var collider = triggerObject.AddComponent<BoxCollider2D>();

            collider.isTrigger = true;
            collider.offset = rect.center + new Vector2(_offset.x, _offset.y);
            collider.size = rect.size;

            var triggerBehaviour = triggerObject.AddComponent<TraversalNodeTriggerBehaviour>();

            triggerBehaviour._director = this;
            triggerBehaviour._node = node;

            triggerObject.transform.parent = transform;
        }

        /// <summary>
        /// A trigger has been hit - spawn some monsters
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="node"></param>
        public void OnTrigger(Collider2D collider, DungeonNode node)
        {
            _currentPlayerNode = node;

            var nodeNeighbours = node.Neighbours;

            if (nodeNeighbours != null)
            {
                foreach (var n in nodeNeighbours)
                {
                    if (!_closedSet.Contains(n))
                    {
                        for (int i = 0; i < _spawnCountPerTrigger; ++i)
                        {
                            var obj = Instantiate(_monsterPrefab);
                            _monsterPrefab.transform.position = n.Rect.center;
                            _monsterPrefab.transform.position += _offset + _spawnOffsets[Random.Range(0, _spawnOffsets.Length)]
                                                                 * Random.Range(1, 3);

                            obj.transform.parent = transform;
                        }

                        _closedSet.Add(n);
                    }
                }
           }
        }
     }
}
