// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Mocks_List = Rhino.Mocks.Constraints.List;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence
{
  [TestFixture]
  public class PersistenceManagerTest : ClientTransactionBaseTest
  {
    private PersistenceManager _persistenceManager;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _persistenceManager = new PersistenceManager (NullPersistenceExtension.Instance);
    }

    public override void TearDown ()
    {
      _persistenceManager.Dispose ();
      base.TearDown ();
    }

    [Test]
    public void Initialize ()
    {
      var persistenceTracer = MockRepository.GenerateStub<IPersistenceExtension>();
      using (var persistenceManager = new PersistenceManager (persistenceTracer))
      {
        Assert.That (persistenceManager.StorageProviderManager, Is.Not.Null);

        using (var storageProvider = persistenceManager.StorageProviderManager.GetMandatory (c_testDomainProviderID))
        {
          Assert.That (storageProvider.PersistenceExtension, Is.SameAs (persistenceTracer));
        }
      }
    }

    [Test]
    public void LoadDataContainer ()
    {
      var actualDataContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (DomainObjectIDs.Order1, actualDataContainer.ID);
    }

    [Test]
    public void LoadDataContainers ()
    {
      Assert.AreNotEqual (DomainObjectIDs.Order1.StorageProviderDefinition.Name, DomainObjectIDs.Official1, "Different storage providers");
      var officialStorageProvider =
          (UnitTestStorageProviderStub)
          _persistenceManager.StorageProviderManager.GetMandatory (DomainObjectIDs.Official1.StorageProviderDefinition.Name);
      var storageNameProvider = new ReflectionBasedStorageNameProvider();

      var mockRepository = new MockRepository();
      var mockProvider = mockRepository.StrictMock<StorageProvider> (
          officialStorageProvider.StorageProviderDefinition, storageNameProvider, SqlDialect.Instance, NullPersistenceExtension.Instance);

      var officialDC1 = DataContainer.CreateNew (DomainObjectIDs.Official1);
      var officialDC2 = DataContainer.CreateNew (DomainObjectIDs.Official2);

      var officialDCs = new List<ObjectLookupResult<DataContainer>>();
      officialDCs.Add (new ObjectLookupResult<DataContainer>(DomainObjectIDs.Official1, officialDC1));
      officialDCs.Add (new ObjectLookupResult<DataContainer>(DomainObjectIDs.Official2, officialDC2));

      Expect.Call (mockProvider.LoadDataContainers (null)).Constraints (
          Mocks_List.Equal (
              new object[]
              {
                  DomainObjectIDs.Official1,
                  DomainObjectIDs.Official2
              })).Return (officialDCs);

      mockRepository.ReplayAll();

      DataContainerCollection actualDataContainers;
      using (UnitTestStorageProviderStub.EnterMockStorageProviderScope (mockProvider))
      {
        actualDataContainers = _persistenceManager.LoadDataContainers (
            new[]
            {
                DomainObjectIDs.Order1, DomainObjectIDs.Official1, DomainObjectIDs.Order2,
                DomainObjectIDs.Official2
            },
            true);
      }

      mockRepository.VerifyAll();

      Assert.AreEqual (4, actualDataContainers.Count);
      Assert.AreEqual (DomainObjectIDs.Order1, actualDataContainers[0].ID);
      Assert.AreSame (officialDC1, actualDataContainers[1]);
      Assert.AreEqual (DomainObjectIDs.Order2, actualDataContainers[2].ID);
      Assert.AreSame (officialDC2, actualDataContainers[3]);
    }

    [Test]
    [ExpectedException (typeof (BulkLoadException), ExpectedMessage = "There were errors when loading a bulk of DomainObjects:\r\n"
                                                                      +
                                                                      "Object 'Order|11111111-1111-1111-1111-111111111111|System.Guid' could not be found.\r\n"
                                                                      +
                                                                      "Object 'Order|22222222-2222-2222-2222-222222222222|System.Guid' could not be found.\r\n"
        )]
    public void LoadDataContainers_ThrowOnNotFound ()
    {
      Guid guid1 = new Guid ("11111111111111111111111111111111");
      Guid guid2 = new Guid ("22222222222222222222222222222222");
      _persistenceManager.LoadDataContainers (
          new [] { new ObjectID (typeof (Order), guid1), new ObjectID (typeof (Order), guid2), DomainObjectIDs.Order1 }, true);
    }

    [Test]
    public void LoadDataContainers_NoThrowOnNotFound ()
    {
      Guid guid1 = new Guid ("11111111111111111111111111111111");
      Guid guid2 = new Guid ("22222222222222222222222222222222");
      DataContainerCollection dataContainers = _persistenceManager.LoadDataContainers (
          new [] { new ObjectID (typeof (Order), guid1), new ObjectID (typeof (Order), guid2), DomainObjectIDs.Order1 }, false);
      Assert.AreEqual (1, dataContainers.Count);
      Assert.AreEqual (DomainObjectIDs.Order1, dataContainers[0].ID);
    }

    [Test]
    public void LoadRelatedDataContainer ()
    {
      DataContainer orderTicketContainer = _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create (DomainObjectIDs.Order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));

      var checker = new DataContainerChecker();
      checker.Check (TestDataContainerObjectMother.CreateOrderTicket1DataContainer(), orderTicketContainer);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "LoadRelatedDataContainer can only be used with virtual end points.\r\nParameter name: relationEndPointID")]
    public void LoadRelatedDataContainer_NonVirtualEndPoint ()
    {
      var relationEndPointID = RelationEndPointID.Create (DomainObjectIDs.OrderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
      _persistenceManager.LoadRelatedDataContainer (relationEndPointID);
    }

    [Test]
    public void LoadRelatedDataContainer_OptionalNullID ()
    {
      var id = new ObjectID ("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      DataContainer relatedDataContainer = _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create (id, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional"));

      Assert.IsNull (relatedDataContainer);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "Cannot load related DataContainer of object 'ClassWithGuidKey|672c8754-c617-4b7a-890c-bfef8ac86564|System.Guid' over mandatory relation "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations:"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations.ClassWithGuidKeyNonOptional->"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional'.")]
    public void LoadRelatedDataContainer_NonOptionalNullID ()
    {
      var id = new ObjectID ("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create(id, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "Cannot load related DataContainer of object 'Partner|a65b123a-6e17-498e-a28e-946217c0ae30|System.Guid' over mandatory relation "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo:Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company->"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Ceo'.")]
    public void LoadRelatedDataContainer_NonOptionalNullID_WithInheritance ()
    {
      _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create (DomainObjectIDs.PartnerWithoutCeo, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Ceo"));
    }

    [Test]
    public void LoadRelatedDataContainer_OverValidMandatoryRelation ()
    {
      var id = new ObjectID ("ClassWithGuidKey", new Guid ("{D0A1BDDE-B13F-47c1-98BD-EBAE21189B01}"));

      DataContainer relatedContainer = _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create(id, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional"));

      ObjectID expectedID = new ObjectID ("ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      Assert.IsNotNull (relatedContainer);
      Assert.AreEqual (expectedID, relatedContainer.ID);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "Cannot load related DataContainer of object 'ClassWithGuidKey|672c8754-c617-4b7a-890c-bfef8ac86564|System.Guid' over mandatory relation "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations:"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations.ClassWithGuidKeyNonOptional->"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional'.")]
    public void LoadRelatedDataContainer_OverInvalidNonOptionalRelation ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create (id, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "Cannot load a single related data container for one-to-many relation 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem:"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'.")]
    public void LoadRelatedDataContainer_ForOneToManyRelation ()
    {
      _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create (DomainObjectIDs.Order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "The property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company' of the loaded DataContainer "
        + "'Ceo|c3db20d6-138e-4ced-8576-e81bb4b7961f|System.Guid' refers to ClassID 'Customer', but the actual ClassID is 'Company'.")]
    public void LoadRelatedDataContainerW_ithInvalidClassIDOverVirtualEndPoint ()
    {
      ObjectID companyID = new ObjectID ("Company", new Guid ("{C3DB20D6-138E-4ced-8576-E81BB4B7961F}"));

      RelationEndPointID endPointID = RelationEndPointID.Create (companyID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Ceo");

      _persistenceManager.LoadRelatedDataContainer (endPointID);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException),
        ExpectedMessage =
            "Multiple related DataContainers where found for property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Person.AssociatedPartnerCompany' of"
            + " DataContainer 'Person|911957d1-483c-4a8b-aa53-ff07464c58f9|System.Guid'.")]
    public void LoadRelatedDataContainer_OverOneToOneRelation_WithMultipleFound ()
    {
      var endPointID = RelationEndPointID.Create (
          DomainObjectIDs.ContactPersonInTwoOrganizations,
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Person.AssociatedPartnerCompany");

      _persistenceManager.LoadRelatedDataContainer (endPointID);
    }

    [Test]
    public void LoadRelatedDataContainers ()
    {
      DataContainerCollection collection = _persistenceManager.LoadRelatedDataContainers (
          RelationEndPointID.Create (DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.IsNotNull (collection);
      Assert.AreEqual (2, collection.Count, "DataContainerCollection.Count");
      Assert.IsNotNull (collection[DomainObjectIDs.Order1], "ID of Order with OrdnerNo 1");
      Assert.IsNotNull (collection[DomainObjectIDs.OrderWithoutOrderItem], "ID of Order with OrdnerNo 2");
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "A DataContainerCollection cannot be loaded for a relation with a non-virtual end point, relation: "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order:Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer->Remotion.Data."
        + "UnitTests.DomainObjects.TestDomain.Customer.Orders', "
        + "property: 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer'. Check your mapping configuration.")]
    public void LoadRelatedDataContainers_ForNonVirtualEndPoint ()
    {
      _persistenceManager.LoadRelatedDataContainers (
          RelationEndPointID.Create (DomainObjectIDs.Order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "Collection for mandatory relation 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem:"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' "
        + "(property: 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems', object: "
        + "'Order|f4016f41-f4e4-429e-b8d1-659c8c480a67|System.Guid') contains no items.")]
    public void LoadRelatedDataContainers_Empty_ForMandatoryRelation ()
    {
      _persistenceManager.LoadRelatedDataContainers (
          RelationEndPointID.Create (DomainObjectIDs.OrderWithoutOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"));
    }

    [Test]
    public void LoadRelatedDataContainers_Empty_ForMandatoryRelationWithOptionalOppositeEndPoint ()
    {
      DataContainerCollection orderContainers = _persistenceManager.LoadRelatedDataContainers (
          RelationEndPointID.Create (DomainObjectIDs.Customer2, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.AreEqual (0, orderContainers.Count);
    }


    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "Cannot load multiple related data containers for one-to-one relation 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket:"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order->Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket'.")]
    public void LoadRelatedDataContainers_ForOneToOneRelation ()
    {
      _persistenceManager.LoadRelatedDataContainers (
          RelationEndPointID.Create(DomainObjectIDs.Order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "The property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' of the loaded DataContainer "
        + "'Order|da658f26-8107-44ce-9dd0-1804503eccaf|System.Guid' refers to ClassID 'Company', but the actual ClassID is 'Customer'.")]
    public void LoadRelatedDataContainers_WithInvalidClassID ()
    {
      ObjectID customerID = new ObjectID ("Customer", new Guid ("{DA658F26-8107-44ce-9DD0-1804503ECCAF}"));

      RelationEndPointID endPointID = RelationEndPointID.Create (customerID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");

      _persistenceManager.LoadRelatedDataContainers (endPointID);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = "Save does not support multiple storage providers.")]
    public void SaveInDifferentStorageProviders ()
    {
      DataContainer orderContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Order1);
      DataContainer officialContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Official1);

      ClientTransactionTestHelper.RegisterDataContainer (ClientTransactionMock, orderContainer);
      ClientTransactionTestHelper.RegisterDataContainer (ClientTransactionMock, officialContainer);

      var dataContainers = new DataContainerCollection { orderContainer, officialContainer };

      orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 42;
      officialContainer[typeof (Official).FullName + ".Name"] = "Zaphod"; //Stub implementation

      _persistenceManager.Save (dataContainers);
    }

    [Test]
    public void Save_DeletedDataContainersAreIgnoredForUpdateTimestamps ()
    {
      SetDatabaseModifyable();

      var dataContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.That (dataContainer, Is.Not.Null);
      dataContainer.Delete ();

      var timestampBefore = dataContainer.Timestamp;
      _persistenceManager.Save (new DataContainerCollection { dataContainer });
      Assert.That (dataContainer.Timestamp, Is.SameAs (timestampBefore));

      Assert.That (() => _persistenceManager.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1), Throws.TypeOf<ObjectNotFoundException>());
    }

    [Test]
    public void CreateNewObjectID ()
    {
      ClassDefinition orderClass = MappingConfiguration.Current.GetTypeDefinition (typeof (Order));
      ObjectID id1 = _persistenceManager.CreateNewObjectID (orderClass);
      Assert.IsNotNull (id1);
      ObjectID id2 = _persistenceManager.CreateNewObjectID (orderClass);
      Assert.IsNotNull (id2);
      Assert.AreNotEqual (id1, id2);
    }

    [Test]
    public void CreateNewDataContainer ()
    {
      ClassDefinition orderClass = MappingConfiguration.Current.GetTypeDefinition (typeof (Order));
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
    
    private DataContainer CreateDataContainer (ClassDefinition classDefinition)
    {
      return DataContainer.CreateNew (_persistenceManager.CreateNewObjectID (classDefinition));
    }

  }
}