﻿using NUnit.Framework;
using System.Linq;
using Tds.DungeonGeneration;
using Tds.PathFinder;
using Tds.Util;

public class TestDungeonSearchService
{
    [Test]
    public void TestInitialization_ExpectInitialValuesToBeCorrect()
    {
        var service = new PathfindingService<DungeonNode>().Initialize(2, 4, 32,() => DungeonSearch.CreatePathfinder(16, 1, 1, 1));

        Assert.IsTrue(service.TimeStamp == 0);
        Assert.IsTrue(service.ScheduledSearches.Count() == 0);
        Assert.IsTrue(service.SearchesInProgress.Count() == 0);
        Assert.IsTrue(service.CompletedSearches.Count() == 0);
    }

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

        var service = new PathfindingService<DungeonNode>().Initialize(2, 4, 4, () => DungeonSearch.CreatePathfinder(16, 1, 1, 1));

        var id = service.BeginSearch(nodeA, nodeD);
        var store = new DungeonNode[8];

        Assert.IsTrue(id >= 0);
        Assert.IsTrue(service.TimeStamp == 0);
        Assert.IsTrue(service.ScheduledSearches.Count() == 1);
        Assert.IsTrue(service.SearchesInProgress.Count() == 0);
        Assert.IsTrue(service.CompletedSearches.Count() == 0);
        Assert.IsTrue(service.RetrieveResult(id, nodeA, nodeD, store) == null);

        service.Update(1);

        Assert.IsTrue(service.TimeStamp == 1);
        Assert.IsTrue(service.ScheduledSearches.Count() == 0);
        Assert.IsTrue(service.SearchesInProgress.Count() == 1);
        Assert.IsTrue(service.CompletedSearches.Count() == 0);
        Assert.IsTrue(service.RetrieveResult(id, nodeA, nodeD, store) == null);

        service.Update(-1);

        Assert.IsTrue(service.TimeStamp == 2);
        Assert.IsTrue(service.ScheduledSearches.Count() == 0);
        Assert.IsTrue(service.SearchesInProgress.Count() == 0);
        Assert.IsTrue(service.CompletedSearches.Count() == 1);
        Assert.IsTrue(service.RetrieveResult(id, nodeA, nodeD, store) == store);

        Assert.IsTrue(store[0] == nodeA);
        Assert.IsTrue(store[1] == nodeC);
        Assert.IsTrue(store[2] == nodeD);
        Assert.IsTrue(store[3] == null);
    }

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

        var service = new PathfindingService<DungeonNode>().Initialize(2, 4, 4, () => DungeonSearch.CreatePathfinder(16, 1, 1, 1));

        var id1 = service.BeginSearch(nodeA, nodeD);
        var id2 = service.BeginSearch(nodeD, nodeA);
        var store1 = new DungeonNode[8];
        var store2 = new DungeonNode[8];

        Assert.IsTrue(id1 >= 0);
        Assert.IsTrue(id2 >= 0);

        Assert.IsTrue(service.ScheduledSearches.Count() == 1);
        Assert.IsTrue(service.RetrieveResult(id1, nodeA, nodeD, store1) == null);

        service.Update(-1);
        service.Update(-1);

        Assert.IsTrue(service.TimeStamp == 2);
        Assert.IsTrue(service.ScheduledSearches.Count() == 0);
        Assert.IsTrue(service.CompletedSearches.Count() == 1);
        Assert.IsTrue(service.RetrieveResult(id1, nodeA, nodeD, store1) == store1);
        Assert.IsTrue(service.RetrieveResult(id2, nodeD, nodeA, store2) == store2);
        
        Assert.IsTrue(store1[0] == nodeA);
        Assert.IsTrue(store1[1] == nodeC);
        Assert.IsTrue(store1[2] == nodeD);
        Assert.IsTrue(store1[3] == null);

        Assert.IsTrue(store2[0] == nodeD);
        Assert.IsTrue(store2[1] == nodeC);
        Assert.IsTrue(store2[2] == nodeA);
        Assert.IsTrue(store2[3] == null);
    }
}

