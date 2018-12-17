using NUnit.Framework;
using System.Linq;
using Tds.DungeonGeneration;
using Tds.PathFinder;
using Tds.Util;
using UnityEngine;

public class DungeonGenerationUtilTest
{
    [Test]
    public void TestGenerateFromStringArray_ExpectDungeonCreated()
    {
        string[] input =
        {
            " #",
            "##"
        };
        var result = DungeonGenerationUtil.CreateFrom(input, 2, 2);

        Assert.IsTrue(result.Width == 2);
        Assert.IsTrue(result.Height == 2);
        Assert.IsTrue(result[0, 0] != null);
        Assert.IsTrue(result[0, 1] == null);
        Assert.IsTrue(result[1, 0] != null);
        Assert.IsTrue(result[1, 1] != null);

        Assert.IsTrue(result[0, 0].Rect.min == Vector2Int.zero );
        Assert.IsTrue(result[1, 0].Rect.min == Vector2Int.right * 2);
        Assert.IsTrue(result[1, 1].Rect.min == Vector2Int.one * 2);

        Assert.IsTrue(result[0, 0].Rect.width == 2);
        Assert.IsTrue(result[0, 0].Rect.height == 2);

        Assert.IsTrue(result[1, 0].Rect.width == 2);
        Assert.IsTrue(result[1, 0].Rect.height == 2);

        Assert.IsTrue(result[1, 1].Rect.width == 2);
        Assert.IsTrue(result[1, 1].Rect.height == 2);


        Assert.IsTrue(result[0, 0].Neighbours.Count() == 1);
        Assert.IsTrue(result[1, 0].Neighbours.Count() == 2);
        Assert.IsTrue(result[1, 1].Neighbours.Count() == 1);

        Assert.IsTrue(result[0, 0].GetEdgeTo(result[0, 0]) == null);
        Assert.IsTrue(result[0, 0].GetEdgeTo(result[1, 0]) != null);

        Assert.IsTrue(result[1, 0].GetEdgeTo(result[0, 0]) != null);
        Assert.IsTrue(result[1, 0].GetEdgeTo(result[1, 1]) != null);

        Assert.IsTrue(result[1, 1].GetEdgeTo(result[0, 0]) == null);
        Assert.IsTrue(result[1, 1].GetEdgeTo(result[1, 0]) != null);
    }
}

