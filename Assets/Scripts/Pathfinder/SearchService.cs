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

    public class SearchRecord<T> where T : class
    {
        /// <summary>
        /// Parameters defining a search 
        /// </summary>
        public SearchParameters<T> searchParameters;

        /// <summary>
        /// Number of entities having a reference to the parameters
        /// </summary>
        public int referenceCount;

        /// <summary>
        /// Collection this record belongs to
        /// </summary>
        public LinkedList<SearchRecord<T>> collection;


        public int searchIndex; 
    }

    /// <summary>
    /// Service which encapsulates one or more BestFistSearch algorithms, allowing agents to share 
    /// searches and search results rather than having to instantiate a searchalgorithm themselves/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SearchService<T> where T : class
    {
        /// <summary>
        /// Algorithms used for the actual search 
        /// </summary>
        private BestFistSearch<T>[] _searchAlgorithms;

        /// <summary>
        /// Lookup for search records by id
        /// </summary>
        private readonly Dictionary<int, SearchRecord<T>> _recordLookup = new Dictionary<int, SearchRecord<T>>();

        /// <summary>
        /// Buffered searchparameters. 
        /// </summary>
        private LinkedList<SearchRecord<T>> _availableSearches;

        /// <summary>
        /// Searches which are currently waiting to be started
        /// </summary>
        private LinkedList<SearchRecord<T>> _scheduledSearches;

        /// <summary>
        /// Searches currently in progress
        /// </summary>
        private LinkedList<SearchRecord<T>> _searchesInProgress;

        /// <summary>
        /// Searches completed
        /// </summary>
        private LinkedList<SearchRecord<T>> _completedSearches;

        /// <summary>
        ///  Assumption is a gameplay clock with 20 updates / seconds, so
        ///  every 0.5 seconds a search is considered 'outdated' and can be re-used
        /// </summary>
        public int MaxAgeCompletedSearch
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the searches available. Use for debugging / testing purposes only.
        /// </summary>
        public IEnumerable<SearchRecord<T>> AvailableSearches
        {
            get
            {
                return _availableSearches;
            }
        }

        public IEnumerable<SearchRecord<T>> ScheduledSearches
        {
            get
            {
                return _scheduledSearches;
            }
        }

        public IEnumerable<SearchRecord<T>> SearchesInProgress
        {
            get
            {
                return _searchesInProgress;
            }
        }
        
        public IEnumerable<SearchRecord<T>> CompletedSearches
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

            _availableSearches = new LinkedList<SearchRecord<T>>();
            _scheduledSearches = new LinkedList<SearchRecord<T>>();
            _searchesInProgress = new LinkedList<SearchRecord<T>>();
            _completedSearches = new LinkedList<SearchRecord<T>>();
                        
            for (int i = 0; i < searchResultCount; i++)
            {
                var record = new SearchRecord<T>()
                {
                    searchParameters = new SearchParameters<T>()
                    {
                        Nodes = new T[estimatedSearchDepth],
                        Id = id++
                    },
                    referenceCount = 0,
                    collection = _availableSearches
                };

                _recordLookup[record.searchParameters.Id] = record;
                _availableSearches.AddLast(record);
            }

            TimeStamp = 0;

            return this;
        }

        /// <summary>
        /// Clears all outstanding searches and resets the TimeStamp
        /// </summary>
        public void Clear()
        {
            foreach (var searchAlgorithm in _searchAlgorithms)
            {
                searchAlgorithm.EndSearch();
            }

            MoveSearches(_scheduledSearches, _availableSearches);
            MoveSearches(_searchesInProgress, _availableSearches);
            MoveSearches(_completedSearches, _availableSearches);
            TimeStamp = 0;
        }

        /// <summary>
        /// Start a new search from a from-element to a to-element
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>The id of the search or a negative number if no search is available</returns>
        public int BeginSearch(T from, T to, int debugId)
        {
            var result = FindExistingSearch(from, to);

            if (result == null)
            {
                result = ObtainSearchResult();

                if (result != null)
                {
                    result.Value.referenceCount = 1;

                    var parameters = result.Value.searchParameters;
                    
                    parameters.IsComplete = false;
                    parameters.FromNode = from;
                    parameters.ToNode = to;
                    parameters.Length = 0;

                    // clear all elements 
                    for (int i = 0; i < parameters.Nodes.Length; ++i)
                    {
                        parameters.Nodes[i] = null;
                    }
                }
            }
            else
            {
                result.Value.referenceCount++;
            }

            return result == null ? -1 : result.Value.searchParameters.Id;
        }

        /// <summary>
        /// Stop the search with the given id (ticket)
        /// </summary>
        /// <param name="id"></param>
        public void CancelSearch(int id)
        {
            var record = id >= 0 ? _recordLookup[id] : null;

            // can this search be canceled
            if ( record != null && (record.collection != _availableSearches))
            {
                record.referenceCount--;

                if (record.referenceCount == 0)
                {
                    if (record.collection == _searchesInProgress)
                    {
                        _searchAlgorithms[record.searchIndex].EndSearch();
                    }

                    record.collection.Remove(record);
                    _availableSearches.AddLast(record);
                    record.collection = _availableSearches;
                }
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
            var completedSearch = id >= 0 ? _recordLookup[id] : null;

            if ( completedSearch != null 
                    && completedSearch.collection == _completedSearches 
                    && completedSearch.searchParameters.IsMatch(from, to))
            {
                var search = completedSearch.searchParameters;
                if (from == search.FromNode)
                {
                    Array.Copy(search.Nodes, store, Mathf.Min(store.Length, search.Nodes.Length));
                }
                else
                {
                    search.ReverseCopyNodes(store);
                }

                return store;
            }

            return null;
        }

        /// <summary>
        /// Debug method to see if the given ticket is still valid, ie: it should exist
        /// in any of the active searches and the from and to node should match
        /// </summary>
        /// <param name="id"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool ValidateTicket(int id, T from, T to)
        {
            return _scheduledSearches.Any(x => x.searchParameters.Id == id && x.searchParameters.IsMatch(from, to))
                || _searchesInProgress.Any(x => x.searchParameters.Id == id && x.searchParameters.IsMatch(from, to))
                || _completedSearches.Any(x => x.searchParameters.Id == id && x.searchParameters.IsMatch(from, to));
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
                                                LinkedList<SearchRecord<T>> planned,
                                                LinkedList<SearchRecord<T>> inProgress) {

            while (inProgress.Count < searchers.Length && planned.Count > 0)
            {
                var nextSearchParameters = planned.First;
                planned.RemoveFirst();
                inProgress.AddLast(nextSearchParameters);

                nextSearchParameters.Value.collection = inProgress;

                for( int i = 0; i < searchers.Length; ++i)
                {
                    if (searchers[i].Start == null)
                    {
                        searchers[i].BeginSearch(nextSearchParameters.Value.searchParameters.FromNode,
                                                       nextSearchParameters.Value.searchParameters.ToNode);
                        nextSearchParameters.Value.searchIndex = i;
                        break;
                    }
                }
                
            }
        }

        private void UpdateSearchesInProgress(int maxIterations, 
                                                BestFistSearch<T>[] searchers,
                                                LinkedList<SearchRecord<T>> searchesInProgress,
                                                LinkedList<SearchRecord<T>> completedSearches)
        {
            for (int i = 0; i < searchers.Length; ++i)
            {
                if (searchers[i].Start != null)
                {
                    if (_searchAlgorithms[i].Iterate(maxIterations) <= 0)
                    {
                        var searchResult = searchesInProgress.FirstOrDefault(
                            x => x.searchParameters.IsMatch(_searchAlgorithms[i].Start, _searchAlgorithms[i].End)).Value;
                        var parameters = searchResult.searchParameters;

                        parameters.IsComplete = true;
                        parameters.TimeStamp = TimeStamp;

                        parameters.Length = searchers[i].GetBestPath(parameters.Nodes);                        
                        searchers[i].EndSearch();

                        searchesInProgress.Remove(searchResult);
                        completedSearches.AddLast(searchResult);
                        searchResult.collection = completedSearches;
                    }
                }
            }
        }

        private void MoveSearches(LinkedList<SearchRecord<T>> from, LinkedList<SearchRecord<T>> to)
        {
            while (from.Count > 0)
            {
                var record = from.First;

                from.RemoveFirst();
                to.AddFirst(record);
            }
        }

        private LinkedListNode<SearchRecord<T>> ObtainSearchResult()
        {
            LinkedListNode<SearchRecord<T>> result = null;
            var source = _availableSearches;

            result = _availableSearches.First;

            if (result == null)
            {
                source = _completedSearches;
                result = _completedSearches.FirstOrDefault(x => TimeStamp - x.searchParameters.TimeStamp > MaxAgeCompletedSearch);

                if ( result != null)
                {
                    //Debug.Log("obtained from completed " + result.Value.searchParameters.TimeStamp + " / " + TimeStamp);
                }
            }

            if (result != null)
            {
                source.Remove(result);
                _scheduledSearches.AddLast(result);
                result.Value.collection = _scheduledSearches;
            }

            return result;
        }

        private LinkedListNode<SearchRecord<T>> FindExistingSearch(T from, T to)
        {
            var result = _scheduledSearches.FirstOrDefault(x => x.searchParameters.IsMatch(from, to));
            
            if ( result == null )
            {
                result = _completedSearches.FirstOrDefault(x => x.searchParameters.IsMatch(from, to));
            }

            return result;
        }
    }
}
