// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  [Obsolete ("LazyInterlockedCache is marked obsolete because it is still in prototype state.")]
  public class LazyInterlockedCacheTest : CacheTest
  {
    protected override ICache<TKey, TValue> CreateCache<TKey, TValue> ()
    {
      return new LazyInterlockedCache<TKey, TValue> ();
    }

    [Test]
    public void TestCreateAndTryGet ()
    {
      LazyInterlockedCache<string,string> cache = new LazyInterlockedCache<string,string> ();
      string value = cache.GetOrCreateValue ("key", delegate(string key) {
        return "value"; });

      Assert.AreEqual ("value", value);

      bool hasValue = cache.TryGetValue ("key", out value);

      Assert.IsTrue (hasValue);
      Assert.AreEqual ("value", value);
    }

    [Test]
    public void TestCreateTwice()
    {
      LazyInterlockedCache<string,string> cache = new LazyInterlockedCache<string,string> ();
      string value = cache.GetOrCreateValue ("key", delegate(string key) {
        return "value 1"; });

      Assert.AreEqual ("value 1", value);

      value = cache.GetOrCreateValue ("key", delegate(string key) {
        return "value 2"; });

      Assert.AreEqual ("value 1", value);
    }

    [Test]
    public void TestAddAndTryGet()
    {
      LazyInterlockedCache<string,string> cache = new LazyInterlockedCache<string,string> ();
      cache.Add ("key", "value");

      string value;
      bool hasValue = cache.TryGetValue ("key", out value);

      Assert.IsTrue (hasValue);
      Assert.AreEqual ("value", value);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void TestAddTwice()
    {
      LazyInterlockedCache<string,string> cache = new LazyInterlockedCache<string,string> ();
      cache.Add ("key", "value 1");
      cache.Add ("key", "value 2");
    }
  }
}
