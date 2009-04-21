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
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class CacheTest
  {
    private ICache<string, object> _cache;

    [SetUp]
    public void SetUp ()
    {
      _cache = CreateCache<string, object> ();
    }

    protected virtual ICache<TKey, TValue> CreateCache<TKey, TValue> ()
    {
      return new Cache<TKey, TValue> ();
    }

    private void Add (string key, object value)
    {
      _cache.GetOrCreateValue (key, delegate { return value; });
    }

    [Test]
    public void TryGet_WithResultNotInCache ()
    {
      object actual;
      Assert.IsFalse (_cache.TryGetValue ("key1", out actual));
      Assert.IsNull (actual);
    }

    [Test]
    public void GetOrCreateValue ()
    {
      object expected = new object ();
      Assert.AreSame (expected, _cache.GetOrCreateValue ("key1", delegate (string key) { return expected; }));
    }

    [Test]
    public void GetOrCreateValue_TryGetValue ()
    {
      object expected = new object ();

      _cache.GetOrCreateValue ("key1", delegate (string key) { return expected; });
      object actual;
      Assert.IsTrue (_cache.TryGetValue ("key1", out actual));
      Assert.AreSame (expected, actual);
    }

    [Test]
    public void Add_GetOrCreateValue ()
    {
      object expected = new object ();

      Add ("key1", expected);
      Assert.AreSame (expected, _cache.GetOrCreateValue ("key1", delegate (string key) { throw new InvalidOperationException ("The valueFactory must not be invoked."); }));
    }

    [Test]
    public void Add_TryGetValue ()
    {
      object expected = new object();

      Add ("key1", expected);
      object actual;
      Assert.IsTrue (_cache.TryGetValue ("key1", out actual));
      Assert.AreSame (expected, actual);
    }

    [Test]
    public void Add_TryGetValue_Clear_TryGetValue ()
    {
      object expected = new object ();

      Add ("key1", expected);
      object actual;
      Assert.IsTrue (_cache.TryGetValue ("key1", out actual));
      Assert.AreSame (expected, actual);
      _cache.Clear ();
      Assert.IsFalse (_cache.TryGetValue ("key1", out actual));
      Assert.IsNull (actual);
    }

    [Test]
    public void Add_Null ()
    {
      Add ("key1", null);
      object actual;
      Assert.IsTrue (_cache.TryGetValue ("key1", out actual));
      Assert.IsNull (actual);
    }

    [Test]
    public void GetIsNull()
    {
      Assert.IsFalse (_cache.IsNull);
    }

    [Test]
    public void SerializeEmptyCache ()
    {
      ICache<string, object> deserializedCache = Serializer.SerializeAndDeserialize (_cache);
      Assert.IsNotNull (deserializedCache);
      
      object result;
      Assert.IsFalse (deserializedCache.TryGetValue ("bla", out result));
      deserializedCache.GetOrCreateValue ("bla", delegate { return "foo"; });
      Assert.IsTrue (deserializedCache.TryGetValue ("bla", out result));
      
      Assert.AreEqual ("foo", result);

      Assert.IsFalse (_cache.TryGetValue ("bla", out result));
    }

    [Test]
    public void SerializeNonEmptyCache ()
    {
      object result;

      _cache.GetOrCreateValue ("bla", delegate { return "foo"; });
      Assert.IsTrue (_cache.TryGetValue ("bla", out result));

      ICache<string, object> deserializedCache = Serializer.SerializeAndDeserialize (_cache);
      Assert.IsNotNull (deserializedCache);

      Assert.IsTrue (deserializedCache.TryGetValue ("bla", out result));
      Assert.AreEqual ("foo", result);

      deserializedCache.GetOrCreateValue ("whatever", delegate { return "fred"; });
      Assert.IsTrue (deserializedCache.TryGetValue ("whatever", out result));
      Assert.IsFalse (_cache.TryGetValue ("whatever", out result));
    }
  }
}
