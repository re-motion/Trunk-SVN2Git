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
  }
}