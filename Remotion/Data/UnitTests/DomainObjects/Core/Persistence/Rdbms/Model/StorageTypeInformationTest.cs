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
using Remotion.ObjectBinding;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class StorageTypeInformationTest
  {
    private StorageTypeInformation _storageTypeInformation;
    private BooleanConverter _booleanConverter;

    [SetUp]
    public void SetUp ()
    {
      _booleanConverter = new BooleanConverter();
      _storageTypeInformation = new StorageTypeInformation ("test", DbType.Boolean, typeof(bool), _booleanConverter);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_storageTypeInformation.StorageType, Is.EqualTo ("test"));
      Assert.That (_storageTypeInformation.DbType, Is.EqualTo (DbType.Boolean));
      Assert.That (_storageTypeInformation.ParameterValueType, Is.EqualTo(typeof(bool)));
      Assert.That (_storageTypeInformation.TypeConverter, Is.SameAs(_booleanConverter));
    }

    [Test]
    public void Equals_True ()
    {
      Assert.That (_storageTypeInformation.Equals (new StorageTypeInformation ("test", DbType.Boolean, typeof(bool), new BooleanConverter())), Is.True);
    }

    [Test]
    public void Equals_DifferentStorageType_False ()
    {
      Assert.That (_storageTypeInformation.Equals (new StorageTypeInformation ("test2", DbType.Boolean, typeof(bool), _booleanConverter)), Is.False);
    }

    [Test]
    public void Equals_DifferentDbType_False ()
    {
      Assert.That (_storageTypeInformation.Equals (new StorageTypeInformation ("test", DbType.String, typeof(bool), _booleanConverter)), Is.False);
    }

    [Test]
    public void Equals_DifferentParameterType_False ()
    {
      Assert.That (_storageTypeInformation.Equals (new StorageTypeInformation ("test", DbType.Boolean, typeof (string), _booleanConverter)), Is.False);
    }

    [Test]
    public void Equals_DifferentTypeConverter_False ()
    {
      Assert.That (_storageTypeInformation.Equals (new StorageTypeInformation ("test", DbType.Boolean, typeof (bool), new StringConverter())), Is.False);
    }

    [Test]
    public void GetHashcode_EqualObjects ()
    {
      var storageTypeInformation = new StorageTypeInformation ("test", DbType.Boolean, typeof(bool), new BooleanConverter());

      Assert.That (_storageTypeInformation.GetHashCode(), Is.EqualTo (storageTypeInformation.GetHashCode()));
    }
  }
}