using NUnit.Framework;
using Tds.DungeonGeneration;
using Tds.PathFinder;

class DungeonSearchTest
{
    [Test]
    public void TestOneNodeTest_ExpectOneNodePath()
    {
        var node = new DungeonNode(0, 0, 1, 1);
        var search = new DungeonSearch(256).BeginSearch(node, node);
        
        Assert.IsTrue(search.Iterate() == 0);

        var path = search.GetBestPath();

        Assert.IsTrue(path.Count == 1);
        Assert.IsTrue(path[0] == node);
    }

    [Test]
    public void TestTwoNodeTest_ExpectTwoNodePath()
    {
        var nodeA = new DungeonNode(0, 0, 1, 1);
        var nodeB = new DungeonNode(1, 0, 1, 1);

        DungeonNode.Connect(nodeA, nodeB);

        var search = new DungeonSearch(256).BeginSearch(nodeA, nodeB);
         
        Assert.IsTrue(search.Iterate(1) == 1);
        Assert.IsTrue(search.Iterate(1) == 0);

        var path = search.GetBestPath();

        Assert.IsTrue(path.Count == 2);
        Assert.IsTrue(path[0] == nodeA);
        Assert.IsTrue(path[1] == nodeB);
    }

    [Test]
    public void TestFourNodeTest_ExpectToAvoidDeadEnd()
    {
        var nodeA = new DungeonNode(0, 0, 1, 1);
        var nodeB = new DungeonNode(1, 0, 1, 1);
        var nodeC = new DungeonNode(-1, 0, 1, 1);
        var nodeD = new DungeonNode(2, 0, 1, 1);

        DungeonNode.Connect(nodeA, nodeB);
        DungeonNode.Connect(nodeA, nodeC);
        DungeonNode.Connect(nodeC, nodeD);

        var pathfinder = new DungeonSearch(256).BeginSearch(nodeA, nodeD);

        Assert.IsTrue(pathfinder.Iterate(1) == 2);
        Assert.IsTrue(pathfinder.Iterate(1) == 2);
        Assert.IsTrue(pathfinder.Iterate(1) == 4);
        Assert.IsTrue(pathfinder.Iterate(1) == 0);

        // test get best path without providing an array...
        var path = pathfinder.GetBestPath();

        Assert.IsTrue(path.Count == 3);
        Assert.IsTrue(path[0] == nodeA);
        Assert.IsTrue(path[1] == nodeC);
        Assert.IsTrue(path[2] == nodeD);
    }

    [Test]
    public void TestFourNodeTestWithArrayPathRetrieval_ExpectToWorkJustAsWellAsAList()
    {
        var nodeA = new DungeonNode(0, 0, 1, 1);
        var nodeB = new DungeonNode(1, 0, 1, 1);
        var nodeC = new DungeonNode(-1, 0, 1, 1);
        var nodeD = new DungeonNode(2, 0, 1, 1);

        DungeonNode.Connect(nodeA, nodeB);
        DungeonNode.Connect(nodeA, nodeC);
        DungeonNode.Connect(nodeC, nodeD);

        var search = new DungeonSearch(256).BeginSearch(nodeA, nodeD);

        Assert.IsTrue(search.Iterate(-1) == 0);

        // test with providing an array
        var arrayPath = search.GetBestPath(new DungeonNode[3]);

        Assert.IsTrue(arrayPath[0] == nodeA);
        Assert.IsTrue(arrayPath[1] == nodeC);
        Assert.IsTrue(arrayPath[2] == nodeD);

        arrayPath = search.GetBestPath(new DungeonNode[2]);

        Assert.IsTrue(arrayPath[0] == nodeA);
        Assert.IsTrue(arrayPath[1] == nodeC);

        arrayPath = search.GetBestPath(new DungeonNode[4]);
        Assert.IsTrue(arrayPath[0] == nodeA);
        Assert.IsTrue(arrayPath[1] == nodeC);
        Assert.IsTrue(arrayPath[2] == nodeD);
        Assert.IsTrue(arrayPath[3] == null);
    }

    
}

