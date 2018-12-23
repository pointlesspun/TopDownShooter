using UnityEngine;
using NUnit.Framework;
using Tds.Util;
using Tds.DungeonGeneration;
using Tds.PathFinder;
using System.Linq;

public class DungeonSubdivisionTest
{
    /// <summary>
    /// When there is only one rect with a minimum size, there should be no subdivision.
    /// </summary>
    [Test]
    public void TestDivisionWithMinRect_ExpectNoDivision()
    {
        var algorithm = new DungeonSubdivision();
        var result = algorithm.Subdivide(new Rect(0, 0, algorithm._minRectWidth, algorithm._minRectHeight));

        Assert.IsTrue(result.Nodes.Count() == 1);
        Assert.IsTrue(result.Nodes.ElementAt(0).Bounds.Equals(new Rect(0, 0, algorithm._minRectWidth, algorithm._minRectHeight)));
        Assert.IsTrue(result.Nodes.ElementAt(0).Edges == null);
    }

    /// <summary>
    /// When executing one division, we should end up with two nodes
    /// </summary>
    [Test]
    public void TestOneSubdivision_ExpectTwoConnectedNodes()
    {
        var algorithm = new DungeonSubdivision()
        {
            _minDepth = 0,
            _maxDepth = 1
        };

        var result = algorithm.Subdivide(new Rect(0, 0, algorithm._minRectWidth*2, algorithm._minRectHeight*2), UnityEngine.Animations.Axis.X);
        var nodes = result.Nodes.ToList();

        Assert.IsTrue(nodes.Count == 2);
        Assert.IsTrue(nodes[0].Edges.Count() == 1);
        Assert.IsTrue(nodes[1].Edges.Count() == 1);
        Assert.IsTrue(nodes[0].Edges.ElementAt(0) == nodes[1].Edges.ElementAt(0));
        Assert.IsTrue(nodes[0].Edges.ElementAt(0).GetOther(nodes[0]) == nodes[1]);
        Assert.IsTrue(nodes[1].Edges.ElementAt(0).GetOther(nodes[1]) == nodes[0]);
        Assert.IsTrue(nodes[0].Height + nodes[1].Height == algorithm._minRectHeight * 2);
        Assert.IsTrue(nodes[0].Width == algorithm._minRectHeight * 2);
        Assert.IsTrue(nodes[1].Width == algorithm._minRectHeight * 2);
    }

    /// <summary>
    /// When executing twos division, we should end up with four nodes
    /// </summary>
    [Test]
    public void TestTwoSubdivisions_ExpectFourConnectedNodes()
    {
        var algorithm = new DungeonSubdivision()
        {
            _minDepth = 0,
            _maxDepth = 2
        };

        var result = algorithm.Subdivide(new Rect(0, 0, algorithm._minRectWidth * 2, algorithm._minRectHeight * 2), UnityEngine.Animations.Axis.X);
        var nodes = result.Nodes.ToList();

        nodes.Sort((n1, n2) => n1.Min.x.CompareTo(n2.Min.x));

        Assert.IsTrue(nodes.Count == 4);

        foreach ( var node in nodes )
        {
            Assert.IsTrue(node.Edges.Count() == 2);
        }

        Assert.IsTrue(nodes[0].Width + nodes[2].Width == algorithm._minRectWidth * 2);
        Assert.IsTrue(nodes[1].Width + nodes[3].Width == algorithm._minRectWidth * 2);

        Assert.IsTrue(nodes[0].Height + nodes[1].Height == algorithm._minRectHeight * 2);
        Assert.IsTrue(nodes[2].Height + nodes[3].Height == algorithm._minRectHeight * 2);

    }
}

