﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class InvalidationTokenBasedCacheDecoratorTest
  {
    [Test]
    public void GetOrCreateValue_WithValueInCache_ReturnsValue ()
    {
      var cache = new Cache<object, string>();
      var decorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());
      var key = new object();
      cache.Add (key, "Value");

      var value = decorator.GetOrCreateValue (key, o => { throw new InvalidOperationException(); });

      Assert.That (value, Is.EqualTo ("Value"));
    }

    [Test]
    public void GetOrCreateValue_WithValueNotInCache_CreatedValue_ReturnsValue ()
    {
      var cache = new Cache<object, string>();
      var decorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());
      var key = new object();

      var value = decorator.GetOrCreateValue (key, o => "Value");

      Assert.That (value, Is.EqualTo ("Value"));

      string cachedValue;
      cache.TryGetValue (key, out cachedValue);
      Assert.That (cachedValue, Is.EqualTo ("Value"));
    }

    [Test]
    public void GetOrCreateValue_AfterTokenWasInvalidated_CreatesNewValue_ReturnsValue ()
    {
      var cache = new Cache<object, string>();
      var decorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());
      var key = new object();
      cache.Add (key, "Value1");

      decorator.CacheInvalidationToken.Invalidate();

      var value = decorator.GetOrCreateValue (key, o => "Value2");

      Assert.That (value, Is.EqualTo ("Value2"));

      string cachedValue;
      cache.TryGetValue (key, out cachedValue);
      Assert.That (cachedValue, Is.EqualTo ("Value2"));
    }

    [Test]
    public void GetOrCreateValue_AfterTokenWasInvalidated_RefreshesRevision_ReturnsValue ()
    {
      var cache = new Cache<object, string>();
      var decorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());
      var key = new object();
      cache.Add (key, "Value1");

      decorator.CacheInvalidationToken.Invalidate();

      var valueOnFirstCall = decorator.GetOrCreateValue (key, o => "Value2");
      Assert.That (valueOnFirstCall, Is.EqualTo ("Value2"));

      var valueOnSecondCall = decorator.GetOrCreateValue (key, o => { throw new InvalidOperationException(); });
      Assert.That (valueOnSecondCall, Is.EqualTo ("Value2"));
    }

    [Test]
    public void TryGetValue_WithValueInCache_ReturnsTrueAndSetsOutValue ()
    {
      var cache = new Cache<object, string>();
      var decorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());
      var key = new object();
      cache.Add (key, "Value");

      string value;
      var result = decorator.TryGetValue (key, out value);

      Assert.That (result, Is.True);
      Assert.That (value, Is.EqualTo ("Value"));
    }

    [Test]
    public void TryGetValue_WithValueNotInCache_ReturnsFalse ()
    {
      var cache = new Cache<object, string>();
      var decorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());
      var key = new object();

      string value;
      var result = decorator.TryGetValue (key, out value);

      Assert.That (result, Is.False);
      Assert.That (value, Is.Null);
    }

    [Test]
    public void TryGetValue_AfterTokenWasInvalidated_ReturnsFalse ()
    {
      var cache = new Cache<object, string>();
      var decorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());
      var key = new object();
      cache.Add (key, "Value");

      decorator.CacheInvalidationToken.Invalidate();

      string value;
      var result = decorator.TryGetValue (key, out value);

      Assert.That (result, Is.False);
      Assert.That (value, Is.Null);

      string cachedValue;
      bool cachedResult = cache.TryGetValue (key, out cachedValue);
      Assert.That (cachedResult, Is.False);
    }

    [Test]
    public void TryGetValue_AfterTokenWasInvalidated_RefreshesRevision_ReturnsFalse ()
    {
      var cache = new Cache<object, string>();
      var decorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());
      var key = new object();

      decorator.CacheInvalidationToken.Invalidate();

      cache.Add (key, "Value");
      string valueOnFirstCall;
      var resultOnFirstCall = decorator.TryGetValue (key, out valueOnFirstCall);
      Assert.That (resultOnFirstCall, Is.False);
      Assert.That (valueOnFirstCall, Is.Null);

      cache.Add (key, "Value2");
      string valueOnSecondCall;
      var resultOnSecondCall = decorator.TryGetValue (key, out valueOnSecondCall);
      Assert.That (resultOnSecondCall, Is.True);
      Assert.That (valueOnSecondCall, Is.EqualTo ("Value2"));
    }

    [Test]
    public void Clear_ClearsInnerCache ()
    {
      var cache = new Cache<object, string>();
      var decorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());
      var key = new object();
      cache.Add (key, "Value");

      ((ICache<object, string>) decorator).Clear();

      string cachedValue;
      bool cachedResult = cache.TryGetValue (key, out cachedValue);
      Assert.That (cachedResult, Is.False);
    }

    [Test]
    public void Clear_DoesNotInvalidateToken ()
    {
      var cache = new Cache<object, string>();
      var decorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());

      var revision = decorator.CacheInvalidationToken.GetCurrent();

      ((ICache<object, string>) decorator).Clear();

      Assert.That (decorator.CacheInvalidationToken.IsCurrent (revision), Is.True);
    }

    [Test]
    public void Clear_RefreshesRevision ()
    {
      var cache = new Cache<object, string>();
      var decorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());

      decorator.CacheInvalidationToken.Invalidate();
      ((ICache<object, string>) decorator).Clear();
      var key = new object();
      cache.Add (key, "Value");

      string value;
      var result = decorator.TryGetValue (key, out value);

      Assert.That (result, Is.True);
      Assert.That (value, Is.EqualTo ("Value"));
    }

    [Test]
    public void IsNull_WithNonNullCache_ReturnsFalse ()
    {
      var cache = new Cache<object, string>();
      var decorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());

      Assert.That (((ICache<object, string>) decorator).IsNull, Is.False);
    }

    [Test]
    public void IsNull_WithNullCache_ReturnsTrue ()
    {
      var cache = new NullCache<object, string>();
      var decorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());

      Assert.That (((ICache<object, string>) decorator).IsNull, Is.True);
    }

    [Test]
    public void Serializable ()
    {
      var cache = new Cache<object, string>();
      var invalidationTokenBasedCacheDecorator = new InvalidationTokenBasedCacheDecorator<object, string> (cache, CacheInvalidationToken.Create());

      var deserializedInstance = Serializer.SerializeAndDeserialize (invalidationTokenBasedCacheDecorator);

      Assert.That (deserializedInstance, Is.Not.SameAs (invalidationTokenBasedCacheDecorator));
      Assert.That (deserializedInstance.CacheInvalidationToken, Is.Not.SameAs (invalidationTokenBasedCacheDecorator.CacheInvalidationToken));
    }
  }
}