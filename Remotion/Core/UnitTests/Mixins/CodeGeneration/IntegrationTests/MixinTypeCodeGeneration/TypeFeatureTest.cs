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
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class TypeFeatureTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsMarkerInterface ()
    {
      Type generatedType = GetGeneratedType(typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      Assert.That (typeof (IGeneratedMixinType).IsAssignableFrom (generatedType), Is.True);
    }

    [Test]
    public void GeneratedMixinTypeHasMixinTypeAttribute ()
    {
      Type generatedType = GetGeneratedType (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      Assert.That (generatedType.IsDefined (typeof (ConcreteMixinTypeAttribute), false), Is.True);

      var attributes = (ConcreteMixinTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false);
      Assert.That (attributes.Length, Is.EqualTo (1));
    }

    [Test]
    public void MixinTypeAttribute_CanBeUsedToGetIdentifier ()
    {
      var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (
          typeof (ClassOverridingMixinMembers),
          GenerationPolicy.GenerateOnlyIfConfigured);

      MixinDefinition mixinDefinition = 
          TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.That (mixinDefinition, Is.Not.Null);

      Type generatedType = ConcreteTypeBuilder.Current
          .GetConcreteMixinType (requestingClass, mixinDefinition.GetConcreteMixinTypeIdentifier ())
          .GeneratedType;
      Assert.That (generatedType.IsDefined (typeof (ConcreteMixinTypeAttribute), false), Is.True);

      var attributes = (ConcreteMixinTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false);
      Assert.That (attributes[0].GetIdentifier(), Is.EqualTo (mixinDefinition.GetConcreteMixinTypeIdentifier()));
    }

    [Test]
    public void NameProviderIsUsedWhenTypeIsGenerated ()
    {
      var builder = new ConcreteTypeBuilder { Scope = SavedTypeBuilder.Scope };
      var repository = new MockRepository ();
      var nameProviderMock = repository.StrictMock<INameProvider> ();
      builder.MixinTypeNameProvider = nameProviderMock;
      ConcreteTypeBuilder.SetCurrent (builder);

      var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (
          typeof (ClassOverridingMixinMembers),
          GenerationPolicy.GenerateOnlyIfConfigured);

      MixinDefinition mixinDefinition =
          TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.That (mixinDefinition, Is.Not.Null);

      Expect.Call (nameProviderMock.GetNewTypeName (mixinDefinition)).Return ("Bra");

      repository.ReplayAll ();

      Type generatedType = ConcreteTypeBuilder.Current
          .GetConcreteMixinType (requestingClass, mixinDefinition.GetConcreteMixinTypeIdentifier())
          .GeneratedType;

      Assert.That (generatedType.FullName, Is.EqualTo ("Bra"));

      repository.VerifyAll ();
    }

    [Test]
    public void NamesOfNestedTypesAreFlattened ()
    {
      var builder = new ConcreteTypeBuilder { Scope = SavedTypeBuilder.Scope };
      var repository = new MockRepository ();
      var nameProviderMock = repository.StrictMock<INameProvider> ();
      builder.MixinTypeNameProvider = nameProviderMock;
      ConcreteTypeBuilder.SetCurrent (builder);

      Expect.Call (nameProviderMock.GetNewTypeName (Arg<MixinDefinition>.Is.Anything)).Return ("Bra+Oof");

      repository.ReplayAll ();

      var generatedType = GetGeneratedType (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      Assert.That (generatedType.FullName, Is.EqualTo ("Bra/Oof"));

      repository.VerifyAll ();
    }

    [Test]
    public void AttributesAreReplicatedIfNecessary ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithProtectedOverriderAndAttributes)).EnterScope())
      {
        var generatedType = GetGeneratedType (typeof (NullTarget), typeof (MixinWithProtectedOverriderAndAttributes));
        Assert.That (generatedType, Is.Not.SameAs (typeof (MixinWithProtectedOverriderAndAttributes)));

        object[] inheritableAttributes = generatedType.GetCustomAttributes (typeof (InheritableAttribute), true);
        object[] nonInheritableAttributes = generatedType.GetCustomAttributes (typeof (NonInheritableAttribute), true);

        Assert.That (inheritableAttributes.Length, Is.EqualTo (1));
        Assert.That (inheritableAttributes, Is.EquivalentTo (typeof (MixinWithProtectedOverriderAndAttributes).GetCustomAttributes (typeof (InheritableAttribute), true)));

        Assert.That (nonInheritableAttributes.Length, Is.EqualTo (1));
        Assert.That (nonInheritableAttributes, Is.EquivalentTo (typeof (MixinWithProtectedOverriderAndAttributes).GetCustomAttributes (typeof (NonInheritableAttribute), true)));
      }
    }

    [Test]
    public void CopyTemplatesAreNotReplicated ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithProtectedOverriderAndAttributes)).EnterScope())
      {
        var generatedType = GetGeneratedType (typeof (NullTarget), typeof (MixinWithProtectedOverriderAndAttributes));
        Assert.That (generatedType, Is.Not.SameAs (typeof (MixinWithProtectedOverriderAndAttributes)));

        object[] copiedAttributes = generatedType.GetCustomAttributes (typeof (SampleCopyTemplateAttribute), true);
        Assert.That (copiedAttributes.Length, Is.EqualTo (0));
      }
    }

    [Test]
    public void IdentifierMember_HoldsIdentifier ()
    {
      var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (
          typeof (ClassOverridingMixinMembers),
          GenerationPolicy.GenerateOnlyIfConfigured);

      MixinDefinition mixinDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.That (mixinDefinition, Is.Not.Null);
      var generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (requestingClass, mixinDefinition.GetConcreteMixinTypeIdentifier ()).GeneratedType;

      var identifier = generatedType.GetField ("__identifier").GetValue (null);

      Assert.That (identifier, Is.EqualTo (mixinDefinition.GetConcreteMixinTypeIdentifier()));
    }

    [Test]
    public void ClassContextMember_HoldsRequestingClass ()
    {
      var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (
          typeof (ClassOverridingMixinMembers),
          GenerationPolicy.GenerateOnlyIfConfigured);

      MixinDefinition mixinDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.That (mixinDefinition, Is.Not.Null);
      var generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (requestingClass, mixinDefinition.GetConcreteMixinTypeIdentifier ()).GeneratedType;

      var classContext = generatedType.GetField ("__requestingClassContext").GetValue (null);

      Assert.That (classContext, Is.EqualTo (requestingClass));
    }
    
    [Test]
    public void AbstractMixinWithoutAbstractMembers()
    {
      var instance = CreateMixedObject<NullTarget> (typeof (AbstractMixinWithoutAbstractMembers));
      var m1 = Mixin.Get<AbstractMixinWithoutAbstractMembers> (instance);
      Assert.That (m1, Is.Not.Null);
      Assert.That (m1, Is.InstanceOfType (typeof (AbstractMixinWithoutAbstractMembers)));
      Assert.That (m1.GetType (), Is.Not.SameAs (typeof (AbstractMixinWithoutAbstractMembers)));
      Assert.That (m1.M1 (), Is.EqualTo ("AbstractMixinWithoutAbstractMembers.M1"));
    }

    private Type GetGeneratedType (Type targetType, Type mixinType)
    {
      var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (
          targetType,
          GenerationPolicy.GenerateOnlyIfConfigured);

      MixinDefinition mixinDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[mixinType];
      Assert.That (mixinDefinition, Is.Not.Null);

      return ConcreteTypeBuilder.Current.GetConcreteMixinType (requestingClass, mixinDefinition.GetConcreteMixinTypeIdentifier ()).GeneratedType;
    }
  }
}
