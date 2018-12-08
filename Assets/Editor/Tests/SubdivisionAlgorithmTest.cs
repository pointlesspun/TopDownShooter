using UnityEngine;
using NUnit.Framework;
using Tds.DungeonGeneration;
using System.Collections.Generic;
using UnityEngine.Animations;

public class SubdivisionAlgorithmTest
{
    [Test]
    public void TestRandomYSplits()
    {
        var split = new SplitRect()
        {
            _rect = new RectInt(Vector2Int.zero, Vector2Int.one * 4),
            Neighbours = new HashSet<SplitRect>()
        };

        var divisionAlgorithm = new SubdivisionAlgorithm()
        {
            _minRectWidth = 1,
            _minRectHeight = 1
        };

        var widthCount = new int[5];

        for ( int i = 0; i < 200; i++)
        {
            var result = divisionAlgorithm.SplitRectRandomly(split, Axis.Y);
            Assert.IsTrue(result[0].width >= divisionAlgorithm._minRectWidth);
            Assert.IsTrue(result[1].width >= divisionAlgorithm._minRectWidth);
            Assert.IsTrue(result[0].width < split._rect.width);
            Assert.IsTrue(result[1].width < split._rect.width);
            Assert.IsTrue(result[0].height == split._rect.height);
            Assert.IsTrue(result[1].height == split._rect.height);

            widthCount[result[0].width]++;
            widthCount[result[1].width]++;
        }

        Assert.IsTrue(widthCount[0] == 0 );
        Assert.IsTrue(widthCount[1] > 0);
        Assert.IsTrue(widthCount[2] > 0);
        Assert.IsTrue(widthCount[3] > 0);
        Assert.IsTrue(widthCount[4] == 0);

    }

    [Test]
    public void TestRandomXSplits()
    {
        var split = new SplitRect()
        {
            _rect = new RectInt(Vector2Int.zero, Vector2Int.one * 4),
            Neighbours = new HashSet<SplitRect>()
        };

        var divisionAlgorithm = new SubdivisionAlgorithm()
        {
            _minRectWidth = 1,
            _minRectHeight = 1
        };

        var heightCount = new int[5];

        for (int i = 0; i < 200; i++)
        {
            var result = divisionAlgorithm.SplitRectRandomly(split, Axis.X);
            Assert.IsTrue(result[0].height >= divisionAlgorithm._minRectHeight);
            Assert.IsTrue(result[1].height >= divisionAlgorithm._minRectHeight);
            Assert.IsTrue(result[0].height < split._rect.height);
            Assert.IsTrue(result[1].height < split._rect.height);
            Assert.IsTrue(result[0].width == split._rect.width);
            Assert.IsTrue(result[1].width == split._rect.width);

            heightCount[result[0].height]++;
            heightCount[result[1].height]++;
        }

        Assert.IsTrue(heightCount[0] == 0);
        Assert.IsTrue(heightCount[1] > 0);
        Assert.IsTrue(heightCount[2] > 0);
        Assert.IsTrue(heightCount[3] > 0);
        Assert.IsTrue(heightCount[4] == 0);
    }

    [Test]
    public void TestSingleSplitGeneration()
    {
        var splitSource = new SplitRect()
        {
            _rect = new RectInt(Vector2Int.zero, Vector2Int.one * 4),
            Neighbours = new HashSet<SplitRect>()
        };

        var divisionAlgorithm = new SubdivisionAlgorithm()
        {
            _minRectWidth = 1,
            _minRectHeight = 1
        };

        var rectangles = divisionAlgorithm.SplitRectRandomly(splitSource, Axis.Y);
        var splitResult = divisionAlgorithm.CreateSplits(splitSource, rectangles);

        Assert.IsTrue(splitResult[0]._rect.height == splitSource._rect.height);
        Assert.IsTrue(splitResult[1]._rect.height == splitSource._rect.height);

        Assert.IsTrue(splitResult[0]._rect.width < splitSource._rect.width);
        Assert.IsTrue(splitResult[1]._rect.width < splitSource._rect.width);

        Assert.IsTrue(splitResult[0].Neighbours.Contains(splitResult[1]));
        Assert.IsTrue(splitResult[0].Neighbours.Count == 1);

        Assert.IsTrue(splitResult[1].Neighbours.Contains(splitResult[0]));
        Assert.IsTrue(splitResult[1].Neighbours.Count == 1);
    }
}

