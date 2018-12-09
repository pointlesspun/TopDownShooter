/*
 * TDS(c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.If not, see<http://creativecommons.org/licenses/by-sa/4.0/>.
 */

namespace Tds.Pathfinder
{
    using System.Collections.Generic;
    using Tds.DungeonGeneration;

    public class SearchResult
    {
        public List<TraversalNode> _path;
        public bool _isReversed; 
    }

    public class AStarService 
    {
        private class SearchParameters
        {
            public int _ticketId;
            public TraversalNode _from;
            public TraversalNode _to;
        }

        private Dictionary<TraversalNode, Dictionary<TraversalNode, List<TraversalNode>>> _cache 
                            = new Dictionary<TraversalNode, Dictionary<TraversalNode, List<TraversalNode>>>();
        private Dictionary<TraversalNode, HashSet<TraversalNode>> _reverseCache = new Dictionary<TraversalNode, HashSet<TraversalNode>>();

        private AStar _search = new AStar(256);

        private SearchParameters _currentSearch = null;
        private int _currentSearchCounter = 0;
        
        private Dictionary<int, SearchResult> _resolvedPaths = new Dictionary<int, SearchResult>();

        private List<SearchParameters> _outstandingSearches = new List<SearchParameters>(); 

        public int Search(TraversalNode from, TraversalNode to)
        {
            var ticketId = -1;

            Dictionary<TraversalNode, List<TraversalNode>> destination = null;

            if ( _cache.TryGetValue(from, out destination))
            {
                List<TraversalNode> path = null;
                
                if (destination.TryGetValue(to, out path))
                {
                    _resolvedPaths[_currentSearchCounter] = new SearchResult() { _path = path, _isReversed = false };
                    ticketId = _currentSearchCounter;
                    _currentSearchCounter++;
                }
                else
                {
                    ticketId = PushSearch(from, to);
                }
            }
            // else try reverse cache
            else
            {
                HashSet<TraversalNode> reverseSearches = null;

                if (_reverseCache.TryGetValue(to, out reverseSearches) && reverseSearches.Contains(from))
                {
                    var path = _cache[from][to];
                    _resolvedPaths[_currentSearchCounter] = new SearchResult() { _path = path, _isReversed = true };
                    ticketId = _currentSearchCounter;
                    _currentSearchCounter++;
                }
                else
                {
                    // start  a new saerch
                    ticketId = PushSearch(from, to);
                }
            }

            return ticketId;
        }

        public SearchResult ObtainPath(int ticketId)
        {
            SearchResult result = null;

            if (_resolvedPaths.TryGetValue(ticketId, out result))
            {
                _resolvedPaths.Remove(ticketId);
            }

            return result;
        }

        public void Update()
        {
            if (_currentSearch == null)
            {
                if (_outstandingSearches.Count > 0)
                {
                    _currentSearch = _outstandingSearches[0];
                    _outstandingSearches.RemoveAt(0);

                    _search.BeginSearch(_currentSearch._from, _currentSearch._to, (node1, node2) =>
                    {
                        return (node2._split._rect.center - node1._split._rect.center).sqrMagnitude;
                    });
                }
            }

            if (_currentSearch != null)
            {
                if (_search.Iterate() <= 0)
                {
                    var path = _search.GetBestPath();
                    StoreSearch(_currentSearch, path);
                    _resolvedPaths[_currentSearch._ticketId] = new SearchResult() { _isReversed = false, _path = path };
                    _currentSearch = null;
                }
            }
        }

        private void StoreSearch(SearchParameters parameters, List<TraversalNode> path)
        {
            Dictionary<TraversalNode, List <TraversalNode>> destinations = null;

            if ( !_cache.TryGetValue(parameters._from, out destinations))
            {
                destinations = new Dictionary<TraversalNode, List<TraversalNode>>();
                destinations[parameters._to] = path;
                _cache[parameters._from] = destinations;
            }

            destinations[parameters._to] = path;

            HashSet<TraversalNode> reverseEntry = null;

            if ( !_reverseCache.TryGetValue(parameters._to, out reverseEntry ))
            {
                reverseEntry = new HashSet<TraversalNode>();
                _reverseCache[parameters._to] = reverseEntry;
            }

            reverseEntry.Add(parameters._from);
        }

        private int PushSearch(TraversalNode from, TraversalNode to)
        {
            _outstandingSearches.Add(new SearchParameters()
            {
                _ticketId = _currentSearchCounter,
                _from = from,
                _to = to
            });

            _currentSearchCounter++;

            return _currentSearchCounter - 1;
        }
    }
}
