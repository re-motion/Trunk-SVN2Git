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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class StorageTypeInformationTest
  {
    private StorageTypeInformation _storageTypeInformation;
    private TypeConverter _typeConverterStub;

    [SetUp]
    public void SetUp ()
    {
      _typeConverterStub = MockRepository.GenerateStub<TypeConverter>();
      _storageTypeInformation = new StorageTypeInformation ("test", DbType.Boolean, typeof(bool), _typeConverterStub);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_storageTypeInformation.StorageType, Is.EqualTo ("test"));
      Assert.That (_storageTypeInformation.DbType, Is.EqualTo (DbType.Boolean));
      Assert.That (_storageTypeInformation.ParameterValueType, Is.EqualTo(typeof(bool)));
      Assert.That (_storageTypeInformation.TypeConverter, Is.SameAs(_typeConverterStub));
    }

    [Test]
    public void CreateDataParameter ()
    {
      var commandMock = MockRepository.GenerateStrictMock<IDbCommand> ();
      var dataParameterMock = MockRepository.GenerateStrictMock<IDbDataParameter> ();

      _typeConverterStub.Stub (stub => stub.ConvertTo ("value", _storageTypeInformation.ParameterValueType)).Return ("converted value");

      commandMock.Expect (mock => mock.CreateParameter()).Return (dataParameterMock);
      commandMock.Replay();

      dataParameterMock.Expect (mock => mock.DbType = _storageTypeInformation.DbType);
      dataParameterMock.Expect (mock => mock.Value = "converted value");
      dataParameterMock.Replay();

      var result = _storageTypeInformation.CreateDataParameter (commandMock, "value");

      commandMock.VerifyAllExpectations();
      dataParameterMock.VerifyAllExpectations();

      Assert.That (result, Is.SameAs (dataParameterMock));
    }

    [Test]
    public void CreateDataParameter_Null ()
    {
      var commandMock = MockRepository.GenerateStrictMock<IDbCommand> ();
      var dataParameterMock = MockRepository.GenerateStrictMock<IDbDataParameter> ();

      _typeConverterStub.Stub (stub => stub.ConvertTo ("value", _storageTypeInformation.ParameterValueType)).Return (null);

      commandMock.Expect (mock => mock.CreateParameter ()).Return (dataParameterMock);
      commandMock.Replay ();

      dataParameterMock.Expect (mock => mock.DbType = _storageTypeInformation.DbType);
      dataParameterMock.Expect (mock => mock.Value = DBNull.Value);
      dataParameterMock.Replay ();

      var result = _storageTypeInformation.CreateDataParameter (commandMock, "value");

      commandMock.VerifyAllExpectations ();
      dataParameterMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (dataParameterMock));
    }

    [Test]
    public void Read ()
    {
      var dataReaderMock = MockRepository.GenerateStrictMock<IDataReader>();
      dataReaderMock.Expect (mock => mock[17]).Return ("value");
      dataReaderMock.Replay();

      _typeConverterStub.Stub (stub => stub.ConvertFrom ("value")).Return ("converted value");

      var result = _storageTypeInformation.Read (dataReaderMock, 17);

      dataReaderMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("converted value"));
    }

    [Test]
    public void Read_DBNull ()
    {
      var dataReaderMock = MockRepository.GenerateStrictMock<IDataReader> ();
      dataReaderMock.Expect (mock => mock[17]).Return (DBNull.Value);
      dataReaderMock.Replay ();

      _typeConverterStub.Stub (stub => stub.ConvertFrom (null)).Return ("converted null value");

      var result = _storageTypeInformation.Read (dataReaderMock, 17);

      dataReaderMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("converted null value"));
    }

    [Test]
    public void Equals_True ()
    {
      var one = new StorageTypeInformation ("test", DbType.Boolean, typeof (bool), new BooleanConverter ());
      var two = new StorageTypeInformation ("test", DbType.Boolean, typeof (bool), new BooleanConverter ());

      Assert.That (one.Equals (two), Is.True);
    }

    [Test]
    public void Equals_DifferentStorageType_False ()
    {
      var one = new StorageTypeInformation ("test", DbType.Boolean, typeof (bool), new BooleanConverter ());
      var two = new StorageTypeInformation ("test2", DbType.Boolean, typeof (bool), new BooleanConverter ());

      Assert.That (one.Equals (two), Is.False);
    }

    [Test]
    public void Equals_DifferentDbType_False ()
    {
      var one = new StorageTypeInformation ("test", DbType.Boolean, typeof (bool), new BooleanConverter ());
      var two = new StorageTypeInformation ("test", DbType.Int64, typeof (bool), new BooleanConverter ());

      Assert.That (one.Equals (two), Is.False);
    }

    [Test]
    public void Equals_DifferentParameterType_False ()
    {
      var one = new StorageTypeInformation ("test", DbType.Boolean, typeof (bool), new BooleanConverter ());
      var two = new StorageTypeInformation ("test", DbType.Boolean, typeof (bool?), new BooleanConverter ());

      Assert.That (one.Equals (two), Is.False);
    }

    [Test]
    public void Equals_DifferentTypeConverter_False ()
    {
      var one = new StorageTypeInformation ("test", DbType.Boolean, typeof (bool), new BooleanConverter ());
      var two = new StorageTypeInformation ("test", DbType.Boolean, typeof (bool), new StringConverter ());

      Assert.That (one.Equals (two), Is.False);
    }

    [Test]
    public void GetHashcode_EqualObjects ()
    {
      var one = new StorageTypeInformation ("test", DbType.Boolean, typeof (bool), new BooleanConverter ());
      var two = new StorageTypeInformation ("test", DbType.Boolean, typeof (bool), new BooleanConverter ());

      Assert.That (one.GetHashCode(), Is.EqualTo (two.GetHashCode()));
    }
    
  }
}