using NUnit.Framework;
using Tds.DungeonGeneration;
using Tds.Pathfinder;

class DungeonSearchTest
{
    [Test]
    public void TestOneNodeTest_ExpectOneNodePath()
    {
        var node = new DungeonNode(0, 0, 1, 1);
        var dungeon = new DungeonLayout(node);
        var search = new DungeonSearch(256);

        search.BeginSearch(node, node);

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

        var dungeon = new DungeonLayout(nodeA, nodeB);
        var search = new DungeonSearch(256);

        search.BeginSearch(nodeA, nodeB);
         
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

        var dungeon = new DungeonLayout(nodeA, nodeB, nodeC, nodeD);
        var search = new DungeonSearch(256);

        search.BeginSearch(nodeA, nodeD);

        Assert.IsTrue(search.Iterate(1) == 2);
        Assert.IsTrue(search.Iterate(1) == 2);
        Assert.IsTrue(search.Iterate(1) == 4);
        Assert.IsTrue(search.Iterate(1) == 0);

        var path = search.GetBestPath();

        Assert.IsTrue(path.Count == 3);
        Assert.IsTrue(path[0] == nodeA);
        Assert.IsTrue(path[1] == nodeC);
        Assert.IsTrue(path[2] == nodeD);
    }
}

