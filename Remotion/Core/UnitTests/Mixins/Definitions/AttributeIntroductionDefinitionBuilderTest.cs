// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class AttributeIntroductionDefinitionBuilderTest
  {
    [Test]
    public void MixinsIntroduceAttributes ()
    {
      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
      Assert.AreEqual (2, bt1.CustomAttributes.Count);
      Assert.IsTrue (bt1.CustomAttributes.ContainsKey (typeof (BT1Attribute)));
      Assert.IsTrue (bt1.CustomAttributes.ContainsKey (typeof (DefaultMemberAttribute)));

      MixinDefinition mixin1 = bt1.Mixins[typeof (BT1Mixin1)];
      Assert.AreEqual (1, mixin1.CustomAttributes.Count);
      Assert.IsTrue (mixin1.CustomAttributes.ContainsKey (typeof (BT1M1Attribute)));
      Assert.AreEqual (1, mixin1.AttributeIntroductions.Count);
      Assert.IsTrue (mixin1.AttributeIntroductions.ContainsKey (typeof (BT1M1Attribute)));

      MixinDefinition mixin2 = bt1.Mixins[typeof (BT1Mixin2)];
      Assert.AreEqual (0, mixin2.CustomAttributes.Count);
      Assert.AreEqual (0, mixin2.AttributeIntroductions.Count);

      Assert.AreEqual (1, bt1.ReceivedAttributes.Count);
      Assert.AreSame (mixin1.CustomAttributes[0], bt1.ReceivedAttributes[0].Attribute);
      Assert.AreSame (mixin1.AttributeIntroductions[0], bt1.ReceivedAttributes[0]);
      Assert.AreSame (mixin1, bt1.ReceivedAttributes[0].Parent);
      Assert.AreEqual (mixin1.CustomAttributes[0].FullName, bt1.ReceivedAttributes[0].FullName);
      Assert.AreEqual (mixin1.CustomAttributes[0].AttributeType, bt1.ReceivedAttributes[0].AttributeType);
    }

    [Test]
    public void MixinsIntroduceAttributesToMembers ()
    {
      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
      MethodDefinition member = bt1.Methods[typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes)];
      Assert.AreEqual (1, member.CustomAttributes.Count);
      Assert.AreEqual (1, member.ReceivedAttributes.Count);

      MixinDefinition mixin = bt1.Mixins[typeof (BT1Mixin1)];
      MethodDefinition mixinMember = mixin.Methods[typeof (BT1Mixin1).GetMethod ("VirtualMethod")];

      Assert.IsTrue (member.ReceivedAttributes.ContainsKey (typeof (BT1M1Attribute)));
      Assert.AreEqual (1, mixinMember.CustomAttributes.Count);
      Assert.AreSame (mixinMember.CustomAttributes[0], member.ReceivedAttributes[0].Attribute);
      Assert.AreSame (mixinMember, member.ReceivedAttributes[0].Parent);
      Assert.AreEqual (mixinMember.CustomAttributes[0].FullName, member.ReceivedAttributes[0].FullName);
      Assert.AreEqual (mixinMember.CustomAttributes[0].AttributeType, member.ReceivedAttributes[0].AttributeType);
    }

    [Test]
    public void MultipleAttributes ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
          typeof (BaseTypeWithAllowMultiple), typeof (MixinAddingAllowMultipleToClassAndMember), typeof (MixinAddingAllowMultipleToClassAndMember2));
      Assert.AreEqual (2, definition.ReceivedAttributes.GetItemCount (typeof (MultiAttribute)));
      Assert.AreEqual (1, definition.CustomAttributes.GetItemCount (typeof (MultiAttribute)));
    }

    [Test]
    public void MultipleAttributesOnMembers ()
    {
      TargetClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseTypeWithAllowMultiple),
              typeof (MixinAddingAllowMultipleToClassAndMember), typeof (MixinAddingAllowMultipleToClassAndMember2));
      MethodDefinition member = bt1.Methods[typeof (BaseTypeWithAllowMultiple).GetMethod ("Foo")];

      Assert.AreEqual (1, member.CustomAttributes.Count);
      Assert.AreEqual (2, member.Overrides.Count);
      Assert.AreEqual (2, member.ReceivedAttributes.Count);

      MixinDefinition mixin1 = bt1.Mixins[typeof (MixinAddingAllowMultipleToClassAndMember)];
      MethodDefinition mixinMember1 = mixin1.Methods[typeof (MixinAddingAllowMultipleToClassAndMember).GetMethod ("Foo")];
      Assert.AreEqual (1, mixinMember1.CustomAttributes.Count);

      MixinDefinition mixin2 = bt1.Mixins[typeof (MixinAddingAllowMultipleToClassAndMember2)];
      MethodDefinition mixinMember2 = mixin2.Methods[typeof (MixinAddingAllowMultipleToClassAndMember).GetMethod ("Foo")];
      Assert.AreEqual (1, mixinMember2.CustomAttributes.Count);

      Assert.IsTrue (member.ReceivedAttributes.ContainsKey (typeof (MultiAttribute)));
      Assert.AreEqual (2, member.ReceivedAttributes.GetItemCount (typeof (MultiAttribute)));

      List<AttributeDefinition> attributes =
          new List<AttributeIntroductionDefinition> (member.ReceivedAttributes[typeof (MultiAttribute)])
              .ConvertAll<AttributeDefinition> (delegate (AttributeIntroductionDefinition intro) { return intro.Attribute; });
      Assert.That (attributes, Is.EquivalentTo (new AttributeDefinition[] { mixinMember1.CustomAttributes[0], mixinMember2.CustomAttributes[0] }));
    }

    [Test]
    public void NonInheritedAttributesAreNotIntroduced ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
          typeof (BaseType1), typeof (MixinAddingNonInheritedAttribute));
      Assert.AreEqual (0, definition.ReceivedAttributes.GetItemCount (typeof (NonInheritedAttribute)));
    }

    [Test]
    public void WithNonMultipleInheritedAttributesTheTargetClassWins ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
          typeof (BaseType1), typeof (MixinAddingBT1Attribute));
      Assert.AreEqual (0, definition.ReceivedAttributes.GetItemCount (typeof (BT1Attribute)));
      Assert.AreEqual (1, definition.CustomAttributes.GetItemCount (typeof (BT1Attribute)));
    }

    [Test]
    public void WithNonMultipleInheritedAttributesOnMemberTheTargetClassWins ()
    {
      MethodDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
          typeof (BaseType1), typeof (MixinAddingBT1AttributeToMember)).Methods[typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes)];
      Assert.AreEqual (0, definition.ReceivedAttributes.GetItemCount (typeof (BT1Attribute)));
      Assert.AreEqual (1, definition.CustomAttributes.GetItemCount (typeof (BT1Attribute)));
    }

    [Test]
    public void ImplicitlyNonIntroducedAttribute ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<BaseType1> ().AddMixins (typeof (MixinAddingBT1Attribute)).EnterScope ())
      {
        TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (BT1Attribute)), Is.False);

        NonAttributeIntroductionDefinition nonIntroductionDefinition =
            definition.Mixins[typeof (MixinAddingBT1Attribute)].NonAttributeIntroductions.GetFirstItem (typeof (BT1Attribute));
        Assert.That (nonIntroductionDefinition.IsExplicitlySuppressed, Is.False);
        Assert.That (nonIntroductionDefinition.IsShadowed, Is.True);
        Assert.That (nonIntroductionDefinition.Attribute, Is.SameAs (definition.Mixins[typeof (MixinAddingBT1Attribute)].CustomAttributes.GetFirstItem (typeof (BT1Attribute))));
        Assert.That (nonIntroductionDefinition.Parent, Is.SameAs (definition.Mixins[typeof (MixinAddingBT1Attribute)]));
      }
    }

    [Test]
    public void ExplicitlyNonIntroducedAttribute ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<NullTarget> ().AddMixins (typeof (MixinNonIntroducingSimpleAttribute)).EnterScope ())
      {
        TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (SimpleAttribute)), Is.False);

        NonAttributeIntroductionDefinition nonIntroductionDefinition =
            definition.Mixins[typeof (MixinNonIntroducingSimpleAttribute)].NonAttributeIntroductions.GetFirstItem (typeof (SimpleAttribute));
        Assert.That (nonIntroductionDefinition.IsExplicitlySuppressed, Is.True);
        Assert.That (nonIntroductionDefinition.IsShadowed, Is.False);
        Assert.That (nonIntroductionDefinition.Attribute, Is.SameAs (definition.Mixins[typeof (MixinNonIntroducingSimpleAttribute)].CustomAttributes.GetFirstItem (typeof (SimpleAttribute))));
        Assert.That (nonIntroductionDefinition.Parent, Is.SameAs (definition.Mixins[typeof (MixinNonIntroducingSimpleAttribute)]));
      }
    }

    [Test]
    public void IndirectAttributeIntroduction_ViaCopy ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<NullTarget> ().Clear ().AddMixins (typeof (MixinIndirectlyAddingAttribute)).EnterScope ())
      {
        TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
        Assert.IsFalse (definition.ReceivedAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)));
        Assert.IsTrue (definition.ReceivedAttributes.ContainsKey (typeof (AttributeWithParameters)));

        List<AttributeIntroductionDefinition> introductions =
            new List<AttributeIntroductionDefinition> (definition.ReceivedAttributes[typeof (AttributeWithParameters)]);
        List<AttributeDefinition> attributes =
            new List<AttributeDefinition> (definition.Mixins[typeof (MixinIndirectlyAddingAttribute)].CustomAttributes[typeof (AttributeWithParameters)]);

        Assert.AreEqual (1, introductions.Count);
        Assert.AreEqual (1, attributes.Count);

        Assert.AreSame (attributes[0], introductions[0].Attribute);
      }
    }

    [Test]
    public void IndirectAttributeIntroduction_OfNonInheritedAttribute_ViaCopy ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<NullTarget> ().Clear ().AddMixins (typeof (MixinIndirectlyAddingNonInheritedAttribute)).EnterScope ())
      {
        TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
        Assert.IsFalse (definition.ReceivedAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)));
        Assert.IsTrue (definition.ReceivedAttributes.ContainsKey (typeof (NonInheritedAttribute)));
      }
    }

    [Test]
    public void IndirectAttributeIntroduction_ViaCopy_OnMember ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<NullTarget> ().Clear ().AddMixins (typeof (MixinIndirectlyAddingAttribute)).EnterScope ())
      {
        MethodDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Methods[typeof (object).GetMethod ("ToString")];
        Assert.IsFalse (definition.ReceivedAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)));
        Assert.IsTrue (definition.ReceivedAttributes.ContainsKey (typeof (AttributeWithParameters)));

        List<AttributeIntroductionDefinition> introductions =
            new List<AttributeIntroductionDefinition> (definition.ReceivedAttributes[typeof (AttributeWithParameters)]);
        List<AttributeDefinition> attributes = new List<AttributeDefinition> (
            definition.Overrides[typeof (MixinIndirectlyAddingAttribute)].DeclaringClass.Methods[typeof (MixinIndirectlyAddingAttribute).GetMethod ("ToString")].CustomAttributes[typeof (AttributeWithParameters)]);

        Assert.AreEqual (1, introductions.Count);
        Assert.AreEqual (1, attributes.Count);

        Assert.AreSame (attributes[0], introductions[0].Attribute);
      }
    }

    [Test]
    public void IntroducedAttribute_SuppressedByTargetClass ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<TargetClassSuppressingBT1Attribute> ().AddMixin<MixinAddingBT1Attribute> ().EnterScope ())
      {
        TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClassSuppressingBT1Attribute));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (BT1Attribute)), Is.False);
        
        Assert.That (definition.Mixins[typeof(MixinAddingBT1Attribute)].SuppressedAttributeIntroductions.ContainsKey (typeof (BT1Attribute)), Is.True);

        SuppressedAttributeIntroductionDefinition[] suppressedAttributes =
            EnumerableUtility.ToArray (definition.Mixins[typeof (MixinAddingBT1Attribute)].SuppressedAttributeIntroductions[typeof (BT1Attribute)]);
        Assert.That (suppressedAttributes.Length, Is.EqualTo (1));
        Assert.That (suppressedAttributes[0].Attribute,
            Is.SameAs (definition.Mixins[typeof (MixinAddingBT1Attribute)].CustomAttributes.GetFirstItem (typeof (BT1Attribute))));
        Assert.That (suppressedAttributes[0].AttributeType, Is.EqualTo (typeof (BT1Attribute)));
        Assert.That (suppressedAttributes[0].FullName, Is.EqualTo (typeof (BT1Attribute).FullName));
        Assert.That (suppressedAttributes[0].Parent, Is.SameAs (definition));
        Assert.That (suppressedAttributes[0].Suppressor, Is.SameAs (definition.CustomAttributes.GetFirstItem (typeof (SuppressAttributesAttribute))));
        Assert.That (suppressedAttributes[0].Target, Is.SameAs (definition));
      }
    }

    [Test]
    public void IntroducedAttribute_NotSuppressedDueToType ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<TargetClassSuppressingBT1Attribute> ().AddMixin<MixinAddingSimpleAttribute> ().EnterScope ())
      {
        TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClassSuppressingBT1Attribute));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (SimpleAttribute)), Is.True);
      }
    }

    [Test]
    public void IntroducedAttribute_SuppressedByMixin ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<NullTarget> ().AddMixin<MixinAddingBT1Attribute> ().AddMixin<MixinSuppressingBT1Attribute> ().EnterScope ())
      {
        TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (BT1Attribute)), Is.False);
        Assert.That (definition.Mixins[typeof (MixinAddingBT1Attribute)].SuppressedAttributeIntroductions.ContainsKey (typeof (BT1Attribute)), Is.True);

        SuppressedAttributeIntroductionDefinition[] suppressedAttributes =
            EnumerableUtility.ToArray (definition.Mixins[typeof (MixinAddingBT1Attribute)].SuppressedAttributeIntroductions[typeof (BT1Attribute)]);
        Assert.That (suppressedAttributes.Length, Is.EqualTo (1));
        Assert.That (suppressedAttributes[0].Attribute,
            Is.SameAs (definition.Mixins[typeof (MixinAddingBT1Attribute)].CustomAttributes.GetFirstItem (typeof (BT1Attribute))));
        Assert.That (suppressedAttributes[0].AttributeType, Is.EqualTo (typeof (BT1Attribute)));
        Assert.That (suppressedAttributes[0].FullName, Is.EqualTo (typeof (BT1Attribute).FullName));
        Assert.That (suppressedAttributes[0].Parent, Is.SameAs (definition));
        Assert.That (suppressedAttributes[0].Suppressor,
            Is.SameAs (definition.Mixins[typeof (MixinSuppressingBT1Attribute)].CustomAttributes.GetFirstItem (typeof (SuppressAttributesAttribute))));
        Assert.That (suppressedAttributes[0].Target, Is.SameAs (definition));
      }
    }
    
    [Test]
    public void IntroducedAttribute_NoSelfSuppress ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<NullTarget> ().AddMixin<MixinSuppressingAndAddingBT1Attribute> ().EnterScope ())
      {
        TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (BT1Attribute)), Is.True);
      }
    }
  }
}
