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
    private SimpleStoragePropertyDefinition _objectIDColumn;
    private SimpleStoragePropertyDefinition _classIDColumn;
    private ObjectIDStoragePropertyDefinition _objectIDStoragePropertyDefinition;
    private IDataReader _dataReaderStub;
    private IColumnOrdinalProvider _columnOrdinalProviderStub;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameter1Stub;
    private IDbDataParameter _dbDataParameter2Stub;


    public override void SetUp ()
    {
      base.SetUp();

      _objectIDColumn = SimpleStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      _classIDColumn = new SimpleStoragePropertyDefinition (ColumnDefinitionObjectMother.CreateColumn ("Order"));
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
      Assert.That (_objectIDStoragePropertyDefinition.GetColumnForLookup(), Is.SameAs (_objectIDColumn.ColumnDefinition));
    }

    [Test]
    public void GetColumnForForeignKey ()
    {
      Assert.That (_objectIDStoragePropertyDefinition.GetColumnForForeignKey(), Is.SameAs (_objectIDColumn.ColumnDefinition));
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (
          _objectIDStoragePropertyDefinition.GetColumns(), Is.EqualTo (new[] { _objectIDColumn.ColumnDefinition, _classIDColumn.ColumnDefinition }));
    }

    [Test]
    public void Read ()
    {
      _columnOrdinalProviderStub.Stub (stub => stub.GetOrdinal (_objectIDColumn.ColumnDefinition, _dataReaderStub)).Return (2);
      _dataReaderStub.Stub (stub => stub[2]).Return (DomainObjectIDs.Order1.Value.ToString());

      var result = _objectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.TypeOf (typeof (ObjectID)));
      Assert.That (((ObjectID) result).Value.ToString(), Is.EqualTo (DomainObjectIDs.Order1.Value.ToString()));
      Assert.That (((ObjectID) result).ClassID, Is.EqualTo ("Order"));
    }

    [Test]
    public void Read_ValueIsNull_ReturnsNull ()
    {
      var typeConverterStub = MockRepository.GenerateStub<StringConverter>();
      var storageTypeInformation = new StorageTypeInformation ("varchar", DbType.String, typeof (string), typeConverterStub);
      var idColumnDefinition = new ColumnDefinition ("Test", typeof (string), storageTypeInformation, true, false);
      _objectIDStoragePropertyDefinition = new ObjectIDStoragePropertyDefinition (
          new SimpleStoragePropertyDefinition (idColumnDefinition), _classIDColumn);

      typeConverterStub.Stub (stub => stub.ConvertFrom (null)).Return (null);
      _columnOrdinalProviderStub.Stub (stub => stub.GetOrdinal (_objectIDColumn.ColumnDefinition, _dataReaderStub)).Return (2);
      _dataReaderStub.Stub (stub => stub[2]).Return (null);

      var result = _objectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void CreateDataParameters ()
    {
      var objectIDStoragePropertyDefinition =
          new ObjectIDStoragePropertyDefinition (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("ID"), _classIDColumn);

      var result = objectIDStoragePropertyDefinition.CreateDataParameters (_dbCommandStub, DomainObjectIDs.Order1, "key").ToArray();

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0].ParameterName, Is.EqualTo ("key"));
      Assert.That (result[0].Value, Is.EqualTo (DomainObjectIDs.Order1.Value.ToString()));
      Assert.That (result[0].DbType, Is.EqualTo (DbType.String));
      Assert.That (result[1].ParameterName, Is.EqualTo ("keyClassID"));
      Assert.That (result[1].Value, Is.EqualTo (DomainObjectIDs.Order1.ClassID));
      Assert.That (result[1].DbType, Is.EqualTo (DbType.String));
    }
  }
}