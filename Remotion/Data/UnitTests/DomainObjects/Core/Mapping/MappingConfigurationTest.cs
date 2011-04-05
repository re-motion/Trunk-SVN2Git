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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.DomainObjectTypeIsNotGenericValidationRule;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.RelationEndPointPropertyTypeIsSupportedValidationRule;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using ClassDefinitionValidator = Remotion.Data.DomainObjects.Mapping.Validation.ClassDefinitionValidator;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class MappingConfigurationTest : MappingReflectionTestBase
  {
    private ClassDefinition[] _emptyClassDefinitions;
    private RelationDefinition[] _emptyRelationDefinitions;

    private MockRepository _mockRepository;
    private IMappingLoader _mockMappingLoader;
    private ReflectionBasedNameResolver _nameResolver;

    private TableDefinition _fakeStorageEntityDefinition = new TableDefinition (
        DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition,
        "Test",
        "Test",
        new SimpleColumnDefinition[0],
        new ITableConstraintDefinition[0]);

    public override void SetUp ()
    {
      base.SetUp();

      _emptyClassDefinitions = new ClassDefinition[0];
      _emptyRelationDefinitions = new RelationDefinition[0];

      _nameResolver = new ReflectionBasedNameResolver();
      _mockRepository = new MockRepository();
      _mockMappingLoader = _mockRepository.StrictMock<IMappingLoader>();
    }

    [Test]
    public void Initialize ()
    {
      StubMockMappingLoader (_emptyClassDefinitions, _emptyRelationDefinitions, true);

      _mockRepository.ReplayAll();

      var configuration = new MappingConfiguration (
          _mockMappingLoader, new PersistenceModelLoader (new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));
      ClassDefinitionCollection actualClassDefinitionCollection = configuration.ClassDefinitions;
      RelationDefinitionCollection actualRelationDefinitionCollection = configuration.RelationDefinitions;

      _mockRepository.VerifyAll();

      Assert.That (actualClassDefinitionCollection, Is.EqualTo (_emptyClassDefinitions));
      Assert.That (actualRelationDefinitionCollection, Is.EqualTo (_emptyRelationDefinitions));
      Assert.That (configuration.ResolveTypes, Is.True);
      Assert.That (configuration.NameResolver, Is.SameAs (_nameResolver));
      Assert.That (configuration.ClassDefinitions.IsReadOnly, Is.True);
      Assert.That (configuration.RelationDefinitions.IsReadOnly, Is.True);
    }

    [Test]
    public void SetCurrent ()
    {
      try
      {
        StubMockMappingLoader (_emptyClassDefinitions, _emptyRelationDefinitions, true);

        _mockRepository.ReplayAll();

        var configuration = new MappingConfiguration (
            _mockMappingLoader, new PersistenceModelLoader (new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));
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
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        + "DomainObjectTypeIsNotGenericValidationRule.GenericTypeDomainObject`1[System.String]")]
    public void ClassDefinitionsAreValidated ()
    {
      var type = typeof (GenericTypeDomainObject<string>);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);

      StubMockMappingLoader (classDefinition);
      _mockRepository.ReplayAll();

      new MappingConfiguration (
          _mockMappingLoader, new PersistenceModelLoader (new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Only StorageClass.Persistent and StorageClass.Transaction are supported for property 'PropertyWithStorageClassNone' of class "
        + "'DerivedValidationDomainObjectClass'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
        + "Property: PropertyWithStorageClassNone")]
    public void PropertyDefinitionsAreValidated ()
    {
      var type = typeof (DerivedValidationDomainObjectClass);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          type.Name, type.Name, TestDomainStorageProviderDefinition, type, false);
      var propertyInfo = type.GetProperty ("PropertyWithStorageClassNone");
      var propertyDefinition = new TestablePropertyDefinition (classDefinition, propertyInfo, 20, StorageClass.None);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      StubMockMappingLoader (classDefinition);
      _mockRepository.ReplayAll();

      new MappingConfiguration (
          _mockMappingLoader, new PersistenceModelLoader (new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The property type of an uni-directional relation property must be assignable to 'DomainObject'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order\r\n"
        + "Property: OrderNumber")]
    public void RelationDefinitionsAreValidated ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (RelationEndPointPropertyClass));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      var relationDefinition =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.Order.Customer->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders"];

      StubMockMappingLoader (new[] { classDefinition }, new[] { relationDefinition }, true);
      _mockRepository.ReplayAll();

      new MappingConfiguration (
          _mockMappingLoader, new PersistenceModelLoader (new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Neither class 'DerivedValidationDomainObjectClass' nor its base classes are mapped to a table. Make class 'DerivedValidationDomainObjectClass' "
        + "abstract or define a table for it or one of its base classes.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass")]
    public void PersistenceModelIsValidated ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition,
          "Test",
          new IEntityDefinition[]
          { new TableDefinition (UnitTestDomainStorageProviderDefinition, "Test", "TestView", new IColumnDefinition[0], new ITableConstraintDefinition[0]) },
          new IColumnDefinition[0]);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "NonAbstractClassHasEntityNameDomainObject",
          null,
          UnitTestDomainStorageProviderDefinition,
          typeof (DerivedValidationDomainObjectClass),
          false);
      classDefinition.SetStorageEntity (unionViewDefinition);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection());

      StubMockMappingLoader (classDefinition);

      var persistenceModelLoaderStub = _mockRepository.Stub<IPersistenceModelLoader>();
      persistenceModelLoaderStub
          .Stub (stub => stub.ApplyPersistenceModelToHierarchy (Arg<ClassDefinition>.Is.Anything));
      persistenceModelLoaderStub
          .Stub (stub => stub.CreatePersistenceMappingValidator (Arg<ClassDefinition>.Is.Anything))
          .Return (new PersistenceMappingValidator (new ClassAboveTableIsAbstractValidationRule()));

      _mockRepository.ReplayAll();

      new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderStub);
    }

    [Test]
    public void PersistenceModelIsValidated_OverAllRootClasses ()
    {
      var persistenceModelLoaderMock = MockRepository.GenerateStrictMock<IPersistenceModelLoader>();
      var validatorMock1 = MockRepository.GenerateStrictMock<IPersistenceMappingValidator>();
      var validatorMock2 = MockRepository.GenerateStrictMock<IPersistenceMappingValidator>();

      var rootClass1 = ClassDefinitionFactory.CreateFinishedClassDefinition (typeof (Order));
      var rootClass2 = ClassDefinitionFactory.CreateFinishedClassDefinition (typeof (OrderTicket));

      persistenceModelLoaderMock
          .Expect (mock => mock.ApplyPersistenceModelToHierarchy (rootClass1))
          .WhenCalled (mi => rootClass1.SetStorageEntity (_fakeStorageEntityDefinition));
      persistenceModelLoaderMock
          .Expect (mock => mock.ApplyPersistenceModelToHierarchy (rootClass2))
          .WhenCalled (mi => rootClass2.SetStorageEntity (_fakeStorageEntityDefinition));

      persistenceModelLoaderMock.Expect (mock => mock.CreatePersistenceMappingValidator (rootClass1)).Return (validatorMock1);
      persistenceModelLoaderMock.Expect (mock => mock.CreatePersistenceMappingValidator (rootClass2)).Return (validatorMock2);
      persistenceModelLoaderMock.Replay();

      validatorMock1
          .Expect (mock => mock.Validate (Arg<IEnumerable<ClassDefinition>>.List.Equal (new[] { rootClass1 })))
          .Return (new MappingValidationResult[0]);
      validatorMock2
          .Expect (mock => mock.Validate (Arg<IEnumerable<ClassDefinition>>.List.Equal (new[] { rootClass2 })))
          .Return (new MappingValidationResult[0]);
      validatorMock1.Replay();
      validatorMock2.Replay();

      StubMockMappingLoader (new[] { rootClass1, rootClass2 }, new RelationDefinition[0], true);
      _mockRepository.ReplayAll();

      new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderMock);

      persistenceModelLoaderMock.VerifyAllExpectations();
      validatorMock1.VerifyAllExpectations();
      validatorMock2.VerifyAllExpectations();
    }

    [Test]
    public void PersistenceModelIsValidated_OverAllDerivedClasses ()
    {
      var persistenceModelLoaderMock = MockRepository.GenerateStrictMock<IPersistenceModelLoader>();
      var validatorMock = MockRepository.GenerateStrictMock<IPersistenceMappingValidator>();

      var rootClass = ClassDefinitionFactory.CreateFinishedFileSystemItemDefinitionWithDerivedClasses();
      var derivedClass1 = rootClass.DerivedClasses[0];
      var derivedClass2 = rootClass.DerivedClasses[1];

      persistenceModelLoaderMock
          .Expect (mock => mock.ApplyPersistenceModelToHierarchy (rootClass)).WhenCalled (
              mi =>
              {
                rootClass.SetStorageEntity (_fakeStorageEntityDefinition);
                derivedClass1.SetStorageEntity (_fakeStorageEntityDefinition);
                derivedClass2.SetStorageEntity (_fakeStorageEntityDefinition);
              });
      persistenceModelLoaderMock.Expect (mock => mock.CreatePersistenceMappingValidator (rootClass)).Return (validatorMock);
      persistenceModelLoaderMock.Replay();

      validatorMock
          .Expect (mock => mock.Validate (Arg<IEnumerable<ClassDefinition>>.List.Equal (new[] { rootClass, derivedClass1, derivedClass2 })))
          .Return (new MappingValidationResult[0]);
      validatorMock.Replay();

      StubMockMappingLoader (new[] { rootClass, derivedClass1, derivedClass2 }, new RelationDefinition[0], true);
      _mockRepository.ReplayAll();

      new MappingConfiguration (_mockMappingLoader, persistenceModelLoaderMock);

      persistenceModelLoaderMock.VerifyAllExpectations();
      validatorMock.VerifyAllExpectations();
    }

    [Test]
    public void PersistenceModelIsLoaded ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition,
          typeof (Order),
          "OrderNumber",
          typeof (int),
          false,
          null,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("OrderNumber"),
          new SimpleColumnDefinition ("FakeColumn1", typeof (string), "varchar", true, false));
      var propertyDefinition2 = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition,
          typeof (Order),
          "DeliveryDate",
          typeof (DateTime),
          false,
          null,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("DeliveryDate"),
          new SimpleColumnDefinition ("FakeColumn2", typeof (string), "varchar", true, false));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition1, propertyDefinition2 }, true));
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection());

      Assert.That (classDefinition.StorageEntityDefinition, Is.Null);

      StubMockMappingLoader (classDefinition);
      _mockRepository.ReplayAll();

      new MappingConfiguration (
          _mockMappingLoader, new PersistenceModelLoader (new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));

      Assert.That (classDefinition.StorageEntityDefinition, Is.Not.Null);
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).TableName, Is.EqualTo ("Order"));
      Assert.That (classDefinition.MyPropertyDefinitions.Count, Is.EqualTo (2));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).GetColumns().Count, Is.EqualTo (4));
      Assert.That (classDefinition.MyPropertyDefinitions["OrderNumber"].StoragePropertyDefinition, Is.Not.Null);
      Assert.That (
          ((SimpleColumnDefinition) classDefinition.MyPropertyDefinitions["OrderNumber"].StoragePropertyDefinition).Name, Is.EqualTo ("FakeColumn1"));
      Assert.That (classDefinition.MyPropertyDefinitions["DeliveryDate"].StoragePropertyDefinition, Is.Not.Null);
      Assert.That (
          ((SimpleColumnDefinition) classDefinition.MyPropertyDefinitions["DeliveryDate"].StoragePropertyDefinition).Name, Is.EqualTo ("FakeColumn2"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The persistence model loader did not assign a storage entity to class 'Order'.")]
    public void VerifyPersistenceModelApplied_NoStorageEntityIsAppliedToTheRootClass ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());

      Assert.That (classDefinition.StorageEntityDefinition, Is.Null);

      var persistenceModelStub = MockRepository.GenerateStub<IPersistenceModelLoader>();

      StubMockMappingLoader (classDefinition);
      _mockRepository.ReplayAll();

      new MappingConfiguration (_mockMappingLoader, persistenceModelStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The persistence model loader did not assign a storage property to property 'Fake' of class 'Order'.")]
    public void VerifyPersistenceModelApplied_NoStoragePropertyIsAppliedToTheRootClassProperty ()
    {
      var fakeStorageEntityDefinition = _fakeStorageEntityDefinition;
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (classDefinition, "Fake", "Fake");
      PrivateInvoke.SetNonPublicField (propertyDefinition, "_storagePropertyDefinition", null);
      
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      Assert.That (classDefinition.StorageEntityDefinition, Is.Null);
      Assert.That (propertyDefinition.StoragePropertyDefinition, Is.Null);

      var persistenceModelStub = MockRepository.GenerateStub<IPersistenceModelLoader>();

      StubMockMappingLoader (classDefinition);
      _mockRepository.ReplayAll();

      persistenceModelStub.Stub (stub => stub.ApplyPersistenceModelToHierarchy (classDefinition)).WhenCalled (
          mi =>
          classDefinition.SetStorageEntity (fakeStorageEntityDefinition));

      new MappingConfiguration (_mockMappingLoader, persistenceModelStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The persistence model loader did not assign a storage entity to class 'Partner'.")]
    public void VerifyPersistenceModelApplied_NoStorageEntityIsAppliedToDerivedClass ()
    {
      var fakeStorageEntityDefinition = _fakeStorageEntityDefinition;
      var companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Company), null);
      var partnerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Partner), companyClass);

      companyClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      partnerClass.SetPropertyDefinitions (new PropertyDefinitionCollection());

      companyClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { partnerClass }, true, true));

      Assert.That (companyClass.StorageEntityDefinition, Is.Null);
      Assert.That (partnerClass.StorageEntityDefinition, Is.Null);

      var persistenceModelStub = MockRepository.GenerateStub<IPersistenceModelLoader>();

      StubMockMappingLoader (companyClass, partnerClass);
      _mockRepository.ReplayAll();

      persistenceModelStub.Stub (stub => stub.ApplyPersistenceModelToHierarchy (companyClass)).WhenCalled (
          mi => companyClass.SetStorageEntity (fakeStorageEntityDefinition));

      new MappingConfiguration (_mockMappingLoader, persistenceModelStub);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Argument 'mappingConfiguration' must have property 'ResolveTypes' set.\r\nParameter name: mappingConfiguration")]
    public void SetCurrentRejectsUnresolvedTypes ()
    {
      StubMockMappingLoader (_emptyClassDefinitions, _emptyRelationDefinitions, false);

      _mockRepository.ReplayAll();

      var configuration = new MappingConfiguration (
          _mockMappingLoader, new PersistenceModelLoader (new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));

      _mockRepository.VerifyAll();

      MappingConfiguration.SetCurrent (configuration);
    }

    [Test]
    public void ContainsClassDefinition ()
    {
      Assert.IsFalse (MappingConfiguration.Current.Contains (FakeMappingConfiguration.Current.ClassDefinitions[typeof (Order)]));
      Assert.IsTrue (MappingConfiguration.Current.Contains (MappingConfiguration.Current.ClassDefinitions[typeof (Order)]));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNull ()
    {
      MappingConfiguration.Current.Contains ((ClassDefinition) null);
    }

    [Test]
    public void ContainsPropertyDefinition ()
    {
      Assert.IsFalse (
          MappingConfiguration.Current.Contains (
              FakeMappingConfiguration.Current.ClassDefinitions[typeof (Order)][
                  "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"]));
      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.ClassDefinitions[typeof (Order)][
                  "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"]));
    }

    [Test]
    public void ContainsRelationDefinition ()
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
                                                               +
                                                               "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order->"
                                                               +
                                                               "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"
                  ]));
    }

    [Test]
    public void ContainsRelationEndPointDefinition ()
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
                                                               +
                                                               "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order->"
                                                               +
                                                               "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"
                  ]
                  .EndPointDefinitions[0]));
      Assert.IsTrue (
          MappingConfiguration.Current.Contains (
              MappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:"
                                                               +
                                                               "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order->" +
                                                               "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"
                  ].EndPointDefinitions[1]));
    }

    [Test]
    public void ContainsRelationEndPointDefinitionNotInMapping ()
    {
      ClassDefinition orderDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "Order", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      ClassDefinition orderTicketDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "OrderTicket", "OrderTicket", UnitTestDomainStorageProviderDefinition, typeof (OrderTicket), false);
      orderTicketDefinition.SetPropertyDefinitions (
          new PropertyDefinitionCollection (
              new[]
              {
                  ReflectionBasedPropertyDefinitionFactory.Create (
                      orderTicketDefinition, typeof (OrderTicket), "Order", "OrderID", typeof (ObjectID), false)
              },
              true));

      VirtualRelationEndPointDefinition orderEndPointDefinition =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              orderDefinition,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderTicket",
              true,
              CardinalityType.One,
              typeof (OrderTicket));

      var orderTicketEndPointdefinition =
          new RelationEndPointDefinition (
              orderTicketDefinition["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order"], true);

      new RelationDefinition ("RelationIDNotInMapping", orderEndPointDefinition, orderTicketEndPointdefinition);

      Assert.IsFalse (MappingConfiguration.Current.Contains (orderEndPointDefinition));
    }

    private void StubMockMappingLoader (params ClassDefinition[] classDefinitions)
    {
      StubMockMappingLoader (classDefinitions, new RelationDefinition[0], true);
    }

    private void StubMockMappingLoader (
        IEnumerable<ClassDefinition> classDefinitions, IEnumerable<RelationDefinition> relationDefinitions, bool resolveTypes)
    {
      SetupResult.For (_mockMappingLoader.GetClassDefinitions()).Return (classDefinitions);
      SetupResult.For (_mockMappingLoader.GetRelationDefinitions (Arg<ClassDefinitionCollection>.Is.Anything)).Return (relationDefinitions);
      SetupResult.For (_mockMappingLoader.ResolveTypes).Return (resolveTypes);
      SetupResult.For (_mockMappingLoader.NameResolver).Return (_nameResolver);
      SetupResult.For (_mockMappingLoader.CreateClassDefinitionValidator()).Return (CreateClassDefinitionValidator());
      SetupResult.For (_mockMappingLoader.CreatePropertyDefinitionValidator()).Return (CreatePropertyDefinitionValidator());
      SetupResult.For (_mockMappingLoader.CreateRelationDefinitionValidator()).Return (CreateRelationDefinitionValidator());
      SetupResult.For (_mockMappingLoader.CreateSortExpressionValidator()).Return (CreateSortExpressionValidator());
    }

    private ClassDefinitionValidator CreateClassDefinitionValidator ()
    {
      return new ClassDefinitionValidator (new DomainObjectTypeIsNotGenericValidationRule());
    }

    private PropertyDefinitionValidator CreatePropertyDefinitionValidator ()
    {
      return new PropertyDefinitionValidator (new StorageClassIsSupportedValidationRule());
    }

    private RelationDefinitionValidator CreateRelationDefinitionValidator ()
    {
      return new RelationDefinitionValidator (new RelationEndPointPropertyTypeIsSupportedValidationRule());
    }

    private SortExpressionValidator CreateSortExpressionValidator ()
    {
      return new SortExpressionValidator (new SortExpressionIsValidValidationRule());
    }
  }
}