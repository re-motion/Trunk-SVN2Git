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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ColumnDefinitionTest
  {
    private ColumnDefinition _columnDefinition;
    private Type _type;
    public string DummyProperty { get; set; }
    public string OtherProperty { get; set; }

    [SetUp]
    public void SetUp ()
    {
      _type = typeof (string);
      _columnDefinition = new ColumnDefinition ("Name", _type, new StorageTypeInformation("varchar", DbType.String), true, true);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_columnDefinition.Name, Is.EqualTo ("Name"));
      Assert.That (_columnDefinition.PropertyType, Is.SameAs(_type));
      Assert.That (_columnDefinition.StorageTypeInfo.StorageType, Is.EqualTo("varchar"));
      Assert.That (_columnDefinition.StorageTypeInfo.DbType, Is.EqualTo (DbType.String));
      Assert.That (_columnDefinition.IsNullable, Is.True);
      Assert.That (_columnDefinition.IsPartOfPrimaryKey, Is.True);
    }

    [Test]
    public void Equals_True ()
    {
      var other = new ColumnDefinition ("Name", _type, new StorageTypeInformation("varchar", DbType.String), true, false);

      Assert.That (_columnDefinition.Equals (other), Is.True);
      Assert.That (_columnDefinition.Equals ((object) other), Is.True);
    }

    [Test]
    public void Equals_False_DifferentType ()
    {
      var other = SimpleStoragePropertyDefinitionObjectMother.ObjectIDProperty;

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_DifferentName ()
    {
      var other = new ColumnDefinition ("Name2", _type, new StorageTypeInformation("varchar", DbType.String), true, false);

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_DifferentPropertyType ()
    {
      var other = new ColumnDefinition ("Name", typeof (object), new StorageTypeInformation("varchar", DbType.String), true, false);

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_DifferentStorageTypeInfo ()
    {
      var other = new ColumnDefinition ("Name", _type, new StorageTypeInformation("varchar2", DbType.String), true, false);

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_DifferentNullability ()
    {
      var other = new ColumnDefinition ("Name", _type, new StorageTypeInformation("varchar", DbType.String), false, false);

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
      var other = new ColumnDefinition ("Name", _type, new StorageTypeInformation("varchar", DbType.String), true, false);

      Assert.That (_columnDefinition.GetHashCode (), Is.EqualTo (other.GetHashCode ()));
    }

    [Test]
    public void To_String ()
    {
      Assert.That (_columnDefinition.ToString (), Is.EqualTo ("Name varchar NULL"));
    }
  }
}