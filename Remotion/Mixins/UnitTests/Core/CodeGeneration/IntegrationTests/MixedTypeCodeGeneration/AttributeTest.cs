// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class AttributeTest : CodeGenerationBaseTest
  {
    [Test]
    public void AttributesReplicatedFromMixinViaIntroduction()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinWithPropsEventAtts));

      Assert.IsFalse (bt1.GetType().IsDefined (typeof (BT1Attribute), false));
      Assert.IsTrue (bt1.GetType().IsDefined (typeof (BT1Attribute), true), "Attribute is inherited");
      Assert.IsTrue (bt1.GetType().IsDefined (typeof (ReplicatableAttribute), false));

      var atts = (ReplicatableAttribute[]) bt1.GetType().GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual (1, atts.Length);
      Assert.AreEqual (4, atts[0].I);

      PropertyInfo property = bt1.GetType().GetProperty (typeof (IMixinWithPropsEventsAtts).FullName + ".Property",
                                                         BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (property);
      atts = (ReplicatableAttribute[]) property.GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual (1, atts.Length);
      Assert.AreEqual ("bla", atts[0].S);
      Assert.IsTrue (property.GetGetMethod (true).IsSpecialName);
      atts = (ReplicatableAttribute[]) property.GetGetMethod (true).GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual (1.0, atts[0].Named2);

      Assert.IsTrue (property.GetSetMethod (true).IsSpecialName);
      atts = (ReplicatableAttribute[]) property.GetSetMethod (true).GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual (2.0, atts[0].Named2);

      EventInfo eventInfo = bt1.GetType().GetEvent (typeof (IMixinWithPropsEventsAtts).FullName + ".Event",
                                                    BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (eventInfo);
      atts = (ReplicatableAttribute[]) eventInfo.GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual (1, atts.Length);
      Assert.AreEqual ("blo", atts[0].S);
      Assert.IsTrue (eventInfo.GetAddMethod (true).IsSpecialName);
      Assert.IsTrue (eventInfo.GetAddMethod (true).IsDefined (typeof (ReplicatableAttribute), false));
      Assert.IsTrue (eventInfo.GetRemoveMethod (true).IsSpecialName);
      Assert.IsTrue (eventInfo.GetRemoveMethod (true).IsDefined (typeof (ReplicatableAttribute), false));
    }

    [Test]
    public void IntroducedAttributes()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.AreEqual (1, concreteType.GetCustomAttributes (typeof (BT1Attribute), true).Length);
      Assert.AreEqual (1, concreteType.GetCustomAttributes (typeof (BT1M1Attribute), true).Length);

      MethodInfo bt1VirtualMethod = concreteType.GetMethod ("VirtualMethod", Type.EmptyTypes);
      Assert.AreEqual (1, bt1VirtualMethod.GetCustomAttributes (typeof (BT1M1Attribute), true).Length);

      PropertyInfo bt1VirtualProperty = concreteType.GetProperty ("VirtualProperty",
                                                                  BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.AreEqual (1, bt1VirtualProperty.GetCustomAttributes (typeof (BT1M1Attribute), true).Length);

      EventInfo bt1VirtualEvent = concreteType.GetEvent ("VirtualEvent");
      Assert.AreEqual (1, bt1VirtualEvent.GetCustomAttributes (typeof (BT1M1Attribute), true).Length);
    }

    [Test]
    public void IntroducedMultiAttributes()
    {
      Type concreteType = CreateMixedType (
          typeof (BaseTypeWithAllowMultiple),
          typeof (MixinAddingAllowMultipleToClassAndMember),
          typeof (MixinAddingAllowMultipleToClassAndMember2));

      Assert.AreEqual (4, concreteType.GetCustomAttributes (typeof (MultiAttribute), true).Length);
      Assert.AreEqual (3, concreteType.GetMethod ("Foo").GetCustomAttributes (typeof (MultiAttribute), true).Length);
    }

    [Test]
    public void IntroducedAttributesTargetClassWins()
    {
      Type concreteType = CreateMixedType (typeof (BaseType1), typeof (MixinAddingBT1Attribute));
      Assert.AreEqual (1, concreteType.GetCustomAttributes (typeof (BT1Attribute), true).Length);

      concreteType = CreateMixedType (typeof (BaseType1), typeof (MixinAddingBT1AttributeToMember));
      Assert.AreEqual (1, concreteType.GetMethod ("VirtualMethod", Type.EmptyTypes).GetCustomAttributes (typeof (BT1Attribute), true).Length);
    }

    private object[] GetRelevantAttributes (ICustomAttributeProvider source)
    {
      object[] attributes = source.GetCustomAttributes (true);
      return Array.FindAll (attributes,
                            o =>
                            o is MultiInheritedAttribute || o is MultiNonInheritedAttribute || o is NonMultiInheritedAttribute
                            || o is NonMultiNonInheritedAttribute);
    }

    [Test]
    public void AttributesOnMixedTypesBehaveLikeOnDerivedTypes()
    {
      object[] attributes = GetRelevantAttributes (CreateMixedType (typeof (TargetWithoutAttributes), typeof (MixinWithAttributes)));
      Assert.AreEqual (2, attributes.Length);
      Assert.That (
          attributes, Is.EquivalentTo (new object[] {new MultiInheritedAttribute(), new NonMultiInheritedAttribute()}));

      attributes = GetRelevantAttributes (CreateMixedType (typeof (TargetWithAttributes), typeof (MixinWithAttributes)));
      Assert.AreEqual (5, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[]
                                                  {
                                                      new MultiInheritedAttribute(), new MultiInheritedAttribute(),
                                                      new NonMultiNonInheritedAttribute(), new MultiNonInheritedAttribute(),
                                                      new NonMultiInheritedAttribute()
                                                  }));
    }

    [Test]
    public void AttributesSuppressedByMixin_AreNotReplicatedFromBaseType()
    {
      object[] attributes = GetRelevantAttributes (CreateMixedType (typeof (TargetWithNonInheritedAttributes), typeof (MixinSuppressingAllAttributes)));
      Assert.AreEqual (0, attributes.Length);
    }

    [Test]
    public void AttributesSuppressedByMixin_AreNotIntroducedFromOtherMixin()
    {
      object[] attributes =
          GetRelevantAttributes (CreateMixedType (typeof (NullTarget), typeof (MixinSuppressingAllAttributes), typeof (MixinAddingAttributes)));
      Assert.AreEqual (0, attributes.Length);
    }

    [Test]
    public void AttributesSuppressedByMixin_AreIntroducedForSameMixin()
    {
      object[] attributes = GetRelevantAttributes (CreateMixedType (typeof (NullTarget), typeof (MixinSuppressingAllAttributesAddingAttributes)));
      Assert.AreEqual (2, attributes.Length);
      Assert.That (attributes, Is.EquivalentTo (new object[] {new MultiInheritedAttribute(), new NonMultiInheritedAttribute()}));
    }

    [Test]
    public void AttributesOnDerivedMethodsBehaveLikeOnDerivedTypes()
    {
      object[] attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithoutAttributes), typeof (MixinWithAttributes)).GetMethod ("Method"));
      Assert.AreEqual (2, attributes.Length);
      Assert.That (
          attributes, Is.EquivalentTo (new object[] {new MultiInheritedAttribute(), new NonMultiInheritedAttribute()}));

      attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithAttributes), typeof (MixinWithAttributes)).GetMethod ("Method"));
      Assert.AreEqual (5, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[]
                                                  {
                                                      new MultiInheritedAttribute(), new MultiInheritedAttribute(),
                                                      new NonMultiNonInheritedAttribute(), new MultiNonInheritedAttribute(),
                                                      new NonMultiInheritedAttribute()
                                                  }));
    }

    [Test]
    [Ignore ("TODO: This does not work on the build server, check why.")]
    public void AttributesOnDerivedPropertiesBehaveLikeMethods ()
    {
      object[] attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithoutAttributes), typeof (MixinWithAttributes)).GetProperty ("Property"));
      Assert.AreEqual (2, attributes.Length);
      Assert.That (
          attributes, Is.EquivalentTo (new object[] {new MultiInheritedAttribute(), new NonMultiInheritedAttribute()}));

      attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithAttributes), typeof (MixinWithAttributes)).GetProperty ("Property"));
      Assert.AreEqual (5, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[]
                                                  {
                                                      new MultiInheritedAttribute(), new MultiInheritedAttribute(),
                                                      new NonMultiNonInheritedAttribute(), new MultiNonInheritedAttribute(),
                                                      new NonMultiInheritedAttribute()
                                                  }));
    }

    [Test]
    public void AttributesOnDerivedPropertiesBehaveLikeMethodsTemp()
    {
      const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
      object[] attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithoutAttributes), typeof (MixinWithAttributes)).GetProperty ("Property",
                                                                                                                               bindingFlags));
      Assert.AreEqual (2, attributes.Length);
      Assert.That (attributes, Is.EquivalentTo (new object[] {new MultiInheritedAttribute(), new NonMultiInheritedAttribute()}));

      attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithAttributes), typeof (MixinWithAttributes)).GetProperty ("Property", bindingFlags));
      Assert.AreEqual (5, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[]
                                                  {
                                                      new MultiInheritedAttribute(), new MultiInheritedAttribute(),
                                                      new NonMultiNonInheritedAttribute(), new MultiNonInheritedAttribute(),
                                                      new NonMultiInheritedAttribute()
                                                  }));
    }

    [Test]
    public void AttributesOnDerivedEventsBehaveLikeMethods()
    {
      object[] attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithoutAttributes), typeof (MixinWithAttributes)).GetEvent ("Event"));
      Assert.AreEqual (2, attributes.Length);
      Assert.That (
          attributes, Is.EquivalentTo (new object[] {new MultiInheritedAttribute(), new NonMultiInheritedAttribute()}));

      attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithAttributes), typeof (MixinWithAttributes)).GetEvent ("Event"));
      Assert.AreEqual (5, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[]
                                                  {
                                                      new MultiInheritedAttribute(), new MultiInheritedAttribute(),
                                                      new NonMultiNonInheritedAttribute(), new MultiNonInheritedAttribute(),
                                                      new NonMultiInheritedAttribute()
                                                  }));
    }
  }
}
