// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.Context;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class ReadOnlyContextCollectionTest
  {
    private ReadOnlyContextCollection<string, int> _collection;

    [SetUp]
    public void SetUp ()
    {
      _collection = new ReadOnlyContextCollection<string, int> (
          delegate (int i)
          {
            return i.ToString ();
          }, new int[] { 1, 2, 3 });
    }

    [Test]
    public void NewCollection ()
    {
      Assert.AreEqual (3, _collection.Count);
    }

    [Test]
    public void NewCollection_Duplicates ()
    {
      ReadOnlyContextCollection<string, int> collection = new ReadOnlyContextCollection<string, int> (
          delegate (int i) { return i.ToString(); },
          new int[] {1, 2, 3, 3, 2, 1, 2, 1, 3, 2});

      Assert.That (collection, Is.EquivalentTo (new int[] {1, 2, 3}));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The items 1 and 2 are identified by the same key 1 and cannot both be added "
        + "to the collection.\r\nParameter name: values")]
    public void NewCollection_DuplicateKeys_DifferentValues ()
    {
      new ReadOnlyContextCollection<string, int> (delegate { return "1"; }, new int[] { 1, 2 });
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: values[0]")]
    public void NewCollection_NullValue ()
    {
      new ReadOnlyContextCollection<string, string> ( delegate { return ""; }, new string[] { null });
    }

    [Test]
    public void Contains_Key ()
    {
      Assert.IsTrue (_collection.ContainsKey ("1"));
      Assert.IsTrue (_collection.ContainsKey ("2"));
      Assert.IsTrue (_collection.ContainsKey ("3"));
      Assert.IsFalse (_collection.ContainsKey ("4"));
      Assert.IsFalse (_collection.ContainsKey ("§"));
    }

    [Test]
    public void Contains_Value ()
    {
      ReadOnlyContextCollection<string, int> collection = new ReadOnlyContextCollection<string, int> (
          delegate (int i)
          {
            if (i > 2)
              return ">2";
            else
              return i.ToString ();
          }, new int[] { 1, 2, 3 });

      Assert.IsTrue (collection.Contains (1));
      Assert.IsTrue (collection.Contains (2));
      Assert.IsTrue (collection.Contains (3));
      Assert.IsFalse (collection.Contains (4));
    }

    [Test]
    public void Get ()
    {
      Assert.AreEqual (1, _collection["1"]);
      Assert.AreEqual (2, _collection["2"]);
      Assert.AreEqual (3, _collection["3"]);
      Assert.AreEqual (0, _collection["4"]);
      Assert.AreEqual (0, _collection["soigfusolh"]);
    }

    [Test]
    public void GetEnumerator ()
    {
      List<int> values = new List<int> (_collection);
      Assert.That (values, Is.EqualTo (new int[] {1, 2, 3}));
    }

    [Test]
    public void GetEnumerator_NonGeneric ()
    {
      IEnumerable collectionAsEnumerable = _collection;
      List<object> values = new List<object> (EnumerableUtility.Cast<object> (collectionAsEnumerable));
      Assert.That (values, Is.EqualTo (new int[] { 1, 2, 3 }));
    }

    [Test]
    public void CopyTo ()
    {
      int[] values = new int[5];
      _collection.CopyTo (values, 1);
      Assert.That (values, Is.EqualTo (new int[] {0, 1, 2, 3, 0}));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Add ()
    {
      ((ICollection<int>) _collection).Add (0);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Clear ()
    {
      ((ICollection<int>) _collection).Clear ();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Remove ()
    {
      ((ICollection<int>) _collection).Remove (1);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.IsTrue (((ICollection<int>) _collection).IsReadOnly);
    }

    [Test]
    public void CopyTo_NonGeneric ()
    {
      object[] values = new object[5];
      ((ICollection) _collection).CopyTo (values, 1);
      Assert.That (values, Is.EqualTo (new object[] { null, 1, 2, 3, null }));
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.IsFalse (((ICollection) _collection).IsSynchronized);
    }

    [Test]
    public void SyncRoot ()
    {
      Assert.IsNotNull (((ICollection) _collection).SyncRoot);
    }
  }
}
