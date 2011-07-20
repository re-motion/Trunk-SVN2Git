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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderLoadDataContainersTest : SqlProviderBaseTest
  {
    [Test]
    public void LoadMultipleDataContainers ()
    {
      var ids = new[] { DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1, DomainObjectIDs.Order1, DomainObjectIDs.OrderItem2};

      var containers = Provider.LoadDataContainers (ids).ToArray();

      Assert.IsNotNull (containers);
      Assert.AreEqual (ids[0], containers[0].LocatedDataContainer.ID);
      Assert.AreEqual (ids[1], containers[1].LocatedDataContainer.ID);
      Assert.AreEqual (ids[2], containers[2].LocatedDataContainer.ID);
      Assert.AreEqual (ids[3], containers[3].LocatedDataContainer.ID);
    }

    [Test]
    public void LoadDataContainersWithGuidID ()
    {
      var id = new ObjectID ("ClassWithGuidKey", new Guid ("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

      var container = Provider.LoadDataContainers (new[] { id }).ToArray()[0].LocatedDataContainer;

      Assert.IsNotNull (container);
      Assert.AreEqual (container.ID, id);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Error while executing SQL command.")]
    public void LoadDataContainersWithInvalidIDType ()
    {
      var id = new ObjectID ("ClassWithKeyOfInvalidType", new Guid ("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

      try
      {
        var container = Provider.LoadDataContainers (new[] { id }).ToArray()[0];
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
      var id = new ObjectID ("ClassWithoutIDColumn", new Guid ("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

      try
      {
        var container = Provider.LoadDataContainers (new[] { id }).ToArray()[0];
      }
      catch (RdbmsProviderException e)
      {
        Assert.AreEqual (typeof (SqlException), e.InnerException.GetType ());
        throw e;
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Error while executing SQL command.")]
    public void LoadDataContainersWithoutClassIDColumn ()
    {
      var id = new ObjectID ("ClassWithoutClassIDColumn", new Guid ("{DDD02092-355B-4820-90B6-7F1540C0547E}"));

      Provider.LoadDataContainers (new[] { id });
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Error while executing SQL command.")]
    public void LoadDataContainersWithoutTimestampColumn ()
    {
      var id = new ObjectID ("ClassWithoutTimestampColumn", new Guid ("{027DCBD7-ED68-461d-AE80-B8E145A7B816}"));

      Provider.LoadDataContainers (new[] { id });
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Mapping does not contain class 'NonExistingClassID'.")]
    public void LoadDataContainersWithNonExistingClassID ()
    {
      var id = new ObjectID ("ClassWithGuidKey", new Guid ("{C9F16F93-CF42-4357-B87B-7493882AAEAF}"));

      Provider.LoadDataContainers (new[] { id });
    }

    //TODO: Improove this message to state that the passed ClassID and the ClassID in the database to not match.
    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = 
        "Error while reading property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber' of object "
        + "'Order|895853eb-06cd-4291-b467-160560ae8ec1|System.Guid': The column 'OrderNo' could not be found.\r\nParameter name: columnDefinition")]
    public void LoadDataContainersWithClassIDFromOtherClass ()
    {
      var id = new ObjectID ("ClassWithGuidKey", new Guid ("{895853EB-06CD-4291-B467-160560AE8EC1}"));

      Provider.LoadDataContainers (new[] { id });
    }

    [Test]
    public void LoadDataContainersByNonExistingID ()
    {
      var id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{E067A627-BA3F-4ee5-8B61-1F46DC28DFC3}"));

      var result = Provider.LoadDataContainers (new[] { id }).ToList();

      Assert.That (result.Count, Is.EqualTo(1));
      Assert.That (result[0].LocatedDataContainer, Is.Null);
      Assert.That (result[0].ObjectID, Is.SameAs(id));
    }

    [Test]
    public void LoadDataContainersByID ()
    {
      var id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      var actualContainer = Provider.LoadDataContainers (new[] { id }).ToArray()[0].LocatedDataContainer;

      var expectedContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();

      var checker = new DataContainerChecker ();
      checker.Check (expectedContainer, actualContainer);
    }

    [Test]
    public void LoadDerivedDataContainerByID ()
    {
      var actualContainer = Provider.LoadDataContainers (new[] { DomainObjectIDs.Partner1 }).ToArray()[0].LocatedDataContainer;
      var expectedContainer = TestDataContainerFactory.CreatePartner1DataContainer ();

      var checker = new DataContainerChecker ();
      checker.Check (expectedContainer, actualContainer);
    }

    [Test]
    public void LoadTwiceDerivedDataContainerByID ()
    {
      var actualContainer = Provider.LoadDataContainers (new[] { DomainObjectIDs.Distributor2 }).ToArray()[0].LocatedDataContainer;
      var expectedContainer = TestDataContainerFactory.CreateDistributor2DataContainer ();

      var checker = new DataContainerChecker ();
      checker.Check (expectedContainer, actualContainer);
    }

    [Test]
    public void LoadDataContainersWithNullForeignKey ()
    {
      var id = new ObjectID ("ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      var container = Provider.LoadDataContainers (new[] { id }).ToArray()[0].LocatedDataContainer;

      var actualPropertyValue = container.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional"];

      Assert.IsNotNull (actualPropertyValue, "PropertyValue");
      Assert.IsNull (actualPropertyValue.Value, "PropertyValue.Value");
    }

    [Test]
    public void LoadDataContainersWithRelation ()
    {
      var orderTicketContainer = Provider.LoadDataContainers (new[] { DomainObjectIDs.OrderTicket1 }).ToArray()[0].LocatedDataContainer;
      Assert.AreEqual (DomainObjectIDs.Order1, orderTicketContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"));
    }

    [Test]
    public void LoadDataContainersWithRelationAndInheritance ()
    {
      var ceoContainer = Provider.LoadDataContainers (new[] { DomainObjectIDs.Ceo7 }).ToArray()[0].LocatedDataContainer;
      Assert.AreEqual (DomainObjectIDs.Partner2, ceoContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company"));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Error while executing SQL command.")]
    public void LoadDataContainersWithoutRelatedIDColumn ()
    {
      var id = new ObjectID ("ClassWithoutRelatedClassIDColumn", new Guid ("{CD3BE83E-FBB7-4251-AAE4-B216485C5638}"));

      Provider.LoadDataContainers (new[] { id });
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Error while executing SQL command.")]
    public void LoadDataContainersWithoutRelatedIDColumnAndDerivation ()
    {
      var id = new ObjectID ("ClassWithoutRelatedClassIDColumnAndDerivation",
          new Guid ("{4821D7F7-B586-4435-B572-8A96A44B113E}"));

      Provider.LoadDataContainers (new[] { id });
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The StorageProviderID 'UnitTestStorageProviderStub' of the provided ObjectID 'Official|1|System.Int32' does not match with this "
        + "StorageProvider's ID 'TestDomain'.\r\nParameter name: ids")]
    public void LoadDataContainersWithObjectIDWithWrongStorageProviderID ()
    {
      var invalidID = new ObjectID (DomainObjectIDs.Official1.ClassID, (int) DomainObjectIDs.Official1.Value);

      Provider.LoadDataContainers (new[] { invalidID });
    }

    [Ignore("TODO: RM-4148")]
    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = 
        "Error while reading property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey' "
        +"of object 'ClassWithRelatedClassIDColumnAndNoInheritance|cb72715d-f419-4ab9-8d49-abcba4e9edb4|System.Guid':"
        + " Incorrect database format encountered."
        + " Entity 'TableWithRelatedClassIDColumnAndNoInheritance' must not contain column 'TableWithGuidKeyIDClassID',"
        + " because opposite class 'ClassWithGuidKey' is not part of an inheritance hierarchy.")]
    public void LoadDataContainersWithRelatedClassIDColumnAndNoInheritance ()
    {
      var id = new ObjectID ("ClassWithRelatedClassIDColumnAndNoInheritance", new Guid ("{CB72715D-F419-4ab9-8D49-ABCBA4E9EDB4}"));

      Provider.LoadDataContainers (new[] { id });
    }
  }
}
