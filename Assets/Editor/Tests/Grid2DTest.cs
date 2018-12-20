using UnityEngine;
using NUnit.Framework;
using Tds.Util;
using Tds.DungeonGeneration;
using Tds.PathFinder;
using System.Linq;
using System.Collections.Generic;

public class Grid2DTest
{
    [Test]
    public void TestFindNearestAroundCenter_ExpectCenterElementFound()
    {
        var grid = new Grid2D<string>(3, 3, () => "");

        grid[1, 1] = "foo";

        var result = grid.FindNearestAround(Vector2Int.one, grid.Bounds, (value, distance) => !string.IsNullOrEmpty(value));

        Assert.IsTrue(result == "foo");
    }

    [Test]
    public void TestTriangleFunction()
    {
        for ( int i = 0; i < 4; ++i )
        {
            for ( var x = 0; x < (i*4); ++x )
            {
                var value = MathUtil.TriangleWave(x+i*2, i*2) - i;
                Debug.Log("period: " + i + ", x: "  + x + " => " + value);
            }

            Debug.Log("---------------");
        }
    }

    [Test]
    public void TestFindNearestAroundCenter_ExpectBottomElementFound()
    {
        var grid = new Grid2D<string>(3, 3, () => "");

        grid[1, 0] = "foo";

        var result = grid.FindNearestAround(Vector2Int.one, grid.Bounds, (value, distance) => !string.IsNullOrEmpty(value));

        Assert.IsTrue(result == "foo");
    }

    [Test]
    public void TestFindNearestAroundTopRight_ExpectBottomLeftElementFound()
    {
        var grid = new Grid2D<string>(3, 3, () => "");

        grid[0, 0] = "foo";

        var result = grid.FindNearestAround(Vector2Int.one * 2, grid.Bounds, (value, distance) => !string.IsNullOrEmpty(value));

        Assert.IsTrue(result == "foo");
    }

    [Test]
    public void TestFindNearestAroundTopRight_ExpectNoElementFoundWhenOutOfBounds()
    {
        var grid = new Grid2D<string>(3, 3, () => "");

        grid[0, 0] = "foo";

        var bounds = new RectInt(1, 1, 2, 2);
        var result = grid.FindNearestAround(Vector2Int.one * 2, bounds, (value, distance) => !string.IsNullOrEmpty(value));

        Assert.IsTrue(result == null);
    }

    /// <summary>
    /// Check if the closest elements are found
    /// </summary>
    [Test]
    public void TestFindNearestAroundCenterWithMultipleElements_ExpectClosestElementFound()
    {
        var grid = new Grid2D<string>(5, 5, () => "");

        grid[3, 3] = "foo";
        grid[4, 2] = "bar";

        var result = grid.FindNearestAround(Vector2Int.one * 2, grid.Bounds, (value, distance) => !string.IsNullOrEmpty(value));

        Assert.IsTrue(result == "foo");

        grid = new Grid2D<string>(5, 5, () => "");

        grid[3, 3] = "foo";
        grid[3, 2] = "bar";

        result = grid.FindNearestAround(Vector2Int.one * 2, grid.Bounds, (value, distance) => !string.IsNullOrEmpty(value));

        Assert.IsTrue(result == "bar");
    }

    [Test]
    public void TestRandomFindNearestAroundCenterWithMultipleElements_ExpectRandomElementFound()
    {
        var grid = new Grid2D<string>(5, 5, () => "");
        var results = new Dictionary<string, int>();

        // make sure there a predictable outcome
        Random.InitState(42);

        grid[2, 4] = "foo";
        grid[4, 2] = "bar";
        grid[2, 0] = "baz";

        results["foo"] = 0;
        results["bar"] = 0;
        results["baz"] = 0;

        for (int i = 0; i < 100; ++i)
        {
            var str = grid.FindNearestAround(Vector2Int.one * 2, grid.Bounds, (value, distance) => !string.IsNullOrEmpty(value), true);
            Assert.IsTrue(!string.IsNullOrEmpty(str));
           results[str]++;
        }

        Assert.IsTrue(results["foo"] > 0);
        Assert.IsTrue(results["bar"] > 0);
        Assert.IsTrue(results["baz"] > 0);

    }
}

