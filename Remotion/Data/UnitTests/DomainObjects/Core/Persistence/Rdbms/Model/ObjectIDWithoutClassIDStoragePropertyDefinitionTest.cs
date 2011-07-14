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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ObjectIDWithoutClassIDStoragePropertyDefinitionTest : StandardMappingTest
  {
    private ClassDefinition _classDefinition;
    private SimpleStoragePropertyDefinition _valueProperty;
    private ObjectIDWithoutClassIDStoragePropertyDefinition _objectIDWithoutClassIDStorageDefinition;
    private IDataReader _dataReaderStub;
    private IColumnOrdinalProvider _columnOrdinalProviderStub;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      _valueProperty = new SimpleStoragePropertyDefinition (
          new ColumnDefinition (
              Guid.NewGuid().ToString(),
              typeof (string),
              new StorageTypeInformation ("varchar", DbType.String, typeof (string), new GuidConverter()),
              true,
              false));
      _objectIDWithoutClassIDStorageDefinition = new ObjectIDWithoutClassIDStoragePropertyDefinition (
          _valueProperty, _classDefinition);

      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _columnOrdinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter>();
      _dbCommandStub.Stub (stub => stub.CreateParameter()).Return (_dbDataParameterStub).Repeat.Once();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_objectIDWithoutClassIDStorageDefinition.ValueProperty, Is.SameAs (_valueProperty));
      Assert.That (_objectIDWithoutClassIDStorageDefinition.ClassDefinition, Is.SameAs (_classDefinition));
    }

    [Test]
    public void GetColumnForLookup ()
    {
      Assert.That (_objectIDWithoutClassIDStorageDefinition.GetColumnForLookup(), Is.SameAs (_valueProperty.ColumnDefinition));
    }

    [Test]
    public void GetColumnForForeignKey ()
    {
      Assert.That (_objectIDWithoutClassIDStorageDefinition.GetColumnForForeignKey(), Is.SameAs (_valueProperty.ColumnDefinition));
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (_objectIDWithoutClassIDStorageDefinition.GetColumns(), Is.EqualTo (_valueProperty.GetColumns()));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_objectIDWithoutClassIDStorageDefinition.Name, Is.EqualTo (_valueProperty.Name));
    }

    [Test]
    public void Read ()
    {
      _columnOrdinalProviderStub.Stub (stub => stub.GetOrdinal (_valueProperty.ColumnDefinition, _dataReaderStub)).Return (2);
      _dataReaderStub.Stub (stub => stub[2]).Return (DomainObjectIDs.Order1.Value.ToString());

      var result = _objectIDWithoutClassIDStorageDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.TypeOf (typeof (ObjectID)));
      Assert.That (((ObjectID) result).Value.ToString(), Is.EqualTo (DomainObjectIDs.Order1.Value.ToString()));
      Assert.That (((ObjectID) result).ClassDefinition, Is.SameAs (_classDefinition));
    }

    [Test]
    public void Read_ValueIsNull_ReturnsNull ()
    {
      var typeConverterStub = MockRepository.GenerateStub<StringConverter>();
      var storageTypeInformation = new StorageTypeInformation ("varchar", DbType.String, typeof (string), typeConverterStub);
      var idColumnDefinition = new ColumnDefinition ("Test", typeof (string), storageTypeInformation, true, false);
      _objectIDWithoutClassIDStorageDefinition =
          new ObjectIDWithoutClassIDStoragePropertyDefinition (new SimpleStoragePropertyDefinition (idColumnDefinition), _classDefinition);

      typeConverterStub.Stub (stub => stub.ConvertFrom (null)).Return (null);
      _columnOrdinalProviderStub.Stub (stub => stub.GetOrdinal (idColumnDefinition, _dataReaderStub)).Return (2);
      _dataReaderStub.Stub (stub => stub[2]).Return (null);

      var result = _objectIDWithoutClassIDStorageDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void CreateDataParameters ()
    {
      var objectIDWithoutClassIDStoragePropertyDefinition =
          new ObjectIDWithoutClassIDStoragePropertyDefinition (
              SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("ID"), _classDefinition);

      var result = objectIDWithoutClassIDStoragePropertyDefinition.CreateDataParameters (_dbCommandStub, DomainObjectIDs.Order1, "key").ToArray();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result[0].ParameterName, Is.EqualTo ("key"));
      Assert.That (result[0].Value, Is.EqualTo (DomainObjectIDs.Order1.Value.ToString()));
      Assert.That (result[0].DbType, Is.EqualTo (DbType.String));
    }
  }
}