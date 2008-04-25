using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.CodeGeneration.SampleTypes;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.CodeGeneration.MixedTypeCodeGeneration
{
  [TestFixture]
  public class GenericsTest : CodeGenerationBaseTest
  {
    [Test]
    public void GenericMixinsAreSpecialized ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin3<,>)).With ();
      object mixin = Mixin.Get (typeof (BT3Mixin3<,>), bt3);
      Assert.IsNotNull (mixin);

      PropertyInfo thisProperty = mixin.GetType ().BaseType.GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (thisProperty);

      Assert.IsNotNull (thisProperty.GetValue (mixin, null));
      Assert.AreSame (bt3, thisProperty.GetValue (mixin, null));
      Assert.AreEqual (typeof (BaseType3), thisProperty.PropertyType);

      PropertyInfo baseProperty = mixin.GetType ().BaseType.GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (baseProperty);

      Assert.IsNotNull (baseProperty.GetValue (mixin, null));
      Assert.AreSame (bt3.GetType ().GetField ("__first").FieldType, baseProperty.GetValue (mixin, null).GetType ());
      Assert.AreEqual (typeof (IBaseType33), baseProperty.PropertyType);
    }

    [Test]
    public void MuchGenericityWithoutOverriding ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (VeryGenericMixin<,>), typeof (BT3Mixin4)).With ();
      IVeryGenericMixin m = bt3 as IVeryGenericMixin;
      Assert.IsNotNull (m);
      Assert.AreEqual ("IVeryGenericMixin.GenericIfcMethod-5", m.GetMessage ("5"));
    }

    [Test]
    public void MuchGenericityWithOverriding ()
    {
      ClassOverridingUltraGenericStuff cougs = CreateMixedObject<ClassOverridingUltraGenericStuff> (typeof (AbstractDerivedUltraGenericMixin<,>),
          typeof (BT3Mixin4)).With ();
      IUltraGenericMixin m = cougs as IUltraGenericMixin;
      Assert.IsNotNull (m);
      Assert.AreEqual ("String-IVeryGenericMixin.GenericIfcMethod-5", m.GetMessage ("5"));
    }

  }
}
