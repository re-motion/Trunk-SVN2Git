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

      _idPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (_objectID);
      _timestampPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (_timestamp);
      StubPersistenceModelProviderForProperty (typeof (OrderTicket), "FileName", _fileNamePropertyStub);
      StubPersistenceModelProviderForProperty (typeof (OrderTicket), "Order", _orderPropertyStub);
      
      var dataContainer = _dataContainerReader.Read (_dataReaderStub);

      Assert.That (dataContainer, Is.Not.Null);
      Assert.That (dataContainer.ID, Is.SameAs (_objectID));
      Assert.That (dataContainer.Timestamp, Is.SameAs (_timestamp));
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

      _idPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (DomainObjectIDs.OrderTicket1).Repeat.Once();
      _timestampPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (_timestamp);
      _idPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (DomainObjectIDs.OrderTicket2).Repeat.Once ();
      _timestampPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (_timestamp);
      _idPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (DomainObjectIDs.OrderTicket3).Repeat.Once ();
      _timestampPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (_timestamp);

      var count = 0;
      _dataReaderStub.Stub (stub => stub.Read ()).Return (true).WhenCalled (
          mi =>
          {
            count++;
            if (count > 2)
            {
              _dataReaderStub.BackToRecord ();
              _dataReaderStub.Stub (stub => stub.Read ()).Return (false);
            }
          });

      var result = _dataContainerReader.ReadSequence (_dataReaderStub).ToArray ();
      Assert.That (result.Length, Is.EqualTo (3));
      Assert.That (result[0].ID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (result[1].ID, Is.EqualTo (DomainObjectIDs.OrderTicket2));
      Assert.That (result[2].ID, Is.EqualTo (DomainObjectIDs.OrderTicket3));
    }

    [Test]
    public void ReadSequence_DataReaderReadTrue_NullIDIsReturned_Supported ()
    {
      _dataReaderStub.Stub (stub => stub.Read()).Return (true).WhenCalled (
          mi =>
          {
            _dataReaderStub.BackToRecord();
            _dataReaderStub.Stub (stub => stub.Read()).Return (false);
          });

      var result = _dataContainerReader.ReadSequence (_dataReaderStub).ToArray();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result[0], Is.Null);
    }

    public void StubPersistenceModelProviderForProperty (
        Type declaringType, string shortPropertyName, IRdbmsStoragePropertyDefinition storagePropertyDefinitionStub)
    {
      var propertyDefinition = GetPropertyDefinition (declaringType, shortPropertyName);
      _persistenceModelProviderStub.Stub (stub => stub.GetColumnDefinition (propertyDefinition)).Return (storagePropertyDefinitionStub);
      storagePropertyDefinitionStub.Stub (stub => stub.Read (_dataReaderStub, _ordinalProviderStub)).Return (propertyDefinition.DefaultValue);
    }
  }
}