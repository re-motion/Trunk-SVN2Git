/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;
using Mocks_List = Rhino.Mocks.Constraints.List;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence
{
  [TestFixture]
  public class PersistenceManagerTest : ClientTransactionBaseTest
  {
    PersistenceManager _persistenceManager;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();
      _persistenceManager = new PersistenceManager ();
    }

    public override void TearDown ()
    {
      base.TearDown ();
      _persistenceManager.Dispose ();
    }

    [Test]
    public void LoadDataContainer ()
    {
      DataContainer actualDataContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (DomainObjectIDs.Order1, actualDataContainer.ID);
    }

    [Test]
    public void LoadDataContainers ()
    {
      Assert.AreNotEqual (DomainObjectIDs.Order1.StorageProviderID, DomainObjectIDs.Official1, "Different storage providers");
      UnitTestStorageProviderStub officialStorageProvider =
          (UnitTestStorageProviderStub) _persistenceManager.StorageProviderManager.GetMandatory (DomainObjectIDs.Official1.StorageProviderID);

      MockRepository mockRepository = new MockRepository();
      StorageProvider mockProvider = mockRepository.CreateMock<StorageProvider> (officialStorageProvider.Definition);

      DataContainer officialDC1 = DataContainer.CreateNew (DomainObjectIDs.Official1);
      DataContainer officialDC2 = DataContainer.CreateNew (DomainObjectIDs.Official2);
      
      DataContainerCollection officialDCs = new DataContainerCollection();
      officialDCs.Add (officialDC1);
      officialDCs.Add (officialDC2);

      Expect.Call (mockProvider.LoadDataContainers (null)).Constraints (Mocks_List.Equal (new object[] {DomainObjectIDs.Official1,
          DomainObjectIDs.Official2})).Return (officialDCs);

      mockRepository.ReplayAll();

      officialStorageProvider.InnerProvider = mockProvider;

      DataContainerCollection actualDataContainers =
          _persistenceManager.LoadDataContainers (new ObjectID[] { DomainObjectIDs.Order1, DomainObjectIDs.Official1, DomainObjectIDs.Order2,
              DomainObjectIDs.Official2 }, true);

      mockRepository.VerifyAll ();

      Assert.AreEqual (4, actualDataContainers.Count);
      Assert.AreEqual (DomainObjectIDs.Order1, actualDataContainers[0].ID);
      Assert.AreSame (officialDC1, actualDataContainers[1]);
      Assert.AreEqual (DomainObjectIDs.Order2, actualDataContainers[2].ID);
      Assert.AreSame (officialDC2, actualDataContainers[3]);
    }

    [Test]
    [ExpectedException (typeof (BulkLoadException), ExpectedMessage = "There were errors when loading a bulk of DomainObjects:\r\n"
          + "Object 'Order|11111111-1111-1111-1111-111111111111|System.Guid' could not be found.\r\n"
          + "Object 'Order|22222222-2222-2222-2222-222222222222|System.Guid' could not be found.\r\n")]
    public void LoadDataContainers_ThrowOnNotFound ()
    {
      Guid guid1 = new Guid ("11111111111111111111111111111111");
      Guid guid2 = new Guid ("22222222222222222222222222222222");
      _persistenceManager.LoadDataContainers (
          new ObjectID[] { new ObjectID (typeof (Order), guid1), new ObjectID (typeof (Order), guid2), DomainObjectIDs.Order1}, true);
    }

    [Test]
    public void LoadDataContainers_NoThrowOnNotFound ()
    {
      Guid guid1 = new Guid ("11111111111111111111111111111111");
      Guid guid2 = new Guid ("22222222222222222222222222222222");
      DataContainerCollection dataContainers = _persistenceManager.LoadDataContainers (
          new ObjectID[] { new ObjectID (typeof (Order), guid1), new ObjectID (typeof (Order), guid2), DomainObjectIDs.Order1 }, false);
      Assert.AreEqual (1, dataContainers.Count);
      Assert.AreEqual (DomainObjectIDs.Order1, dataContainers[0].ID);
    }

    [Test]
    public void LoadRelatedDataContainer ()
    {
      DataContainer orderTicketContainer = TestDataContainerFactory.CreateOrderTicket1DataContainer ();

      DataContainer orderContainer = _persistenceManager.LoadRelatedDataContainer (
          orderTicketContainer, new RelationEndPointID (orderTicketContainer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));

      DataContainerChecker checker = new DataContainerChecker ();
      checker.Check (TestDataContainerFactory.CreateOrder1DataContainer (), orderContainer);
    }

    [Test]
    public void LoadDataContainerOverVirtualEndPoint ()
    {
      DataContainer orderContainer = TestDataContainerFactory.CreateOrder1DataContainer ();

      DataContainer orderTicketContainer = _persistenceManager.LoadRelatedDataContainer (
          orderContainer, new RelationEndPointID (orderContainer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));

      DataContainerChecker checker = new DataContainerChecker ();
      checker.Check (TestDataContainerFactory.CreateOrderTicket1DataContainer (), orderTicketContainer);
    }

    [Test]
    public void LoadRelatedDataContainerByOptionalNullID ()
    {
      ObjectID id = new ObjectID ("ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      DataContainer dataContainer = _persistenceManager.LoadDataContainer (id);

      DataContainer relatedDataContainer = _persistenceManager.LoadRelatedDataContainer (
          dataContainer, new RelationEndPointID (dataContainer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional"));

      Assert.IsNull (relatedDataContainer);
    }

    [Test]
    public void LoadRelatedDataContainerByOptionalNullIDVirtual ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      DataContainer dataContainer = _persistenceManager.LoadDataContainer (id);

      DataContainer relatedDataContainer = _persistenceManager.LoadRelatedDataContainer (
          dataContainer, new RelationEndPointID (dataContainer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional"));

      Assert.IsNull (relatedDataContainer);
    }

    [Test]
    public void LoadRelatedDataContainerByNonOptionalNullID ()
    {
      ClassDefinition classWithValidRelations = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("ClassWithValidRelations");
      DataContainer dataContainer = CreateDataContainer (classWithValidRelations);

      try
      {
        _persistenceManager.LoadRelatedDataContainer (
            dataContainer, new RelationEndPointID (dataContainer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyNonOptional"));

        Assert.Fail ("Test expects a PersistenceException.");
      }
      catch (PersistenceException e)
      {
        Assert.AreEqual (typeof (PersistenceException), e.GetType ());

        string expectedMessage = string.Format (
            "Cannot load related DataContainer of object 'ClassWithValidRelations|{0}|System.Guid'"
            + " over mandatory relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyNonOptional'.", dataContainer.ID.Value);

        Assert.AreEqual (expectedMessage, e.Message);
      }
    }

    private DataContainer CreateDataContainer (ClassDefinition classDefinition)
    {
      return DataContainer.CreateNew (_persistenceManager.CreateNewObjectID (classDefinition));
    }

    [Test]
    public void LoadRelatedDataContainerByNonOptionalNullIDWithInheritance ()
    {
      ClassDefinition distributorClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Distributor");
      DataContainer dataContainer = CreateDataContainer (distributorClass);

      try
      {
        _persistenceManager.LoadRelatedDataContainer (dataContainer, new RelationEndPointID (dataContainer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson"));
        Assert.Fail ("Test expects a PersistenceException.");
      }
      catch (PersistenceException e)
      {
        Assert.AreEqual (typeof (PersistenceException), e.GetType ());

        string expectedMessage = string.Format (
            "Cannot load related DataContainer of object 'Distributor|{0}|System.Guid'"
            + " over mandatory relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson'.", dataContainer.ID.Value);

        Assert.AreEqual (expectedMessage, e.Message);
      }
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = 
        "Cannot load related DataContainer of object 'ClassWithGuidKey|672c8754-c617-4b7a-890c-bfef8ac86564|System.Guid'"
       + " over mandatory relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyNonOptional'.")]
    public void LoadRelatedDataContainerByNonOptionalNullIDVirtual ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      DataContainer dataContainer = _persistenceManager.LoadDataContainer (id);

      _persistenceManager.LoadRelatedDataContainer (
          dataContainer, new RelationEndPointID (dataContainer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = 
        "Cannot load related DataContainer of object 'Partner|a65b123a-6e17-498e-a28e-946217c0ae30|System.Guid'"
       + " over mandatory relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company'.")]
    public void LoadRelatedDataContainerByNonOptionalNullIDVirtualWithInheritance ()
    {
      DataContainer dataContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.PartnerWithoutCeo);

      _persistenceManager.LoadRelatedDataContainer (
          dataContainer, new RelationEndPointID (dataContainer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo"));
    }

    [Test]
    public void LoadRelatedDataContainerOverValidMandatoryRelation ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{D0A1BDDE-B13F-47c1-98BD-EBAE21189B01}"));

      DataContainer classWithGuidKey = _persistenceManager.LoadDataContainer (id);

      DataContainer relatedContainer = _persistenceManager.LoadRelatedDataContainer (
          classWithGuidKey, new RelationEndPointID (classWithGuidKey.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional"));

      ObjectID expectedID = new ObjectID ("ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      Assert.IsNotNull (relatedContainer);
      Assert.AreEqual (expectedID, relatedContainer.ID);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = 
        "Cannot load related DataContainer of object 'ClassWithGuidKey|672c8754-c617-4b7a-890c-bfef8ac86564|System.Guid'"
        + " over mandatory relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyNonOptional'.")]
    public void LoadRelatedDataContainerOverInvalidNonOptionalRelation ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      DataContainer classWithGuidKey = _persistenceManager.LoadDataContainer (id);

      DataContainer relatedContainer = _persistenceManager.LoadRelatedDataContainer (
          classWithGuidKey, new RelationEndPointID (classWithGuidKey.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = 
        "Property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithInvalidRelation.ClassWithGuidKey' of object "
        + "'ClassWithInvalidRelation|afa9cf46-8e77-4da8-9793-53caa86a277c|System.Guid' refers"
        + " to non-existing object 'ClassWithGuidKey|a53f679d-0e91-4504-aee8-59250de249b3|System.Guid'.")]
    public void LoadRelatedDataContainerByInvalidID ()
    {
      ObjectID id = new ObjectID ("ClassWithInvalidRelation", new Guid ("{AFA9CF46-8E77-4da8-9793-53CAA86A277C}"));

      DataContainer dataContainer = _persistenceManager.LoadDataContainer (id);

      _persistenceManager.LoadRelatedDataContainer (
          dataContainer, new RelationEndPointID (dataContainer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithInvalidRelation.ClassWithGuidKey"));
    }

    [Test]
    public void LoadRelatedDataContainerFromDifferentProvider ()
    {
      DataContainer orderContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Order1);

      DataContainer officialContainer = _persistenceManager.LoadRelatedDataContainer (
          orderContainer, new RelationEndPointID (orderContainer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official"));

      Assert.IsNotNull (officialContainer);
      Assert.AreEqual ("UnitTestStorageProviderStub", officialContainer.ID.StorageProviderID, "StorageProviderID");
      Assert.AreEqual ("Official", officialContainer.ID.ClassID, "ClassID");
      Assert.AreEqual (1, officialContainer.ID.Value, "Value");
    }

    [Test]
    public void LoadRelatedDataContainers ()
    {
      DataContainerCollection collection = _persistenceManager.LoadRelatedDataContainers (
          new RelationEndPointID (DomainObjectIDs.Customer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.IsNotNull (collection);
      Assert.AreEqual (2, collection.Count, "DataContainerCollection.Count");
      Assert.IsNotNull (collection[DomainObjectIDs.Order1], "ID of Order with OrdnerNo 1");
      Assert.IsNotNull (collection[DomainObjectIDs.OrderWithoutOrderItem], "ID of Order with OrdnerNo 2");
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = 
        "A DataContainerCollection cannot be loaded for a relation with a non-virtual end point, "
        + "relation: 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer', "
        + "property: 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer'. Check your mapping configuration.")]
    public void LoadRelatedDataContainersForNonVirtualEndPoint ()
    {
      _persistenceManager.LoadRelatedDataContainers (new RelationEndPointID (DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
       "Collection for mandatory relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order' "
       + "(property: 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems', object: 'Order|f4016f41-f4e4-429e-b8d1-659c8c480a67|System.Guid') contains no items.")]
    public void LoadEmptyRelatedDataContainersForMandatoryRelation ()
    {
      _persistenceManager.LoadRelatedDataContainers (
          new RelationEndPointID (DomainObjectIDs.OrderWithoutOrderItem, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"));
    }

    [Test]
    public void LoadEmptyRelatedDataContainersForMandatoryRelationWithOptionalOppositeEndPoint ()
    {
      DataContainerCollection orderContainers = _persistenceManager.LoadRelatedDataContainers (
          new RelationEndPointID (DomainObjectIDs.Customer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.AreEqual (0, orderContainers.Count);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = 
        "Cannot load a single related data container for one-to-many relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order'.")]
    public void LoadRelatedDataContainerForOneToManyRelation ()
    {
      DataContainer orderContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Order1);
      _persistenceManager.LoadRelatedDataContainer (
          orderContainer, new RelationEndPointID (orderContainer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = 
        "Cannot load multiple related data containers for one-to-one relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order'.")]
    public void LoadRelatedDataContainersForOneToOneRelation ()
    {
      _persistenceManager.LoadRelatedDataContainers (new RelationEndPointID (DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = "Save does not support multiple storage providers.")]
    public void SaveInDifferentStorageProviders ()
    {
      DataContainer orderContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Order1);
      DataContainer officialContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Official1);
      ClientTransactionMock.SetClientTransaction (orderContainer);
      ClientTransactionMock.SetClientTransaction (officialContainer);

      DataContainerCollection dataContainers = new DataContainerCollection ();
      dataContainers.Add (orderContainer);
      dataContainers.Add (officialContainer);

      orderContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 42;
      officialContainer[typeof (Official).FullName + ".Name"] = "Zaphod"; //Stub implementation

      _persistenceManager.Save (dataContainers);
    }

    [Test]
    public void CreateNewObjectID ()
    {
      ClassDefinition orderClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      ObjectID id1 = _persistenceManager.CreateNewObjectID (orderClass);
      Assert.IsNotNull (id1);
      ObjectID id2 = _persistenceManager.CreateNewObjectID (orderClass);
      Assert.IsNotNull (id2);
      Assert.AreNotEqual (id1, id2);
    }

    [Test]
    public void CreateNewDataContainer ()
    {
      ClassDefinition orderClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      DataContainer container = CreateDataContainer (orderClass);

      Assert.IsNotNull (container);
      Assert.AreEqual (StateType.New, container.State);
      Assert.IsNotNull (container.ID);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'Order|c3f486ae-ba6a-4bac-a084-0ccbf445523e|System.Guid' could not be found.")]
    public void LoadDataContainerWithNonExistingValue ()
    {
      Guid nonExistingID = new Guid ("{C3F486AE-BA6A-4bac-A084-0CCBF445523E}");
      ObjectID id = new ObjectID ("Order", nonExistingID);

      _persistenceManager.LoadDataContainer (id);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException),
        ExpectedMessage = "The ClassID of the provided ObjectID 'Distributor|5587a9c0-be53-477d-8c0a-4803c7fae1a9|System.Guid'"
        + " and the ClassID of the loaded DataContainer 'Partner|5587a9c0-be53-477d-8c0a-4803c7fae1a9|System.Guid' differ.")]
    public void LoadDataContainerWithInvalidClassID ()
    {
      ObjectID id = new ObjectID ("Distributor", (Guid) DomainObjectIDs.Partner1.Value);
      _persistenceManager.LoadDataContainer (id);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = 
        "The property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' of the provided DataContainer "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' refers to ClassID 'Company', but the ClassID of the loaded DataContainer is 'Customer'.")]
    public void LoadRelatedDataContainerWithInvalidClassIDOverEndPoint ()
    {
      DataContainer orderContainer = CreateOrder1DataContainerWithInvalidCustomer ();
      RelationEndPointID endPointID = new RelationEndPointID (orderContainer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");

      _persistenceManager.LoadRelatedDataContainer (orderContainer, endPointID);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "The property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company' of the loaded DataContainer "
        + "'Ceo|c3db20d6-138e-4ced-8576-e81bb4b7961f|System.Guid' refers to ClassID 'Customer', but the actual ClassID is 'Company'.")]
    public void LoadRelatedDataContainerWithInvalidClassIDOverVirtualEndPoint ()
    {
      ObjectID companyID = new ObjectID ("Company", new Guid ("{C3DB20D6-138E-4ced-8576-E81BB4B7961F}"));

      DataContainer companyContainer = _persistenceManager.LoadDataContainer (companyID);
      RelationEndPointID endPointID = new RelationEndPointID (companyContainer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo");

      _persistenceManager.LoadRelatedDataContainer (companyContainer, endPointID);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = 
        "The property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' of the loaded DataContainer "
        + "'Order|da658f26-8107-44ce-9dd0-1804503eccaf|System.Guid' refers to ClassID 'Company', but the actual ClassID is 'Customer'.")]
    public void LoadRelatedDataContainersWithInvalidClassID ()
    {
      ObjectID customerID = new ObjectID ("Customer", new Guid ("{DA658F26-8107-44ce-9DD0-1804503ECCAF}"));

      RelationEndPointID endPointID = new RelationEndPointID (customerID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");

      _persistenceManager.LoadRelatedDataContainers (endPointID);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException),
       ExpectedMessage = "Multiple related DataContainers where found for property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany' of"
        + " DataContainer 'Person|911957d1-483c-4a8b-aa53-ff07464c58f9|System.Guid'.")]
    public void LoadRelatedDataContainersOverOneToOneRelationWithMultipleFound ()
    {
      DataContainer contactPersonInTwoOrganizations = _persistenceManager.LoadDataContainer (DomainObjectIDs.ContactPersonInTwoOrganizations);
      RelationEndPointID endPointID = new RelationEndPointID (contactPersonInTwoOrganizations.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany");

      _persistenceManager.LoadRelatedDataContainer (contactPersonInTwoOrganizations, endPointID);
    }
    
    private DataContainer CreateOrder1DataContainerWithInvalidCustomer ()
    {
      DataContainer dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, 
          delegate (PropertyDefinition propertyDefinition) {
            switch (propertyDefinition.PropertyName)
            {
              case "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber": 
                return 1;
              case "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official":
                return DomainObjectIDs.Official1;
              case "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer":
                return new ObjectID ("Company", (Guid) DomainObjectIDs.Customer1.Value);
              case "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate":
                return new DateTime (2005, 1, 1);
              default:
                return propertyDefinition.DefaultValue;
            }
          });

      ClientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }
  }
}
