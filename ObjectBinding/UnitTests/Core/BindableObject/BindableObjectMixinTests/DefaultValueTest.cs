using System;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class DefaultValueTest
  {
    public class DefaultValueTrueMixin : Mixin<BindableObjectMixin>
    {
      [OverrideTarget]
      public bool IsDefaultValue (PropertyBase property, object nativeValue)
      {
        return true;
      }
    }

    [Test]
    public void GetProperty_NormallyReturnsNonNull ()
    {
      ClassWithValueType<int> instance = ObjectFactory.Create<ClassWithValueType<int>> ().With ();
      IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

      Assert.IsNotNull (instanceAsIBusinessObject.GetProperty ("Scalar"));
      Assert.AreEqual (instance.Scalar, instanceAsIBusinessObject.GetProperty ("Scalar"));
    }

    [Test]
    public void GetProperty_ReturnsNull_WhenDefaultValueTrue ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (BindableObjectMixin)).AddMixins (typeof (DefaultValueTrueMixin)).EnterScope())
      {
        ClassWithValueType<int> instance = ObjectFactory.Create<ClassWithValueType<int>>().With();
        IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

        Assert.IsNull (instanceAsIBusinessObject.GetProperty ("Scalar"));
      }
    }

    [Test]
    public void GetProperty_ReturnsNonNull_WhenDefaultValueTrueOnList ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (BindableObjectMixin)).AddMixins (typeof (DefaultValueTrueMixin)).EnterScope())
      {
        ClassWithValueType<int> instance = ObjectFactory.Create<ClassWithValueType<int>> ().With ();
        IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

        Assert.IsNotNull (instanceAsIBusinessObject.GetProperty ("List"));
        Assert.AreEqual (instance.List, instanceAsIBusinessObject.GetProperty ("List"));
      }
    }
  }
}