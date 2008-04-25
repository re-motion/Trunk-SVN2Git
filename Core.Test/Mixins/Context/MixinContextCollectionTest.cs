using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins.Context;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class MixinContextCollectionTest
  {
    private MixinContextCollection _collection;
    private MixinContext _mcObject;
    private MixinContext _mcString;
    private MixinContext _mcList;
    private MixinContext _mcDerived;

    [SetUp]
    public void SetUp ()
    {
      _mcObject = new MixinContext (typeof (object));
      _mcString = new MixinContext (typeof (string));
      _mcList = new MixinContext (typeof (List<int>));
      _mcDerived = new MixinContext (typeof (DerivedNullMixin));
      _collection = new MixinContextCollection (new MixinContext[] { _mcObject, _mcString, _mcList, _mcDerived});
    }

    [Test]
    public void ContainsAssignableMixin ()
    {
      Assert.IsTrue (_collection.ContainsAssignableMixin (typeof (object)));
      Assert.IsTrue (_collection.ContainsAssignableMixin (typeof (string)));
      Assert.IsTrue (_collection.ContainsAssignableMixin (typeof (ICollection<int>)));
      Assert.IsFalse (_collection.ContainsAssignableMixin (typeof (ICollection<string>)));
      Assert.IsFalse (_collection.ContainsAssignableMixin (typeof (double)));
      Assert.IsFalse (_collection.ContainsAssignableMixin (typeof (MixinContextCollectionTest)));
    }

    [Test]
    public void ContainsOverrideForMixin ()
    {
      Assert.IsTrue (_collection.ContainsOverrideForMixin (typeof (NullMixin)));
      Assert.IsTrue (_collection.ContainsOverrideForMixin (typeof (object)));
      Assert.IsFalse (_collection.ContainsOverrideForMixin (typeof (DerivedDerivedNullMixin)));
      Assert.IsFalse (_collection.ContainsOverrideForMixin (typeof (MixinContextCollectionTest)));
    }
  }
}