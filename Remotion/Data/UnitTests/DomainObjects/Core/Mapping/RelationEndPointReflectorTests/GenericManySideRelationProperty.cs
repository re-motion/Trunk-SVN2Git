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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class GenericManySideRelationProperty : MappingReflectionTestBase
  {
    private ClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "ClosedGenericClassWithManySideRelationProperties",
          "ClosedGenericClassWithManySideRelationProperties",
          UnitTestDomainStorageProviderDefinition,
          typeof (ClosedGenericClassWithRealRelationEndPoints),
          false);
    }

    [Test]
    public void GetMetadata_Unidirectional ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseUnidirectional")))
        .Return (null);
      DomainModelConstraintProviderMock
        .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseUnidirectional")))
        .Return (true);
      DomainModelConstraintProviderMock.Replay();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseUnidirectional");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreSame (GetPropertyDefinition ("BaseUnidirectional"), relationEndPointDefinition.PropertyDefinition);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToOne")))
        .Return (null);
      DomainModelConstraintProviderMock
        .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToOne")))
        .Return (true);
      DomainModelConstraintProviderMock.Replay ();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToOne");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreSame (GetPropertyDefinition ("BaseBidirectionalOneToOne"), relationEndPointDefinition.PropertyDefinition);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToMany")))
        .Return (null);
      DomainModelConstraintProviderMock
        .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToMany")))
        .Return (true);
      DomainModelConstraintProviderMock.Replay();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToMany");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreSame (GetPropertyDefinition ("BaseBidirectionalOneToMany"), relationEndPointDefinition.PropertyDefinition);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }


    [Test]
    public void IsVirtualEndRelationEndpoint_Unidirectional ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseUnidirectional")))
        .Return (null);
      DomainModelConstraintProviderMock.Replay();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseUnidirectional");

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToOne")))
        .Return (null);
      DomainModelConstraintProviderMock.Replay();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToOne");

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToMany")))
        .Return (null);
      DomainModelConstraintProviderMock.Replay();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToMany");

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    private RdbmsRelationEndPointReflector CreateRelationEndPointReflector (string propertyName)
    {
      PropertyReflector propertyReflector = CreatePropertyReflector (propertyName);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      return new RdbmsRelationEndPointReflector (
          _classDefinition, propertyReflector.PropertyInfo, MappingConfiguration.Current.NameResolver, DomainModelConstraintProviderMock);
    }

    private PropertyReflector CreatePropertyReflector (string property)
    {
      Type type = typeof (ClosedGenericClassWithRealRelationEndPoints);
      var propertyInfo =
          PropertyInfoAdapter.Create (type.GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

      return new PropertyReflector (_classDefinition, propertyInfo, MappingConfiguration.Current.NameResolver, DomainModelConstraintProviderMock);
    }

    private PropertyDefinition GetPropertyDefinition (string propertyName)
    {
      return _classDefinition.MyPropertyDefinitions[
          string.Format ("{0}.{1}", typeof (GenericClassWithRealRelationEndPointsNotInMapping<>).FullName, propertyName)];
    }
  }
}