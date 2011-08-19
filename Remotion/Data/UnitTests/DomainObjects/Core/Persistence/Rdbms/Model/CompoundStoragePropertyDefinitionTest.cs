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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class CompoundStoragePropertyDefinitionTest
  {
    private CompoundStoragePropertyDefinition _compoundStoragePropertyDefinition;
    private IRdbmsStoragePropertyDefinition _property1Stub;
    private IRdbmsStoragePropertyDefinition _property2Stub;
    private IRdbmsStoragePropertyDefinition _property3Stub;
    private CompoundStoragePropertyDefinition.NestedPropertyInfo _yearProperty;
    private CompoundStoragePropertyDefinition.NestedPropertyInfo _monthProperty;
    private CompoundStoragePropertyDefinition.NestedPropertyInfo _dayProperty;
    private ColumnDefinition _columnDefinition1;
    private ColumnDefinition _columnDefinition2;
    private ColumnDefinition _columnDefinition3;

    [SetUp]
    public void SetUp ()
    {
      var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation();
      _columnDefinition1 = new ColumnDefinition ("Year", typeof (string), storageTypeInformation, true, false);
      _columnDefinition2 = new ColumnDefinition ("Month", typeof (string), storageTypeInformation, true, false);
      _columnDefinition3 = new ColumnDefinition ("Month", typeof (string), storageTypeInformation, true, false);

      _property1Stub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _property2Stub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _property3Stub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();

      _yearProperty = new CompoundStoragePropertyDefinition.NestedPropertyInfo (_property1Stub, o => ((DateTime) o).Year);
      _monthProperty = new CompoundStoragePropertyDefinition.NestedPropertyInfo (_property2Stub, o => ((DateTime) o).Month);
      _dayProperty = new CompoundStoragePropertyDefinition.NestedPropertyInfo (_property3Stub, o => ((DateTime) o).Day);

      _compoundStoragePropertyDefinition = new CompoundStoragePropertyDefinition (
          new[] { _yearProperty, _monthProperty, _dayProperty },
          objects => new DateTime ((int) objects[0], (int) objects[1], (int) objects[2]));
    }

    [Test]
    public void GetColumns ()
    {
      _property1Stub.Stub (stub => stub.GetColumns()).Return (new[] { _columnDefinition1 });
      _property2Stub.Stub (stub => stub.GetColumns()).Return (new[] { _columnDefinition2 });
      _property3Stub.Stub (stub => stub.GetColumns()).Return (new[] { _columnDefinition3 });

      var result = _compoundStoragePropertyDefinition.GetColumns();

      Assert.That (result, Is.EqualTo (new[] { _columnDefinition1, _columnDefinition2, _columnDefinition3 }));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Compound properties cannot be used to look up values.")]
    public void GetColumnForLookup ()
    {
      _compoundStoragePropertyDefinition.GetColumnForLookup();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Compound properties cannot be used as a foreign key.")]
    public void GetColumnForForeignKey ()
    {
      _compoundStoragePropertyDefinition.GetColumnForForeignKey();
    }

    [Test]
    public void Read ()
    {
      var dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      var columnOrdinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();

      _property1Stub.Stub (stub => stub.Read (dataReaderStub, columnOrdinalProviderStub)).Return (2011);
      _property2Stub.Stub (stub => stub.Read (dataReaderStub, columnOrdinalProviderStub)).Return (5);
      _property3Stub.Stub (stub => stub.Read (dataReaderStub, columnOrdinalProviderStub)).Return (17);

      var result = _compoundStoragePropertyDefinition.Read (dataReaderStub, columnOrdinalProviderStub);

      Assert.That (result, Is.EqualTo (new DateTime (2011, 5, 17)));
    }

    [Test]
    public void SplitValue ()
    {
      var dateTime = new DateTime (2011, 7, 18);
      var columnValue1 = new ColumnValue (_columnDefinition1, dateTime);
      var columnValue2 = new ColumnValue (_columnDefinition2, dateTime);
      var columnValue3 = new ColumnValue (_columnDefinition3, dateTime);

      _property1Stub.Stub (stub => stub.SplitValue (2011)).Return (new[] { columnValue1 });
      _property2Stub.Stub (stub => stub.SplitValue (7)).Return (new[] { columnValue2 });
      _property3Stub.Stub (stub => stub.SplitValue (18)).Return (new[] { columnValue3 });

      var result = _compoundStoragePropertyDefinition.SplitValue (dateTime).ToArray();

      Assert.That (result, Is.EqualTo (new[] { columnValue1, columnValue2, columnValue3 }));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Compound properties cannot be used to look up values for comparison.")]
    public void SplitValueForComparison ()
    {
      _compoundStoragePropertyDefinition.SplitValueForComparison (null);
    }
  }
}