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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ObjectIDStoragePropertyDefinitionTest
  {
    private SimpleStoragePropertyDefinition _objectIDColumn;
    private SimpleStoragePropertyDefinition _classIDColumn;
    private ObjectIDStoragePropertyDefinition _columnDefinition;

    [SetUp]
    public void SetUp ()
    {
      _objectIDColumn = SimpleStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      _classIDColumn = SimpleStoragePropertyDefinitionObjectMother.ClassIDProperty;
      _columnDefinition = new ObjectIDStoragePropertyDefinition (_objectIDColumn, _classIDColumn);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_columnDefinition.ValueProperty, Is.SameAs (_objectIDColumn));
      Assert.That (_columnDefinition.ClassIDProperty, Is.SameAs (_classIDColumn));
      Assert.That (((IRdbmsStoragePropertyDefinition) _columnDefinition).Name, Is.EqualTo ("ID"));
    }

    [Test]
    public void GetColumnForLookup ()
    {
      Assert.That (_columnDefinition.GetColumnForLookup(), Is.SameAs (_objectIDColumn.ColumnDefinition));
    }

    [Test]
    public void GetColumnForForeignKey ()
    {
      Assert.That (_columnDefinition.GetColumnForForeignKey(), Is.SameAs (_objectIDColumn.ColumnDefinition));
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (_columnDefinition.GetColumns(), Is.EqualTo (new[] { _objectIDColumn.ColumnDefinition, _classIDColumn.ColumnDefinition }));
    }

    [Test]
    public void Equals_True_WithClassIDColumns ()
    {
      var other = new ObjectIDStoragePropertyDefinition (SimpleStoragePropertyDefinitionObjectMother.ObjectIDProperty, SimpleStoragePropertyDefinitionObjectMother.ClassIDProperty);

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
    public void Equals_False_DifferentObjectIDColumn ()
    {
      var other = new ObjectIDStoragePropertyDefinition (
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("ObjectID"), SimpleStoragePropertyDefinitionObjectMother.ClassIDProperty);

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_DifferentClassIDColumn ()
    {
      var other = new ObjectIDStoragePropertyDefinition (
          SimpleStoragePropertyDefinitionObjectMother.ObjectIDProperty, SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Class_ID"));

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_Null ()
    {
      Assert.That (_columnDefinition.Equals ((IRdbmsStoragePropertyDefinition) null), Is.False);
      Assert.That (_columnDefinition.Equals ((object) null), Is.False);
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var other = new ObjectIDStoragePropertyDefinition (SimpleStoragePropertyDefinitionObjectMother.ObjectIDProperty, SimpleStoragePropertyDefinitionObjectMother.ClassIDProperty);

      Assert.That (_columnDefinition.GetHashCode(), Is.EqualTo (other.GetHashCode()));
    }
    
  }
}