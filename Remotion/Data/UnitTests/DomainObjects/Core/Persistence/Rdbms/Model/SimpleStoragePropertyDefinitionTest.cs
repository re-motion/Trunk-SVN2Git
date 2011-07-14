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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class SimpleStoragePropertyDefinitionTest
  {
    private SimpleStoragePropertyDefinition _storagePropertyDefinition;
    private ColumnDefinition _innerColumnDefinition;
    private IDataReader _dataReaderStub;
    private IColumnOrdinalProvider _columnOrdinalProviderStub;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;

    [SetUp]
    public void SetUp ()
    {
      _storagePropertyDefinition = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column1");
      _innerColumnDefinition = _storagePropertyDefinition.ColumnDefinition;

      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _columnOrdinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter> ();
      _dbCommandStub.Stub (stub => stub.CreateParameter ()).Return (_dbDataParameterStub);
    }

    [Test]
    public void Name ()
    {
      Assert.That (_storagePropertyDefinition.Name, Is.EqualTo (_innerColumnDefinition.Name));
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (_storagePropertyDefinition.GetColumns (), Is.EqualTo (new[] { _innerColumnDefinition }));
    }

    [Test]
    public void Read ()
    {
      _columnOrdinalProviderStub.Stub (stub => stub.GetOrdinal (_innerColumnDefinition, _dataReaderStub)).Return (5);
      _dataReaderStub.Stub (stub => stub[5]).Return ("test");

      var result = _storagePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.EqualTo ("test"));
    }

    [Test]
    public void Read_DbNullValue_ReturnsEmptyString ()
    {
      _columnOrdinalProviderStub.Stub (stub => stub.GetOrdinal (_innerColumnDefinition, _dataReaderStub)).Return (3);
      _dataReaderStub.Stub (stub => stub[3]).Return (DBNull.Value);

      var result = _storagePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.EqualTo (string.Empty));
    }

    [Test]
    public void CreateDataParameters ()
    {
      var result = _storagePropertyDefinition.CreateDataParameters (_dbCommandStub, "test", "key").ToArray();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result[0].ParameterName, Is.EqualTo ("key"));
      Assert.That (result[0].Value, Is.EqualTo ("test"));
      Assert.That (result[0].DbType, Is.EqualTo (DbType.String));
    }

    [Test]
    public void CreateDataParameters_ValueIsNull ()
    {
      var typeConverterStub = MockRepository.GenerateStub<StringConverter> ();
      var storageTypeInformation = new StorageTypeInformation ("varchar", DbType.String, typeof (string), typeConverterStub);
      var idColumnDefinition = new ColumnDefinition ("Test", typeof (string), storageTypeInformation, true, false);
      _storagePropertyDefinition = new SimpleStoragePropertyDefinition (idColumnDefinition);

      typeConverterStub.Stub (stub => stub.ConvertTo ("test", typeof(string))).Return (null);

      var result = _storagePropertyDefinition.CreateDataParameters (_dbCommandStub, "test", "key").ToArray ();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result[0].ParameterName, Is.EqualTo ("key"));
      Assert.That (result[0].Value, Is.EqualTo (DBNull.Value));
      Assert.That (result[0].DbType, Is.EqualTo (DbType.String));
    }
  }
}