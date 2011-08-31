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
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ObjectIDStoragePropertyDefinitionTest : StandardMappingTest
  {
    private ColumnDefinition _columnDefinition1;
    private ColumnDefinition _columnDefinition2;

    private IRdbmsStoragePropertyDefinition _valuePropertyStub;
    private IRdbmsStoragePropertyDefinition _classIDPropertyStub;

    private ObjectIDStoragePropertyDefinition _objectIDStoragePropertyDefinition;

    private IDataReader _dataReaderStub;
    private IColumnOrdinalProvider _columnOrdinalProviderStub;
    private IDbDataParameter _dbDataParameter1Stub;
    private IDbDataParameter _dbDataParameter2Stub;
    private IDbCommand _dbCommandStub;

    public override void SetUp ()
    {
      base.SetUp();

      _columnDefinition1 = ColumnDefinitionObjectMother.CreateColumn("Column1");
      _columnDefinition2 = ColumnDefinitionObjectMother.CreateColumn ("Column2");

      _valuePropertyStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _valuePropertyStub.Stub (stub => stub.GetColumnForLookup()).Return (_columnDefinition1);
      _valuePropertyStub.Stub (stub => stub.GetColumnForForeignKey ()).Return (_columnDefinition1);
      _valuePropertyStub.Stub (stub => stub.GetColumns()).Return (new[] { _columnDefinition1 });
      
      _classIDPropertyStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _classIDPropertyStub.Stub (stub => stub.GetColumns ()).Return (new[] { _columnDefinition2 });
      
      _objectIDStoragePropertyDefinition = new ObjectIDStoragePropertyDefinition (_valuePropertyStub, _classIDPropertyStub);

      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _columnOrdinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbDataParameter1Stub = MockRepository.GenerateStub<IDbDataParameter>();
      _dbDataParameter2Stub = MockRepository.GenerateStub<IDbDataParameter> ();
      _dbCommandStub.Stub (stub => stub.CreateParameter()).Return (_dbDataParameter1Stub).Repeat.Once();
      _dbCommandStub.Stub (stub => stub.CreateParameter ()).Return (_dbDataParameter2Stub).Repeat.Once ();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_objectIDStoragePropertyDefinition.ValueProperty, Is.SameAs (_valuePropertyStub));
      Assert.That (_objectIDStoragePropertyDefinition.ClassIDProperty, Is.SameAs (_classIDPropertyStub));
      Assert.That (_objectIDStoragePropertyDefinition.ValueProperty, Is.SameAs (_valuePropertyStub));
      Assert.That (_objectIDStoragePropertyDefinition.ClassIDProperty, Is.SameAs (_classIDPropertyStub));
    }

    [Test]
    public void GetColumnForLookup ()
    {
      Assert.That (_objectIDStoragePropertyDefinition.GetColumnForLookup(), Is.SameAs (_columnDefinition1));
    }

    [Test]
    public void GetColumnForForeignKey ()
    {
      Assert.That (_objectIDStoragePropertyDefinition.GetColumnForForeignKey(), Is.SameAs (_columnDefinition1));
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (_objectIDStoragePropertyDefinition.GetColumns(), Is.EqualTo (new[] { _columnDefinition1, _columnDefinition2 }));
    }

    [Test]
    public void Read ()
    {
      _valuePropertyStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (DomainObjectIDs.Order1.Value);
      _classIDPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return ("Order");
      
      var result = _objectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.TypeOf (typeof (ObjectID)));
      Assert.That (((ObjectID) result).Value.ToString(), Is.EqualTo (DomainObjectIDs.Order1.Value.ToString()));
      Assert.That (((ObjectID) result).ClassID, Is.EqualTo ("Order"));
    }

    [Test]
    public void Read_ValueAndClassIdIsNull_ReturnsNull ()
    {
      var result = _objectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = 
      "Incorrect database value encountered. The value read from 'Column2' must contain null.")]
    public void Read_ValueIsNullAndClassIDIsNotNull_ThrowsException ()
    {
      _classIDPropertyStub.Stub (stub => stub.GetColumns ()).Return (new[] { _columnDefinition1 });
      _classIDPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return ("Order");

      _objectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
      "Incorrect database value encountered. The value read from 'Column2' must not contain null.")]
    public void Read_ValueIsNotNullAndClassIDIsNull_ThrowsException ()
    {
      _classIDPropertyStub.Stub (stub => stub.GetColumns()).Return (new[] { _columnDefinition1 });
      _valuePropertyStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (DomainObjectIDs.Order1.Value);

      _objectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);
    }

    [Test]
    public void SplitValue ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition1, DomainObjectIDs.Order1);
      var columnValue2 = new ColumnValue (_columnDefinition1, DomainObjectIDs.Order2);

      _valuePropertyStub.Stub (stub => stub.SplitValue (DomainObjectIDs.Order1.Value)).Return (new[] { columnValue1 });
      _classIDPropertyStub.Stub (stub => stub.SplitValue (DomainObjectIDs.Order1.ClassID)).Return (new[] { columnValue2 });
      
      var result = _objectIDStoragePropertyDefinition.SplitValue (DomainObjectIDs.Order1);

      Assert.That (result, Is.EqualTo (new[] { columnValue1, columnValue2 }));
    }

    [Test]
    public void SplitValue_NullValue ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition1, null);
      var columnValue2 = new ColumnValue (_columnDefinition2, null);

      _valuePropertyStub.Stub (stub => stub.SplitValue (null)).Return (new[]{ columnValue1 });
      _classIDPropertyStub.Stub (stub => stub.SplitValue (null)).Return (new[] { columnValue2});

      var result = _objectIDStoragePropertyDefinition.SplitValue (null);

      Assert.That (result, Is.EqualTo (new[] { columnValue1, columnValue2 }));
    }

    [Test]
    public void SplitValueForComparison ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition1, 12);
      _valuePropertyStub.Stub (stub => stub.SplitValueForComparison (DomainObjectIDs.Order1.Value)).Return (new[] { columnValue1 });

      var result = _objectIDStoragePropertyDefinition.SplitValueForComparison (DomainObjectIDs.Order1).ToArray();

      Assert.That (result, Is.EqualTo (new[] { columnValue1 }));
    }

    [Test]
    public void SplitValueForComparison_NullValue ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition1, null);
      _valuePropertyStub.Stub (stub => stub.SplitValueForComparison (null)).Return (new[] { columnValue1 });

      var result = _objectIDStoragePropertyDefinition.SplitValueForComparison (null).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { columnValue1 }));
    }

    [Test]
    public void SplitValuesForComparison ()
    {
      var row1 = new ColumnValueTable.Row (new[] { "1" });
      var row2 = new ColumnValueTable.Row (new[] { "2" });
      var columnValueTable = new ColumnValueTable (new[] { _columnDefinition1 }, new[] { row1, row2 });

      _valuePropertyStub
          .Stub (stub => stub.SplitValuesForComparison (Arg<IEnumerable<object>>.List.Equal (
              new[] { DomainObjectIDs.Order1.Value, DomainObjectIDs.Order2.Value })))
          .Return (columnValueTable);

      var result = _objectIDStoragePropertyDefinition.SplitValuesForComparison (new object[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 });

      ColumnValueTableTestHelper.CheckTable (columnValueTable, result);
    }

    [Test]
    public void SplitValuesForComparison_NullValue ()
    {
      var row1 = new ColumnValueTable.Row (new[] { "1" });
      var row2 = new ColumnValueTable.Row (new[] { "2" });
      var columnValueTable = new ColumnValueTable (new[] { _columnDefinition1 }, new[] { row1, row2 });

      // Bug in Rhino Mocks: List.Equal constraint cannot handle nulls within the sequence
      _valuePropertyStub
          .Stub (stub => stub.SplitValuesForComparison (
              Arg<IEnumerable<object>>.Matches (seq => seq.SequenceEqual (new[] { null, DomainObjectIDs.Order2.Value }))))
          .Return (columnValueTable);

      var result = _objectIDStoragePropertyDefinition.SplitValuesForComparison (new object[] { null, DomainObjectIDs.Order2 });

      ColumnValueTableTestHelper.CheckTable (columnValueTable, result);
    }
  }
}