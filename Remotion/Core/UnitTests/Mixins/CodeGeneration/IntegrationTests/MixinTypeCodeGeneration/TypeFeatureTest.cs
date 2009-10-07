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
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class TypeFeatureTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsMarkerInterface ()
    {
      Type generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      Assert.That (typeof (IGeneratedMixinType).IsAssignableFrom (generatedType), Is.True);
    }

    [Test]
    public void GeneratedMixinTypeHasMixinTypeAttribute ()
    {
      Type generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      Assert.That (generatedType.IsDefined (typeof (ConcreteMixinTypeAttribute), false), Is.True);

      var attributes = (ConcreteMixinTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false);
      Assert.That (attributes.Length, Is.EqualTo (1));
    }

    [Test]
    public void MixinTypeAttribute_CanBeUsedToGetIdentifier ()
    {
      var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (typeof (ClassOverridingMixinMembers));

      MixinDefinition mixinDefinition = DefinitionObjectMother.GetTargetClassDefinition (requestingClass).Mixins[typeof (MixinWithAbstractMembers)];
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
      var nameProviderMock = repository.StrictMock<IConcreteMixinTypeNameProvider> ();
      builder.MixinTypeNameProvider = nameProviderMock;
      ConcreteTypeBuilder.SetCurrent (builder);

      nameProviderMock.Expect (mock => mock.GetNameForConcreteMixinType (Arg<ConcreteMixinTypeIdentifier>.Is.Anything)).Return ("Bra");

      repository.ReplayAll ();

      var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (typeof (ClassOverridingMixinMembers));
      MixinDefinition mixinDefinition = DefinitionObjectMother.GetTargetClassDefinition (requestingClass).Mixins[typeof (MixinWithAbstractMembers)];

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
      var nameProviderMock = repository.StrictMock<IConcreteMixinTypeNameProvider> ();
      builder.MixinTypeNameProvider = nameProviderMock;
      ConcreteTypeBuilder.SetCurrent (builder);

      Expect.Call (nameProviderMock.GetNameForConcreteMixinType (Arg<ConcreteMixinTypeIdentifier>.Is.Anything)).Return ("Bra+Oof");

      repository.ReplayAll ();

      var generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      Assert.That (generatedType.FullName, Is.EqualTo ("Bra/Oof"));

      repository.VerifyAll ();
    }

    [Test]
    public void IdentifierMember_HoldsIdentifier ()
    {
      var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (typeof (ClassOverridingMixinMembers));

      MixinDefinition mixinDefinition = DefinitionObjectMother.GetTargetClassDefinition (requestingClass).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.That (mixinDefinition, Is.Not.Null);
      var generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (requestingClass, mixinDefinition.GetConcreteMixinTypeIdentifier ()).GeneratedType;

      var identifier = generatedType.GetField ("__identifier").GetValue (null);

      Assert.That (identifier, Is.EqualTo (mixinDefinition.GetConcreteMixinTypeIdentifier()));
    }

    [Test]
    public void ClassContextMember_HoldsRequestingClass ()
    {
      var generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));

      var classContext = (ClassContext) generatedType.GetField ("__requestingClassContext").GetValue (null);
      // Note: Cannot compare this classContext to the requesting one because we don't know if the type was generated for this context or for another
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinWithAbstractMembers)), Is.Not.Null); 
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

    [Test]
    public void NestedInterfaceWithOverrides ()
    {
      var generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      var overrideInterface = generatedType.GetNestedType ("IOverriddenMethods");

      Assert.That (overrideInterface, Is.Not.Null);
      
      var method = overrideInterface.GetMethod ("AbstractMethod");
      Assert.That (method, Is.Not.Null);
      Assert.That (method.ReturnType, Is.SameAs (typeof (string)));

      var parameters = method.GetParameters();
      Assert.That (parameters.Length, Is.EqualTo (1));
      Assert.That (parameters[0].ParameterType, Is.SameAs (typeof (int)));
      Assert.That (parameters[0].Name, Is.EqualTo ("i"));

      var propertyAccessor = overrideInterface.GetMethod ("get_AbstractProperty");
      Assert.That (propertyAccessor, Is.Not.Null);

      var eventAccessor = overrideInterface.GetMethod ("add_AbstractEvent");
      Assert.That (eventAccessor, Is.Not.Null);
    }
  }
}
