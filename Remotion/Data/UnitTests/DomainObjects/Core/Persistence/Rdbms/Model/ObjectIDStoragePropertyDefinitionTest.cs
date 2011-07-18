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
using System.ComponentModel;
using System.Data;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ObjectIDStoragePropertyDefinitionTest : StandardMappingTest
  {
    private IRdbmsStoragePropertyDefinition _objectIDColumn;
    private IRdbmsStoragePropertyDefinition _classIDColumn;
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

      _columnDefinition1 = ColumnDefinitionObjectMother.CreateColumn();
      _columnDefinition2 = ColumnDefinitionObjectMother.CreateColumn ();

      _objectIDColumn = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _objectIDColumn.Stub (stub => stub.Name).Return ("ID");
      _objectIDColumn.Stub (stub => stub.GetColumnForLookup()).Return (_columnDefinition1);
      _objectIDColumn.Stub (stub => stub.GetColumnForForeignKey ()).Return (_columnDefinition1);
      _objectIDColumn.Stub (stub => stub.GetColumns()).Return (new[] { _columnDefinition1 });
      
      _classIDColumn = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _classIDColumn.Stub (stub => stub.Name).Return ("Order");
      _classIDColumn.Stub (stub => stub.GetColumns ()).Return (new[] { _columnDefinition2 });
      
      _objectIDStoragePropertyDefinition = new ObjectIDStoragePropertyDefinition (_objectIDColumn, _classIDColumn);

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
      Assert.That (_objectIDStoragePropertyDefinition.ValueProperty, Is.SameAs (_objectIDColumn));
      Assert.That (_objectIDStoragePropertyDefinition.ClassIDProperty, Is.SameAs (_classIDColumn));
      Assert.That (((IRdbmsStoragePropertyDefinition) _objectIDStoragePropertyDefinition).Name, Is.EqualTo ("ID"));
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
      _objectIDColumn.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (DomainObjectIDs.Order1.Value);

      var result = _objectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.TypeOf (typeof (ObjectID)));
      Assert.That (((ObjectID) result).Value.ToString(), Is.EqualTo (DomainObjectIDs.Order1.Value.ToString()));
      Assert.That (((ObjectID) result).ClassID, Is.EqualTo ("Order"));
    }

    [Test]
    public void Read_ValueIsNull_ReturnsNull ()
    {
      _objectIDColumn.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (null);

      var result = _objectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void CreateDataParameters ()
    {
      _objectIDColumn.Stub (stub => stub.CreateDataParameters (_dbCommandStub, DomainObjectIDs.Order1.Value.ToString(), "key")).Return (
          new[] { _dbDataParameter1Stub });
      _classIDColumn.Stub (stub => stub.CreateDataParameters (_dbCommandStub, DomainObjectIDs.Order1.ClassID, "keyClassID")).Return (
          new[] { _dbDataParameter2Stub });

      var objectIDStoragePropertyDefinition = new ObjectIDStoragePropertyDefinition (_objectIDColumn, _classIDColumn);

      var result = objectIDStoragePropertyDefinition.CreateDataParameters (_dbCommandStub, DomainObjectIDs.Order1, "key");

      Assert.That (result, Is.EqualTo (new[]{_dbDataParameter1Stub, _dbDataParameter2Stub}));
    }

    [Test]
    public void SplitValue ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition1, DomainObjectIDs.Order1);
      var columnValue2 = new ColumnValue (_columnDefinition1, DomainObjectIDs.Order2);

      _objectIDColumn.Stub (stub => stub.SplitValue (DomainObjectIDs.Order1.Value)).Return (new[] { columnValue1 });
      _classIDColumn.Stub (stub => stub.SplitValue (DomainObjectIDs.Order1.ClassID)).Return (new[] { columnValue2 });
      
      var result = _objectIDStoragePropertyDefinition.SplitValue (DomainObjectIDs.Order1);

      Assert.That (result, Is.EqualTo (new[] { columnValue1, columnValue2 }));
    }

    [Test]
    public void SplitValue_NullValue ()
    {
      var columnValue = new ColumnValue (_columnDefinition1, DomainObjectIDs.Order1);

      _objectIDColumn.Stub (stub => stub.SplitValue (null)).Return (new[]{ columnValue });

      var result = _objectIDStoragePropertyDefinition.SplitValue (null);

      Assert.That (result, Is.EqualTo (new[] { columnValue }));
    }
  }
}