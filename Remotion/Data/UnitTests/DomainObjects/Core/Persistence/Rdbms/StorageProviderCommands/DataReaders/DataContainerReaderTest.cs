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
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands.DataReaders
{
  [TestFixture]
  public class DataContainerReaderTest : SqlProviderBaseTest
  {
    private IDataReader _dataReaderStub;
    private DataContainerReader _factory;
    private IValueConverter _valueConverterStub;
    private ObjectID _objectID;
    private object _timestamp;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _valueConverterStub = MockRepository.GenerateStub<IValueConverter>();
      _factory = new DataContainerReader (_valueConverterStub);

      _objectID = new ObjectID ("OrderTicket", Guid.NewGuid());
      _timestamp = new object();

      // TODO Review 4058: Remove when value converter is mocked (here and in other tests)
      OppositeClassDefinitionRetriever.ResetCache();
    }

    public override void TearDown ()
    {
      base.TearDown();
      // TODO Review 4058: Remove when value converter is mocked
      OppositeClassDefinitionRetriever.ResetCache();
    }

    [Test]
    public void Read_DataReaderReadFalse ()
    {
      _dataReaderStub.Stub (stub => stub.Read()).Return (false);

      var result = _factory.Read (_dataReaderStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void Read_DataReaderReadTrue_ValueIDNotNull ()
    {
      _dataReaderStub.Stub (stub => stub.Read()).Return (true);

      _valueConverterStub.Stub (stub => stub.GetID (_dataReaderStub)).Return (_objectID);
      _valueConverterStub.Stub (stub => stub.GetTimestamp (_dataReaderStub)).Return (_timestamp);

      StubValueConverterForProperty (typeof (OrderTicket), "FileName", _objectID);
      StubValueConverterForProperty (typeof (OrderTicket), "Order", _objectID);

      var dataContainer = _factory.Read (_dataReaderStub);

      Assert.That (dataContainer, Is.Not.Null);
      Assert.That (dataContainer.ID, Is.SameAs (_objectID));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException),
        ExpectedMessage = "An object returned from the database had a NULL ID, which is not supported.")]
    public void Read_DataReaderReadTrue_ValueIDNull ()
    {
      _dataReaderStub.Stub (stub => stub.Read()).Return (true);
      _valueConverterStub.Stub (stub => stub.GetID (_dataReaderStub)).Return (null);

      _factory.Read (_dataReaderStub);
    }

    [Test]
    public void ReadSequence_DataReaderReadFalse ()
    {
      _dataReaderStub.Stub (stub => stub.Read()).Return (false);

      var result = _factory.ReadSequence (_dataReaderStub, false);

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void ReadSequence_DataReaderReadTrue()
    {
      StubValueConverterForProperty (typeof (OrderTicket), "FileName", DomainObjectIDs.OrderTicket1);
      StubValueConverterForProperty (typeof (OrderTicket), "Order", DomainObjectIDs.OrderTicket1);
      StubValueConverterForProperty (typeof (OrderTicket), "FileName", DomainObjectIDs.OrderTicket2);
      StubValueConverterForProperty (typeof (OrderTicket), "Order", DomainObjectIDs.OrderTicket2);
      StubValueConverterForProperty (typeof (OrderTicket), "FileName", DomainObjectIDs.OrderTicket3);
      StubValueConverterForProperty (typeof (OrderTicket), "Order", DomainObjectIDs.OrderTicket3);

      _valueConverterStub.Stub (stub => stub.GetID (_dataReaderStub)).Return (DomainObjectIDs.OrderTicket1).Repeat.Once();
      _valueConverterStub.Stub (stub => stub.GetID (_dataReaderStub)).Return (DomainObjectIDs.OrderTicket2).Repeat.Once ();
      _valueConverterStub.Stub (stub => stub.GetID (_dataReaderStub)).Return (DomainObjectIDs.OrderTicket3).Repeat.Once ();
      _valueConverterStub.Stub (stub => stub.GetTimestamp (_dataReaderStub)).Return (_timestamp);

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

      var result = _factory.ReadSequence (_dataReaderStub, true).ToArray ();
      Assert.That (result.Length, Is.EqualTo (3));
      Assert.That (result[0].ID, Is.EqualTo(DomainObjectIDs.OrderTicket1));
      Assert.That (result[1].ID, Is.EqualTo(DomainObjectIDs.OrderTicket2));
      Assert.That (result[2].ID, Is.EqualTo(DomainObjectIDs.OrderTicket3));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "A database query returned duplicates of the domain object "
                                                                           +
                                                                           "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid', which is not supported."
        )]
    public void ReadSequence_DataReaderReadTrue_Duplicates ()
    {
      _valueConverterStub.Stub (stub => stub.GetID (_dataReaderStub)).Return (DomainObjectIDs.OrderTicket1);
      _valueConverterStub.Stub (stub => stub.GetTimestamp (_dataReaderStub)).Return (_timestamp);

      StubValueConverterForProperty (typeof (OrderTicket), "FileName", _objectID);
      StubValueConverterForProperty (typeof (OrderTicket), "Order", _objectID);

      var count = 0;
      _dataReaderStub.Stub (stub => stub.Read ()).Return (true).WhenCalled (
          mi =>
          {
            count++;
            if (count > 2)
            {
              _dataReaderStub.BackToRecord();
              _dataReaderStub.Stub (stub => stub.Read()).Return (false);
            }
          });

      _factory.ReadSequence (_dataReaderStub, true).ToArray ();
    }

    [Test]
    public void ReadSequence_DataReaderReadTrue_NullIDIsReturned_Supported ()
    {
      _dataReaderStub.Stub (stub => stub.Read ()).Return (true).WhenCalled (
          mi =>
          {
            _dataReaderStub.BackToRecord ();
            _dataReaderStub.Stub (stub => stub.Read ()).Return (false);
          });

      var result = _factory.ReadSequence (_dataReaderStub, true).ToArray ();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result[0], Is.Null);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException),
        ExpectedMessage = "An object returned from the database had a NULL ID, which is not supported.")]
    public void ReadSequence_DataReaderReadTrue_NullIDIsReturned_NotSupported ()
    {
      _dataReaderStub.Stub (stub => stub.Read()).Return (true).WhenCalled (
          mi =>
          {
            _dataReaderStub.BackToRecord();
            _dataReaderStub.Stub (stub => stub.Read()).Return (false);
          });

      var result = _factory.ReadSequence (_dataReaderStub, false);

      Assert.That (result, Is.Empty);
    }

    public void StubValueConverterForProperty (Type declaringType, string shortPropertyName, ObjectID objectID)
    {
      var propertyDefinition = GetPropertyDefinition (declaringType, shortPropertyName);
      _valueConverterStub
          .Stub (stub => stub.GetValue (objectID.ClassDefinition, propertyDefinition, _dataReaderStub))
          .Return (propertyDefinition.DefaultValue);
    }
    
  }
}