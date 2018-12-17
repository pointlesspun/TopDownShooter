/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.PathFinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tds.Util;
    using UnityEngine;

    public class PathfindingService<T> where T : class
    {
        private PathfinderAlgorithm<T>[] _pathfinders;

        private LinkedList<SearchParameters<T>> _availableSearches;
        private LinkedList<SearchParameters<T>> _scheduledSearches;
        private LinkedList<SearchParameters<T>> _searchesInProgress;
        private LinkedList<SearchParameters<T>> _completedSearches;

        /// <summary>
        ///  Assumption is a gameplay clock with 20 updates / seconds, so
        ///  every 0.5 seconds a search is considered 'outdated' and can be re-used
        /// </summary>
        public int MaxAgeCompletedSearch
        {
            get;
            set;
        } 

        public IEnumerable<SearchParameters<T>> ScheduledSearches
        {
            get
            {
                return _scheduledSearches;
            }
        }

        public IEnumerable<SearchParameters<T>> SearchesInProgress
        {
            get
            {
                return _searchesInProgress;
            }
        }
        
        public IEnumerable<SearchParameters<T>> CompletedSearches
        {
            get
            {
                return _completedSearches;
            }
        }

        public int TimeStamp
        {
            get;
            private set;
        }

        public PathfindingService()
        {
            MaxAgeCompletedSearch = 10;
        }

        public PathfindingService<T> Initialize( int searchAlgorithmCount, int searchResultCount, 
                                                    int estimatedSearchDepth, Func<PathfinderAlgorithm<T>> factoryMethod )
        {
            _pathfinders = new PathfinderAlgorithm<T>[searchAlgorithmCount];
            
            for ( var i = 0; i < searchAlgorithmCount; ++i )
            {
                _pathfinders[i] = factoryMethod(); 
            }

            var id = 0;

            _availableSearches = new LinkedList<SearchParameters<T>>();
            _scheduledSearches = new LinkedList<SearchParameters<T>>();
            _searchesInProgress = new LinkedList<SearchParameters<T>>();
            _completedSearches = new LinkedList<SearchParameters<T>>();
                        
            for (int i = 0; i < searchResultCount; i++)
            {
                _availableSearches.AddLast( new SearchParameters<T>()
                {
                    Nodes = new T[estimatedSearchDepth],
                    Id = id++
                });
            }

            TimeStamp = 0;

            return this;
        }

        public int BeginSearch(T from, T to)
        {
            var result = FindExistingSearch(from, to);

            if (result == null)
            {
                var searchResultNode = ObtainSearchResult();

                if (searchResultNode != null)
                {
                    result = searchResultNode;
                    searchResultNode.Value.IsComplete = false;
                    searchResultNode.Value.FromNode = from;
                    searchResultNode.Value.ToNode = to;
                }
            }

            return result == null ? -1 : result.Value.Id;
        }

        public void CancelSearch(int id)
        {
            var node = _scheduledSearches.FirstOrDefault(x => x.Id == id);

            if ( node != null)
            {
                _scheduledSearches.Remove(node);
                _availableSearches.AddLast(node);
            } 
        }

        public T[] RetrieveResult(int id, T from, T to, T[] store)
        {
            var completedSearch = _completedSearches.FirstOrDefault(x => x.Id == id);

            if ( completedSearch != null && completedSearch.Value.IsMatch(from, to))
            {
                if (from == completedSearch.Value.FromNode)
                {
                    Array.Copy(completedSearch.Value.Nodes, store, Mathf.Min(store.Length, completedSearch.Value.Nodes.Length));
                }
                else
                {
                    var length = Mathf.Min(store.Length, completedSearch.Value.Length);

                    if (completedSearch.Value.Length <= store.Length && completedSearch.Value.Nodes[completedSearch.Value.Length - 1] == null)
                    {
                        length--;
                    } 

                    Array.Copy(completedSearch.Value.Nodes, store, length);
                    Array.Reverse(store, 0, length);
                }

                return store;
            }

            return null;
        }

        public void Update(int maxIterations)
        {
            TryStartNewSearches(_pathfinders, _scheduledSearches, _searchesInProgress);
            UpdateSearchesInProgress(maxIterations, _pathfinders, _searchesInProgress, _completedSearches);
            TimeStamp++;
        }

        private void TryStartNewSearches(PathfinderAlgorithm<T>[] searchers,
                                                LinkedList<SearchParameters<T>> planned,
                                                LinkedList<SearchParameters<T>> inProgress) {

            while (inProgress.Count < searchers.Length && planned.Count > 0)
            {
                var nextSearchParameters = planned.First;
                planned.RemoveFirst();
                inProgress.AddLast(nextSearchParameters);

                searchers.First(s => s.Start == null)
                    .BeginSearch(nextSearchParameters.Value.FromNode, nextSearchParameters.Value.ToNode);
            }
        }

        private void UpdateSearchesInProgress(int maxIterations, 
                                                PathfinderAlgorithm<T>[] searchers,
                                                LinkedList<SearchParameters<T>> searchesInProgress,
                                                LinkedList<SearchParameters<T>> completedSearches)
        {
            for (int i = 0; i < searchers.Length; ++i)
            {
                if (searchers[i].Start != null)
                {
                    if (_pathfinders[i].Iterate(maxIterations) <= 0)
                    {
                        var searchResult = searchesInProgress.FirstOrDefault(x => x.IsMatch(_pathfinders[i].Start, _pathfinders[i].End));

                        searchResult.Value.IsComplete = true;
                        searchResult.Value.TimeStamp = TimeStamp;

                        searchResult.Value.Length = searchers[i].GetBestPath(searchResult.Value.Nodes);                        
                        searchers[i].EndSearch();

                        searchesInProgress.Remove(searchResult);
                        completedSearches.AddLast(searchResult);
                    }
                }
            }
        }

        private LinkedListNode<SearchParameters<T>> ObtainSearchResult()
        {
            LinkedListNode<SearchParameters<T>> result = null;
            var source = _availableSearches;

            result = _availableSearches.First;

            if (result == null)
            {
                source = _completedSearches;
                result = _completedSearches.FirstOrDefault(x => TimeStamp - x.TimeStamp > MaxAgeCompletedSearch);
            }

            if (result != null)
            {
                source.Remove(result);
                _scheduledSearches.AddLast(result);
            }

            return result;
        }

        private LinkedListNode<SearchParameters<T>> FindExistingSearch(T from, T to)
        {
            var result = _scheduledSearches.FirstOrDefault(x => x.IsMatch(from, to));
            
            if ( result == null )
            {
                result = _completedSearches.FirstOrDefault(x => x.IsMatch(from, to));
            }

            return result;
        }
    }
}
