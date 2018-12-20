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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Behaviour wrapper around the PathfindingService
    /// </summary>
    public class GridSearchServiceBehaviour : MonoBehaviour
    {
        private SearchService<LevelElement> _service;

        /// <summary>
        /// Settings used 
        /// </summary>
        public AgentPathfindingSettings _pathfindingSettings = new AgentPathfindingSettings();

        /// <summary>
        /// Number iterations the service is run on each update
        /// </summary>
        public int _iterationCount = 16;

        /// <summary>
        /// Number of pathfinding algorithms in the service
        /// </summary>
        public int _algorithmCount = 4;
        public int _searchResultCount = 32;
        public int _searchDepth = 48;
        public int _algorithmPoolCount = 256;

        public GameObject _levelGridObject; 

        private AgentPathingContext<LevelElement> _context;
        public LevelGridSearchSpace GridSearchSpace
        {
            get;
            set;
        }

        public SearchService<LevelElement> PathfindingService
        {
            get
            {
                if (_service == null)
                {
                    _service = CreatePathfindingService();
                }

                return _service;
            }
        }

        public void Awake()
        {
            _service = CreatePathfindingService();
            _context = CreatePathingContext();
        }

        public void Update()
        {
            _service.Update(_iterationCount);
        }

        private AgentPathingContext<LevelElement> CreatePathingContext()
        {
            return new AgentPathingContext<LevelElement>()
            {
                searchSpace = null,
                service = PathfindingService,
                settings = _pathfindingSettings,
                time = Time.realtimeSinceStartup
            };
        }

        public void UpdateAgentPathing(AgentPathingState<LevelElement> pathfindingState)
        {
            _context.time = Time.time;
            _context.searchSpace = GridSearchSpace;

            AgentPathingService.UpdateState(pathfindingState, _context);
        }

        private SearchService<LevelElement> CreatePathfindingService()
        {
            return new SearchService<LevelElement>()
                                 .Initialize(_algorithmCount, _searchResultCount, _searchDepth,
                                                         () => CreatePathfinder(_algorithmPoolCount, 1, 1, 1));
        }

        public BestFistSearch<LevelElement> CreatePathfinder(int poolSize, float lengthWeight, float goalDistanceWeight, float nextNodeDistanceWeight)
        {
            var distanceFunction = new Func<LevelElement, LevelElement, float>((n1, n2) => n1.Distance(n2));
            var neighbourFunction = new Action<PathNode<LevelElement>, Action<LevelElement>>((node,action) => 
            {
                GridSearchSpace.Grid.ForEachNeigbours(node._data._position, (element) =>
                {
                    if (element._id == LevelElementDefinitions.FloorTileIndex)
                    {
                        action(element);
                    }
                });
            });

            return BestFistSearch<LevelElement>.Instantiate(poolSize, lengthWeight, goalDistanceWeight, 
                                            nextNodeDistanceWeight, distanceFunction, neighbourFunction);
        }
    }
}