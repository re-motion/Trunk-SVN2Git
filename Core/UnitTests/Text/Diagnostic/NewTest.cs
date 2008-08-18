using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using System.Collections.Generic;
using Remotion.Text.Diagnostic;

namespace Remotion.UnitTests.Text.Diagnostic
{
  [NUnit.Framework.TestFixture]
  public class NewTest
  {

    [Test]
    public void NewArrayTest ()
    {
      var collection = New.Array (New.Array(7, 11),New.Array( 13, 17));
      var collectionExpected = new int[][] { new int[] { 7, 11 }, new int[] { 13, 17 } };
      Assert.That (collection, Is.EqualTo (collectionExpected));
    }


    [Test]
    public void NewListTest ()
    {
      var collection = New.List(7, 11, 13, 17);
      var collectionExpected = new List<int> {7, 11, 13, 17 };
      Assert.That (collection, Is.EqualTo (collectionExpected));
    }

    [Test]
    public void NewQueueTest ()
    {
      //var listList = New.List( New.List( New.List(1,2),New.List(3,4) ), New.List( New.List(5,6),New.List(7,8) ));
      //var listList2 = New.List (New.List (1, 2), New.List (3, 4));

      var collection = New.Queue (7, 11, 13, 17);
      //var collectionExpected = new Queue<int> { 7, 11, 13, 17 };
      var collectionExpected = new Queue<int>();
      collectionExpected.Enqueue(7);
      collectionExpected.Enqueue (11);
      collectionExpected.Enqueue (13);
      collectionExpected.Enqueue (17);
      Assert.That (collection, Is.EqualTo (collectionExpected));
    }


    [Test]
    public void NewDictionary1Test ()
    {
      var collection = New.Dictionary ("B", 2);
      var collectionExpected = new Dictionary<string, int> ();
      collectionExpected["B"] = 2;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }

    [Test]
    public void NewDictionary2Test ()
    {
      var collection = New.Dictionary ("B", 2, "D", 4);
      var collectionExpected = new Dictionary<string, int> ();
      collectionExpected["B"] = 2;
      collectionExpected["D"] = 4;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }

    [Test]
    public void NewDictionary3Test ()
    {
      var collection = New.Dictionary ("B", 2, "D", 4, "C", 3);
      var collectionExpected = new Dictionary<string, int> ();
      collectionExpected["B"] = 2;
      collectionExpected["C"] = 3;
      collectionExpected["D"] = 4;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }

    [Test]
    public void NewDictionaryTest4 ()
    {
      var collection = New.Dictionary ("B", 2, "D", 4, "C", 3, "A", 1);
      //var collectionExpected = new Queue<int> { 7, 11, 13, 17 };
      var collectionExpected = new Dictionary<string, int> ();
      collectionExpected["A"] = 1;
      collectionExpected["B"] = 2;
      collectionExpected["C"] = 3;
      collectionExpected["D"] = 4;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }



    [Test]
    public void NewDictionaryKVTest ()
    {
      var collection = New.Dictionary (New.KV ("B", 2), New.KV ("D", 4), New.KV ("C", 3), New.KV ("A", 1), New.KV ("XYZ", 321));
      //var collectionExpected = new Queue<int> { 7, 11, 13, 17 };
      var collectionExpected = new Dictionary<string, int> ();
      collectionExpected["XYZ"] = 321;
      collectionExpected["A"] = 1;
      collectionExpected["B"] = 2;
      collectionExpected["C"] = 3;
      collectionExpected["D"] = 4;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }

    [Test]
    public void NewDictionaryKeyValueTest ()
    {
      var collection = New.Dictionary (New.KeyValue ("B", 2), New.KeyValue ("D", 4), New.KeyValue ("C", 3), New.KeyValue ("A", 1), New.KeyValue ("XYZ", 321));
      //var collectionExpected = new Queue<int> { 7, 11, 13, 17 };
      var collectionExpected = new Dictionary<string, int> ();
      collectionExpected["XYZ"] = 321;
      collectionExpected["A"] = 1;
      collectionExpected["B"] = 2;
      collectionExpected["C"] = 3;
      collectionExpected["D"] = 4;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }
  }
}