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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class OneSideRelationProperty : MappingReflectionTestBase
  {
    private ClassDefinition _classDefinition;
    private Type _classType;

    public override void SetUp ()
    {
      base.SetUp();

      _classType = typeof (ClassWithVirtualRelationEndPoints);
      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: _classType);
    }

    [Test]
    public void GetMetadata_ForOptional ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("NoAttribute"));
      var relationEndPointReflector = new RdbmsRelationEndPointReflector (
          _classDefinition, propertyInfo, Configuration.NameResolver, DomainModelConstraintProviderStub);

      DomainModelConstraintProviderStub.Stub (stub => stub.IsNullable (propertyInfo)).Return (true);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (VirtualRelationEndPointDefinition), actual);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NoAttribute",
          actual.PropertyName);
      Assert.IsFalse (actual.IsMandatory);
      DomainModelConstraintProviderStub.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_ForMandatory ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("NotNullable"));
      var relationEndPointReflector = new RdbmsRelationEndPointReflector (
          _classDefinition, propertyInfo, Configuration.NameResolver, DomainModelConstraintProviderStub);

      DomainModelConstraintProviderStub.Stub (stub => stub.IsNullable (propertyInfo)).Return (false);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (VirtualRelationEndPointDefinition), actual);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NotNullable",
          actual.PropertyName);
      Assert.IsTrue (actual.IsMandatory);
      DomainModelConstraintProviderStub.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("BidirectionalOneToOne"));
      var relationEndPointReflector = new RdbmsRelationEndPointReflector (
          _classDefinition, propertyInfo, Configuration.NameResolver, DomainModelConstraintProviderStub);

      DomainModelConstraintProviderStub.Stub (stub => stub.IsNullable (propertyInfo)).Return (true);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToOne",
          relationEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ClassWithRealRelationEndPoints), relationEndPointDefinition.PropertyInfo.PropertyType);
      Assert.AreEqual (CardinalityType.One, relationEndPointDefinition.Cardinality);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
      DomainModelConstraintProviderStub.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("BidirectionalOneToMany"));
      var relationEndPointReflector = new RdbmsRelationEndPointReflector (
          _classDefinition, propertyInfo, Configuration.NameResolver, DomainModelConstraintProviderStub);

      DomainModelConstraintProviderStub.Stub (stub => stub.IsNullable (propertyInfo)).Return (true);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToMany",
          relationEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithRealRelationEndPoints>), relationEndPointDefinition.PropertyInfo.PropertyType);
      Assert.AreEqual (CardinalityType.Many, relationEndPointDefinition.Cardinality);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
      Assert.AreEqual ("NoAttribute", relationEndPointDefinition.SortExpressionText);
      DomainModelConstraintProviderStub.VerifyAllExpectations();
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("BidirectionalOneToOne"));
      var relationEndPointReflector = new RdbmsRelationEndPointReflector (
          _classDefinition, propertyInfo, Configuration.NameResolver, DomainModelConstraintProviderStub);

      Assert.IsTrue (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("BidirectionalOneToMany"));
      var relationEndPointReflector = new RdbmsRelationEndPointReflector (
          _classDefinition, propertyInfo, Configuration.NameResolver, DomainModelConstraintProviderStub);

      Assert.IsTrue (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }
  }
}