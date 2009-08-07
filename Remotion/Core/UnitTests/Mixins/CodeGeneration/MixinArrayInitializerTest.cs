// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class MixinArrayInitializerTest
  {
    [Test]
    public void GetMixinArray_ForOrdinaryMixin ()
    {
      MixinArrayInitializer initializer = CreateInitializer (
          typeof (NullTarget),
          new MixinArrayInitializer.ExpectedMixinInfo (typeof (NullMixin), false));

      var mixinArray = initializer.CreateMixinArray (null);

      Assert.That (mixinArray.Length, Is.EqualTo (1));
      Assert.That (mixinArray[0], Is.InstanceOfType (typeof (NullMixin)));
    }

    [Test]
    public void GetMixinArray_ForValueTypeMixin ()
    {
      MixinArrayInitializer initializer = CreateInitializer (
          typeof (BaseType1),
          new MixinArrayInitializer.ExpectedMixinInfo (typeof (ValueTypeMixin), false));

      var mixinArray = initializer.CreateMixinArray (null);

      Assert.That (mixinArray.Length, Is.EqualTo (1));
      Assert.That (mixinArray[0], Is.InstanceOfType (typeof (ValueTypeMixin)));
    }

    [Test]
    public void GetMixinArray_ForMixedMixin ()
    {
      var mixinInfos = new[] { new MixinArrayInitializer.ExpectedMixinInfo (typeof (NullMixin), false) };
      using (MixinConfiguration.BuildNew ()
          .ForClass<NullTarget> ().AddMixin<NullMixin> ()
          .ForClass<NullMixin> ().AddMixin<NullMixin2> ().EnterScope ())
      {
        var targetClassDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
        var initializer = new MixinArrayInitializer (typeof (NullTarget), mixinInfos, targetClassDefinition);

        var mixinArray = initializer.CreateMixinArray (null);

        Assert.That (mixinArray.Length, Is.EqualTo (1));
        Assert.That (mixinArray[0], Is.InstanceOfType (typeof (NullMixin)));
        Assert.That (Mixin.Get<NullMixin2> (mixinArray[0]), Is.Not.Null);
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Cannot instantiate mixin "
        + "'Remotion.Mixins.CodeGeneration.MixinArrayInitializer' applied to class 'Remotion.UnitTests.Mixins.SampleTypes.NullTarget', "
        + "there is no visible default constructor.")]
    public void GetMixinArray_MixinCtorNotFound ()
    {
      MixinArrayInitializer initializer = CreateInitializer (
          typeof (NullTarget),
          new MixinArrayInitializer.ExpectedMixinInfo (typeof (MixinArrayInitializer), false));
      initializer.CreateMixinArray (null);
    }

    [Test]
    public void GetMixinArray_ForDerivedMixin ()
    {
      MixinArrayInitializer initializer = CreateInitializer (
          typeof (ClassOverridingMixinMembers),
          new MixinArrayInitializer.ExpectedMixinInfo (typeof (MixinWithAbstractMembers), true));

      var mixinArray = initializer.CreateMixinArray (null);

      Assert.That (mixinArray.Length, Is.EqualTo (1));
      Assert.That (mixinArray[0], Is.InstanceOfType (typeof (MixinWithAbstractMembers)));
      Assert.That (mixinArray[0].GetType(), Is.Not.SameAs (typeof (MixinWithAbstractMembers)));
    }

    [Test]
    public void GetMixinArray_ForSeveralMixins ()
    {
      MixinArrayInitializer initializer = CreateInitializer (
          typeof (NullTarget),
          new MixinArrayInitializer.ExpectedMixinInfo (typeof (NullMixin), false),
          new MixinArrayInitializer.ExpectedMixinInfo (typeof (NullMixin2), false));

      var mixinArray = initializer.CreateMixinArray (null);

      Assert.That (mixinArray.Length, Is.EqualTo (2));
      Assert.That (mixinArray[0], Is.InstanceOfType (typeof (NullMixin)));
      Assert.That (mixinArray[1], Is.InstanceOfType (typeof (NullMixin2)));
    }

    [Test]
    public void GetMixinArray_WithSuppliedMixins ()
    {
      var mixin1 = new NullMixin();
      var mixin3 = new NullMixin3();

      MixinArrayInitializer initializer = CreateInitializer (
          typeof (NullTarget),
          new MixinArrayInitializer.ExpectedMixinInfo (typeof (NullMixin), false),
          new MixinArrayInitializer.ExpectedMixinInfo (typeof (NullMixin2), false),
          new MixinArrayInitializer.ExpectedMixinInfo (typeof (NullMixin3), false));

      var mixinArray = initializer.CreateMixinArray(new object[] { mixin1, mixin3 } );

      Assert.That (mixinArray.Length, Is.EqualTo (3));
      Assert.That (mixinArray[0], Is.SameAs (mixin1));
      Assert.That (mixinArray[1], Is.InstanceOfType (typeof (NullMixin2)));
      Assert.That (mixinArray[2], Is.SameAs (mixin3));
    }

    [Test]
    public void GetMixinArray_WithSuppliedAssignableMixin ()
    {
      var mixin1 = new DerivedNullMixin();
      MixinArrayInitializer initializer = CreateInitializer (
          typeof (NullTarget),
          new MixinArrayInitializer.ExpectedMixinInfo (typeof (NullMixin), false));

      var mixinArray = initializer.CreateMixinArray(new object[] { mixin1 } );

      Assert.That (mixinArray.Length, Is.EqualTo (1));
      Assert.That (mixinArray[0], Is.SameAs (mixin1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The supplied mixin of type "
        + "'Remotion.UnitTests.Mixins.SampleTypes.NullMixin2' is not valid for target type 'Remotion.UnitTests.Mixins.SampleTypes.NullTarget' in the "
        + "current configuration.")]
    public void GetMixinArray_WithNonFittingSupplied ()
    {
      var mixin1 = new NullMixin2();
      MixinArrayInitializer initializer = CreateInitializer (
          typeof (NullTarget),
          new MixinArrayInitializer.ExpectedMixinInfo (typeof (NullMixin), false));

      initializer.CreateMixinArray (new object[] { mixin1 });
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Two mixins were supplied that would match the expected mixin type "
        + "'Remotion.UnitTests.Mixins.SampleTypes.NullMixin' on target class 'Remotion.UnitTests.Mixins.SampleTypes.NullTarget'.")]
    public void GetMixinArray_WithTwoFittingSupplied ()
    {
      var mixin1 = new NullMixin ();
      MixinArrayInitializer initializer = CreateInitializer (
          typeof (NullTarget),
          new MixinArrayInitializer.ExpectedMixinInfo (typeof (NullMixin), false));

      initializer.CreateMixinArray (new object[] { mixin1, mixin1 });
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A mixin was supplied that would match the expected mixin type "
        + "'Remotion.UnitTests.Mixins.SampleTypes.MixinWithVirtualMethod' on target class "
        + "'Remotion.UnitTests.Mixins.SampleTypes.ClassOverridingSpecificMixinMember'. However, a derived "
        + "type must be generated for that mixin type, so the supplied instance cannot be used.")]
    public void GetMixinArray_WithFittingSuppliedForDerivedMixin ()
    {
      var mixin1 = new MixinWithVirtualMethod ();
      MixinArrayInitializer initializer = CreateInitializer (
          typeof (ClassOverridingSpecificMixinMember),
          new MixinArrayInitializer.ExpectedMixinInfo (typeof (MixinWithVirtualMethod), true));

      initializer.CreateMixinArray (new object[] { mixin1 });
    }

    [Test]
    public void GetMixinArray_WithFittingDerivedSuppliedForDerivedMixin ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<ClassOverridingSpecificMixinMember> ().AddMixin<MixinWithVirtualMethod> ().EnterScope ())
      {
        var mixinInfos = new[] { new MixinArrayInitializer.ExpectedMixinInfo (typeof (MixinWithVirtualMethod), true) };

        var targetClassDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingSpecificMixinMember));
        var initializer = new MixinArrayInitializer (typeof (ClassOverridingSpecificMixinMember), mixinInfos, targetClassDefinition);

        var concreteMixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (targetClassDefinition.Mixins[0]).GeneratedType;
        var mixin1 = Activator.CreateInstance (concreteMixinType);

        var mixins = initializer.CreateMixinArray (new[] { mixin1 });
        Assert.That (mixins, Is.EqualTo (new[] { mixin1 }));
      }
    }

    private MixinArrayInitializer CreateInitializer (
        Type targetType, params MixinArrayInitializer.ExpectedMixinInfo[] mixinInfos)
    {
      var mixinConfigurationBuilder = MixinConfiguration.BuildNew();
      foreach (var mixinInfo in mixinInfos)
        mixinConfigurationBuilder.ForClass (targetType).AddMixin (mixinInfo.ExpectedMixinType);

      var targetClassDefinition = TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfigurationBuilder.BuildConfiguration());
      return new MixinArrayInitializer (targetType, mixinInfos, targetClassDefinition);
    }
  }
}