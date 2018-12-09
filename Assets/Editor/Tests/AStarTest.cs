using UnityEngine;
using NUnit.Framework;
using Tds.Util;
using Tds.DungeonGeneration;
using Tds.Pathfinder;

public class AStarTest
{
    [Test]
    public void ShouldFindTheResultInASingleNodeDungeon()
    {
        var search = new AStar(256);
        var node = new TraversalNode()
        {
            _split = new SplitRect()
            {
                _rect = new RectInt(0, 0, 1, 1)
            }
        };

        search.BeginSearch(node, node, (n1, n2) => (n2._split._rect.center - n1._split._rect.center).magnitude);
        Assert.IsTrue(search.Iterate() == 0);
        var path = search.GetBestPath();

        Assert.IsTrue(path.Count == 1);
        Assert.IsTrue(path[0] == node);
    }

    [Test]
    public void ShouldFindTheResultInATwoNodeDungeon()
    {
        var search = new AStar(256);
        var nodeA = new TraversalNode(0, 0, 1, 1);
        var nodeB = nodeA.AddChild(new TraversalNode(1, 0, 1, 1));
        
        search.BeginSearch(nodeA, nodeB, (n1, n2) => (n2._split._rect.center - n1._split._rect.center).magnitude);
        Assert.IsTrue(search.Iterate() > 0);
        Assert.IsTrue(search.Iterate() == 0);
        var path = search.GetBestPath();

        Assert.IsTrue(path.Count == 2);
        Assert.IsTrue(path[0] == nodeA);
        Assert.IsTrue(path[1] == nodeB);
    }

    [Test]
    public void ShouldAvoidDeadEnds()
    {
        var search = new AStar(256);
        var nodeA = new TraversalNode(0, 0, 1, 1);
        var nodeB = nodeA.AddChild(new TraversalNode(1, 0, 1, 1));
        var nodeC = nodeA.AddChild(new TraversalNode(-1, 0, 1, 1));
        var nodeD = nodeC.AddChild(new TraversalNode(2, 0, 1, 1));

        search.BeginSearch(nodeA, nodeD, (n1, n2) => (n2._split._rect.center - n1._split._rect.center).magnitude);
        Assert.IsTrue(search.Iterate() == 3);
        Assert.IsTrue(search.Iterate() == 7);
        Assert.IsTrue(search.Iterate() == 0);
        var path = search.GetBestPath();

        Assert.IsTrue(path.Count == 3);
        Assert.IsTrue(path[0] == nodeA);
        Assert.IsTrue(path[1] == nodeC);
        Assert.IsTrue(path[2] == nodeD);
    }

    [Test]
    public void ShouldReturnTheBestNodeInCaseOfNoSolution()
    {
        var search = new AStar(256);
        var nodeA = new TraversalNode(0, 0, 1, 1);
        var nodeB = nodeA.AddChild(new TraversalNode(1, 0, 1, 1));
        var nodeC = nodeA.AddChild(new TraversalNode(-1, 0, 1, 1));
        var nodeD = new TraversalNode(2, 0, 1, 1);

        search.BeginSearch(nodeA, nodeD, (n1, n2) => (n2._split._rect.center - n1._split._rect.center).magnitude);
        Assert.IsTrue(search.Iterate() == 3);
        Assert.IsTrue(search.Iterate() == -1);
        var path = search.GetBestPath();

        Assert.IsTrue(path.Count == 2);
        Assert.IsTrue(path[0] == nodeA);
        Assert.IsTrue(path[1] == nodeB);
    }
}

