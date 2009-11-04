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
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderLoadDataContainersTest : SqlProviderBaseTest
  {
    [Test]
    public void LoadMultipleDataContainers ()
    {
      ObjectID[] ids = new ObjectID[] { DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1, DomainObjectIDs.Order1, DomainObjectIDs.OrderItem2};

      DataContainerCollection containers = Provider.LoadDataContainers (ids);

      Assert.IsNotNull (containers);
      Assert.AreEqual (ids[0], containers[0].ID);
      Assert.AreEqual (ids[1], containers[1].ID);
      Assert.AreEqual (ids[2], containers[2].ID);
      Assert.AreEqual (ids[3], containers[3].ID);
    }

    [Test]
    public void LoadDataContainersWithGuidID ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

      DataContainer container = Provider.LoadDataContainers (new ObjectID[] { id })[0];

      Assert.IsNotNull (container);
      Assert.AreEqual (container.ID, id);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Error while executing SQL command.")]
    public void LoadDataContainersWithInvalidIDType ()
    {
      ObjectID id = new ObjectID ("ClassWithKeyOfInvalidType", new Guid ("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

      try
      {
        DataContainer container = Provider.LoadDataContainers (new ObjectID[] { id })[0];
      }
      catch (RdbmsProviderException e)
      {
        Assert.AreEqual (typeof (SqlException), e.InnerException.GetType ());
        throw e;
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Error while executing SQL command.")]
    public void LoadDataContainersWithoutIDColumn ()
    {
      ObjectID id = new ObjectID ("ClassWithoutIDColumn", new Guid ("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

      try
      {
        DataContainer container = Provider.LoadDataContainers (new ObjectID[] { id })[0];
      }
      catch (RdbmsProviderException e)
      {
        Assert.AreEqual (typeof (SqlException), e.InnerException.GetType ());
        throw e;
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "The mandatory column 'ClassID' could not be found.")]
    public void LoadDataContainersWithoutClassIDColumn ()
    {
      ObjectID id = new ObjectID ("ClassWithoutClassIDColumn", new Guid ("{DDD02092-355B-4820-90B6-7F1540C0547E}"));

      Provider.LoadDataContainers (new ObjectID[] { id });
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "The mandatory column 'Timestamp' could not be found.")]
    public void LoadDataContainersWithoutTimestampColumn ()
    {
      ObjectID id = new ObjectID ("ClassWithoutTimestampColumn", new Guid ("{027DCBD7-ED68-461d-AE80-B8E145A7B816}"));

      Provider.LoadDataContainers (new ObjectID[] { id });
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException),
        ExpectedMessage = "Invalid ClassID 'NonExistingClassID' for ID 'c9f16f93-cf42-4357-b87b-7493882aaeaf' encountered.")]
    public void LoadDataContainersWithNonExistingClassID ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{C9F16F93-CF42-4357-B87B-7493882AAEAF}"));

      Provider.LoadDataContainers (new ObjectID[] { id });
    }

    //TODO: Improove this message to state that the passed ClassID and the ClassID in the database to not match.
    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = 
        "Error while reading property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber' of object "
        + "'Order|895853eb-06cd-4291-b467-160560ae8ec1|System.Guid': The mandatory column 'OrderNo' could not be found.")]
    public void LoadDataContainersWithClassIDFromOtherClass ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{895853EB-06CD-4291-B467-160560AE8EC1}"));

      Provider.LoadDataContainers (new ObjectID[] { id });
    }

    [Test]
    public void LoadDataContainersByNonExistingID ()
    {
      ObjectID id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{E067A627-BA3F-4ee5-8B61-1F46DC28DFC3}"));

      Assert.IsEmpty (Provider.LoadDataContainers (new ObjectID[] { id }));
    }

    [Test]
    public void LoadDataContainersByID ()
    {
      ObjectID id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      DataContainer actualContainer = Provider.LoadDataContainers (new ObjectID[] { id })[0];

      DataContainer expectedContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();

      DataContainerChecker checker = new DataContainerChecker ();
      checker.Check (expectedContainer, actualContainer);
    }

    [Test]
    public void LoadDerivedDataContainerByID ()
    {
      DataContainer actualContainer = Provider.LoadDataContainers (new ObjectID[] { DomainObjectIDs.Partner1 })[0];
      DataContainer expectedContainer = TestDataContainerFactory.CreatePartner1DataContainer ();

      DataContainerChecker checker = new DataContainerChecker ();
      checker.Check (expectedContainer, actualContainer);
    }

    [Test]
    public void LoadTwiceDerivedDataContainerByID ()
    {
      DataContainer actualContainer = Provider.LoadDataContainers (new ObjectID[] { DomainObjectIDs.Distributor2 })[0];
      DataContainer expectedContainer = TestDataContainerFactory.CreateDistributor2DataContainer ();

      DataContainerChecker checker = new DataContainerChecker ();
      checker.Check (expectedContainer, actualContainer);
    }

    [Test]
    public void LoadDataContainersWithNullForeignKey ()
    {
      ObjectID id = new ObjectID ("ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      DataContainer container = Provider.LoadDataContainers (new ObjectID[] { id })[0];

      PropertyValue actualPropertyValue = container.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional"];

      Assert.IsNotNull (actualPropertyValue, "PropertyValue");
      Assert.IsNull (actualPropertyValue.Value, "PropertyValue.Value");
    }

    [Test]
    public void LoadDataContainersWithRelation ()
    {
      DataContainer orderTicketContainer = Provider.LoadDataContainers (new ObjectID[] { DomainObjectIDs.OrderTicket1 })[0];
      Assert.AreEqual (DomainObjectIDs.Order1, orderTicketContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"));
    }

    [Test]
    public void LoadDataContainersWithRelationAndInheritance ()
    {
      DataContainer ceoContainer = Provider.LoadDataContainers (new ObjectID[] { DomainObjectIDs.Ceo7 })[0];
      Assert.AreEqual (DomainObjectIDs.Partner2, ceoContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company"));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = 
        "Error while reading property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithoutRelatedClassIDColumn.Distributor' "
        + "of object 'ClassWithoutRelatedClassIDColumn|cd3be83e-fbb7-4251-aae4-b216485c5638|System.Guid':"
        + " Incorrect database format encountered."
        + " Entity 'TableWithoutRelatedClassIDColumn' must have column 'DistributorIDClassID' defined, because opposite class 'Distributor' is part of an inheritance hierarchy.")]
    public void LoadDataContainersWithoutRelatedIDColumn ()
    {
      ObjectID id = new ObjectID ("ClassWithoutRelatedClassIDColumn", new Guid ("{CD3BE83E-FBB7-4251-AAE4-B216485C5638}"));

      Provider.LoadDataContainers (new ObjectID[] { id });
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = 
        "Error while reading property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithoutRelatedClassIDColumnAndDerivation.Company' "
        + "of object 'ClassWithoutRelatedClassIDColumnAndDerivation|4821d7f7-b586-4435-b572-8a96a44b113e|System.Guid':"
        + " Incorrect database format encountered."
        + " Entity 'TableWithoutRelatedClassIDColumnAndDerivation' must have column 'CompanyIDClassID' defined, because opposite class 'Company' is part of an inheritance hierarchy.")]
    public void LoadDataContainersWithoutRelatedIDColumnAndDerivation ()
    {
      ObjectID id = new ObjectID ("ClassWithoutRelatedClassIDColumnAndDerivation",
          new Guid ("{4821D7F7-B586-4435-B572-8A96A44B113E}"));

      Provider.LoadDataContainers (new ObjectID[] { id });
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The StorageProviderID 'UnitTestStorageProviderStub' of the provided ObjectID 'Official|1|System.Int32' does not match with this "
        + "StorageProvider's ID 'TestDomain'.\r\nParameter name: ids")]
    public void LoadDataContainersWithObjectIDWithWrongStorageProviderID ()
    {
      ObjectID invalidID = new ObjectID (DomainObjectIDs.Official1.ClassID, (int) DomainObjectIDs.Official1.Value);

      Provider.LoadDataContainers (new ObjectID[] { invalidID });
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = 
        "Error while reading property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey' "
        +"of object 'ClassWithRelatedClassIDColumnAndNoInheritance|cb72715d-f419-4ab9-8d49-abcba4e9edb4|System.Guid':"
        + " Incorrect database format encountered."
        + " Entity 'TableWithRelatedClassIDColumnAndNoInheritance' must not contain column 'TableWithGuidKeyIDClassID',"
        + " because opposite class 'ClassWithGuidKey' is not part of an inheritance hierarchy.")]
    public void LoadDataContainersWithRelatedClassIDColumnAndNoInheritance ()
    {
      ObjectID id = new ObjectID ("ClassWithRelatedClassIDColumnAndNoInheritance", new Guid ("{CB72715D-F419-4ab9-8D49-ABCBA4E9EDB4}"));

      Provider.LoadDataContainers (new ObjectID[] { id });
    }
  }
}
