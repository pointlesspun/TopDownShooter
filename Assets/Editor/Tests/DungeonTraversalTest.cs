using UnityEngine;
using NUnit.Framework;
using Tds.Util;
using Tds.DungeonGeneration;
using Tds.Pathfinder;
using System.Linq;
using System.Collections.Generic;

public class DungeonTraversalTest
{
    /// <summary>
    /// traverse through a dungeon with one node, expect a path of one 
    /// </summary>
    [Test]
    public void TestOneNodeExample_ExpectOneNodeTraversalDungeon()
    {
        var dungeon = new DungeonLayout(new List<DungeonNode>() { new DungeonNode(new RectInt(0, 0, 1, 1)) });
        var traversalAlgorithm = new DungeonTraversal()
        {
            _maxDepth = 1
        };

        var traversal = traversalAlgorithm.Traverse(dungeon);

        Assert.IsTrue(traversal.Nodes.Count() == 1);
        Assert.IsTrue(traversal.Start != null);
        Assert.IsTrue(traversal.End != null);
        Assert.IsTrue(traversal.Start == traversal.End);
        Assert.IsTrue(traversal.Start.Edges == null);
    }

    /// <summary>
    /// traverse through a dungeon with two nodes, expect a path of two nodes
    /// </summary>
    [Test]
    public void TestTwoNodeExample_ExpectTwoNodeTraversalDungeon()
    {
        var nodeA = new DungeonNode(new RectInt(0, 0, 1, 1)) { Name = "nodeA"};
        var nodeB = new DungeonNode(new RectInt(1, 0, 1, 1)) { Name = "nodeB" };

        DungeonNode.Connect(nodeA, nodeB);

        var dungeon = new DungeonLayout(new List<DungeonNode>() { nodeA, nodeB });

        var traversalAlgorithm = new DungeonTraversal()
        {
            _maxDepth = 2,
            _requiredIntersectionLength = 1.0f
        };

        var traversal = traversalAlgorithm.Traverse(dungeon, nodeA);

        Assert.IsTrue(traversal.Nodes.Count() == 2);
        Assert.IsTrue(traversal.Start != null);
        Assert.IsTrue(traversal.End != null);
        Assert.IsTrue(traversal.Start.Name == nodeA.Name);
        Assert.IsTrue(traversal.End.Name == nodeB.Name);

        Assert.IsTrue(traversal.Start.Edges.Count() == 1);
        Assert.IsTrue(traversal.End.Edges.Count() == 1);

        Assert.IsTrue(traversal.Start.Edges.ElementAt(0).GetOther(traversal.Start).Name == nodeB.Name);
        Assert.IsTrue(traversal.End.Edges.ElementAt(0).GetOther(traversal.End).Name == nodeA.Name);
    }


    /// <summary>
    /// traverse through a dungeon with two nodes, expect a path of two nodes
    /// </summary>
    [Test]
    public void TestThreeNodeExample_ExpectTwoNodeTraversalDungeon()
    {
        var nodeA = new DungeonNode(new RectInt(0, 0, 1, 1)) { Name = "nodeA" };
        var nodeB = new DungeonNode(new RectInt(1, 0, 1, 1)) { Name = "nodeB" };
        var nodeC = new DungeonNode(new RectInt(3, 0, 1, 1)) { Name = "nodeC" };

        DungeonNode.Connect(nodeA, nodeB);
        DungeonNode.Connect(nodeB, nodeC);

        var dungeon = new DungeonLayout(new List<DungeonNode>() { nodeA, nodeB, nodeC });

        var traversalAlgorithm = new DungeonTraversal()
        {
            _maxDepth = 3,
            _requiredIntersectionLength = 1.0f
        };

        var traversal = traversalAlgorithm.Traverse(dungeon, nodeA);

        Assert.IsTrue(traversal.Nodes.Count() == 3);
        Assert.IsTrue(traversal.Start != null);
        Assert.IsTrue(traversal.End != null);
        Assert.IsTrue(traversal.Start.Name == nodeA.Name);
        Assert.IsTrue(traversal.End.Name == nodeC.Name);

        var nodeAA = traversal.Nodes.First(n => n.Name == nodeA.Name);
        var nodeBB = traversal.Nodes.First(n => n.Name == nodeB.Name);
        var nodeCC = traversal.Nodes.First(n => n.Name == nodeC.Name);

        Assert.IsTrue(nodeAA.Edges.Count() == 1);
        Assert.IsTrue(nodeBB.Edges.Count() == 2);
        Assert.IsTrue(nodeCC.Edges.Count() == 1);
    }
}

