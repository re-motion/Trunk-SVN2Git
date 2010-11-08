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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using
    Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.
        ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Reflection
{
  [TestFixture]
  public class ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRuleTest : ValidationRuleTestBase
  {
    private ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule _validationRule;
    private ClassDefinition _classDefinition;
    private RelationDefinition _relationDefinition;
    private VirtualRelationEndPointDefinition _endPoint1;
    private RelationEndPointDefinition _endPoint2;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule();
      _classDefinition = new ReflectionBasedClassDefinition (
          "ID",
          "EntityName",
          "SPID",
          typeof (ForeignKeyIsSupportedClass),
          false,
          null,
          new PersistentMixinFinderMock (typeof (ForeignKeyIsSupportedClass), new Type[0]));
      _relationDefinition =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer"];
      _endPoint1 = (VirtualRelationEndPointDefinition) _relationDefinition.EndPointDefinitions[0];
      _endPoint2 = (RelationEndPointDefinition) _relationDefinition.EndPointDefinitions[1];
    }

    [TearDown]
    public void TearDown ()
    {
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { _endPoint1, _endPoint2 });
    }

    [Test]
    public void RelationDefinitionWithAnonymousEndPointDefinitions ()
    {
      var endPoint1Stub = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      var endPoint2Stub = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      var relationDefinition = new RelationDefinition ("Test", endPoint1Stub, endPoint2Stub);

      endPoint1Stub.Stub (stub => stub.IsAnonymous).Return (true);
      endPoint2Stub.Stub (stub => stub.IsAnonymous).Return (true);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PropertyInfoIsNotResolved ()
    {
      var endPoint1Stub = MockRepository.GenerateStub<IRelationEndPointDefinition> ();
      var endPoint2Stub = MockRepository.GenerateStub<IRelationEndPointDefinition> ();
      var relationDefinition = new RelationDefinition ("Test", endPoint1Stub, endPoint2Stub);

      endPoint1Stub.Stub (stub => stub.IsAnonymous).Return (false);
      endPoint2Stub.Stub (stub => stub.IsAnonymous).Return (false);
      endPoint1Stub.Stub (stub => stub.IsPropertyInfoResolved).Return (false);
      endPoint1Stub.Stub (stub => stub.ClassDefinition).Return (_classDefinition);
      endPoint1Stub.Stub (stub => stub.PropertyName).Return ("Test");
      
      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void NoReflectionBasedVirtualRelationEndPointDefinition ()
    {
      var endPointDefinition = new AnonymousRelationEndPointDefinition (_classDefinition);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, _endPoint1 });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyWithNoDbBidirectionalRelationAttribute ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition,
          "PropertyWithNoDbBidirectionalRelationAttribute",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (ForeignKeyIsSupportedClass).GetProperty ("PropertyWithNoDbBidirectionalRelationAttribute"));
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, _endPoint1 });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void NoCollectionProperty_ContainsForeignKeyIsTrue ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition,
          "NoCollectionProperty_ContainsForeignKey",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (ForeignKeyIsSupportedClass).GetProperty ("NoCollectionProperty_ContainsForeignKey"));
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { _endPoint1, endPointDefinition });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void NoCollectionProperty_ContainsForeignKeyIsFalse ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition,
          "NoCollectionProperty_ContainsNoForeignKey",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (ForeignKeyIsSupportedClass).GetProperty ("NoCollectionProperty_ContainsNoForeignKey"));
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { _endPoint1, endPointDefinition });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void CollectionProperty_ContainsForeignKeyIsTrue ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition,
          "CollectionProperty_ContainsForeignKey",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (ForeignKeyIsSupportedClass).GetProperty ("CollectionProperty_ContainsForeignKey"));
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, _endPoint1 });

      var validationResult = _validationRule.Validate (_relationDefinition);

      var expectedMessage = "Only relation end points with a property type of 'Remotion.Data.DomainObjects.DomainObject' can contain the foreign key.\r\n\r\n"
        + "Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        + "ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule.ForeignKeyIsSupportedClass'\r\nProperty: 'CollectionProperty_ContainsForeignKey'";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void CollectionProperty_ContainsForeignKeyIsTrue_BothEndPoints ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition,
          "CollectionProperty_ContainsForeignKey",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (ForeignKeyIsSupportedClass).GetProperty ("CollectionProperty_ContainsForeignKey"));
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, endPointDefinition });

      var validationResult = _validationRule.Validate (_relationDefinition);

      var expectedMessage = "Only relation end points with a property type of 'Remotion.Data.DomainObjects.DomainObject' can contain the foreign key.\r\n\r\n"
        + "Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        + "ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule.ForeignKeyIsSupportedClass'\r\nProperty: 'CollectionProperty_ContainsForeignKey'";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void CollectionProperty_ContainsForeignKeyIsFalse ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition,
          "CollectionProperty_ContainsNoForeignKey",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (ForeignKeyIsSupportedClass).GetProperty ("CollectionProperty_ContainsNoForeignKey"));
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, _endPoint1 });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }
  }
}