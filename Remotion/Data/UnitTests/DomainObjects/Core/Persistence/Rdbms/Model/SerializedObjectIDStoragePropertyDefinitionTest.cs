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
  public class SerializedObjectIDStoragePropertyDefinitionTest : StandardMappingTest
  {
    private SimpleStoragePropertyDefinition _serializedIDProperty;
    private SerializedObjectIDStoragePropertyDefinition _serializedObjectIDStoragePropertyDefinition;
    private IDataReader _dataReaderStub;
    private IColumnOrdinalProvider _columnOrdinalProviderStub;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;

    public override void SetUp ()
    {
      base.SetUp();

      _serializedIDProperty = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty();
      _serializedObjectIDStoragePropertyDefinition = new SerializedObjectIDStoragePropertyDefinition (_serializedIDProperty);

      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _columnOrdinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand> ();
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter> ();
      _dbCommandStub.Stub (stub => stub.CreateParameter ()).Return (_dbDataParameterStub).Repeat.Once ();
    }

    // TODO Review 4129: Rewrite tests using stubs

    [Test]
    public void Initialization ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.SerializedIDProperty, Is.SameAs (_serializedIDProperty));
    }

    [Test]
    public void GetColumnForLookup ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.GetColumnForLookup(), Is.SameAs (_serializedIDProperty.ColumnDefinition));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "String-serialized ObjectID values cannot be used as foreign keys.")]
    public void GetColumnForForeignKey ()
    {
      _serializedObjectIDStoragePropertyDefinition.GetColumnForForeignKey();
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.GetColumns(), Is.EqualTo (_serializedIDProperty.GetColumns()));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.Name, Is.EqualTo (_serializedIDProperty.Name));
    }

    [Test]
    public void Read ()
    {
      _columnOrdinalProviderStub.Stub (stub => stub.GetOrdinal (_serializedIDProperty.ColumnDefinition, _dataReaderStub)).Return (2);
      _dataReaderStub.Stub (stub => stub[2]).Return (DomainObjectIDs.Order1.ToString());

      var result = _serializedObjectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.TypeOf (typeof (ObjectID)));
      Assert.That (((ObjectID) result).Value.ToString (), Is.EqualTo (DomainObjectIDs.Order1.Value.ToString ()));
      Assert.That (((ObjectID) result).ClassID, Is.EqualTo ("Order"));
    }

    [Test]
    public void Read_ValueIsNull_ReturnsNull ()
    {
      var typeConverterStub = MockRepository.GenerateStub<StringConverter>();
      var storageTypeInformation = new StorageTypeInformation ("varchar", DbType.String, typeof (string), typeConverterStub);
      var idColumnDefinition = new ColumnDefinition ("Test", typeof (string), storageTypeInformation, true, false);
      _serializedObjectIDStoragePropertyDefinition =
          new SerializedObjectIDStoragePropertyDefinition (new SimpleStoragePropertyDefinition (idColumnDefinition));

      typeConverterStub.Stub (stub => stub.ConvertFrom (null)).Return (null);
      _columnOrdinalProviderStub.Stub (stub => stub.GetOrdinal (idColumnDefinition, _dataReaderStub)).Return (2);
      _dataReaderStub.Stub (stub => stub[2]).Return (null);

      var result = _serializedObjectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void CreateDataParameters ()
    {
      var serializedObjectIDStoragePropertyDefinition =
          new SerializedObjectIDStoragePropertyDefinition (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("ID"));

      var result = serializedObjectIDStoragePropertyDefinition.CreateDataParameters (_dbCommandStub, DomainObjectIDs.Order1, "key").ToArray ();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result[0].ParameterName, Is.EqualTo ("key"));
      Assert.That (result[0].Value, Is.EqualTo (DomainObjectIDs.Order1.ToString()));
      Assert.That (result[0].DbType, Is.EqualTo (DbType.String));
    }
  }
}