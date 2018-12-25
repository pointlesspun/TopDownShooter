using NUnit.Framework;
using Tds.DungeonGeneration;
using Tds.PathFinder;
using Tds.Util;
using UnityEngine;

public class TestSpatialTreeConstruction
{
    private class TestRectangle : IBounds
    {
        public Rect Bounds
        {
            get;
            set;
        }

        public TestRectangle( Rect b )
        {
            Bounds = b;
        }

        public TestRectangle(float x, float y, float width, float height)
        {
            Bounds = new Rect(x,y,width,height);
        }

        public override string ToString()
        {
            return Bounds.ToString();
        }
    }

    /// <summary>
    /// Should find the best pivot after the 0th element (so at 1)
    /// </summary>
    [Test]
    public void TestFindHorizontalPivotInSimpleTestCase_ExpectPivotToBe1()
    {
        TestRectangle[] data = { new TestRectangle(0, 0, 1, 1), new TestRectangle(1, 0, 1, 1) };

        Rect rect = Rect.zero;
        var result = SpatialBinaryTree<TestRectangle>.FindHorizontalPivot(data, 0, data.Length, ref rect);

        Assert.IsTrue(result == 1);
        Assert.IsTrue(rect.xMin == 0 && rect.yMin == 0 && rect.width == 2 && rect.height == 1);
    }

    /// <summary>
    /// Should find the best pivot after the 1th element (so at 2)
    /// </summary>
    [Test]
    public void TestFindHorizontalPivotInSimpleTestCaseWithMultipleSolutions_ExpectPivotToBe2()
    {
        TestRectangle[] data = { new TestRectangle(0, 0, 1, 1), new TestRectangle(1, 0, 1, 1),
                                    new TestRectangle(2, 0, 1, 1), new TestRectangle(3, 0, 1, 1)};

        Rect rect = Rect.zero;
        var result = SpatialBinaryTree<TestRectangle>.FindHorizontalPivot(data, 0, data.Length, ref rect);

        Assert.IsTrue(result == 2);
        Assert.IsTrue(rect.xMin == 0 && rect.yMin == 0 && rect.width == 4 && rect.height == 1);
    }

    /// <summary>
    /// Should not be able to find a 'best' pivot because the elements overlap
    /// </summary>
    [Test]
    public void TestFindHorizontalPivotInWithoutSolutions_ExpectPivotToBeNegative()
    {
        TestRectangle[] data = { new TestRectangle(0, 0, 1, 1), new TestRectangle(1, 0, 1, 1),
                                    new TestRectangle(2, 0, 1, 1), new TestRectangle(0, 1, 3, 1)};

        Rect rect = Rect.zero;
        var result = SpatialBinaryTree<TestRectangle>.FindHorizontalPivot(data, 0, data.Length, ref rect);

        Assert.IsTrue(result == -1);
        Assert.IsTrue(rect.xMin == 0 && rect.yMin == 0 && rect.width == 3 && rect.height == 2);
    }

    /// <summary>
    /// Should be able to find a 'best' solution with overlapping elements 
    /// </summary>
    [Test]
    public void TestFindHorizontalPivotInWithOverlappingSolutions_ExpectPivotToBeThree()
    {
        TestRectangle[] data = { new TestRectangle(0, 0, 1, 1), new TestRectangle(0, 1, 2, 1),
                                    new TestRectangle(1, 0, 1, 1), new TestRectangle(2, 0, 1, 1)};

        Rect rect = Rect.zero;
        var result = SpatialBinaryTree<TestRectangle>.FindHorizontalPivot(data, 0, data.Length, ref rect);

        Assert.IsTrue(result == 3);
        Assert.IsTrue(rect.xMin == 0 && rect.yMin == 0 && rect.width == 3 && rect.height == 2);
    }

    /// <summary>
    /// Should be able to find a 'best' solution with overlapping elements, even though they not in order 
    /// </summary>
    [Test]
    public void TestFindHorizontalPivotInWithOverlappingUnsortedSolutions_ExpectPivotToBeThree()
    {
        TestRectangle[] data = { new TestRectangle(0, 1, 2, 1),
                                  new TestRectangle(0, 0, 1, 1),
                                  new TestRectangle(2, 0, 1, 1),
                                   new TestRectangle(1, 0, 1, 1)};

        Rect rect = Rect.zero;
        var result = SpatialBinaryTree<TestRectangle>.FindHorizontalPivot(data, 0, data.Length, ref rect);

        Assert.IsTrue(result == 3);
        Assert.IsTrue(rect.xMin == 0 && rect.yMin == 0 && rect.width == 3 && rect.height == 2);
    }

    /// <summary>
    /// Create a tree from a single data point. The root should be the same as the datapoint.
    /// </summary>
    [Test]
    public void TestCreateSpatialTreeFromSimpleDataSet_ExpectSingleNode()
    {
        TestRectangle[] data = { new TestRectangle(0, 0, 1, 1) };
        var tree = new SpatialBinaryTree<TestRectangle>(data);

        Assert.IsTrue(tree.data == data);
        Assert.IsTrue(tree.root.index == 0);
        Assert.IsTrue(tree.root.length == 1);
        Assert.IsTrue(tree.root.bounds == data[0].Bounds);
        Assert.IsTrue(tree.root.left == null);
        Assert.IsTrue(tree.root.right == null);
    }

    /// <summary>
    /// Create a tree from a two data points. The root should have a left and right node.
    /// </summary>
    [Test]
    public void TestCreateSpatialTreeFromTwoNodeDataSet_ExpectRootWithChildren()
    {
        TestRectangle[] data = { new TestRectangle(0, 0, 1, 1), new TestRectangle(1, 0, 1, 1) };
        var tree = new SpatialBinaryTree<TestRectangle>(data);

        Assert.IsTrue(tree.data == data);
        Assert.IsTrue(tree.root.index == 0);
        Assert.IsTrue(tree.root.length == 2);
        Assert.IsTrue(tree.root.bounds == data[0].Bounds.Union(data[1].Bounds));
        Assert.IsTrue(tree.root.left != null);
        Assert.IsTrue(tree.root.left.index == 0);
        Assert.IsTrue(tree.root.left.length == 1);
        Assert.IsTrue(tree.root.left.bounds == data[0].Bounds);
        Assert.IsTrue(tree.root.right != null);
        Assert.IsTrue(tree.root.right.index == 1);
        Assert.IsTrue(tree.root.right.length == 1);
        Assert.IsTrue(tree.root.right.bounds == data[1].Bounds);
    }

    /// <summary>
    /// Create a tree from a four data points. The root should have a left and right node which should themselves be split.
    /// </summary>
    [Test]
    public void TestCreateSpatialTreeFromFourNodeDataSet_ExpectTreeWithDepth2()
    {
        TestRectangle[] data = { new TestRectangle(0, 0, 1, 1), new TestRectangle(1, 0, 1, 1),
                                   new TestRectangle(0, 1, 1, 1), new TestRectangle(1, 1, 1, 1) };
        var tree = new SpatialBinaryTree<TestRectangle>(data, UnityEngine.Animations.Axis.Y);

        Assert.IsTrue(tree.data == data);
        Assert.IsTrue(tree.root.index == 0);
        Assert.IsTrue(tree.root.length == 4);
        Assert.IsTrue(tree.root.bounds == new Rect(0, 0, 2, 2));

        Assert.IsTrue(tree.root.left != null);
        Assert.IsTrue(tree.root.left.index == 0);
        Assert.IsTrue(tree.root.left.length == 2);
        Assert.IsTrue(tree.root.left.bounds == new Rect(0, 0, 1, 2));

        Assert.IsTrue(tree.root.left.left != null);
        Assert.IsTrue(tree.root.left.left.index == 0);
        Assert.IsTrue(tree.root.left.left.length == 1);
        Assert.IsTrue(tree.root.left.left.bounds == new Rect(0, 0, 1, 1));

        Assert.IsTrue(tree.root.left.right != null);
        Assert.IsTrue(tree.root.left.right.index == 1);
        Assert.IsTrue(tree.root.left.right.length == 1);
        Assert.IsTrue(tree.root.left.right.bounds == new Rect(0, 1, 1, 1));

        Assert.IsTrue(tree.root.right != null);
        Assert.IsTrue(tree.root.right.index == 2);
        Assert.IsTrue(tree.root.right.length == 2);
        Assert.IsTrue(tree.root.right.bounds == new Rect(1, 0, 1, 2));

        Assert.IsTrue(tree.root.right.left != null);
        Assert.IsTrue(tree.root.right.left.index == 2);
        Assert.IsTrue(tree.root.right.left.length == 1);
        Assert.IsTrue(tree.root.right.left.bounds == new Rect(1, 0, 1, 1));

        Assert.IsTrue(tree.root.right.right != null);
        Assert.IsTrue(tree.root.right.right.index == 3);
        Assert.IsTrue(tree.root.right.right.length == 1);
        Assert.IsTrue(tree.root.right.right.bounds == new Rect(1, 1, 1, 1));
    }

    /// <summary>
    /// Create a tree from a three data points. The root have been divised over the y axis first.
    /// </summary>
    [Test]
    public void TestCreateSpatialTreeFromThreeNodeDataSet_ExpectTreeWithDepth2()
    {
        TestRectangle[] data = { new TestRectangle(0, 0, 1, 1), new TestRectangle(1, 0, 1, 1),
                                   new TestRectangle(0, 1, 2, 1),  };
        var tree = new SpatialBinaryTree<TestRectangle>(data);

        Assert.IsTrue(tree.data == data);
        Assert.IsTrue(tree.root.index == 0);
        Assert.IsTrue(tree.root.length == 3);
        Assert.IsTrue(tree.root.bounds == new Rect(0, 0, 2, 2));

        Assert.IsTrue(tree.root.left != null);
        Assert.IsTrue(tree.root.left.index == 0);
        Assert.IsTrue(tree.root.left.length == 2);
        Assert.IsTrue(tree.root.left.bounds == new Rect(0, 0, 2, 1));
        Assert.IsTrue(tree.root.left.left != null);
        Assert.IsTrue(tree.root.left.right != null);

        Assert.IsTrue(tree.root.right != null);
        Assert.IsTrue(tree.root.right.index == 2);
        Assert.IsTrue(tree.root.right.length == 1);
        Assert.IsTrue(tree.root.right.bounds == new Rect(0, 1, 2, 1));
        Assert.IsTrue(tree.root.right.left == null);
        Assert.IsTrue(tree.root.right.right == null);
    }

    /// <summary>
    /// Create a tree from a four data points which cannot be separated over an axis. 
    /// When created with a 'strictly axis separation flag' on the root should not be divided
    /// </summary>
    [Test]
    public void TestCreateSpatialTreeFromNonDivisibleSet_ExpectTreeRootContainingAllTheSamples()
    {
        TestRectangle[] data = { new TestRectangle(0, 0, 2, 1), new TestRectangle(2, 0, 1, 2),
                                   new TestRectangle(0, 1, 1, 2), new TestRectangle(1, 2, 2, 1) };
        var tree = new SpatialBinaryTree<TestRectangle>(data);

        Assert.IsTrue(tree.data == data);
        Assert.IsTrue(tree.root.index == 0);
        Assert.IsTrue(tree.root.length == 4);
        Assert.IsTrue(tree.root.bounds == new Rect(0, 0, 3, 3));

        Assert.IsTrue(tree.root.left == null);
        Assert.IsTrue(tree.root.right == null);
    }

    /// <summary>
    /// Test case from the game which exposed a problem with the pivot, couldn't find a better name...
    /// </summary>
    [Test]
    public void TestReproCase()
    {
        TestRectangle[] data = { new TestRectangle(0, 0, 2, 1), new TestRectangle(0, 1, 1, 1),
                                   new TestRectangle(1, 1, 2, 1), new TestRectangle(2, 0, 1, 1),
                                    new TestRectangle(3, 0, 1, 1), new TestRectangle(3, 1, 1, 1)};

        var tree = new SpatialBinaryTree<TestRectangle>(data, UnityEngine.Animations.Axis.Y);

        Assert.IsTrue(tree.root.index == 0);
        Assert.IsTrue(tree.root.length == 6);
        Assert.IsTrue(tree.root.bounds == new Rect(0, 0, 4, 2));

        Assert.IsTrue(tree.root.left.bounds == new Rect(0, 0, 3, 2));
        Assert.IsTrue(tree.root.right.bounds == new Rect(3, 0, 1, 2));
    }
}

