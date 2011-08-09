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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class SimpleStoragePropertyDefinitionTest
  {
    private TypeConverter _typeConverterStub;
    private ColumnDefinition _innerColumnDefinition;
    private SimpleStoragePropertyDefinition _storagePropertyDefinition;

    private IDataReader _dataReaderStub;
    private IColumnOrdinalProvider _columnOrdinalProviderStub;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;

    [SetUp]
    public void SetUp ()
    {
      _typeConverterStub = MockRepository.GenerateStub<TypeConverter> ();
      
      var storageTypeInformation = new StorageTypeInformation ("integer", DbType.Int32, typeof (int?), _typeConverterStub);
      _innerColumnDefinition = new ColumnDefinition ("Test", typeof (string), storageTypeInformation, true, false);
      _storagePropertyDefinition = new SimpleStoragePropertyDefinition (_innerColumnDefinition);

      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _columnOrdinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter> ();
      _dbCommandStub.Stub (stub => stub.CreateParameter ()).Return (_dbDataParameterStub);
    }

    [Test]
    public void ColumnDefinition ()
    {
      Assert.That (_storagePropertyDefinition.ColumnDefinition, Is.SameAs (_innerColumnDefinition));
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (_storagePropertyDefinition.GetColumns (), Is.EqualTo (new[] { _innerColumnDefinition }));
    }

    [Test]
    public void GetColumnForLookup ()
    {
      Assert.That (_storagePropertyDefinition.GetColumnForLookup(), Is.SameAs (_innerColumnDefinition));
    }

    [Test]
    public void GetColumnForForeignKey ()
    {
      Assert.That (_storagePropertyDefinition.GetColumnForForeignKey (), Is.SameAs (_innerColumnDefinition));
    }

    [Test]
    public void Read ()
    {
      _columnOrdinalProviderStub.Stub (stub => stub.GetOrdinal (_innerColumnDefinition, _dataReaderStub)).Return (5);
      _dataReaderStub.Stub (stub => stub[5]).Return ("test");
      _typeConverterStub.Stub (mock => mock.ConvertFrom ("test")).Return ("converted");

      var result = _storagePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.EqualTo ("converted"));
    }

    [Test]
    public void Read_DbNullValue_ReturnsEmptyString ()
    {
      _columnOrdinalProviderStub.Stub (stub => stub.GetOrdinal (_innerColumnDefinition, _dataReaderStub)).Return (3);
      _dataReaderStub.Stub (stub => stub[3]).Return (DBNull.Value);
      _typeConverterStub.Stub (mock => mock.ConvertFrom (null)).Return ("converted");

      var result = _storagePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.EqualTo ("converted"));
    }

    [Test]
    public void SplitValue ()
    {
      var value = new object();

      var result = _storagePropertyDefinition.SplitValue (value);

      Assert.That (result, Is.EqualTo (new[] { new ColumnValue(_innerColumnDefinition, value) }));
    }

    [Test]
    public void SplitValue_NullValue ()
    {
      var result = _storagePropertyDefinition.SplitValue (null);

      Assert.That (result, Is.EqualTo (new[] { new ColumnValue (_innerColumnDefinition, null) }));
    }

    [Test]
    public void SplitValueForComparison ()
    {
      var value = new object ();

      var result = _storagePropertyDefinition.SplitValueForComparison (value);

      Assert.That (result, Is.EqualTo (new[] { new ColumnValue (_innerColumnDefinition, value) }));
    }

    [Test]
    public void SplitValueForComparison_NullValue ()
    {
      var result = _storagePropertyDefinition.SplitValueForComparison (null);

      Assert.That (result, Is.EqualTo (new[] { new ColumnValue (_innerColumnDefinition, null) }));
    }
  }
}