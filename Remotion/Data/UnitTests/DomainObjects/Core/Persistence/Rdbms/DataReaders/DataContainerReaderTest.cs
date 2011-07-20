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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class DataContainerReaderTest : SqlProviderBaseTest
  {
    private IDataReader _dataReaderStub;
    private DataContainerReader _dataContainerReader;
    private ObjectID _objectID;
    private object _timestamp;
    private IRdbmsStoragePropertyDefinition _idPropertyStub;
    private IRdbmsStoragePropertyDefinition _timestampPropertyStub;
    private IColumnOrdinalProvider _ordinalProviderStub;
    private IRdbmsPersistenceModelProvider _persistenceModelProviderStub;
    private IRdbmsStoragePropertyDefinition _orderPropertyStub;
    private IRdbmsStoragePropertyDefinition _fileNamePropertyStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _idPropertyStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _timestampPropertyStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _fileNamePropertyStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition> ();
      _orderPropertyStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition> ();
      
      _ordinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();
      _persistenceModelProviderStub = MockRepository.GenerateStub<IRdbmsPersistenceModelProvider>();

      _dataContainerReader = new DataContainerReader (_idPropertyStub, _timestampPropertyStub, _ordinalProviderStub, _persistenceModelProviderStub);

      _objectID = new ObjectID ("OrderTicket", Guid.NewGuid());
      _timestamp = new object();
    }

    [Test]
    public void Read_DataReaderReadFalse ()
    {
      _dataReaderStub.Stub (stub => stub.Read()).Return (false);

      var result = _dataContainerReader.Read (_dataReaderStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void Read_DataReaderReadTrue_ValueIDNotNull ()
    {
      _dataReaderStub.Stub (stub => stub.Read()).Return (true);

      StubPersistenceModelProviderForProperty (typeof (OrderTicket), "FileName", _fileNamePropertyStub);
      StubPersistenceModelProviderForProperty (typeof (OrderTicket), "Order", _orderPropertyStub);

      StubPropertyReadsForOrderTicket (DomainObjectIDs.OrderTicket1, 17, "abc", DomainObjectIDs.Order1);

      var dataContainer = _dataContainerReader.Read (_dataReaderStub);

      Assert.That (dataContainer, Is.Not.Null);
      CheckLoadedDataContainer(dataContainer, DomainObjectIDs.OrderTicket1, 17, "abc", DomainObjectIDs.Order1);
    }

    [Test]
    public void Read_DataReaderReadTrue_ValueIDNull ()
    {
      _dataReaderStub.Stub (stub => stub.Read ()).Return (true);

      _idPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (null);
      _timestampPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).WhenCalled (mi => Assert.Fail ("Should not be called."));

      var dataContainer = _dataContainerReader.Read (_dataReaderStub);

      Assert.That (dataContainer, Is.Null);
    }

    [Test]
    [ExpectedException(typeof(RdbmsProviderException), ExpectedMessage =
      "Error while reading property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.FileName' of object 'OrderTicket*", 
      MatchType = MessageMatch.Regex)]
    public void Read_DataReaderReadTrue_ThrowsException ()
    {
      _dataReaderStub.Stub (stub => stub.Read ()).Return (true);

      _idPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (_objectID);
      _timestampPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (_timestamp);
      var propertyDefinition = GetPropertyDefinition (typeof(OrderTicket), "FileName");
      _persistenceModelProviderStub
        .Stub (stub => stub.GetColumnDefinition (propertyDefinition))
        .WhenCalled (mi => { throw new InvalidOperationException ("TestException"); });
      
      _dataContainerReader.Read (_dataReaderStub);
    }

    [Test]
    public void ReadSequence_DataReaderReadFalse ()
    {
      _dataReaderStub.Stub (stub => stub.Read()).Return (false);

      var result = _dataContainerReader.ReadSequence (_dataReaderStub);

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void ReadSequence_DataReaderReadTrue ()
    {
      StubPersistenceModelProviderForProperty (typeof (OrderTicket), "FileName", _fileNamePropertyStub);
      StubPersistenceModelProviderForProperty (typeof (OrderTicket), "Order", _orderPropertyStub);

      StubPropertyReadsForOrderTicket (DomainObjectIDs.OrderTicket1, 0, "first", DomainObjectIDs.Order1);
      StubPropertyReadsForOrderTicket (DomainObjectIDs.OrderTicket2, 1, "second", DomainObjectIDs.Order2);
      StubPropertyReadsForOrderTicket (DomainObjectIDs.OrderTicket3, 2, "third", DomainObjectIDs.Order3);

      _dataReaderStub.Stub (stub => stub.Read()).Return (true).Repeat.Times (3);
      _dataReaderStub.Stub (stub => stub.Read()).Return (false);

      var result = _dataContainerReader.ReadSequence (_dataReaderStub).ToArray ();

      Assert.That (result.Length, Is.EqualTo (3));

      CheckLoadedDataContainer (result[0], DomainObjectIDs.OrderTicket1, 0, "first", DomainObjectIDs.Order1);
      CheckLoadedDataContainer (result[1], DomainObjectIDs.OrderTicket2, 1, "second", DomainObjectIDs.Order2);
      CheckLoadedDataContainer (result[2], DomainObjectIDs.OrderTicket3, 2, "third", DomainObjectIDs.Order3);
    }

    [Test]
    public void ReadSequence_DataReaderReadTrue_NullIDIsReturned ()
    {
      _dataReaderStub.Stub (stub => stub.Read()).Return (true).Repeat.Once();
      _dataReaderStub.Stub (stub => stub.Read()).Return (false);
      
      _idPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (null);
      _timestampPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).WhenCalled (mi => Assert.Fail ("Should not be called."));
      
      var result = _dataContainerReader.ReadSequence (_dataReaderStub).ToArray();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result[0], Is.Null);
    }

    private void StubPropertyReadsForOrderTicket (ObjectID objectID, object timestamp, string fileName, ObjectID order)
    {
      _idPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (objectID).Repeat.Once ();
      _timestampPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (timestamp).Repeat.Once ();
      _fileNamePropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (fileName).Repeat.Once ();
      _orderPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (order).Repeat.Once ();
    }

    private void StubPersistenceModelProviderForProperty (
        Type declaringType, string shortPropertyName, IRdbmsStoragePropertyDefinition storagePropertyDefinitionStub)
    {
      var propertyDefinition = GetPropertyDefinition (declaringType, shortPropertyName);
      _persistenceModelProviderStub.Stub (stub => stub.GetColumnDefinition (propertyDefinition)).Return (storagePropertyDefinitionStub);
    }

    private void CheckLoadedDataContainer (DataContainer dataContainer, ObjectID expectedID, int expectedTimestamp, string expectedFileName, ObjectID expectedOrder)
    {
      Assert.That (dataContainer.ID, Is.EqualTo (expectedID));
      Assert.That (dataContainer.Timestamp, Is.EqualTo (expectedTimestamp));

      Assert.That (dataContainer.PropertyValues[typeof (OrderTicket).FullName + ".FileName"].Value, Is.EqualTo (expectedFileName));
      Assert.That (dataContainer.PropertyValues[typeof (OrderTicket).FullName + ".Order"].Value, Is.EqualTo (expectedOrder));
    }
  }
}