using UnityEngine;
using NUnit.Framework;
using Tds.Util;
using Tds.PathFinder;

class SearchParametersTest
{
    /// <summary>
    /// Trivial test to see if a search result with 0 nodes will be "reversed" correctly
    /// ie all elements in the store should be null.
    /// </summary>
    [Test]
    public void TestReverseCopyWith0Elements_ExpectStoreFilledWithNulls()
    {
        var searchParams = new SearchParameters<string>()
        {
            Length = 0
        };

        string[]  store = { "foo", "bar", "baz" };

        searchParams.ReverseCopyNodes(store);

        for (int i = 0; i < store.Length; ++i)
        {
            Assert.IsTrue(store[i] == null);
        }
    }

    /// <summary>
    /// Test to see if a search result with 1 nodes will be "reversed" correctly
    /// ie all elements except the first one should be null.
    /// </summary>
    [Test]
    public void TestReverseCopyWith1Elements_ExpectStoreFilledWithNullsExceptTheFirstElement()
    {
        var searchParams = new SearchParameters<string>()
        {
            Length = 1,
            Nodes = new string[] { "xxx" }
        };

        string[] store = { "foo", "bar", "baz" };

        searchParams.ReverseCopyNodes(store);

        Assert.IsTrue(store[0] == "xxx");
        Assert.IsTrue(store[1] == null);
        Assert.IsTrue(store[2] == null);
    }

    [Test]
    public void TestReverseCopyWith2Elements_ExpectStoreFilledWithNullsExceptTheFirstElement()
    {
        var searchParams = new SearchParameters<string>()
        {
            Length = 2,
            Nodes = new string[] { "xxx", null }
        };

        string[] store = { "foo", "bar", "baz" };

        searchParams.ReverseCopyNodes(store);

        Assert.IsTrue(store[0] == "xxx");
        Assert.IsTrue(store[1] == null);
        Assert.IsTrue(store[2] == null);
    }

    [Test]
    public void TestReverseCopyWith2ElementsNoNulls_ExpectStoreFilledWithNullsExceptTheFirstTwoElements()
    {
        var searchParams = new SearchParameters<string>()
        {
            Length = 2,
            Nodes = new string[] { "xxx", "xxy"}
        };

        string[] store = { "foo", "bar", "baz" };

        searchParams.ReverseCopyNodes(store);

        Assert.IsTrue(store[0] == "xxy");
        Assert.IsTrue(store[1] == "xxx");
        Assert.IsTrue(store[2] == null);
    }

    [Test]
    public void TestReverseCopyWith3Elements_ExpectStoreFilledWithNullsExceptTheFirstTwoElements()
    {
        var searchParams = new SearchParameters<string>()
        {
            Length = 3,
            Nodes = new string[] { "xxx", "xxy", null }
        };

        string[] store = { "foo", "bar", "baz" };

        searchParams.ReverseCopyNodes(store);

        Assert.IsTrue(store[0] == "xxy");
        Assert.IsTrue(store[1] == "xxx");
        Assert.IsTrue(store[2] == null);
    }

    [Test]
    public void TestReverseCopyWith3ElementsNoNulls_ExpectStoreFilled()
    {
        var searchParams = new SearchParameters<string>()
        {
            Length = 3,
            Nodes = new string[] { "xxx", "xxy", "xyy" }
        };

        string[] store = { "foo", "bar", "baz" };

        searchParams.ReverseCopyNodes(store);

        Assert.IsTrue(store[0] == "xyy");
        Assert.IsTrue(store[1] == "xxy");
        Assert.IsTrue(store[2] == "xxx");
    }

    [Test]
    public void TestReverseCopyWith4Elements_ExpectStoreFilled()
    {
        var searchParams = new SearchParameters<string>()
        {
            Length = 4,
            Nodes = new string[] { "xxx", "xxy", "xyy", null }
        };

        string[] store = { "foo", "bar", "baz" };

        searchParams.ReverseCopyNodes(store);

        Assert.IsTrue(store[0] == "xyy");
        Assert.IsTrue(store[1] == "xxy");
        Assert.IsTrue(store[2] == "xxx");
    }

    [Test]
    public void TestReverseCopyWith4ElementsNoNulls_ExpectStoreFilled()
    {
        var searchParams = new SearchParameters<string>()
        {
            Length = 4,
            Nodes = new string[] { "xxx", "xxy", "xyy", "yyy"}
        };

        string[] store = { "foo", "bar", "baz" };

        searchParams.ReverseCopyNodes(store);

        Assert.IsTrue(store[0] == "yyy");
        Assert.IsTrue(store[1] == "xyy");
        Assert.IsTrue(store[2] == "xxy");
    }

    [Test]
    public void TestReverseCopyWith5ElementsNoNulls_ExpectStoreFilled()
    {
        var searchParams = new SearchParameters<string>()
        {
            Length = 5,
            Nodes = new string[] { "xxx", "xxy", "xyy", "yyy", null }
        };

        string[] store = { "foo", "bar", "baz" };

        searchParams.ReverseCopyNodes(store);

        Assert.IsTrue(store[0] == "yyy");
        Assert.IsTrue(store[1] == "xyy");
        Assert.IsTrue(store[2] == "xxy");
    }
}

