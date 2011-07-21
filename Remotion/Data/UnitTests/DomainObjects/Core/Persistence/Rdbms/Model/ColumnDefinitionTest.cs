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
using Remotion.Data.UnitTests.DomainObjects.Factories;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ColumnDefinitionTest
  {
    private Type _type;
    private StringConverter _stringConverter;
    private StorageTypeInformation _storageTypeInformation;

    private ColumnDefinition _columnDefinition;

    [SetUp]
    public void SetUp ()
    {
      _type = typeof (string);
      _stringConverter = new StringConverter();
      _storageTypeInformation = new StorageTypeInformation ("varchar", DbType.String, typeof (string), _stringConverter);

      _columnDefinition = new ColumnDefinition ("Name", _type, _storageTypeInformation, true, true);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_columnDefinition.Name, Is.EqualTo ("Name"));
      Assert.That (_columnDefinition.PropertyType, Is.SameAs (_type));
      Assert.That (_columnDefinition.StorageTypeInfo, Is.SameAs (_storageTypeInformation));
      Assert.That (_columnDefinition.IsNullable, Is.True);
      Assert.That (_columnDefinition.IsPartOfPrimaryKey, Is.True);
    }

    [Test]
    public void Equals_True ()
    {
      var other = new ColumnDefinition (
          "Name", _type, new StorageTypeInformation ("varchar", DbType.String, typeof (string), new StringConverter()), true, false);

      Assert.That (_columnDefinition.Equals (other), Is.True);
      Assert.That (_columnDefinition.Equals ((object) other), Is.True);
    }

    [Test]
    public void Equals_False_DifferentType ()
    {
      var other = SimpleStoragePropertyDefinitionObjectMother.IDProperty;

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals (other), Is.False);
    }

    [Test]
    public void Equals_False_DifferentName ()
    {
      var other = new ColumnDefinition (
          "Name2", _type, _storageTypeInformation, true, false);

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_DifferentPropertyType ()
    {
      var other = new ColumnDefinition (
          "Name", typeof (object), _storageTypeInformation, true, false);

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_DifferentStorageTypeInfo ()
    {
      var other = new ColumnDefinition (
          "Name", _type, new StorageTypeInformation ("varchar2", DbType.String, typeof (string), _stringConverter), true, false);

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_DifferentNullability ()
    {
      var other = new ColumnDefinition (
          "Name", _type, _storageTypeInformation, false, false);

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_Null ()
    {
      Assert.That (_columnDefinition.Equals (null), Is.False);
      Assert.That (_columnDefinition.Equals ((object) null), Is.False);
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var other = new ColumnDefinition (
          "Name", _type, _storageTypeInformation, true, false);

      Assert.That (_columnDefinition.GetHashCode(), Is.EqualTo (other.GetHashCode()));
    }

    [Test]
    public void To_String ()
    {
      Assert.That (_columnDefinition.ToString(), Is.EqualTo ("Name varchar NULL"));
    }
  }
}