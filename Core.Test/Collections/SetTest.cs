using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class SetTest
  {
    [Test]
    public void SetInitialization()
    {
      Set<int> set1 = new Set<int>();
      Assert.AreEqual (0, set1.Count);
      Assert.IsFalse (set1.Contains (1));
      Assert.IsFalse (set1.Contains (2));
      Assert.IsFalse (set1.Contains (3));
      Assert.IsFalse (set1.Contains (4));

      Set<int> set2 = new Set<int> (new int[] {1, 2, 3});
      Assert.AreEqual (3, set2.Count);
      Assert.IsTrue (set2.Contains (1));
      Assert.IsTrue (set2.Contains (2));
      Assert.IsTrue (set2.Contains (3));
      Assert.IsFalse (set2.Contains (4));

      Set<string> set3 = new Set<string> (new string[] { "1", "2", "3", "3", "1", "2", "1", "2", "4", "2", "4" });
      Assert.AreEqual (4, set3.Count);
      Assert.IsTrue (set3.Contains ("1"));
      Assert.IsTrue (set3.Contains ("2"));
      Assert.IsTrue (set3.Contains ("3"));
      Assert.IsTrue (set3.Contains ("4"));
      Assert.IsFalse (set3.Contains ("5"));
    }

    [Test]
    public void AddAndAddRangeAndRemove()
    {
      Set<int> set1 = new Set<int> ();
      Assert.AreEqual (0, set1.Count);

      set1.Add (0);
      Assert.AreEqual (1, set1.Count);

      set1.Add (0);
      Assert.AreEqual (1, set1.Count);

      set1.Add (12);
      Assert.AreEqual (2, set1.Count);

      set1.Remove (12);

      set1.AddRange (new int[] {1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 7});
      Assert.AreEqual (8, set1.Count);

      Assert.IsTrue (set1.Contains (0));
      Assert.IsTrue (set1.Contains (1));
      Assert.IsTrue (set1.Contains (2));
      Assert.IsTrue (set1.Contains (3));
      Assert.IsTrue (set1.Contains (4));
      Assert.IsTrue (set1.Contains (5));
      Assert.IsTrue (set1.Contains (6));
      Assert.IsTrue (set1.Contains (7));
      Assert.IsFalse (set1.Contains (8));

      set1.Remove (0);
      Assert.AreEqual (7, set1.Count);
      Assert.IsFalse (set1.Contains (0));
      Assert.IsTrue (set1.Contains (1));
      Assert.IsTrue (set1.Contains (2));
      Assert.IsTrue (set1.Contains (3));
      Assert.IsTrue (set1.Contains (4));
      Assert.IsTrue (set1.Contains (5));
      Assert.IsTrue (set1.Contains (6));
      Assert.IsTrue (set1.Contains (7));
      Assert.IsFalse (set1.Contains (8));

      set1.Remove (6);
      Assert.AreEqual (6, set1.Count);
      Assert.IsFalse (set1.Contains (0));
      Assert.IsTrue (set1.Contains (1));
      Assert.IsTrue (set1.Contains (2));
      Assert.IsTrue (set1.Contains (3));
      Assert.IsTrue (set1.Contains (4));
      Assert.IsTrue (set1.Contains (5));
      Assert.IsFalse (set1.Contains (6));
      Assert.IsTrue (set1.Contains (7));
      Assert.IsFalse (set1.Contains (8));
    }

    [Test]
    public void Clear()
    {
      Set<int> set1 = new Set<int> (new int[] { 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 7 });
      Assert.AreEqual (8, set1.Count);
      set1.Clear();
      Assert.AreEqual (0, set1.Count);
    }

    [Test]
    public void CopyToAndToArray()
    {
      Set<int> set1 = new Set<int> (new int[] { 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 7 });
      int[] array1 = set1.ToArray();
      int[] array2 = new int[set1.Count];
      set1.CopyTo (array2, 0);

      Assert.AreEqual (8, array1.Length);
      Assert.AreEqual (8, array2.Length);
      for (int i = 0; i < array1.Length; ++i)
        Assert.AreEqual (array1[i], array2[i]);

      Assert.Contains (0, array1);
      Assert.Contains (1, array1);
      Assert.Contains (2, array1);
      Assert.Contains (3, array1);
      Assert.Contains (4, array1);
      Assert.Contains (5, array1);
      Assert.Contains (6, array1);
      Assert.Contains (7, array1);

      int[] array3 = new int[] {9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9};
      set1.CopyTo (array3, 2);

      Assert.Contains (0, array3);
      Assert.Contains (1, array3);
      Assert.Contains (2, array3);
      Assert.Contains (3, array3);
      Assert.Contains (4, array3);
      Assert.Contains (5, array3);
      Assert.Contains (6, array3);
      Assert.Contains (7, array3);

      Assert.AreEqual (9, array3[0]);
      Assert.AreEqual (9, array3[1]);
      Assert.AreNotEqual (9, array3[2]);
      Assert.AreNotEqual (9, array3[9]);
      Assert.AreEqual (9, array3[10]);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException), ExpectedMessage = "not long enough", MatchType = MessageMatch.Contains)]
    public void CopyToThrowsOnInvalidArraySize()
    {
      int[] array = new int[1];
      Set<int> set = new Set<int> (new int[] { 1, 2 });

      set.CopyTo (array, 0);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "not long enough", MatchType = MessageMatch.Contains)]
    public void CopyToThrowsOnInvalidIndex1 ()
    {
      int[] array = new int[2];
      Set<int> set = new Set<int> (new int[] { 1, 2 });

      set.CopyTo (array, 1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException), ExpectedMessage = "negative number", MatchType = MessageMatch.Contains)]
    public void CopyToThrowsOnInvalidIndex2 ()
    {
      int[] array = new int[2];
      Set<int> set = new Set<int> (new int[] { 1, 2 });

      set.CopyTo (array, -1);
    }

    [Test]
    public void IsReadOnlyAlwaysFalse()
    {
      ICollection<int> coll = new Set<int>();
      Assert.IsFalse (coll.IsReadOnly);
    }

    [Test]
    public void GetEnumerator()
    {
      Set<string> set1 = new Set<string> (new string[] {"a", "b", "a", "c"});
      List<string> list = new List<string>();
      foreach (string s in set1)
        list.Add (s);
      Assert.AreEqual (3, list.Count);
      Assert.Contains ("a", list);
      Assert.Contains ("b", list);
      Assert.Contains ("c", list);

      list.Clear();

      IEnumerable enumerable = set1;

      foreach (string s in enumerable)
        list.Add (s);
      Assert.AreEqual (3, list.Count);
      Assert.Contains ("a", list);
      Assert.Contains ("b", list);
      Assert.Contains ("c", list);
    }

    public class ToStringEqualityComparer<T> : IEqualityComparer<T>
    {
      public bool Equals (T x, T y)
      {
        if (x == null)
          return y == null;
        else
          return y != null && x.ToString() == y.ToString();
      }

      public int GetHashCode (T obj)
      {
        ArgumentUtility.CheckNotNull ("obj", obj);
        return obj.ToString().GetHashCode();
      }
    }

    [Test]
    public void SpecificComparer1()
    {
      Set<object> set1 = new Set<object> (new ToStringEqualityComparer<object>());
      set1.AddRange (new object[] {1, 2, "a", "1", "2"});

      Assert.AreEqual (3, set1.Count);
      Assert.IsTrue (set1.Contains(1));
      Assert.IsTrue (set1.Contains (2));
      Assert.IsTrue (set1.Contains ("a"));
      Assert.IsTrue (set1.Contains ("1"));
      Assert.IsTrue (set1.Contains ("2"));

      object[] array = set1.ToArray();
      Assert.AreEqual (3, array.Length);
      Assert.Contains (1, array);
      Assert.Contains (2, array);
      Assert.Contains ("a", array);
    }

    [Test]
    public void SpecificComparer2 ()
    {
      Set<object> set1 = new Set<object> (new object[] { 1, 2, "a", "1", "2" }, new ToStringEqualityComparer<object> ());

      Assert.AreEqual (3, set1.Count);
      Assert.IsTrue (set1.Contains (1));
      Assert.IsTrue (set1.Contains (2));
      Assert.IsTrue (set1.Contains ("a"));
      Assert.IsTrue (set1.Contains ("1"));
      Assert.IsTrue (set1.Contains ("2"));

      object[] array = set1.ToArray ();
      Assert.AreEqual (3, array.Length);
      Assert.Contains (1, array);
      Assert.Contains (2, array);
      Assert.Contains ("a", array);
    }

    [Test]
    public void SetIsSerializable()
    {
      Assert.IsTrue (typeof (Set<int>).IsSerializable);

      Set<int> s1 = new Set<int> (new int[] {1, 2, 3, 1, 2});
      Set<int> s2 = Serializer.SerializeAndDeserialize (s1);
      Assert.AreNotSame (s1, s2);
      Assert.AreEqual (s1.Count, s2.Count);

      foreach (int i in s1)
        Assert.IsTrue (s2.Contains (i));

      foreach (int i in s2)
        Assert.IsTrue (s1.Contains (i));
    }

    [Test]
    public void ICollectionCopyTo()
    {
      Set<int> set = new Set<int> (1, 2, 3, 4, 5, 6, 7, 8);
      object[] targetArray = new object[10];
      ((ICollection) set).CopyTo (targetArray, 1);
      Assert.AreEqual (null, targetArray[0]);
      Assert.AreEqual (1, targetArray[1]);
      Assert.AreEqual (2, targetArray[2]);
      Assert.AreEqual (3, targetArray[3]);
      Assert.AreEqual (4, targetArray[4]);
      Assert.AreEqual (5, targetArray[5]);
      Assert.AreEqual (6, targetArray[6]);
      Assert.AreEqual (7, targetArray[7]);
      Assert.AreEqual (8, targetArray[8]);
      Assert.AreEqual (null, targetArray[9]);
    }

    [Test]
    public void ICollectionIsSynchronized()
    {
      Set<int> set = new Set<int> (1, 2, 3, 4, 5, 6, 7, 8);
      Assert.IsFalse (((ICollection) set).IsSynchronized);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ICollectionSyncRootThrows ()
    {
      Set<int> set = new Set<int> (1, 2, 3, 4, 5, 6, 7, 8);
      object root = ((ICollection) set).SyncRoot;
    }

    [Test]
    public void GetAny()
    {
      Set<int> set = new Set<int> (1, 2, 3, 4, 5, 6, 7, 8);
      Assert.IsTrue (set.Contains (set.GetAny()));
      set = new Set<int> (1);
      Assert.AreEqual (1, set.GetAny ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void GetAnyThrowsWhenEmpty()
    {
      Set<int> set = new Set<int> ();
      set.GetAny();
    }
  }
}
