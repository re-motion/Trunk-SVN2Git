using System;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class MixinReflectionTest
  {
    [Test]
    public void FindMixinInstanceInTarget()
    {
      BaseType3 bt3 = ObjectFactory.Create<BaseType3>().With();
      BT3Mixin2 mixin = Mixin.Get<BT3Mixin2> ((object) bt3);
      Assert.IsNotNull (mixin);
    }

    [Test]
    public void NullIfMixinNotFound()
    {
      BT3Mixin2 mixin = Mixin.Get<BT3Mixin2> (new object());
      Assert.IsNull (mixin);
    }

    [Test]
    public void IMixinTarget()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());

      using (context.EnterScope())
      {
        BaseType1 bt1 = ObjectFactory.Create<BaseType1>().With();
        IMixinTarget mixinTarget = bt1 as IMixinTarget;
        Assert.IsNotNull (mixinTarget);

        TargetClassDefinition configuration = mixinTarget.Configuration;
        Assert.IsNotNull (configuration);

        Assert.AreSame (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)), configuration);

        object[] mixins = mixinTarget.Mixins;
        Assert.IsNotNull (mixins);
        Assert.AreEqual (configuration.Mixins.Count, mixins.Length);
      }
    }

    [Test]
    public void GetInitializationMethod ()
    {
      MixinDefinition m1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.IsNull (MixinReflector.GetInitializationMethod (m1.Type, MixinReflector.InitializationMode.Construction));

      MixinDefinition m2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
      Assert.IsNotNull (MixinReflector.GetInitializationMethod (m2.Type, MixinReflector.InitializationMode.Construction));
      Assert.AreEqual (typeof (Mixin<IBaseType31, IBaseType31>).GetMethod ("Initialize",
          BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly), MixinReflector.GetInitializationMethod (m2.Type, MixinReflector.InitializationMode.Construction));

      MixinDefinition m3 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
      Assert.IsNotNull (MixinReflector.GetInitializationMethod (m3.Type, MixinReflector.InitializationMode.Construction));
      Assert.AreEqual (
          typeof (Mixin<IBaseType32>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetInitializationMethod (m3.Type, MixinReflector.InitializationMode.Construction));

      MixinDefinition m4 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType(typeof (BT3Mixin3<,>));
      Assert.IsNotNull (MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Construction));
      Assert.AreNotEqual (typeof (Mixin<,>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
          MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Construction));
      Assert.AreEqual (m4.Type.BaseType.GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
          MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Construction));

      Assert.AreEqual (typeof (Mixin<BaseType3, IBaseType33>).GetMethod ("Initialize",
          BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly), MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Construction));
    }

    [Test]
    public void GetDeserializationMethod ()
    {
      MixinDefinition m1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.IsNull (MixinReflector.GetInitializationMethod (m1.Type, MixinReflector.InitializationMode.Deserialization));

      MixinDefinition m2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
      Assert.IsNotNull (MixinReflector.GetInitializationMethod (m2.Type, MixinReflector.InitializationMode.Deserialization));
      Assert.AreEqual (typeof (Mixin<IBaseType31, IBaseType31>).GetMethod ("Deserialize",
          BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly), MixinReflector.GetInitializationMethod (m2.Type, MixinReflector.InitializationMode.Deserialization));

      MixinDefinition m3 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
      Assert.IsNotNull (MixinReflector.GetInitializationMethod (m3.Type, MixinReflector.InitializationMode.Deserialization));
      Assert.AreEqual (
          typeof (Mixin<IBaseType32>).GetMethod ("Deserialize", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetInitializationMethod (m3.Type, MixinReflector.InitializationMode.Deserialization));

      MixinDefinition m4 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType (typeof (BT3Mixin3<,>));
      Assert.IsNotNull (MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Deserialization));
      Assert.AreNotEqual (typeof (Mixin<,>).GetMethod ("Deserialize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
          MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Deserialization));
      Assert.AreEqual (m4.Type.BaseType.GetMethod ("Deserialize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
          MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Deserialization));

      Assert.AreEqual (typeof (Mixin<BaseType3, IBaseType33>).GetMethod ("Deserialize",
          BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly), MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Deserialization));
    }

    [Test]
    public void GetTargetProperty ()
    {
      MixinDefinition m1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.IsNull (MixinReflector.GetTargetProperty (m1.Type));

      MixinDefinition m2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
      Assert.IsNotNull (MixinReflector.GetTargetProperty (m2.Type));
      Assert.AreEqual (
          typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetTargetProperty (m2.Type));

      MixinDefinition m3 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
      Assert.IsNotNull (MixinReflector.GetTargetProperty (m3.Type));
      Assert.AreEqual (
          typeof (Mixin<IBaseType32>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetTargetProperty (m3.Type));

      MixinDefinition m4 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType(typeof (BT3Mixin3<,>));
      Assert.IsNotNull (MixinReflector.GetTargetProperty (m4.Type));
      Assert.AreNotEqual (
          typeof (Mixin<,>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetTargetProperty (m4.Type));
      Assert.AreEqual (
          m4.Type.BaseType.GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetTargetProperty (m4.Type));

      Assert.AreEqual (typeof (Mixin<BaseType3, IBaseType33>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetTargetProperty (m4.Type));
    }

    [Test]
    public void GetBaseProperty ()
    {
      MixinDefinition m1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.IsNull (MixinReflector.GetBaseProperty (m1.Type));

      MixinDefinition m2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
      Assert.IsNotNull (MixinReflector.GetBaseProperty (m2.Type));
      Assert.AreEqual (
          typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetBaseProperty (m2.Type));

      MixinDefinition m3 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
      Assert.IsNull (MixinReflector.GetBaseProperty (m3.Type));

      MixinDefinition m4 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType(typeof (BT3Mixin3<,>));
      Assert.IsNotNull (MixinReflector.GetBaseProperty (m4.Type));
      Assert.AreNotEqual (
          typeof (Mixin<,>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetBaseProperty (m4.Type));
      Assert.AreEqual (
          m4.Type.BaseType.GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetBaseProperty (m4.Type));
      Assert.AreEqual (typeof (Mixin<BaseType3, IBaseType33>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetBaseProperty(m4.Type));
    }

    [Test]
    public void GetMixinBaseCallProxyType()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1>().With ();
      Type bcpt = MixinReflector.GetBaseCallProxyType (bt1);
      Assert.IsNotNull (bcpt);
      Assert.AreEqual (bt1.GetType ().GetNestedType ("BaseCallProxy"), bcpt);
    }

    [Test]
    [ExpectedException(typeof (ArgumentException), ExpectedMessage = "not a mixin target", MatchType = MessageMatch.Contains)]
    public void GetMixinBaseCallProxyTypeThrowsIfWrongType1 ()
    {
      MixinReflector.GetBaseCallProxyType (new object());
    }

    [Test]
    public void GetMixinConfiguration_ActiveConfiguration ()
    {
      MixinDefinition expected1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      MixinDefinition expected2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin2)];
      
      BaseType1 bt1 = ObjectFactory.Create<BaseType1>().With();
      BT1Mixin1 mixin1 = Mixin.Get<BT1Mixin1> (bt1);
      BT1Mixin2 mixin2 = Mixin.Get<BT1Mixin2> (bt1);
      Assert.AreSame (expected1, MixinReflector.GetMixinConfiguration (mixin1, bt1));
      Assert.AreSame (expected2, MixinReflector.GetMixinConfiguration (mixin2, bt1));
    }

    [Test]
    public void GetMixinConfiguration_NonActiveConfiguration ()
    {
      MixinDefinition expected1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      MixinDefinition expected2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin2)];

      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      BT1Mixin1 mixin1 = Mixin.Get<BT1Mixin1> (bt1);
      BT1Mixin2 mixin2 = Mixin.Get<BT1Mixin2> (bt1);

      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        Assert.AreSame (expected1, MixinReflector.GetMixinConfiguration (mixin1, bt1));
        Assert.AreSame (expected2, MixinReflector.GetMixinConfiguration (mixin2, bt1));
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        ExpectedMessage = "The given mixin is not a part of the given instance.\r\nParameter name: mixin")]
    public void GetMixinConfiguration_InvalidMixin ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      object mixin = new object();
      MixinReflector.GetMixinConfiguration (mixin, bt1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The given instance is not a mixed object.\r\nParameter name: mixedInstance")]
    public void GetMixinConfiguration_UnmixedInstance ()
    {
      BaseType1 bt1 = new BaseType1();
      object mixin = new object ();
      MixinReflector.GetMixinConfiguration (mixin, bt1);
    }
  }
}