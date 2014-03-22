using System;
using NUnit.Framework;
using Remotion.Reflection.TypeDiscovery;
using Remotion.UnitTests.Reflection.TypeDiscovery.BaseTypeCacheTestDomain;

namespace Remotion.UnitTests.Reflection.TypeDiscovery
{
  [TestFixture]
  public class BaseTypeCacheTest
  {
    private readonly Type[] _testDomain = new[]
                                 {
                                     typeof (Cat), typeof (Pet), typeof (Dog), typeof (MaineCoon), typeof (Ragdoll), typeof (Siberian),
                                     typeof (ILongHairedBreed), typeof (IHamster)
                                 };

    [Test]
    public void GetAllTypesFromCache ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (baseTypeCache.GetAllTypesFromCache(), Is.EquivalentTo (_testDomain));
    }

    [Test]
    public void GetAllTypesFromCache_ContainsIntefaceWithoutImplementations ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (baseTypeCache.GetAllTypesFromCache(), Contains.Item (typeof (IHamster)));
    }

    [Test]
    public void GetFromCache_object_ReturnsAll ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (
          baseTypeCache.GetFromCache (typeof (object)),
          Is.EquivalentTo (baseTypeCache.GetAllTypesFromCache()));
    }

    [Test]
    public void GetFromCache_SubHierarchy ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (
          baseTypeCache.GetFromCache (typeof (Cat)),
          Is.EquivalentTo (new[] { typeof (Cat), typeof (MaineCoon), typeof (Ragdoll), typeof (Siberian) }));
    }

    [Test]
    public void GetFromCache_WholeHierarchy ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);
      
      Assert.That (
          baseTypeCache.GetFromCache (typeof (Pet)),
          Is.EquivalentTo (new[] { typeof (Cat), typeof (Pet), typeof (Dog), typeof (MaineCoon), typeof (Ragdoll), typeof (Siberian) }));
    }

    [Test]
    public void GetFromCache_NoDescendingTypes ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (baseTypeCache.GetFromCache (typeof (MaineCoon)), Is.EqualTo (new[] { typeof (MaineCoon) }));
    }

    [Test]
    public void GetFromCache_Interface ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (baseTypeCache.GetFromCache (typeof (ILongHairedBreed)), Is.EquivalentTo (new[] { typeof (MaineCoon), typeof (Siberian) }));
    }
  }
}