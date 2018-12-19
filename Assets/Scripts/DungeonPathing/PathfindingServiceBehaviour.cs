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
    /// Behaviour wrapper around the PathfindingService
    /// </summary>
    public class PathfindingServiceBehaviour : MonoBehaviour
    {
        private SearchService<DungeonNode> _service;

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

        private AgentPathingContext<DungeonNode> _context;

        public SearchService<DungeonNode> PathfindingService
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

        private AgentPathingContext<DungeonNode> CreatePathingContext()
        {
            return new AgentPathingContext<DungeonNode>()
            {
                searchSpace = null,
                service = PathfindingService,
                settings = _pathfindingSettings,
                time = Time.realtimeSinceStartup
            };
        }

        public void UpdateAgentPathing(AgentPathingState<DungeonNode> pathfindingState, DungeonLayout layout)
        {
            _context.time = Time.time;
            _context.searchSpace = layout;

            AgentPathingService.UpdateState(pathfindingState, _context);
        }

        private SearchService<DungeonNode> CreatePathfindingService()
        {
           return new SearchService<DungeonNode>()
                                .Initialize(_algorithmCount, _searchResultCount, _searchDepth,
                                                        () => DungeonSearch.CreatePathfinder(_algorithmPoolCount, 1, 1, 1));
        }
    }
}
