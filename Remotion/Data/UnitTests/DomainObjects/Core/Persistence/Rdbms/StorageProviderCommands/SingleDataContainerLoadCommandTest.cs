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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class SingleDataContainerLoadCommandTest : SqlProviderBaseTest
  {
    private IDbCommandBuilder _dbCommandBuilderStub;
    private IDbCommandFactory _dbCommandFactory;
    private IDbCommand _dbCommandStub;
    private IDbCommandExecutor _dbCommandExecutorStub;
    private MockRepository _mockRepository;
    private IDataReader _dataReaderMock;
    private SingleDataContainerLoadCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _dataReaderMock = _mockRepository.StrictMock<IDataReader>();

      _dbCommandFactory = MockRepository.GenerateStub<IDbCommandFactory>();
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbCommandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dbCommandBuilderStub.Stub (stub => stub.Create (_dbCommandFactory)).Return (_dbCommandStub);

      _dbCommandExecutorStub = MockRepository.GenerateStub<IDbCommandExecutor>();
      _dbCommandExecutorStub.Stub (stub => stub.ExecuteReader (_dbCommandStub, CommandBehavior.SingleRow)).Return(_dataReaderMock);

      _command = new SingleDataContainerLoadCommand (
          _dbCommandBuilderStub, _dbCommandFactory, _dbCommandExecutorStub, Provider.CreateValueConverter());
    }

    [Test]
    public void Execute ()
    {
      SetupOrderTicket (DomainObjectIDs.OrderTicket1, 123, "flop", DomainObjectIDs.Order1, true);
      _mockRepository.ReplayAll ();

      var dataContainer = _command.Execute();
      CheckTicketContainer (dataContainer, DomainObjectIDs.OrderTicket1, 123, "flop", DomainObjectIDs.Order1);
    }

    private void SetupOrderTicket (ObjectID ticketID, int timestamp, string fileName, ObjectID relatedOrder, bool checkOrderIDClassIDNotExists)
    {
      using (_mockRepository.Unordered ())
      {
        Expect.Call (_dataReaderMock.Read ()).Return (true);
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
            Expect.Call (_dataReaderMock.GetOrdinal ("OrderIDClassID")).Throw (new IndexOutOfRangeException ());
          Expect.Call (_dataReaderMock.GetValue (4)).Return (relatedOrder.Value);
          _dataReaderMock.Expect (mock => mock.Dispose());
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