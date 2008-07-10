/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.UnitTests.Mixins.CodeGeneration.SampleTypes;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class AttributeTest : CodeGenerationBaseTest
  {
    public interface IMixinWithPropsEventsAtts
    {
      int Property { get; set; }
      event EventHandler Event;
    }

    public class ReplicatableAttribute : Attribute
    {
      private readonly int _i;
      private readonly string _s;
      private double _named;

      public ReplicatableAttribute (int i)
      {
        _i = i;
      }

      public ReplicatableAttribute (string s)
      {
        _s = s;
      }

      public int I
      {
        get { return _i; }
      }

      public string S
      {
        get { return _s; }
      }

      public double Named2
      {
        get
        {
          return _named;
        }
        set
        {
          _named = value;
        }
      }
    }

    [Replicatable (4)]
    public class MixinWithPropsEventAtts : IMixinWithPropsEventsAtts
    {
      private int _property;

      [Replicatable ("bla")]
      public int Property
      {
        [Replicatable (5, Named2 = 1.0)]
        get { return _property; }
        [Replicatable (5, Named2 = 2.0)]
        set { _property = value; }
      }

      [Replicatable ("blo")]
      public event EventHandler Event
      {
        [Replicatable (1)]
        add { }
        [Replicatable (2)]
        remove { }
      }
    }

    [Test]
    public void AttributesReplicatedFromMixinViaIntroduction ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinWithPropsEventAtts)).With ();

      Assert.IsFalse (bt1.GetType ().IsDefined (typeof (BT1Attribute), false));
      Assert.IsTrue (bt1.GetType ().IsDefined (typeof (BT1Attribute), true), "Attribute is inherited");
      Assert.IsTrue (bt1.GetType ().IsDefined (typeof (ReplicatableAttribute), false));

      ReplicatableAttribute[] atts = (ReplicatableAttribute[]) bt1.GetType ().GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual (1, atts.Length);
      Assert.AreEqual (4, atts[0].I);

      PropertyInfo property = bt1.GetType ().GetProperty (typeof (IMixinWithPropsEventsAtts).FullName + ".Property",
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

      EventInfo eventInfo = bt1.GetType ().GetEvent (typeof (IMixinWithPropsEventsAtts).FullName + ".Event",
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
    public void IntroducedAttributes ()
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
    public void IntroducedMultiAttributes ()
    {
      Type concreteType = CreateMixedType (
          typeof (BaseTypeWithAllowMultiple),
          typeof (MixinAddingAllowMultipleToClassAndMember),
          typeof (MixinAddingAllowMultipleToClassAndMember2));

      Assert.AreEqual (3, concreteType.GetCustomAttributes (typeof (MultiAttribute), true).Length);
      Assert.AreEqual (3, concreteType.GetMethod ("Foo").GetCustomAttributes (typeof (MultiAttribute), true).Length);
    }

    [Test]
    public void IntroducedAttributesTargetClassWins ()
    {
      Type concreteType = CreateMixedType (typeof (BaseType1), typeof (MixinAddingBT1Attribute));
      Assert.AreEqual (1, concreteType.GetCustomAttributes (typeof (BT1Attribute), true).Length);

      concreteType = CreateMixedType (typeof (BaseType1), typeof (MixinAddingBT1AttributeToMember));
      Assert.AreEqual (1, concreteType.GetMethod ("VirtualMethod", Type.EmptyTypes).GetCustomAttributes (typeof (BT1Attribute), true).Length);
    }

    [AttributeUsage (AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class MultiInheritedAttribute : Attribute { }

    [AttributeUsage (AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class MultiNonInheritedAttribute : Attribute { }

    [AttributeUsage (AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class NonMultiInheritedAttribute : Attribute { }

    [AttributeUsage (AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class NonMultiNonInheritedAttribute : Attribute { }

    [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
    public class MixinWithAttributes
    {
      [OverrideTarget]
      [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
      public virtual void Method ()
      {
      }

      [OverrideTarget]
      [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
      public virtual int Property
      {
        get { return 0; }
      }

      [OverrideTarget]
      [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
      public virtual event EventHandler Event;
    }

    [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
    public class TargetWithAttributes
    {
      [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
      public virtual void Method ()
      {
      }

      [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
      public virtual int Property
      {
        get { return 0; }
      }

      [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
      public virtual event EventHandler Event;
    }

    [MultiNonInherited, NonMultiNonInherited]
    public class TargetWithNonInheritedAttributes
    {
    }

    public class TargetWithoutAttributes
    {
      public virtual void Method ()
      {
      }

      public virtual int Property
      {
        get { return 0; }
      }

      public virtual event EventHandler Event;
    }

    [SuppressAttributes (typeof (Attribute))]
    public class MixinSuppressingAllAttributes
    {
    }

    [SuppressAttributes (typeof (Attribute))]
    [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
    public class MixinSuppressingAllAttributesAddingAttributes
    {
    }

    [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
    public class MixinAddingAttributes
    {
    }

    private object[] GetRelevantAttributes (ICustomAttributeProvider source, bool inherit)
    {
      object[] attributes = source.GetCustomAttributes (true);
      return Array.FindAll (attributes, delegate (object o)
      {
        return
            o is MultiInheritedAttribute || o is MultiNonInheritedAttribute || o is NonMultiInheritedAttribute || o is NonMultiNonInheritedAttribute;
      });
    }

    [Test]
    public void AttributesOnMixedTypesBehaveLikeOnDerivedTypes ()
    {
      object[] attributes = GetRelevantAttributes (CreateMixedType (typeof (TargetWithoutAttributes), typeof (MixinWithAttributes)), true);
      Assert.AreEqual (2, attributes.Length);
      Assert.That (
          attributes, Is.EquivalentTo (new object[] {new MultiInheritedAttribute(), new NonMultiInheritedAttribute()}));

      attributes = GetRelevantAttributes (CreateMixedType (typeof (TargetWithAttributes), typeof (MixinWithAttributes)), true);
      Assert.AreEqual (5, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), new MultiInheritedAttribute(),
            new NonMultiNonInheritedAttribute (), new MultiNonInheritedAttribute(), new NonMultiInheritedAttribute() }));
    }

    [Test]
    public void AttributesSuppressedByMixin_AreNotReplicatedFromBaseType ()
    {
      object[] attributes = GetRelevantAttributes (CreateMixedType (typeof (TargetWithNonInheritedAttributes), typeof (MixinSuppressingAllAttributes)), true);
      Assert.AreEqual (0, attributes.Length);
    }

    [Test]
    public void AttributesSuppressedByMixin_AreNotIntroducedFromOtherMixin ()
    {
      object[] attributes = GetRelevantAttributes (CreateMixedType (typeof (NullTarget), typeof (MixinSuppressingAllAttributes), typeof (MixinAddingAttributes)), true);
      Assert.AreEqual (0, attributes.Length);
    }

    [Test]
    public void AttributesSuppressedByMixin_AreIntroducedForSameMixin ()
    {
      object[] attributes = GetRelevantAttributes (CreateMixedType (typeof (NullTarget), typeof (MixinSuppressingAllAttributesAddingAttributes)), true);
      Assert.AreEqual (2, attributes.Length);
      Assert.That (attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), new NonMultiInheritedAttribute() }));
    }

    [Test]
    public void AttributesOnDerivedMethodsBehaveLikeOnDerivedTypes ()
    {
      object[] attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithoutAttributes), typeof (MixinWithAttributes)).GetMethod ("Method"), true);
      Assert.AreEqual (2, attributes.Length);
      Assert.That (
          attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), new NonMultiInheritedAttribute () }));

      attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithAttributes), typeof (MixinWithAttributes)).GetMethod ("Method"), true);
      Assert.AreEqual (5, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), new MultiInheritedAttribute(),
            new NonMultiNonInheritedAttribute (), new MultiNonInheritedAttribute(), new NonMultiInheritedAttribute() }));
    }

    [Test]
    [Ignore ("Due to a missing SRE feature, CustomPropertyEmitter doesn't work as intended currently. Waiting for a service pack...")]
    public void AttributesOnDerivedPropertiesBehaveLikeMethods ()
    {
      object[] attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithoutAttributes), typeof (MixinWithAttributes)).GetProperty ("Property"), true);
      Assert.AreEqual (2, attributes.Length);
      Assert.That (
          attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), new NonMultiInheritedAttribute () }));

      attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithAttributes), typeof (MixinWithAttributes)).GetProperty ("Property"), true);
      Assert.AreEqual (5, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), new MultiInheritedAttribute(),
            new NonMultiNonInheritedAttribute (), new MultiNonInheritedAttribute(), new NonMultiInheritedAttribute() }));
    }

    [Test]
    public void AttributesOnDerivedPropertiesBehaveLikeMethodsTemp ()
    {
      object[] attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithoutAttributes), typeof (MixinWithAttributes)).GetProperty ("Property",
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly), true);
      Assert.AreEqual (2, attributes.Length);
      Assert.That (
          attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), new NonMultiInheritedAttribute () }));

      attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithAttributes), typeof (MixinWithAttributes)).GetProperty ("Property",
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly), true);
      Assert.AreEqual (5, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), new MultiInheritedAttribute(),
            new NonMultiNonInheritedAttribute (), new MultiNonInheritedAttribute(), new NonMultiInheritedAttribute() }));
    }

    [Test]
    public void AttributesOnDerivedEventsBehaveLikeMethods ()
    {
      object[] attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithoutAttributes), typeof (MixinWithAttributes)).GetEvent ("Event"), true);
      Assert.AreEqual (2, attributes.Length);
      Assert.That (
          attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), new NonMultiInheritedAttribute () }));

      attributes =
          GetRelevantAttributes (CreateMixedType (typeof (TargetWithAttributes), typeof (MixinWithAttributes)).GetEvent ("Event"), true);
      Assert.AreEqual (5, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), new MultiInheritedAttribute(),
            new NonMultiNonInheritedAttribute (), new MultiNonInheritedAttribute(), new NonMultiInheritedAttribute() }));
    }
  }
}
