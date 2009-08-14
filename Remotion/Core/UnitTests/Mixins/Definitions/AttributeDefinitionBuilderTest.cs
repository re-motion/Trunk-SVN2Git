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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class AttributeDefinitionBuilderTest
  {
    [Test]
    public void Attributes ()
    {
      TargetClassDefinition targetClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithManyAttributes),
          typeof (ClassWithManyAttributes));
      MixinDefinition mixin = targetClass.Mixins[typeof (ClassWithManyAttributes)];

      CheckAttributes (targetClass);
      CheckAttributes (mixin);

      CheckAttributes (targetClass.Methods[typeof (ClassWithManyAttributes).GetMethod ("Foo")]);
      CheckAttributes (mixin.Methods[typeof (ClassWithManyAttributes).GetMethod ("Foo")]);
    }

    [Test]
    public void SerializableAttributeIsIgnored ()
    {
      TargetClassDefinition bt1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1));
      Assert.That (bt1.CustomAttributes.ContainsKey (typeof (SerializableAttribute)), Is.False);
    }

    [Test]
    public void ExtendsAttributeIsIgnored ()
    {
      MixinDefinition bt1m1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.That (bt1m1.CustomAttributes.ContainsKey (typeof (ExtendsAttribute)), Is.False);
    }

    [Test]
    public void UsesAttributeIsIgnored ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3));
      Assert.That (bt1.CustomAttributes.ContainsKey (typeof (UsesAttribute)), Is.False);
    }

    [Test]
    public void SuppressAttributeIsNotIgnored ()
    {
      TargetClassDefinition classDefinition = 
          DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassWithSuppressAttribute), GenerationPolicy.ForceGeneration);
      Assert.That (classDefinition.CustomAttributes.ContainsKey (typeof (SuppressAttributesAttribute)), Is.True);
    }

    [Test]
    public void OverrideAttributeIsIgnored ()
    {
      MixinDefinition bt1m1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.That (bt1m1.Methods[typeof (BT1Mixin1).GetMethod ("VirtualMethod")].CustomAttributes.ContainsKey (typeof (OverrideTargetAttribute)), Is.False);
    }

    [Test]
    public void InternalAttributesAreIgnored()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<ClassWithInternalAttribute>().Clear().EnterScope())
      {
        Assert.That (
                      DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassWithInternalAttribute)).CustomAttributes.ContainsKey (typeof (InternalStuffAttribute)), Is.False);
      }
    }

    [Test]
    public void CopyAttributes_OnClass ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingAttribute)).EnterScope())
      {
        MixinDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingAttribute)];
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)), Is.False);
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters)), Is.True);

        var attributes = new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters)]);

        Assert.That (attributes.Count, Is.EqualTo (1));
        Assert.That (attributes[0].IsCopyTemplate, Is.True);

        Assert.That (attributes[0].AttributeType, Is.EqualTo (typeof (AttributeWithParameters)));
        Assert.That (
                      attributes[0].Data.Constructor, Is.EqualTo (typeof (AttributeWithParameters).GetConstructor (new[] { typeof (int), typeof (string) })));
        Assert.That (attributes[0].DeclaringDefinition, Is.EqualTo (definition));

        Assert.That (attributes[0].Data.ConstructorArguments.Count, Is.EqualTo (2));
        Assert.That (attributes[0].Data.ConstructorArguments[0].ArgumentType, Is.EqualTo (typeof (int)));
        Assert.That (attributes[0].Data.ConstructorArguments[0].Value, Is.EqualTo (1));
        Assert.That (attributes[0].Data.ConstructorArguments[1].ArgumentType, Is.EqualTo (typeof (string)));
        Assert.That (attributes[0].Data.ConstructorArguments[1].Value, Is.EqualTo ("bla"));

        Assert.That (attributes[0].Data.NamedArguments.Count, Is.EqualTo (2));

        Assert.That (attributes[0].Data.NamedArguments[0].MemberInfo, Is.EqualTo (typeof (AttributeWithParameters).GetField ("Field")));
        Assert.That (attributes[0].Data.NamedArguments[0].TypedValue.ArgumentType, Is.EqualTo (typeof (int)));
        Assert.That (attributes[0].Data.NamedArguments[0].TypedValue.Value, Is.EqualTo (5));

        Assert.That (attributes[0].Data.NamedArguments[1].MemberInfo, Is.EqualTo (typeof (AttributeWithParameters).GetProperty ("Property")));
        Assert.That (attributes[0].Data.NamedArguments[1].TypedValue.ArgumentType, Is.EqualTo (typeof (int)));
        Assert.That (attributes[0].Data.NamedArguments[1].TypedValue.Value, Is.EqualTo (4));
      }
    }

    [Test]
    public void CopyFilteredAttributes_OnClass ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingFilteredAttributes)).EnterScope())
      {
        MixinDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingFilteredAttributes)];
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)), Is.False);
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters)), Is.False);
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters2)), Is.True);
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters3)), Is.True);

        var attributes = new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters2)]);
        Assert.That (attributes.Count, Is.EqualTo (2));

        attributes = new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters3)]);
        Assert.That (attributes.Count, Is.EqualTo (1));
      }
    }

    [Test]
    public void CopyNonInheritedAttributes ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingNonInheritedAttribute)).EnterScope())
      {
        MixinDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingNonInheritedAttribute)];
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)), Is.False);
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (NonInheritedAttribute)), Is.True);
      }
    }

    [Test]
    public void CopyNonInheritedAttributesFromSelf ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingNonInheritedAttributeFromSelf)).EnterScope())
      {
        MixinDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingNonInheritedAttributeFromSelf)];
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)), Is.False);
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (NonInheritedAttribute)), Is.True);
      }
    }

    [Test]
    public void CopyNonInheritedAttributesFromSelf_DosntIntroduceDuplicates ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingInheritedAttributeFromSelf)).EnterScope())
      {
        MixinDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingInheritedAttributeFromSelf)];
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters)), Is.True);
        Assert.That (new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters)]).Count, Is.EqualTo (1));
      }
    }

    [Test]
    public void CopyAttributes_OnMember ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingAttribute)).EnterScope())
      {
        MethodDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingAttribute)].Methods[typeof (MixinIndirectlyAddingAttribute).GetMethod ("ToString")];

        Assert.That (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)), Is.False);
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters)), Is.True);

        var attributes = new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters)]);

        Assert.That (attributes.Count, Is.EqualTo (1));
        Assert.That (attributes[0].AttributeType, Is.EqualTo (typeof (AttributeWithParameters)));
        Assert.That (attributes[0].Data.Constructor, Is.EqualTo (typeof (AttributeWithParameters).GetConstructor (new[] { typeof (int) })));

        Assert.That (attributes[0].IsCopyTemplate, Is.True);
        Assert.That (attributes[0].Data.ConstructorArguments.Count, Is.EqualTo (1));
        Assert.That (attributes[0].Data.ConstructorArguments[0].ArgumentType, Is.EqualTo (typeof (int)));
        Assert.That (attributes[0].Data.ConstructorArguments[0].Value, Is.EqualTo (4));

        Assert.That (attributes[0].Data.NamedArguments.Count, Is.EqualTo (0));
      }
    }

    [Test]
    public void CopyFilteredAttributes_OnMember ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIndirectlyAddingFilteredAttributes)).EnterScope())
      {
        MethodDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingFilteredAttributes)].Methods[typeof (MixinIndirectlyAddingFilteredAttributes).GetMethod ("ToString")];

        Assert.That (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)), Is.False);
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters)), Is.False);
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters2)), Is.True);
        Assert.That (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters3)), Is.True);

        var attributes = new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters2)]);
        Assert.That (attributes.Count, Is.EqualTo (2));

        attributes = new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters3)]);
        Assert.That (attributes.Count, Is.EqualTo (1));
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The CopyCustomAttributes attribute on "
        + ".*MixinWithAmbiguousSource.ToString specifies an ambiguous attribute "
        + "source: The source member string Source matches several members on type "
        + ".*MixinWithAmbiguousSource.", MatchType = MessageMatch.Regex)]
    public void CopyAttributes_Ambiguous ()
    {
      var builder = new AttributeDefinitionBuilder (DefinitionObjectMother.CreateMixinDefinition (typeof (MixinWithAmbiguousSource)));
      var method = typeof (MixinWithAmbiguousSource).GetMethod ("ToString", BindingFlags.NonPublic | BindingFlags.Instance);
      builder.Apply (method, CustomAttributeData.GetCustomAttributes (method), true);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The CopyCustomAttributes attribute on "
        + ".*MixinWithUnknownSource.ToString specifies an unknown attribute "
        + "source .*MixinWithUnknownSource.Source.", MatchType = MessageMatch.Regex)]
    public void CopyAttributes_Unknown ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithUnknownSource)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
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
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
      }
    }

    private static void CheckAttributes (IAttributableDefinition attributableDefinition)
    {
      Assert.That (attributableDefinition.CustomAttributes.ContainsKey (typeof (TagAttribute)), Is.True);
      Assert.That (attributableDefinition.CustomAttributes.GetItemCount (typeof (TagAttribute)), Is.EqualTo (2));

      var attributes = new List<AttributeDefinition> (attributableDefinition.CustomAttributes);
      var attributes2 = new List<AttributeDefinition> (attributableDefinition.CustomAttributes[typeof (TagAttribute)]);
      foreach (AttributeDefinition attribute in attributes2)
      {
        Assert.That (attributes.Contains (attribute), Is.True);
      }

      AttributeDefinition attribute1 = attributes.Find (
          delegate (AttributeDefinition a)
          {
            Assert.That (a.AttributeType, Is.EqualTo (typeof (TagAttribute)));
            return a.Data.Constructor.Equals (typeof (TagAttribute).GetConstructor (Type.EmptyTypes));
          });
      Assert.That (attribute1, Is.Not.Null);
      Assert.That (attribute1.IsCopyTemplate, Is.False);
      Assert.That (attribute1.Data.ConstructorArguments.Count, Is.EqualTo (0));
      Assert.That (attribute1.Data.NamedArguments.Count, Is.EqualTo (0));
      Assert.That (attribute1.DeclaringDefinition, Is.SameAs (attributableDefinition));

      AttributeDefinition attribute2 = attributes.Find (
          delegate (AttributeDefinition a)
          {
            Assert.That (a.AttributeType, Is.EqualTo (typeof (TagAttribute)));
            return a.Data.Constructor.Equals (typeof (TagAttribute).GetConstructor (new[] { typeof (string) }));
          });
      Assert.That (attribute2, Is.Not.Null);
      Assert.That (attribute2.IsCopyTemplate, Is.False);
      Assert.That (attribute2.Data.ConstructorArguments.Count, Is.EqualTo (1));
      Assert.That (attribute2.Data.ConstructorArguments[0].ArgumentType, Is.EqualTo (typeof (string)));
      Assert.That (attribute2.Data.ConstructorArguments[0].Value, Is.EqualTo ("Class!"));
      Assert.That (attribute2.Data.NamedArguments.Count, Is.EqualTo (1));
      Assert.That (attribute2.Data.NamedArguments[0].MemberInfo, Is.EqualTo (typeof (TagAttribute).GetField ("Named")));
      Assert.That (attribute2.Data.NamedArguments[0].TypedValue.ArgumentType, Is.EqualTo (typeof (int)));
      Assert.That (attribute2.Data.NamedArguments[0].TypedValue.Value, Is.EqualTo (5));
      Assert.That (attribute2.DeclaringDefinition, Is.SameAs (attributableDefinition));
      Assert.That (attribute2.Parent, Is.SameAs (attributableDefinition));
    }
  
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class TagAttribute : Attribute
    {
      public TagAttribute () { }
      public TagAttribute (string s) { }

      public int Named;
    }

    [Tag]
    [Tag ("Class!", Named = 5)]
    private class ClassWithManyAttributes
    {
      [Tag]
      [Tag ("Class!", Named = 5)]
      public void Foo ()
      {
      }
    }

    class InternalStuffAttribute : Attribute { }

    [InternalStuff]
    public class ClassWithInternalAttribute { }

    [IgnoreForMixinConfiguration]
    public class MixinWithAmbiguousSource
    {
      private void Source () { }
      private void Source (int i) { }

      [OverrideTarget]
      [CopyCustomAttributes (typeof (MixinWithAmbiguousSource), "Source")]
      protected new string ToString ()
      {
        return "";
      }
    }

    [IgnoreForMixinConfiguration]
    [CopyCustomAttributes (typeof (MixinWithSelfSource))]
    public class MixinWithSelfSource
    {
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
// ReSharper restore UnusedMember.Local
// ReSharper restore UnusedParameter.Local
  }
}
