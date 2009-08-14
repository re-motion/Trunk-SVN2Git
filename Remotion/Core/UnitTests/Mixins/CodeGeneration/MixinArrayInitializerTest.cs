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
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class MixinArrayInitializerTest
  {
    [Test]
    public void CheckMixinArray_MatchingMixins ()
    {
      MixinArrayInitializer initializer = CreateInitializer (typeof (NullTarget), typeof (NullMixin));

      initializer.CheckMixinArray (new object[] { new NullMixin() });
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Invalid mixin instances supplied. Expected the following mixin types "
        + "(in this order): ('Remotion.UnitTests.Mixins.SampleTypes.NullMixin'). The given types were: ('').")]
    public void CheckMixinArray_WrongNumberOfMixins ()
    {
      MixinArrayInitializer initializer = CreateInitializer (typeof (NullTarget), typeof (NullMixin));

      initializer.CheckMixinArray (new object[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Invalid mixin instances supplied. Expected the following mixin types "
        + "(in this order): ('Remotion.UnitTests.Mixins.SampleTypes.NullMixin'). The given types were: "
        + "('Remotion.UnitTests.Mixins.SampleTypes.NullMixin2').")]
    public void CheckMixinArray_NonMatchingMixins ()
    {
      MixinArrayInitializer initializer = CreateInitializer (typeof (NullTarget), typeof (NullMixin));

      initializer.CheckMixinArray (new object[] { new NullMixin2 () });
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Invalid mixin instances supplied. Expected the following mixin types "
        + "(in this order): ('Remotion.UnitTests.Mixins.SampleTypes.NullMixin, Remotion.UnitTests.Mixins.SampleTypes.NullMixin2'). The given types "
        + "were: ('Remotion.UnitTests.Mixins.SampleTypes.NullMixin2, Remotion.UnitTests.Mixins.SampleTypes.NullMixin').")]
    public void CheckMixinArray_NonMatchingMixins_InvalidOrder ()
    {
      MixinArrayInitializer initializer = CreateInitializer (typeof (NullTarget), typeof (NullMixin), typeof (NullMixin2));
      initializer.CheckMixinArray (new object[] { new NullMixin2 (), new NullMixin () });
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = @"Invalid mixin instances supplied\. Expected the following mixin types "
        + @"\(in this order\)\: \('Remotion\.UnitTests\.Mixins\.SampleTypes\.MixinWithVirtualMethod_Mixed_.*'\)\. The given types "
        + @"were\: \('Remotion\.UnitTests\.Mixins\.SampleTypes\.MixinWithVirtualMethod'\)\.", 
        MatchType = MessageMatch.Regex)]
    public void CheckMixinArray_NonMatchingMixins_NeedDerived ()
    {
      Type concreteMixinType = GetGeneratedMixinType (typeof (ClassOverridingSpecificMixinMember), typeof (MixinWithVirtualMethod));

      MixinArrayInitializer initializer = CreateInitializer (typeof (ClassOverridingSpecificMixinMember), concreteMixinType);
      initializer.CheckMixinArray (new object[] { new MixinWithVirtualMethod () });
    }

    [Test]
    public void CheckMixinArray_MatchingMixins_NeedDerived ()
    {
      Type concreteMixinType = GetGeneratedMixinType (typeof (ClassOverridingSpecificMixinMember), typeof (MixinWithVirtualMethod));
      var initializer = CreateInitializer (typeof (ClassOverridingSpecificMixinMember), typeof (MixinWithVirtualMethod));
      var mixin = Activator.CreateInstance (concreteMixinType);

      initializer.CheckMixinArray (new[] { mixin });
    }

    [Test]
    public void GetMixinArray_ForOrdinaryMixin ()
    {
      MixinArrayInitializer initializer = CreateInitializer (typeof (NullTarget), typeof (NullMixin));

      var mixinArray = initializer.CreateMixinArray (null);

      Assert.That (mixinArray.Length, Is.EqualTo (1));
      Assert.That (mixinArray[0], Is.InstanceOfType (typeof (NullMixin)));
    }

    [Test]
    public void GetMixinArray_ForValueTypeMixin ()
    {
      MixinArrayInitializer initializer = CreateInitializer (typeof (BaseType1), typeof (ValueTypeMixin));

      var mixinArray = initializer.CreateMixinArray (null);

      Assert.That (mixinArray.Length, Is.EqualTo (1));
      Assert.That (mixinArray[0], Is.InstanceOfType (typeof (ValueTypeMixin)));
    }

    [Test]
    public void GetMixinArray_ForMixedMixin ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<NullTarget> ().AddMixin<NullMixin> ()
          .ForClass<NullMixin> ().AddMixin<NullMixin2> ().EnterScope ())
      {
        var initializer = CreateInitializer (typeof (NullTarget), typeof (NullMixin));

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
      MixinArrayInitializer initializer = CreateInitializer (typeof (NullTarget), typeof (MixinArrayInitializer));
      initializer.CreateMixinArray (null);
    }

    [Test]
    public void GetMixinArray_ForDerivedMixin ()
    {
      Type concreteMixinType = GetGeneratedMixinType (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      MixinArrayInitializer initializer = CreateInitializer (typeof (ClassOverridingMixinMembers), concreteMixinType);

      var mixinArray = initializer.CreateMixinArray (null);

      Assert.That (mixinArray.Length, Is.EqualTo (1));
      Assert.That (mixinArray[0], Is.InstanceOfType (typeof (MixinWithAbstractMembers)));
      Assert.That (mixinArray[0].GetType(), Is.Not.SameAs (typeof (MixinWithAbstractMembers)));
    }

    [Test]
    public void GetMixinArray_ForSeveralMixins ()
    {
      MixinArrayInitializer initializer = CreateInitializer (typeof (NullTarget), typeof (NullMixin), typeof (NullMixin2));

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

      MixinArrayInitializer initializer = CreateInitializer (typeof (NullTarget), typeof (NullMixin), typeof (NullMixin2), typeof (NullMixin3));

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
      MixinArrayInitializer initializer = CreateInitializer (typeof (NullTarget), typeof (NullMixin));

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
      MixinArrayInitializer initializer = CreateInitializer (typeof (NullTarget), typeof (NullMixin));

      initializer.CreateMixinArray (new object[] { mixin1 });
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Two mixins were supplied that would match the expected mixin type "
        + "'Remotion.UnitTests.Mixins.SampleTypes.NullMixin' on target class 'Remotion.UnitTests.Mixins.SampleTypes.NullTarget'.")]
    public void GetMixinArray_WithTwoFittingSupplied ()
    {
      var mixin1 = new NullMixin ();
      MixinArrayInitializer initializer = CreateInitializer (typeof (NullTarget), typeof (NullMixin));

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
      var concreteMixinType = GetGeneratedMixinType (typeof (ClassOverridingSpecificMixinMember), typeof (MixinWithVirtualMethod));
      MixinArrayInitializer initializer = CreateInitializer (typeof (ClassOverridingSpecificMixinMember), concreteMixinType);

      initializer.CreateMixinArray (new object[] { mixin1 });
    }

    [Test]
    public void GetMixinArray_WithFittingDerivedSuppliedForDerivedMixin ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<ClassOverridingSpecificMixinMember> ().AddMixin<MixinWithVirtualMethod> ().EnterScope ())
      {
        var concreteMixinType = GetGeneratedMixinType (typeof (ClassOverridingSpecificMixinMember), typeof (MixinWithVirtualMethod));
        var initializer = CreateInitializer (typeof (ClassOverridingSpecificMixinMember), concreteMixinType);

        var mixin1 = Activator.CreateInstance (concreteMixinType);

        var mixins = initializer.CreateMixinArray (new[] { mixin1 });
        Assert.That (mixins, Is.EqualTo (new[] { mixin1 }));
      }
    }

    private MixinArrayInitializer CreateInitializer (Type targetType, params Type[] expectedMixinTypes)
    {
      return new MixinArrayInitializer (targetType, expectedMixinTypes);
    }

    private Type GetGeneratedMixinType (Type targetType, Type mixinType)
    {
      var requestingClass = new ClassContext (targetType, mixinType);
      var mixinDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[0];
      return ConcreteTypeBuilder.Current.GetConcreteMixinType (requestingClass, mixinDefinition.GetConcreteMixinTypeIdentifier ()).GeneratedType;
    }
  }
}