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
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class NullCacheTest
  {
    private ICache<string, object> _cache;

    [SetUp]
    public void SetUp ()
    {
      _cache = new NullCache<string, object> ();
    }

    [Test]
    public void TryGetValue ()
    {
      object actual;
      Assert.IsFalse (_cache.TryGetValue ("anyKey", out actual));
    }

    [Test]
    public void GetOrCreateValue ()
    {
      object exptected = new object();
      Assert.AreSame (exptected, _cache.GetOrCreateValue ("anyKey", delegate { return exptected; }));
    }

    [Test]
    public void Add_TryGetValue ()
    {
      _cache.GetOrCreateValue ("key1", delegate { return new object(); });
      object actual;
      Assert.IsFalse (_cache.TryGetValue ("key1", out actual));
      Assert.IsNull (actual);
    }

    [Test]
    public void Clear ()
    {
      _cache.Clear ();
      // Succeed
    }

    [Test]
    public void GetIsNull()
    {
      Assert.IsTrue (_cache.IsNull);
    }

    [Test]
    public void Serialization ()
    {
      ICache<string, object> deserializedCache = Serializer.SerializeAndDeserialize (_cache);
      Assert.IsTrue (deserializedCache is NullCache<string, object>);
      Assert.AreNotSame (_cache, deserializedCache);
    }
  }
}
