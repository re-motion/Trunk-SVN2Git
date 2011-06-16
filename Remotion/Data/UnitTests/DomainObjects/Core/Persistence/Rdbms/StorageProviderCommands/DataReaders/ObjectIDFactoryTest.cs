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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands.DataReaders
{
  [TestFixture]
  public class ObjectIDFactoryTest : SqlProviderBaseTest
  {
    private IDataReader _dataReaderStub;
    private ObjectIDFactory _factory;
    private IValueConverter _valueConverterStub;
    private ObjectID _objectID;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _valueConverterStub = MockRepository.GenerateStub<IValueConverter>();

      _factory = new ObjectIDFactory (_valueConverterStub);

      _objectID = new ObjectID ("Order", Guid.NewGuid());
    }

    [Test]
    public void Read ()
    {
      _valueConverterStub.Stub (stub => stub.GetID (_dataReaderStub)).Return (_objectID); 

      var result = _factory.Read (_dataReaderStub);

      Assert.That (result, Is.SameAs(_objectID));
    }

    [Test]
    public void ReadSequence ()
    {
      var objectID2 = new ObjectID ("OrderItem", Guid.NewGuid ());
      _dataReaderStub.Stub (stub => stub.Read ()).Return (true).Repeat.Twice ();
      _dataReaderStub.Stub (stub => stub.Read ()).Return (false).Repeat.Once ();
      _valueConverterStub.Stub (stub => stub.GetID (_dataReaderStub)).Return (_objectID).Repeat.Once ();
      _valueConverterStub.Stub (stub => stub.GetID (_dataReaderStub)).Return (objectID2).Repeat.Once ();

      var result = _factory.ReadSequence (_dataReaderStub).ToArray ();

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0], Is.SameAs (_objectID));
      Assert.That (result[1], Is.SameAs (objectID2));
    }

    [Test]
    public void ReadSequence_NoData ()
    {
      _dataReaderStub.Stub (stub => stub.Read ()).Return (false).Repeat.Once ();

      var result = _factory.ReadSequence (_dataReaderStub);

      Assert.That (result, Is.Empty);
    }

    
    
  }
}