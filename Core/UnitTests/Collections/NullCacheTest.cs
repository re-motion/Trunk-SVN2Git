// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
