using NUnit.Framework;
using System.Linq;
using Tds.DungeonGeneration;
using Tds.DungeonPathfinding;
using Tds.PathFinder;

public class SearchServiceTest
{
    /// <summary>
    /// Test initialization, expect all datastructures to be there and ready
    /// </summary>
    [Test]
    public void TestInitialization_ExpectInitialValuesToBeCorrect()
    {
        var service = new SearchService<DungeonNode>().Initialize(2, 4, 32,() => DungeonSearch.CreatePathfinder(16, 1, 1, 1));

        Assert.IsTrue(service.TimeStamp == 0);
        Assert.IsTrue(service.ScheduledSearches.Count() == 0);
        Assert.IsTrue(service.SearchesInProgress.Count() == 0);
        Assert.IsTrue(service.CompletedSearches.Count() == 0);
    }

    /// <summary>
    /// Test with a single agent, trying to find a specific path . 
    /// </summary>
    [Test]
    public void TestSingleSearch_ExpectPathFound()
    {
        var nodeA = new DungeonNode(0, 0, 1, 1);
        var nodeB = new DungeonNode(1, 0, 1, 1);
        var nodeC = new DungeonNode(-1, 0, 1, 1);
        var nodeD = new DungeonNode(2, 0, 1, 1);

        DungeonNode.Connect(nodeA, nodeB);
        DungeonNode.Connect(nodeA, nodeC);
        DungeonNode.Connect(nodeC, nodeD);

        var service = new SearchService<DungeonNode>().Initialize(2, 4, 4, () => DungeonSearch.CreatePathfinder(16, 1, 1, 1));

        var id = service.BeginSearch(nodeA, nodeD, 0);
        var store = new DungeonNode[8];

        Assert.IsTrue(id >= 0);
        Assert.IsTrue(service.TimeStamp == 0);
        Assert.IsTrue(service.ScheduledSearches.Count() == 1);
        Assert.IsTrue(service.SearchesInProgress.Count() == 0);
        Assert.IsTrue(service.CompletedSearches.Count() == 0);
        Assert.IsTrue(service.RetrieveResult(id, nodeA, nodeD, store, null) == null);

        service.Update(1);

        Assert.IsTrue(service.TimeStamp == 1);
        Assert.IsTrue(service.ScheduledSearches.Count() == 0);
        Assert.IsTrue(service.SearchesInProgress.Count() == 1);
        Assert.IsTrue(service.CompletedSearches.Count() == 0);
        Assert.IsTrue(service.RetrieveResult(id, nodeA, nodeD, store, null) == null);

        service.Update(-1);

        Assert.IsTrue(service.TimeStamp == 2);
        Assert.IsTrue(service.ScheduledSearches.Count() == 0);
        Assert.IsTrue(service.SearchesInProgress.Count() == 0);
        Assert.IsTrue(service.CompletedSearches.Count() == 1);
        Assert.IsTrue(service.RetrieveResult(id, nodeA, nodeD, store, null) == store);

        Assert.IsTrue(store[0] == nodeA);
        Assert.IsTrue(store[1] == nodeC);
        Assert.IsTrue(store[2] == nodeD);
        Assert.IsTrue(store[3] == null);
    }

    /// <summary>
    /// Test with two agents, trying to find a comparable path (similar start and end nodes) at the same time.
    /// One search should be started only because the search requests are similar 
    /// </summary>
    [Test]
    public void TestDoubleSearch_ExpectOnlyOneSearchStarted()
    {
        var nodeA = new DungeonNode(0, 0, 1, 1);
        var nodeB = new DungeonNode(1, 0, 1, 1);
        var nodeC = new DungeonNode(-1, 0, 1, 1);
        var nodeD = new DungeonNode(2, 0, 1, 1);

        DungeonNode.Connect(nodeA, nodeB);
        DungeonNode.Connect(nodeA, nodeC);
        DungeonNode.Connect(nodeC, nodeD);

        var service = new SearchService<DungeonNode>().Initialize(2, 4, 4, () => DungeonSearch.CreatePathfinder(16, 1, 1, 1));

        var id1 = service.BeginSearch(nodeA, nodeD, 0);
        var id2 = service.BeginSearch(nodeD, nodeA, 0);
        var store1 = new DungeonNode[8];
        var store2 = new DungeonNode[8];

        Assert.IsTrue(id1 >= 0);
        Assert.IsTrue(id2 >= 0);

        Assert.IsTrue(service.ScheduledSearches.Count() == 1);
        Assert.IsTrue(service.RetrieveResult(id1, nodeA, nodeD, store1, null) == null);

        service.Update(-1);

        Assert.IsTrue(service.TimeStamp == 1);
        Assert.IsTrue(service.ScheduledSearches.Count() == 0);
        Assert.IsTrue(service.CompletedSearches.Count() == 1);
        Assert.IsTrue(service.RetrieveResult(id1, nodeA, nodeD, store1, null) == store1);
        Assert.IsTrue(service.RetrieveResult(id2, nodeD, nodeA, store2, null) == store2);
        
        Assert.IsTrue(store1[0] == nodeA);
        Assert.IsTrue(store1[1] == nodeC);
        Assert.IsTrue(store1[2] == nodeD);
        Assert.IsTrue(store1[3] == null);

        Assert.IsTrue(store2[0] == nodeD);
        Assert.IsTrue(store2[1] == nodeC);
        Assert.IsTrue(store2[2] == nodeA);
        Assert.IsTrue(store2[3] == null);
    }

    /// <summary>
    /// When running similar searches in sequence the old result should be returned
    /// </summary>
    [Test]
    public void TestSequentialSearch_ExpectOldResultFound()
    {
        string[] input =
        {
            " #",
            "##",
        };

        var grid = LevelGridFactory.CreateFrom(input);
        var service = new SearchService<DungeonNode>().Initialize(1, 4, 4, () => DungeonSearch.CreatePathfinder(16, 1, 1, 1));

        var id1 = service.BeginSearch(grid[0,0], grid[1, 1], 0);
        Assert.IsTrue(service.ScheduledSearches.Count() == 1);

        service.Update(-1);
        var store1 = new DungeonNode[8];

        Assert.IsTrue(service.CompletedSearches.Count() == 1);
        Assert.IsTrue(service.RetrieveResult(id1, grid[0, 0], grid[1, 1], store1, null) == store1);
        
        var id2 = service.BeginSearch(grid[1, 1], grid[0, 0], 0);
        Assert.IsTrue(id1 == id2);
        Assert.IsTrue(service.ScheduledSearches.Count() == 0);
    }

    /// <summary>
    /// When running similar searches and no search results are available, eventually they
    /// results should time out and a new search should become available
    /// </summary>
    [Test]
    public void TestSequentialSearchWithNoAvailableSearchResults_ExpectSearchResultToHaveNoMoreReferences()
    {
        string[] input =
        {
            " #",
            "##",
        };

        var grid = LevelGridFactory.CreateFrom(input);
        var service = new SearchService<DungeonNode>().Initialize(1, 1, 4, () => DungeonSearch.CreatePathfinder(16, 1, 1, 1));

        var id1 = service.BeginSearch(grid[0, 0], grid[1, 1], 0);
        var id2 = service.BeginSearch(grid[1, 0], grid[0, 0], 0);

        Assert.IsTrue(id1 == 0);
        Assert.IsTrue(service.ScheduledSearches.Count() == 1);

        // no search should be available
        Assert.IsTrue(id2 == -1);

        service.Update(-1);
        var store1 = new DungeonNode[8];

        Assert.IsTrue(service.CompletedSearches.Count() == 1);
        // ref count should be 1 until the owner clears it
        Assert.IsTrue(service.CompletedSearches.First().referenceCount == 1);
        Assert.IsTrue(service.RetrieveResult(id2, grid[1, 0], grid[0, 0], store1, null) == null);

        // search should not be available yet
        id2 = service.BeginSearch(grid[1, 0], grid[0, 0], 0);
        Assert.IsTrue(id2 == -1);

        // now clear the ref count
        service.ReleaseSearch(id1, 0);
        Assert.IsTrue(service.AvailableSearches.Count() == 1);

        //  search should be available as the reference count on the completed searches should be 0
        id2 = service.BeginSearch(grid[1, 0], grid[0, 0], 0);
        Assert.IsTrue(id2 == 0);
        Assert.IsTrue(service.ScheduledSearches.Count() == 1);
        Assert.IsTrue(service.CompletedSearches.Count() == 0);
    }

    /// <summary>
    /// Start a search, then cancel it. The search should revert back
    /// from 'scheduled' to 'available'.
    /// </summary>
    [Test]
    public void TestCancelSearch_ShouldBeAbleToCancelScheduledSearch()
    {
        string[] input =
        {
            " #",
            "##",
        };

        var grid = LevelGridFactory.CreateFrom(input);
        var service = new SearchService<DungeonNode>().Initialize(1, 1, 4, () => DungeonSearch.CreatePathfinder(16, 1, 1, 1));


        Assert.IsTrue(service.AvailableSearches.Count() == 1);
        Assert.IsTrue(service.SearchesInProgress.Count() == 0);

        var id1 = service.BeginSearch(grid[0, 0], grid[1, 1], 0);

        Assert.IsTrue(service.AvailableSearches.Count() == 0);
        Assert.IsTrue(service.ScheduledSearches.Count() == 1);

        service.ReleaseSearch(id1, -1);

        Assert.IsTrue(service.AvailableSearches.Count() == 1);
        Assert.IsTrue(service.ScheduledSearches.Count() == 0);
    }

    /// <summary>
    /// Start a search, then cancel it. The search should revert back
    /// from 'scheduled' to 'available'.
    /// </summary>
    [Test]
    public void TestCancelSearch_ShouldNotBeAbleToCancelScheduledSearchWhenThereAreMoreThan0References()
    {
        string[] input =
        {
            " #",
            "##",
        };

        var grid = LevelGridFactory.CreateFrom(input);
        var service = new SearchService<DungeonNode>().Initialize(1, 1, 4, () => DungeonSearch.CreatePathfinder(16, 1, 1, 1));


        var id1 = service.BeginSearch(grid[0, 0], grid[1, 1], 0);
        var id2 = service.BeginSearch(grid[0, 0], grid[1, 1], 0);

        Assert.IsTrue(service.AvailableSearches.Count() == 0);
        Assert.IsTrue(service.ScheduledSearches.Count() == 1);

        service.ReleaseSearch(id1, -1);

        Assert.IsTrue(service.AvailableSearches.Count() == 0);
        Assert.IsTrue(service.ScheduledSearches.Count() == 1);

        service.ReleaseSearch(id2, -1);

        Assert.IsTrue(service.AvailableSearches.Count() == 1);
        Assert.IsTrue(service.ScheduledSearches.Count() == 0);

    }

    /// <summary>
    /// Start a search, then cancel it. The search should revert back
    /// from 'in progress' to 'available'.
    /// </summary>
    [Test]
    public void TestCancelSearch_ShouldBeAbleToCancelSearchInProgress()
    {
        string[] input =
        {
            " #",
            "##",
        };

        var grid = LevelGridFactory.CreateFrom(input);
        var service = new SearchService<DungeonNode>().Initialize(1, 1, 4, () => DungeonSearch.CreatePathfinder(16, 1, 1, 1));


        Assert.IsTrue(service.AvailableSearches.Count() == 1);
        Assert.IsTrue(service.SearchesInProgress.Count() == 0);

        var id1 = service.BeginSearch(grid[0, 0], grid[1, 1], 0);

        service.Update(1);

        Assert.IsTrue(service.AvailableSearches.Count() == 0);
        Assert.IsTrue(service.SearchesInProgress.Count() == 1);

        service.ReleaseSearch(id1, -1);

        Assert.IsTrue(service.AvailableSearches.Count() == 1);
        Assert.IsTrue(service.SearchesInProgress.Count() == 0);
    }

    /// <summary>
    /// Start a search, then cancel it. The search should revert back
    /// from 'completed' to 'available'.
    /// </summary>
    [Test]
    public void TestCancelSearch_ShouldBeAbleToCancelCompletedSearches()
    {
        string[] input =
        {
            " #",
            "##",
        };

        var grid = LevelGridFactory.CreateFrom(input);
        var service = new SearchService<DungeonNode>().Initialize(1, 1, 4, () => DungeonSearch.CreatePathfinder(16, 1, 1, 1));

        Assert.IsTrue(service.AvailableSearches.Count() == 1);
        Assert.IsTrue(service.CompletedSearches.Count() == 0);

        var id1 = service.BeginSearch(grid[0, 0], grid[1, 1], 0);

        service.Update(-1);

        Assert.IsTrue(service.AvailableSearches.Count() == 0);
        Assert.IsTrue(service.CompletedSearches.Count() == 1);

        service.ReleaseSearch(id1, -1);

        Assert.IsTrue(service.AvailableSearches.Count() == 1);
        Assert.IsTrue(service.CompletedSearches.Count() == 0);
    }

    /// <summary>
    /// Start a search, then cancel it. The search should revert back
    /// from 'completed' to 'available'.
    /// </summary>
    [Test]
    public void TestCancelAndRestartSearch_ShouldBeAbleToCancelCompletedSearches()
    {
        string[] input =
        {
            " ##",
            "## ",
        };

        var grid = LevelGridFactory.CreateFrom(input);
        var service = new SearchService<DungeonNode>().Initialize(1, 1, 4, () => DungeonSearch.CreatePathfinder(16, 1, 1, 1));

        Assert.IsTrue(service.AvailableSearches.Count() == 1);
        Assert.IsTrue(service.CompletedSearches.Count() == 0);

        var id1 = service.BeginSearch(grid[0, 0], grid[1, 1], 0);

        service.Update(1);

        Assert.IsTrue(service.AvailableSearches.Count() == 0);
        Assert.IsTrue(service.SearchesInProgress.Count() == 1);

        service.ReleaseSearch(id1, -1);
        id1 = service.BeginSearch(grid[2, 1], grid[0, 0], 0);

        Assert.IsTrue(service.AvailableSearches.Count() == 0);
        Assert.IsTrue(service.ScheduledSearches.Count() == 1);

        service.Update(-1);

        Assert.IsTrue(service.AvailableSearches.Count() == 0);
        Assert.IsTrue(service.CompletedSearches.Count() == 1);

    }


}

