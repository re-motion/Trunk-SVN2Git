/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.Context;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class SynchronizedReadOnlyContextCollectionTest
  {
    private SynchronizedReadOnlyContextCollection<string, int> _collection;

    [SetUp]
    public void SetUp ()
    {
      _collection = new SynchronizedReadOnlyContextCollection<string, int> (delegate (int i) { return i.ToString(); }, new int[] { 1, 2, 3 });
    }

    [Test]
    public void NewCollection ()
    {
      Assert.AreEqual (3, _collection.Count);
      ReadOnlyContextCollection<string, int> inner = (ReadOnlyContextCollection<string, int>) PrivateInvoke.GetNonPublicField (_collection, "_internalCollection");
      Assert.That (inner, Is.EqualTo (new int[] { 1, 2, 3 }));
    }

    [Test]
    public void MembersDelegated ()
    {
      MockRepository repository = new MockRepository();
      ReadOnlyContextCollection<string, int> innerMock = repository.StrictMock<ReadOnlyContextCollection<string, int>> (
          (Func<int, string>) delegate { return ""; }, new int[0]);
      PrivateInvoke.SetNonPublicField (_collection, "_internalCollection", innerMock);

      using (IEnumerator<int> enumerator = new List<int>().GetEnumerator())
      {
        int[] array = new int[0];

        using (repository.Ordered())
        {
          Expect.Call (innerMock.Count).Return (1);
          Expect.Call (innerMock.Contains (7)).Return (true);
          Expect.Call (innerMock.ContainsKey ("8")).Return (false);
          Expect.Call (innerMock["8"]).Return (1);
          Expect.Call (innerMock.GetEnumerator()).Return (enumerator);
          innerMock.CopyTo (array, 13);
        }

        repository.ReplayAll();

        Assert.AreEqual (1, _collection.Count);
        Assert.AreEqual (true, _collection.Contains (7));
        Assert.AreEqual (false, _collection.ContainsKey ("8"));
        Assert.AreEqual (1, _collection["8"]);
        Assert.AreEqual (enumerator, _collection.GetEnumerator());
        _collection.CopyTo (array, 13);

        repository.VerifyAll();
      }
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
      Assert.IsTrue (((ICollection) _collection).IsSynchronized);
    }

    [Test]
    public void SyncRoot ()
    {
      Assert.IsNotNull (((ICollection) _collection).SyncRoot);
    }
  }
}
