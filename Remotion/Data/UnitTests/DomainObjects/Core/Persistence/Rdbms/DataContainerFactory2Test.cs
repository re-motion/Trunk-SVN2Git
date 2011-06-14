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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class DataContainerFactory2Test : SqlProviderBaseTest
  {
    private IDataReader _dataReaderMock;
    private DataContainerFactory2 _factory;
    private MockRepository _mockRepository;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _dataReaderMock = _mockRepository.StrictMock<IDataReader>();
      _factory = new DataContainerFactory2 (Provider.CreateValueConverter());
      OppositeClassDefinitionRetriever.ResetCache ();
    }

    public override void TearDown ()
    {
      base.TearDown ();
      OppositeClassDefinitionRetriever.ResetCache ();
    }

    [Test]
    public void CreateDataContainer_DataReaderReadFalse ()
    {
      _dataReaderMock.Stub (stub => stub.Read()).Return (false);

      var result = _factory.CreateDataContainer(_dataReaderMock);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void CreateDataContainer_DataReaderReadTrue_ValueIDNotNull ()
    {
      SetupOrderTicket (DomainObjectIDs.OrderTicket1, 123, "flop", DomainObjectIDs.Order1, true);
      _mockRepository.ReplayAll ();

      var dataContainer = _factory.CreateDataContainer (_dataReaderMock);
      CheckTicketContainer (dataContainer, DomainObjectIDs.OrderTicket1, 123, "flop", DomainObjectIDs.Order1);
    }
    
    [Test]
    [ExpectedException (typeof (RdbmsProviderException),
        ExpectedMessage = "An object returned from the database had a NULL ID, which is not supported.")]
    public void CreateDataContainer_DataReaderReadFalse_ValueIDNull ()
    {
      SetupOrderTicket (null, 123, "flop", DomainObjectIDs.Order1, true);
      _mockRepository.ReplayAll();

      _factory.CreateDataContainer(_dataReaderMock);
    }

    //TODO RM-4068: add tests for allowNullsTrue and "error while reading property"-exception !?

    [Test]
    public void CreateCollection ()
    {
      using (_mockRepository.Ordered ())
      {
        SetupOrderTicket (DomainObjectIDs.OrderTicket1, 123, "flip", DomainObjectIDs.Order1, true);
        SetupOrderTicket (DomainObjectIDs.OrderTicket2, 124, "flop", DomainObjectIDs.Order2, false);
        SetupOrderTicket (DomainObjectIDs.OrderTicket3, 125, "flap", DomainObjectIDs.Order3, false);
        Expect.Call (_dataReaderMock.Read ()).Return (false);
      }

      _mockRepository.ReplayAll ();

      var dataContainers = _factory.CreateCollection (_dataReaderMock, false);
      Assert.AreEqual (3, dataContainers.Length);
      CheckTicketContainer (dataContainers[0], DomainObjectIDs.OrderTicket1, 123, "flip", DomainObjectIDs.Order1);
      CheckTicketContainer (dataContainers[1], DomainObjectIDs.OrderTicket2, 124, "flop", DomainObjectIDs.Order2);
      CheckTicketContainer (dataContainers[2], DomainObjectIDs.OrderTicket3, 125, "flap", DomainObjectIDs.Order3);

      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "An object returned from the database had a NULL ID, which is not supported.")]
    public void CreateCollection_WithNullID_AllowNullsFalse ()
    {
      using (_mockRepository.Ordered ())
      {
        SetupOrderTicket (DomainObjectIDs.OrderTicket1, 123, "flip", DomainObjectIDs.Order1, true);
        SetupOrderTicket (null, 124, "flop", DomainObjectIDs.Order2, false);
        SetupOrderTicket (DomainObjectIDs.OrderTicket3, 125, "flap", DomainObjectIDs.Order3, false);
        Expect.Call (_dataReaderMock.Read ()).Return (false);
      }

      _mockRepository.ReplayAll ();

      _factory.CreateCollection (_dataReaderMock, false);
    }

    [Test]
    public void CreateCollection_WithNullID_AllowNullsTrue ()
    {
      using (_mockRepository.Ordered ())
      {
        SetupOrderTicket (DomainObjectIDs.OrderTicket1, 123, "flip", DomainObjectIDs.Order1, true);
        SetupOrderTicket (null, 124, "flop", DomainObjectIDs.Order2, false);
        SetupOrderTicket (DomainObjectIDs.OrderTicket3, 125, "flap", DomainObjectIDs.Order3, false);
        Expect.Call (_dataReaderMock.Read ()).Return (false);
      }

      _mockRepository.ReplayAll ();

      var collection = _factory.CreateCollection (_dataReaderMock, true);
      Assert.That (collection.Length, Is.EqualTo (3));
      Assert.That (collection[0].ID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (collection[1], Is.Null);
      Assert.That (collection[2].ID, Is.EqualTo (DomainObjectIDs.OrderTicket3));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "A database query returned duplicates of the domain object "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid', which is not supported.")]
    public void CreateCollection_WithDuplicates ()
    {
      using (_mockRepository.Ordered ())
      {
        SetupOrderTicket (DomainObjectIDs.OrderTicket1, 123, "flip", DomainObjectIDs.Order1, true);
        SetupOrderTicket (DomainObjectIDs.OrderTicket1, 123, "flip", DomainObjectIDs.Order1, false);
        Expect.Call (_dataReaderMock.Read ()).Return (false);
      }

      _mockRepository.ReplayAll ();

      _factory.CreateCollection (_dataReaderMock, false);

      _mockRepository.VerifyAll ();
    }

    private void SetupOrderTicket (ObjectID ticketID, int timestamp, string fileName, ObjectID relatedOrder, bool checkOrderIDClassIDNotExists)
    {
      using (_mockRepository.Unordered())
      {
        Expect.Call (_dataReaderMock.Read()).Return (true);
        Expect.Call (_dataReaderMock.GetOrdinal ("ID")).Return (0);
        Expect.Call (_dataReaderMock.GetValue (0)).Return (ticketID != null ? ticketID.Value : DBNull.Value);
        if (ticketID != null)
        {
          Expect.Call (_dataReaderMock.GetOrdinal ("ClassID")).Return (1);
          Expect.Call (_dataReaderMock.IsDBNull (1)).Return (false);
          Expect.Call (_dataReaderMock.GetString (1)).Return (ticketID.ClassID);
          Expect.Call (_dataReaderMock.GetOrdinal ("Timestamp")).Return (2);
          Expect.Call (_dataReaderMock.GetValue (2)).Return (timestamp);
          Expect.Call (_dataReaderMock.GetOrdinal ("FileName")).Return (3);
          Expect.Call (_dataReaderMock.GetValue (3)).Return (fileName);
          Expect.Call (_dataReaderMock.GetOrdinal ("OrderID")).Return (4);
          Expect.Call (_dataReaderMock.IsDBNull (4)).Return (false);
          if (checkOrderIDClassIDNotExists)
            Expect.Call (_dataReaderMock.GetOrdinal ("OrderIDClassID")).Throw (new IndexOutOfRangeException());
          Expect.Call (_dataReaderMock.GetValue (4)).Return (relatedOrder.Value);
        }
      }
    }

    private void CheckTicketContainer (
        DataContainer dataContainer,
        ObjectID expectedTicketID,
        int expectedTimeStamp,
        string expectedFileName,
        ObjectID expectedRelatedOrderID)
    {
      Assert.AreEqual (expectedTicketID, dataContainer.ID);
      Assert.AreEqual (expectedTimeStamp, dataContainer.Timestamp);
      Assert.AreEqual (
          expectedFileName, dataContainer.PropertyValues[ReflectionMappingHelper.GetPropertyName (typeof (OrderTicket), "FileName")].Value);
      Assert.AreEqual (
          expectedRelatedOrderID, dataContainer.PropertyValues[ReflectionMappingHelper.GetPropertyName (typeof (OrderTicket), "Order")].Value);
    }
  }
}