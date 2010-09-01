// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration.MixedMapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class ReflectionBasedPropertyResolverTest : MappingReflectionTestBase
  {
    private ReflectionBasedClassDefinition _orderClass;

    private ReflectionBasedClassDefinition _targetClassForPersistentMixinClass;
    private ReflectionBasedClassDefinition _derivedTargetClassForPersistentMixinClass;

    private ReflectionBasedClassDefinition _classWithInterface;
    private ReflectionBasedClassDefinition _classWithInterfaceWithMissingAccessors;
    private ReflectionBasedClassDefinition _classWithMixinAddingInterface;

    public override void SetUp ()
    {
      base.SetUp();

      _orderClass = (ReflectionBasedClassDefinition) FakeMappingConfiguration.Current.ClassDefinitions[typeof (Order)];

      _targetClassForPersistentMixinClass =
          (ReflectionBasedClassDefinition) FakeMappingConfiguration.Current.ClassDefinitions[typeof (TargetClassForPersistentMixin)];
      _derivedTargetClassForPersistentMixinClass =
          (ReflectionBasedClassDefinition) FakeMappingConfiguration.Current.ClassDefinitions[typeof (DerivedTargetClassForPersistentMixin)];

      _classWithInterface = CreateDefinitionForClassWithInterface();
      _classWithInterfaceWithMissingAccessors = CreateDefinitionForClassWithInterfaceWithMissingAccessors();
      _classWithMixinAddingInterface = CreateDefinitionForClassWithMixinAddingInterface();
    }

    [Test]
    public void ResolveDefinition ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (property, _orderClass, _orderClass.GetPropertyDefinition);

      var expected = _orderClass.GetPropertyDefinition (typeof (Order).FullName + ".OrderNumber");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_StorageClassNoneProperty ()
    {
      var property = typeof (Order).GetProperty ("RedirectedOrderNumber");

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (property, _orderClass, _orderClass.GetPropertyDefinition);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_FromInterfaceProperty ()
    {
      var property = typeof (IInterfaceWithProperties).GetProperty ("ImplicitProperty");

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterface, _classWithInterface.GetPropertyDefinition);

      var expected = _classWithInterface.GetMandatoryPropertyDefinition (typeof (ClassWithInterface).FullName + ".ImplicitProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_ExplicitInterfaceImplementation_FromInterfaceProperty ()
    {
      var property = typeof (IInterfaceWithProperties).GetProperty ("ExplicitManagedProperty");

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterface, _classWithInterface.GetPropertyDefinition);

      var expected = _classWithInterface.GetMandatoryPropertyDefinition (
          typeof (ClassWithInterface).FullName + "." + typeof (IInterfaceWithProperties).FullName + ".ExplicitManagedProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_FromImplementationProperty ()
    {
      var property = typeof (ClassWithInterface).GetProperty ("ImplicitProperty");

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterface, _classWithInterface.GetPropertyDefinition);

      var expected = _classWithInterface.GetMandatoryPropertyDefinition (typeof (ClassWithInterface).FullName + ".ImplicitProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_FromInterfaceProperty_GetAccessorOnly ()
    {
      var property = typeof (IInterfaceWithPropertiesWithMissingAccessors).GetProperty ("PropertyWithGetAccessor");

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterfaceWithMissingAccessors, _classWithInterfaceWithMissingAccessors.GetPropertyDefinition);

      var expected = _classWithInterfaceWithMissingAccessors.GetMandatoryPropertyDefinition (
          typeof (ClassWithInterfaceWithMissingAccessors).FullName + ".PropertyWithGetAccessor");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_FromInterfaceProperty_SetAccessorOnly ()
    {
      var property = typeof (IInterfaceWithPropertiesWithMissingAccessors).GetProperty ("PropertyWithSetAccessor");

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterfaceWithMissingAccessors, _classWithInterfaceWithMissingAccessors.GetPropertyDefinition);

      var expected = _classWithInterfaceWithMissingAccessors.GetMandatoryPropertyDefinition (
          typeof (ClassWithInterfaceWithMissingAccessors).FullName + ".PropertyWithSetAccessor");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_NotImplemented ()
    {
      var property = typeof (IInterfaceWithPropertiesAddedByMixin).GetProperty ("ImplicitProperty");

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterface, _classWithInterface.GetPropertyDefinition); // does not implement this property

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ResolveDefinition_ExplicitInterfaceImplementation_FromImplementationProperty ()
    {
      var property = typeof (ClassWithInterface).GetProperty (
          typeof (IInterfaceWithProperties).FullName + ".ExplicitManagedProperty",
          BindingFlags.Instance | BindingFlags.NonPublic);

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithInterface, _classWithInterface.GetPropertyDefinition);

      var expected = _classWithInterface.GetMandatoryPropertyDefinition (
          typeof (ClassWithInterface).FullName + "." + typeof (IInterfaceWithProperties).FullName + ".ExplicitManagedProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_MixinProperty_FromInterfaceProperty ()
    {
      var property = typeof (IInterfaceWithPropertiesAddedByMixin).GetProperty ("ImplicitProperty");

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithMixinAddingInterface, _classWithMixinAddingInterface.GetPropertyDefinition);

      var expected =
          _classWithMixinAddingInterface.GetMandatoryPropertyDefinition (typeof (MixinAddingInterfaceWithProperties).FullName + ".ImplicitProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_ExplicitMixinProperty_FromInterfaceProperty ()
    {
      var property = typeof (IInterfaceWithPropertiesAddedByMixin).GetProperty ("ExplicitManagedProperty");

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithMixinAddingInterface, _classWithMixinAddingInterface.GetPropertyDefinition);

      var expected = _classWithMixinAddingInterface.GetMandatoryPropertyDefinition (
          typeof (MixinAddingInterfaceWithProperties).FullName + "." + typeof (IInterfaceWithPropertiesAddedByMixin).FullName
          + ".ExplicitManagedProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_MixinProperty_FromImplementationProperty ()
    {
      var property = typeof (MixinAddingInterfaceWithProperties).GetProperty ("ImplicitProperty");

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithMixinAddingInterface, _classWithMixinAddingInterface.GetPropertyDefinition);

      var expected =
          _classWithMixinAddingInterface.GetMandatoryPropertyDefinition (typeof (MixinAddingInterfaceWithProperties).FullName + ".ImplicitProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_ExplicitMixinProperty_FromImplementationProperty ()
    {
      var property = typeof (MixinAddingInterfaceWithProperties).GetProperty (
          typeof (IInterfaceWithPropertiesAddedByMixin).FullName + ".ExplicitManagedProperty",
          BindingFlags.Instance | BindingFlags.NonPublic);

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _classWithMixinAddingInterface, _classWithMixinAddingInterface.GetPropertyDefinition);

      var expected = _classWithMixinAddingInterface.GetMandatoryPropertyDefinition (
          typeof (MixinAddingInterfaceWithProperties).FullName + "." + typeof (IInterfaceWithPropertiesAddedByMixin).FullName
          + ".ExplicitManagedProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveDefinition_MixinPropertyOnBaseClass ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("PersistentProperty");

      var result = ReflectionBasedPropertyResolver.ResolveDefinition<PropertyDefinition> (
          property, _derivedTargetClassForPersistentMixinClass, _derivedTargetClassForPersistentMixinClass.GetPropertyDefinition);

      var expected = _targetClassForPersistentMixinClass.GetPropertyDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    private ReflectionBasedClassDefinition CreateDefinitionForClassWithInterface ()
    {
      var classWithInterface = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithInterface));
      classWithInterface.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithInterface,
              typeof (ClassWithInterface),
              "ImplicitProperty",
              "ImplicitProperty",
              typeof (string),
              false,
              100));
      classWithInterface.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithInterface,
              typeof (ClassWithInterface),
              typeof (IInterfaceWithProperties).FullName + ".ExplicitManagedProperty",
              "ExplicitManagedProperty",
              typeof (string),
              false,
              100));
      return classWithInterface;
    }

    private ReflectionBasedClassDefinition CreateDefinitionForClassWithInterfaceWithMissingAccessors ()
    {
      var classWithInterfaceWithMissingAccessors =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithInterfaceWithMissingAccessors));
      classWithInterfaceWithMissingAccessors.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithInterfaceWithMissingAccessors,
              typeof (ClassWithInterfaceWithMissingAccessors),
              "PropertyWithGetAccessor",
              "PropertyWithGetAccessor",
              typeof (string),
              false,
              100));
      classWithInterfaceWithMissingAccessors.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithInterfaceWithMissingAccessors,
              typeof (ClassWithInterfaceWithMissingAccessors),
              "PropertyWithSetAccessor",
              "PropertyWithSetAccessor",
              typeof (string),
              false,
              100));
      return classWithInterfaceWithMissingAccessors;
    }

    private ReflectionBasedClassDefinition CreateDefinitionForClassWithMixinAddingInterface ()
    {
      var classWithMixinAddingInterface =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              typeof (TargetClassForMixinAddingInterfaceWithProperties), typeof (MixinAddingInterfaceWithProperties));
      classWithMixinAddingInterface.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithMixinAddingInterface,
              typeof (MixinAddingInterfaceWithProperties),
              "ImplicitProperty",
              "ImplicitProperty",
              typeof (string),
              false,
              100));
      classWithMixinAddingInterface.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithMixinAddingInterface,
              typeof (MixinAddingInterfaceWithProperties),
              typeof (IInterfaceWithPropertiesAddedByMixin).FullName + ".ExplicitManagedProperty",
              "ExplicitManagedProperty",
              typeof (string),
              false,
              100));
      return classWithMixinAddingInterface;
    }
  }
}