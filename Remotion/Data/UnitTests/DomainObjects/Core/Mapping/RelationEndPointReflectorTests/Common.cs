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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class Common : MappingReflectionTestBase
  {
    [Test]
    public void CreateRelationEndPointReflector ()
    {
      var type = typeof (ClassWithVirtualRelationEndPoints);
      var propertyInfo = PropertyInfoAdapter.Create (type.GetProperty ("NoAttribute"));
      Assert.IsInstanceOf (
          typeof (RdbmsRelationEndPointReflector),
          RelationEndPointReflector.CreateRelationEndPointReflector (
              CreateClassDefinition (type), propertyInfo, Configuration.NameResolver, DomainModelConstraintProviderStub));
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithoutAttribute ()
    {
      var type = typeof (ClassWithRealRelationEndPoints);
      var propertyInfo = PropertyInfoAdapter.Create (type.GetProperty ("NoAttribute"));
      var relationEndPointReflector =
          RelationEndPointReflector.CreateRelationEndPointReflector (
              CreateClassDefinition (type), propertyInfo, Configuration.NameResolver, DomainModelConstraintProviderStub);

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithCollectionPropertyAndWithoutAttribute ()
    {
      var type = typeof (ClassWithInvalidUnidirectionalRelation);
      var propertyInfo = PropertyInfoAdapter.Create (type.GetProperty ("LeftSide"));
      var relationEndPointReflector =
          RelationEndPointReflector.CreateRelationEndPointReflector (
              CreateClassDefinition (type), propertyInfo, Configuration.NameResolver, DomainModelConstraintProviderStub);

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    [Test]
    public void IsVirtualRelationEndPoint_UnidirectionalRelation ()
    {
      var type = typeof (ClassWithRealRelationEndPoints);
      var propertyInfo = PropertyInfoAdapter.Create (type.GetProperty ("Unidirectional"));
      var relationEndPointReflector =
          new RdbmsRelationEndPointReflector (
              CreateClassDefinition (type), propertyInfo, Configuration.NameResolver, DomainModelConstraintProviderStub);

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    [Test]
    public void GetMetadata_NonVirtualEndPoint_PropertyTypeIsNotObjectID ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (typeof (ClassWithRealRelationEndPoints).GetProperty ("Unidirectional"));
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (ClassWithRealRelationEndPoints));
      var propertyDefinition = PropertyDefinitionFactory.Create (classDefinition, "Unidirectional", typeof (string));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      var mappingNameResolverMock = MockRepository.GenerateStrictMock<IMappingNameResolver>();
      mappingNameResolverMock.Expect (mock => mock.GetPropertyName (propertyInfo)).Return ("Unidirectional");
      mappingNameResolverMock.Replay();

      var relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (
          classDefinition, propertyInfo, mappingNameResolverMock, DomainModelConstraintProviderStub);

      var result = relationEndPointReflector.GetMetadata();

      mappingNameResolverMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (TypeNotObjectIDRelationEndPointDefinition)));
    }

    private ClassDefinition CreateClassDefinition (Type type)
    {
      return ClassDefinitionFactory.CreateClassDefinition (type.Name, type.Name, UnitTestDomainStorageProviderDefinition, type, false);
    }
  }
}