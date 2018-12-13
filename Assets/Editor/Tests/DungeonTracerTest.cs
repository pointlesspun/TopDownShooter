using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Tds.DungeonGeneration;
using Tds.PathFinder;
using UnityEngine;

public class DungeonTracerTest
{
    [Test]
    public void TestSingleNodeTrace()
    {
        var position = Vector2.zero;
        var tracer = new DungeonPathTracer(() => position, 1.0f);

        tracer.BeginTrace(Vector2.one * 5.0f, new List<DungeonNode>() { new DungeonNode(0, 0, 5, 5) });

        // test initial conditions
        Assert.IsFalse(tracer.HasReachedEnd(position));
        Assert.IsFalse(tracer.HasReachedTarget(position, tracer.Waypoint));
        Assert.IsTrue(tracer.Waypoint == tracer.EndPoint);
        Assert.IsTrue(tracer.Direction == (tracer.EndPoint - position));

        Assert.IsFalse(tracer.Update());

        // move inside the range to the endpoint
        position += tracer.Direction;
        Assert.IsTrue(tracer.HasReachedEnd(position));
        Assert.IsTrue(tracer.Update());
    }

    [Test]
    public void TestTwoNodeTrace()
    {
        var position = Vector2.zero;
        var tracer = new DungeonPathTracer(() => position, 1.0f);

        var node1 = new DungeonNode(0, 0, 2, 2);
        var node2 = new DungeonNode(2, 0, 2, 2);

        DungeonNode.Connect(node1, node2);

        tracer.BeginTrace(new Vector2(4,2), new List<DungeonNode>() { node1, node2 });

        // test initial conditions
        Assert.IsFalse(tracer.HasReachedEnd(position));
        Assert.IsFalse(tracer.HasReachedTarget(position, tracer.Waypoint));
        Assert.IsTrue(tracer.Waypoint == node1.Edges.ElementAt(0).IntersectionCenter);
        Assert.IsTrue(tracer.Direction == (tracer.Waypoint - position));

        Assert.IsFalse(tracer.Update());

        // move towards the next point
        position += tracer.Direction;
        Assert.IsTrue(tracer.HasReachedTarget(tracer.Waypoint, position));
        Assert.IsFalse(tracer.HasReachedEnd(position));
        Assert.IsFalse(tracer.Update());

        // check if we're at the endpoint
        Assert.IsTrue(tracer.Waypoint == tracer.EndPoint);
        Assert.IsTrue(tracer.Direction == (tracer.EndPoint - position));

        position += tracer.Direction;
        Assert.IsTrue(tracer.Update());
    }
}

