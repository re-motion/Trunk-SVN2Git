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
    private IRdbmsStoragePropertyDefinition _objectIDColumnStub;
    private IRdbmsStoragePropertyDefinition _classIDColumnStub;
    private ObjectIDStoragePropertyDefinition _objectIDStoragePropertyDefinition;
    private IDataReader _dataReaderStub;
    private IColumnOrdinalProvider _columnOrdinalProviderStub;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameter1Stub;
    private IDbDataParameter _dbDataParameter2Stub;
    private ColumnDefinition _columnDefinition1;
    private ColumnDefinition _columnDefinition2;


    public override void SetUp ()
    {
      base.SetUp();

      _columnDefinition1 = ColumnDefinitionObjectMother.CreateColumn("Column1");
      _columnDefinition2 = ColumnDefinitionObjectMother.CreateColumn ("Column2");

      _objectIDColumnStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _objectIDColumnStub.Stub (stub => stub.GetColumnForLookup()).Return (_columnDefinition1);
      _objectIDColumnStub.Stub (stub => stub.GetColumnForForeignKey ()).Return (_columnDefinition1);
      _objectIDColumnStub.Stub (stub => stub.GetColumns()).Return (new[] { _columnDefinition1 });
      
      _classIDColumnStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _classIDColumnStub.Stub (stub => stub.GetColumns ()).Return (new[] { _columnDefinition2 });
      
      _objectIDStoragePropertyDefinition = new ObjectIDStoragePropertyDefinition (_objectIDColumnStub, _classIDColumnStub);

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
      Assert.That (_objectIDStoragePropertyDefinition.ValueProperty, Is.SameAs (_objectIDColumnStub));
      Assert.That (_objectIDStoragePropertyDefinition.ClassIDProperty, Is.SameAs (_classIDColumnStub));
      Assert.That (_objectIDStoragePropertyDefinition.ValueProperty, Is.SameAs (_objectIDColumnStub));
      Assert.That (_objectIDStoragePropertyDefinition.ClassIDProperty, Is.SameAs (_classIDColumnStub));
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
      _objectIDColumnStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (DomainObjectIDs.Order1.Value);
      _classIDColumnStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return ("Order");
      
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
      _classIDColumnStub.Stub (stub => stub.GetColumns ()).Return (new[] { _columnDefinition1 });
      _classIDColumnStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return ("Order");

      _objectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
      "Incorrect database value encountered. The value read from 'Column2' must not contain null.")]
    public void Read_ValueIsNotNullAndClassIDIsNull_ThrowsException ()
    {
      _classIDColumnStub.Stub (stub => stub.GetColumns()).Return (new[] { _columnDefinition1 });
      _objectIDColumnStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (DomainObjectIDs.Order1.Value);

      _objectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);
    }

    [Test]
    public void SplitValue ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition1, DomainObjectIDs.Order1);
      var columnValue2 = new ColumnValue (_columnDefinition1, DomainObjectIDs.Order2);

      _objectIDColumnStub.Stub (stub => stub.SplitValue (DomainObjectIDs.Order1.Value)).Return (new[] { columnValue1 });
      _classIDColumnStub.Stub (stub => stub.SplitValue (DomainObjectIDs.Order1.ClassID)).Return (new[] { columnValue2 });
      
      var result = _objectIDStoragePropertyDefinition.SplitValue (DomainObjectIDs.Order1);

      Assert.That (result, Is.EqualTo (new[] { columnValue1, columnValue2 }));
    }

    [Test]
    public void SplitValue_NullValue ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition1, null);
      var columnValue2 = new ColumnValue (_columnDefinition2, null);

      _objectIDColumnStub.Stub (stub => stub.SplitValue (null)).Return (new[]{ columnValue1 });
      _classIDColumnStub.Stub (stub => stub.SplitValue (null)).Return (new[] { columnValue2});

      var result = _objectIDStoragePropertyDefinition.SplitValue (null);

      Assert.That (result, Is.EqualTo (new[] { columnValue1, columnValue2 }));
    }

    [Test]
    public void SplitValueForComparison ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition1, null);
      _objectIDColumnStub.Stub (stub => stub.SplitValueForComparison (DomainObjectIDs.Order1.Value)).Return (new[] { columnValue1 });

      var result = _objectIDStoragePropertyDefinition.SplitValueForComparison (DomainObjectIDs.Order1).ToArray();

      Assert.That (result, Is.EqualTo (new[] { columnValue1 }));
    }

    [Test]
    public void SplitValueForComparison_NullValue ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition1, null);
      _objectIDColumnStub.Stub (stub => stub.SplitValueForComparison (null)).Return (new[] { columnValue1 });

      var result = _objectIDStoragePropertyDefinition.SplitValueForComparison (null).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { columnValue1 }));
    }
  }
}