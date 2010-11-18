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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.DomainObjectTypeIsNotGenericValidationRule;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.RelationEndPointPropertyTypeIsSupportedValidationRule;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class MappingConfigurationTest : MappingReflectionTestBase
  {
    private MockRepository _mockRepository;
    private IMappingLoader _mockMappingLoader;
    private readonly ClassDefinition[] _classDefinitionCollection = new ClassDefinition[0];
    private readonly RelationDefinition[] _relationDefinitionCollection = new RelationDefinition[0];
    private ReflectionBasedNameResolver _nameResolver;

    public override void SetUp()
    {
      base.SetUp();

      _nameResolver = new ReflectionBasedNameResolver ();
      _mockRepository = new MockRepository ();
      _mockMappingLoader = _mockRepository.StrictMock<IMappingLoader>();
    }

    [Test]
    public void Initialize()
    {
      Expect.Call (_mockMappingLoader.GetClassDefinitions ()).Return ( _classDefinitionCollection );
      Expect.Call (_mockMappingLoader.GetRelationDefinitions (Arg<ClassDefinitionCollection>.Is.Anything)).Return (_relationDefinitionCollection);
      Expect.Call (_mockMappingLoader.ResolveTypes).Return (true);
      Expect.Call (_mockMappingLoader.NameResolver).Return (_nameResolver);

      _mockRepository.ReplayAll();

      MappingConfiguration configuration = new MappingConfiguration (_mockMappingLoader);
      ClassDefinitionCollection actualClassDefinitionCollection = configuration.ClassDefinitions;
      RelationDefinitionCollection actualRelationDefinitionCollection = configuration.RelationDefinitions;

      _mockRepository.VerifyAll();

      Assert.That (actualClassDefinitionCollection, Is.EqualTo(_classDefinitionCollection));
      Assert.That (actualRelationDefinitionCollection, Is.EqualTo(_relationDefinitionCollection));
      Assert.That (configuration.ResolveTypes, Is.True);
      Assert.That (configuration.NameResolver, Is.SameAs (_nameResolver));
      Assert.That (configuration.ClassDefinitions.IsReadOnly, Is.True);
      Assert.That (configuration.RelationDefinitions.IsReadOnly, Is.True);
    }

    [Test]
    public void SetCurrent()
    {
      try
      {
        SetupResult.For (_mockMappingLoader.GetClassDefinitions()).Return (_classDefinitionCollection);
        SetupResult.For (_mockMappingLoader.GetRelationDefinitions (Arg<ClassDefinitionCollection>.Is.Anything)).Return (_relationDefinitionCollection);
        SetupResult.For (_mockMappingLoader.ResolveTypes).Return (true);
        SetupResult.For (_mockMappingLoader.NameResolver).Return (_nameResolver);

        _mockRepository.ReplayAll();

        MappingConfiguration configuration = new MappingConfiguration (_mockMappingLoader);
        MappingConfiguration.SetCurrent (configuration);

        Assert.AreSame (configuration, MappingConfiguration.Current);
      }
      finally
      {
        MappingConfiguration.SetCurrent (null);
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "Generic domain objects are not supported.\r\n\r\n"
      +"Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
      +"DomainObjectTypeIsNotGenericValidationRule.GenericTypeDomainObject`1[System.String]'")]
    public void ClassDefinitionsAreValidated ()
    {
      var type = typeof (GenericTypeDomainObject<string>);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      var classDefinitionCollection = new[] { classDefinition };

      SetupResult.For (_mockMappingLoader.GetClassDefinitions ()).Return (classDefinitionCollection);
      _mockRepository.ReplayAll ();

      new MappingConfiguration (_mockMappingLoader);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "Only StorageClass.Persistent and StorageClass.Transaction are supported for property 'PropertyWithStorageClassNone' of class "
      +"'DerivedValidationDomainObjectClass'.\r\n\r\n"
      +"Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass'\r\n"
      +"Property: 'PropertyWithStorageClassNone'")]
    public void PropertyDefinitionsAreValidated ()
    {
      var type = typeof (DerivedValidationDomainObjectClass);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
      var propertyInfo = type.GetProperty ("PropertyWithStorageClassNone");
      var propertyDefinition = new TestablePropertyDefinition (classDefinition, propertyInfo, 20, StorageClass.None);
      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      var classDefinitionCollection = new[] { classDefinition };

      SetupResult.For (_mockMappingLoader.GetClassDefinitions ()).Return (classDefinitionCollection);
      _mockRepository.ReplayAll ();

      new MappingConfiguration (_mockMappingLoader);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "The property type of an uni-directional relation property must be assignable to Remotion.Data.DomainObjects.DomainObject.\r\n\r\n"
      +"Declaration type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer'\r\n"
      +"Property: 'OrderNumber'\r\n"
      +"----------\r\n"
      +"Opposite relation property 'Orders' declared on type 'Order' does not define a matching 'DBBidirectionalRelationAttribute'.\r\n\r\n"
      +"Declaration type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order'\r\n"
      +"----------\r\n"
      +"The type 'Order' does not match the type of the opposite relation propery 'Orders' declared on type 'Order'.\r\n\r\n"
      +"Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order'")]
    public void RelationDefinitionsAreValidated ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (RelationEndPointPropertyClass));
      var relationDefinition =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              +"TestDomain.Integration.Order.Customer->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders"];
      var classDefinitionCollection = new[] { classDefinition };
      var relationDefinitionCollection = new[] { relationDefinition };

      SetupResult.For (_mockMappingLoader.GetClassDefinitions ()).Return (classDefinitionCollection);
      SetupResult.For (_mockMappingLoader.GetRelationDefinitions (Arg<ClassDefinitionCollection>.Is.Anything)).Return (relationDefinitionCollection);

      _mockRepository.ReplayAll ();

      new MappingConfiguration (_mockMappingLoader);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "Neither class 'DerivedValidationDomainObjectClass' nor its base classes specify an entity name. Make class 'DerivedValidationDomainObjectClass' "
      +"abstract or apply a 'DBTable' attribute to it or one of its base classes.\r\n\r\n"
      +"Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass'")]
    public void PersistenceMappingIsValidated ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "NonAbstractClassHasEntityNameDomainObject",
          null,
          "NonAbstractClassHasEntityNameStorageProviderID",
          typeof (DerivedValidationDomainObjectClass),
          false);
      var classDefinitionCollection = new[] { classDefinition };
      
      SetupResult.For (_mockMappingLoader.GetClassDefinitions ()).Return (classDefinitionCollection);
      SetupResult.For (_mockMappingLoader.GetRelationDefinitions (Arg<ClassDefinitionCollection>.Is.Anything)).Return (new RelationDefinition[0]);

      _mockRepository.ReplayAll ();

      new MappingConfiguration (_mockMappingLoader);
    }

    [Test]
    public void PersistenceModelIsLoaded ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageDefinition (typeof (Order), null);
      Assert.That (classDefinition.StorageEntityDefinition, Is.Null);

      var classDefinitionCollection = new[] { classDefinition };

      SetupResult.For (_mockMappingLoader.GetClassDefinitions ()).Return (classDefinitionCollection);
      SetupResult.For (_mockMappingLoader.GetRelationDefinitions (Arg<ClassDefinitionCollection>.Is.Anything)).Return (new RelationDefinition[0]);
      SetupResult.For (_mockMappingLoader.ResolveTypes).Return (true);
      SetupResult.For (_mockMappingLoader.NameResolver).Return (new ReflectionBasedNameResolver());

      _mockRepository.ReplayAll ();

      new MappingConfiguration (_mockMappingLoader);

      Assert.That (classDefinition.StorageEntityDefinition, Is.Not.Null);
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf(typeof(TableDefinition)));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).TableName, Is.EqualTo("Order"));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).GetColumns().Count, Is.EqualTo (0));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Argument 'mappingConfiguration' must have property 'ResolveTypes' set.\r\nParameter name: mappingConfiguration")]
    public void SetCurrentRejectsUnresolvedTypes()
    {
      SetupResult.For (_mockMappingLoader.GetClassDefinitions()).Return (_classDefinitionCollection);
      SetupResult.For (_mockMappingLoader.GetRelationDefinitions (Arg<ClassDefinitionCollection>.Is.Anything)).Return (_relationDefinitionCollection);
      SetupResult.For (_mockMappingLoader.ResolveTypes).Return (false);
      SetupResult.For (_mockMappingLoader.NameResolver).Return (_nameResolver);

      _mockRepository.ReplayAll();

      MappingConfiguration configuration = new MappingConfiguration (_mockMappingLoader);

      _mockRepository.VerifyAll();

      MappingConfiguration.SetCurrent (configuration);
    }

    [Test]
    public void ContainsClassDefinition()
    {
      Assert.IsFalse (MappingConfiguration.Current.Contains (FakeMappingConfiguration.Current.ClassDefinitions[typeof (Order)]));
      Assert.IsTrue (MappingConfiguration.Current.Contains (MappingConfiguration.Current.ClassDefinitions[typeof (Order)]));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNull()
    {
      MappingConfiguration.Current.Contains ((ClassDefinition) null);
    }

    [Test]
    public void ContainsPropertyDefinition()
    {
      Assert.IsFalse (
          MappingConfiguration.Current.Contains (
              FakeMappingConfiguration.Current.ClassDefinitions[typeof (Order)]["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"]));
      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.ClassDefinitions[typeof (Order)]["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"]));
    }

    [Test]
    public void ContainsRelationDefinition()
    {
      Assert.IsFalse (
          MappingConfiguration.Current.Contains (
              FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.Order.OrderItems"]));
      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order->"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"]));
    }

    [Test]
    public void ContainsRelationEndPointDefinition()
    {
      Assert.IsFalse (
          MappingConfiguration.Current.Contains (
              FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.Order.OrderItems"].
                  EndPointDefinitions[0]));
      Assert.IsFalse (
          MappingConfiguration.Current.Contains (
              FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.Order.OrderItems"].
                  EndPointDefinitions[1]));

      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:"
              +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order->"
              +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"]
              .EndPointDefinitions[0]));
      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:"
              +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order->"+
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"].EndPointDefinitions[1]));
    }

    [Test]
    public void ContainsRelationEndPointDefinitionNotInMapping()
    {
      ReflectionBasedClassDefinition orderDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);
      ReflectionBasedClassDefinition orderTicketDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket), false);
      orderTicketDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create(orderTicketDefinition, typeof (OrderTicket), "Order", "OrderID", typeof (ObjectID), false));

      VirtualRelationEndPointDefinition orderEndPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(orderDefinition, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      RelationEndPointDefinition orderTicketEndPointdefinition =
          new RelationEndPointDefinition (orderTicketDefinition, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order", true);

      new RelationDefinition ("RelationIDNotInMapping", orderEndPointDefinition, orderTicketEndPointdefinition);

      Assert.IsFalse (MappingConfiguration.Current.Contains (orderEndPointDefinition));
    }
  }
}
