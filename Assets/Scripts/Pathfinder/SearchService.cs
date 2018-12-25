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

    using UnityEngine;
    
    using Tds.Util;

    /// <summary>
    /// Service which encapsulates one or more BestFistSearch algorithms, allowing agents to share 
    /// searches and search results rather than having to instantiate a searchalgorithm themselves/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SearchService<T> where T : class
    {
        private BestFistSearch<T>[] _searchAlgorithms;

        /// <summary>
        /// Buffered searchparameters. 
        /// </summary>
        private LinkedList<SearchParameters<T>> _availableSearches;

        /// <summary>
        /// Searches which are currently waiting to be started
        /// </summary>
        private LinkedList<SearchParameters<T>> _scheduledSearches;

        /// <summary>
        /// Searches currently in progress
        /// </summary>
        private LinkedList<SearchParameters<T>> _searchesInProgress;

        /// <summary>
        /// Searches completed
        /// </summary>
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

        /// <summary>
        /// Current timestamp (number of time update has been called on the service)
        /// used to determine which completed searches can be re-used by new queries
        /// </summary>
        public int TimeStamp
        {
            get;
            private set;
        }

        public SearchService()
        {
            MaxAgeCompletedSearch = 30;
        }

        /// <summary>
        /// Initializes the service
        /// </summary>
        /// <param name="searchAlgorithmCount">Number of pathfinders </param>
        /// <param name="searchResultCount">Number of cached search results</param>
        /// <param name="estimatedSearchDepth">Number of nodes in the cached search results</param>
        /// <param name="factoryMethod">Method used to create new pathfinders</param>
        /// <returns></returns>
        public SearchService<T> Initialize( int searchAlgorithmCount, int searchResultCount, 
                                                    int estimatedSearchDepth, Func<BestFistSearch<T>> factoryMethod )
        {
            _searchAlgorithms = new BestFistSearch<T>[searchAlgorithmCount];
            
            for ( var i = 0; i < searchAlgorithmCount; ++i )
            {
                _searchAlgorithms[i] = factoryMethod(); 
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

                    for (int i = 0; i < searchResultNode.Value.Nodes.Length; ++i)
                    {
                        searchResultNode.Value.Nodes[i] = null;
                    }
                }
            }

            return result == null ? -1 : result.Value.Id;
        }

        /// <summary>
        /// Stop the search with the given id (ticket)
        /// </summary>
        /// <param name="id"></param>
        public void CancelSearch(int id)
        {
            var node = _scheduledSearches.FirstOrDefault(x => x.Id == id);

            if ( node != null)
            {
                _scheduledSearches.Remove(node);
                _availableSearches.AddLast(node);
            } 
        }

        /// <summary>
        /// Retrieves a patch research
        /// </summary>
        /// <param name="id">id of the search ticket</param>
        /// <param name="from">from node </param>
        /// <param name="to">to node</param>
        /// <param name="store">the store to put the result in</param>
        /// <returns>Store if a result is found, null otherwise</returns>
        public T[] RetrieveResult(int id, T from, T to, T[] store, ISearchSpace<T, Vector2> searchSpace )
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
                    completedSearch.Value.ReverseCopyNodes(store);
                }


                return store;
            }

            return null;
        }

        /// <summary>
        /// Updates the searches
        /// </summary>
        /// <param name="maxIterations">Number of expansions a pathfinder is allowed to do</param>
        public void Update(int maxIterations)
        {
            TryStartNewSearches(_searchAlgorithms, _scheduledSearches, _searchesInProgress);
            UpdateSearchesInProgress(maxIterations, _searchAlgorithms, _searchesInProgress, _completedSearches);
            TimeStamp++;
        }

        private void TryStartNewSearches(BestFistSearch<T>[] searchers,
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
                                                BestFistSearch<T>[] searchers,
                                                LinkedList<SearchParameters<T>> searchesInProgress,
                                                LinkedList<SearchParameters<T>> completedSearches)
        {
            for (int i = 0; i < searchers.Length; ++i)
            {
                if (searchers[i].Start != null)
                {
                    if (_searchAlgorithms[i].Iterate(maxIterations) <= 0)
                    {
                        var searchResult = searchesInProgress.FirstOrDefault(x => x.IsMatch(_searchAlgorithms[i].Start,
                                                                                            _searchAlgorithms[i].End));

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
