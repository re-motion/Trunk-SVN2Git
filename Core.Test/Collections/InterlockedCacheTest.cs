using System;
using NUnit.Framework;
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class InterlockedCacheTest : CacheTest
  {
    protected override ICache<TKey, TValue> CreateCache<TKey, TValue> ()
    {
      return new InterlockedCache<TKey, TValue> ();
    }
  }
}