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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class AttributeDefinitionBuilderTest
  {
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class TagAttribute : Attribute
    {
      public int Named;

      public TagAttribute () { }

      public TagAttribute (string s) { }
    }

    [Tag]
    [Tag ("Class!", Named = 5)]
    private class ClassWithLotsaAttributes
    {
      [Tag]
      [Tag ("Class!", Named = 5)]
      public void Foo ()
      {
      }
    }

    [Test]
    public void Attributes ()
    {
      TargetClassDefinition targetClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithLotsaAttributes),
          typeof (ClassWithLotsaAttributes));
      MixinDefinition mixin = targetClass.Mixins[typeof (ClassWithLotsaAttributes)];

      CheckAttributes (targetClass);
      CheckAttributes (mixin);

      CheckAttributes (targetClass.Methods[typeof (ClassWithLotsaAttributes).GetMethod ("Foo")]);
      CheckAttributes (mixin.Methods[typeof (ClassWithLotsaAttributes).GetMethod ("Foo")]);
    }

    private static void CheckAttributes (IAttributableDefinition attributableDefinition)
    {
      Assert.IsTrue (attributableDefinition.CustomAttributes.ContainsKey (typeof (TagAttribute)));
      Assert.AreEqual (2, attributableDefinition.CustomAttributes.GetItemCount (typeof (TagAttribute)));

      List<AttributeDefinition> attributes = new List<AttributeDefinition> (attributableDefinition.CustomAttributes);
      List<AttributeDefinition> attributes2 = new List<AttributeDefinition> (attributableDefinition.CustomAttributes[typeof (TagAttribute)]);
      foreach (AttributeDefinition attribute in attributes2)
      {
        Assert.IsTrue (attributes.Contains (attribute));
      }

      AttributeDefinition attribute1 = attributes.Find (
          delegate (AttributeDefinition a)
          {
            Assert.AreEqual (typeof (TagAttribute), a.AttributeType);
            return a.Data.Constructor.Equals (typeof (TagAttribute).GetConstructor (Type.EmptyTypes));
          });
      Assert.IsNotNull (attribute1);
      Assert.IsFalse (attribute1.IsCopyTemplate);
      Assert.AreEqual (0, attribute1.Data.ConstructorArguments.Count);
      Assert.AreEqual (0, attribute1.Data.NamedArguments.Count);
      Assert.AreSame (attributableDefinition, attribute1.DeclaringDefinition);

      AttributeDefinition attribute2 = attributes.Find (
          delegate (AttributeDefinition a)
          {
            Assert.AreEqual (typeof (TagAttribute), a.AttributeType);
            return a.Data.Constructor.Equals (typeof (TagAttribute).GetConstructor (new Type[] { typeof (string) }));
          });
      Assert.IsNotNull (attribute2);
      Assert.IsFalse (attribute2.IsCopyTemplate);
      Assert.AreEqual (1, attribute2.Data.ConstructorArguments.Count);
      Assert.AreEqual (typeof (string), attribute2.Data.ConstructorArguments[0].ArgumentType);
      Assert.AreEqual ("Class!", attribute2.Data.ConstructorArguments[0].Value);
      Assert.AreEqual (1, attribute2.Data.NamedArguments.Count);
      Assert.AreEqual (typeof (TagAttribute).GetField ("Named"), attribute2.Data.NamedArguments[0].MemberInfo);
      Assert.AreEqual (typeof (int), attribute2.Data.NamedArguments[0].TypedValue.ArgumentType);
      Assert.AreEqual (5, attribute2.Data.NamedArguments[0].TypedValue.Value);
      Assert.AreSame (attributableDefinition, attribute2.DeclaringDefinition);
      Assert.AreSame (attributableDefinition, attribute2.Parent);
    }

    [Test]
    public void SerializableAttributeIsIgnored ()
    {
      TargetClassDefinition bt1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1));
      Assert.IsFalse (bt1.CustomAttributes.ContainsKey (typeof (SerializableAttribute)));
    }

    [Test]
    public void ExtendsAttributeIsIgnored ()
    {
      MixinDefinition bt1m1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.IsFalse (bt1m1.CustomAttributes.ContainsKey (typeof (ExtendsAttribute)));
    }

    [Test]
    public void UsesAttributeIsIgnored ()
    {
      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3));
      Assert.IsFalse (bt1.CustomAttributes.ContainsKey (typeof (UsesAttribute)));
    }

    [Test]
    public void SuppressAttributeIsNotIgnored ()
    {
      TargetClassDefinition classDefinition = 
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassWithSuppressAttribute), GenerationPolicy.ForceGeneration);
      Assert.IsTrue (classDefinition.CustomAttributes.ContainsKey (typeof (SuppressAttributesAttribute)));
    }

    [Test]
    public void OverrideAttributeIsIgnored ()
    {
      MixinDefinition bt1m1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.IsFalse (bt1m1.Methods[typeof (BT1Mixin1).GetMethod("VirtualMethod")].CustomAttributes.ContainsKey (typeof (OverrideTargetAttribute)));
    }

    class InternalStuffAttribute : Attribute { }

    [InternalStuff]
    public class ClassWithInternalAttribute { }

    [Test]
    public void InternalAttributesAreIgnored()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<ClassWithInternalAttribute>().Clear().EnterScope())
      {
        Assert.IsFalse (
            TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassWithInternalAttribute)).CustomAttributes.ContainsKey (typeof (InternalStuffAttribute)));
      }
    }

    [Test]
    public void CopyAttributes_OnClass ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingAttribute)).EnterScope())
      {
        MixinDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingAttribute)];
        Assert.IsFalse (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)));
        Assert.IsTrue (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters)));

        List<AttributeDefinition> attributes =
            new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters)]);

        Assert.AreEqual (1, attributes.Count);
        Assert.IsTrue (attributes[0].IsCopyTemplate);

        Assert.AreEqual (typeof (AttributeWithParameters), attributes[0].AttributeType);
        Assert.AreEqual (typeof (AttributeWithParameters).GetConstructor (new Type[] { typeof (int), typeof (string) }),
            attributes[0].Data.Constructor);
        Assert.AreEqual (definition, attributes[0].DeclaringDefinition);

        Assert.AreEqual (2, attributes[0].Data.ConstructorArguments.Count);
        Assert.AreEqual (typeof (int), attributes[0].Data.ConstructorArguments[0].ArgumentType);
        Assert.AreEqual (1, attributes[0].Data.ConstructorArguments[0].Value);
        Assert.AreEqual (typeof (string), attributes[0].Data.ConstructorArguments[1].ArgumentType);
        Assert.AreEqual ("bla", attributes[0].Data.ConstructorArguments[1].Value);

        Assert.AreEqual (2, attributes[0].Data.NamedArguments.Count);

        Assert.AreEqual (typeof (AttributeWithParameters).GetField ("Field"), attributes[0].Data.NamedArguments[0].MemberInfo);
        Assert.AreEqual (typeof (int), attributes[0].Data.NamedArguments[0].TypedValue.ArgumentType);
        Assert.AreEqual (5, attributes[0].Data.NamedArguments[0].TypedValue.Value);

        Assert.AreEqual (typeof (AttributeWithParameters).GetProperty ("Property"), attributes[0].Data.NamedArguments[1].MemberInfo);
        Assert.AreEqual (typeof (int), attributes[0].Data.NamedArguments[1].TypedValue.ArgumentType);
        Assert.AreEqual (4, attributes[0].Data.NamedArguments[1].TypedValue.Value);
      }
    }

    [Test]
    public void CopyFilteredAttributes_OnClass ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingFilteredAttributes)).EnterScope())
      {
        MixinDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingFilteredAttributes)];
        Assert.IsFalse (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)));
        Assert.IsFalse (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters)));
        Assert.IsTrue (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters2)));
        Assert.IsTrue (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters3)));

        List<AttributeDefinition> attributes =
            new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters2)]);
        Assert.AreEqual (2, attributes.Count);

        attributes = new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters3)]);
        Assert.AreEqual (1, attributes.Count);
      }
    }

    [Test]
    public void CopyNonInheritedAttributes ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingNonInheritedAttribute)).EnterScope())
      {
        MixinDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingNonInheritedAttribute)];
        Assert.IsFalse (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)));
        Assert.IsTrue (definition.CustomAttributes.ContainsKey (typeof (NonInheritedAttribute)));
      }
    }

    [Test]
    public void CopyNonInheritedAttributesFromSelf ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingNonInheritedAttributeFromSelf)).EnterScope())
      {
        MixinDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingNonInheritedAttributeFromSelf)];
        Assert.IsFalse (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)));
        Assert.IsTrue (definition.CustomAttributes.ContainsKey (typeof (NonInheritedAttribute)));
      }
    }

    [Test]
    public void CopyNonInheritedAttributesFromSelf_DosntIntroduceDuplicates ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingInheritedAttributeFromSelf)).EnterScope())
      {
        MixinDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingInheritedAttributeFromSelf)];
        Assert.IsTrue (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters)));
        Assert.AreEqual (1, new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters)]).Count);
      }
    }

    [Test]
    public void CopyAttributes_OnMember ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingAttribute)).EnterScope())
      {
        MethodDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingAttribute)].Methods[typeof (MixinIndirectlyAddingAttribute).GetMethod ("ToString")];

        Assert.IsFalse (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)));
        Assert.IsTrue (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters)));

        List<AttributeDefinition> attributes =
            new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters)]);

        Assert.AreEqual (1, attributes.Count);
        Assert.AreEqual (typeof (AttributeWithParameters), attributes[0].AttributeType);
        Assert.AreEqual (typeof (AttributeWithParameters).GetConstructor (new Type[] { typeof (int) }), attributes[0].Data.Constructor);

        Assert.IsTrue (attributes[0].IsCopyTemplate);
        Assert.AreEqual (1, attributes[0].Data.ConstructorArguments.Count);
        Assert.AreEqual (typeof (int), attributes[0].Data.ConstructorArguments[0].ArgumentType);
        Assert.AreEqual (4, attributes[0].Data.ConstructorArguments[0].Value);

        Assert.AreEqual (0, attributes[0].Data.NamedArguments.Count);
      }
    }

    [Test]
    public void CopyFilteredAttributes_OnMember ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingFilteredAttributes)).EnterScope())
      {
        MethodDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingFilteredAttributes)].Methods[typeof (MixinIndirectlyAddingFilteredAttributes).GetMethod ("ToString")];

        Assert.IsFalse (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)));
        Assert.IsFalse (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters)));
        Assert.IsTrue (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters2)));
        Assert.IsTrue (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters3)));

        List<AttributeDefinition> attributes =
            new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters2)]);
        Assert.AreEqual (2, attributes.Count);

        attributes = new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters3)]);
        Assert.AreEqual (1, attributes.Count);
      }
    }

    [IgnoreForMixinConfiguration]
    public class MixinWithAmbiguousSource
    {
      private void Source () { }
      private void Source (int i) { }

      [OverrideTarget]
      [CopyCustomAttributes (typeof (MixinWithAmbiguousSource), "Source")]
      protected new string ToString()
      {
        return "";
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The CopyCustomAttributes attribute on "
        + ".*MixinWithAmbiguousSource.ToString specifies an ambiguous attribute "
        + "source: The source member string Source matches several members on type "
        + ".*MixinWithAmbiguousSource.", MatchType = MessageMatch.Regex)]
    public void CopyAttributes_Ambiguous ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithAmbiguousSource)).EnterScope())
      {
        TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
      }
    }

    [IgnoreForMixinConfiguration]
    public class MixinWithUnknownSource
    {
      [OverrideTarget]
      [CopyCustomAttributes (typeof (MixinWithUnknownSource), "Source")]
      protected new string ToString ()
      {
        return "";
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The CopyCustomAttributes attribute on "
        + ".*MixinWithUnknownSource.ToString specifies an unknown attribute "
        + "source .*MixinWithUnknownSource.Source.", MatchType = MessageMatch.Regex)]
    public void CopyAttributes_Unknown ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithUnknownSource)).EnterScope())
      {
        TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
      }
    }

    [IgnoreForMixinConfiguration]
    public class MixinWithInvalidSourceType
    {
      [OverrideTarget]
      [CopyCustomAttributes (typeof (MixinWithInvalidSourceType))]
      protected new string ToString ()
      {
        return "";
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The CopyCustomAttributes attribute on "
        + ".*MixinWithInvalidSourceType.ToString specifies an attribute source "
        + ".*MixinWithInvalidSourceType of a different member kind.", MatchType = MessageMatch.Regex)]
    public void CopyAttributes_Invalid ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithInvalidSourceType)).EnterScope())
      {
        TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
      }
    }

    [IgnoreForMixinConfiguration]
    [CopyCustomAttributes(typeof (MixinWithSelfSource))]
    public class MixinWithSelfSource
    {
    }
  }
}
