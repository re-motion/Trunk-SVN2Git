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
using Remotion.Data.DomainObjects.Mapping;
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

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      _valueProperty = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty();
      _objectIDWithoutClassIDStorageDefinition = new ObjectIDWithoutClassIDStoragePropertyDefinition (
          _valueProperty, _classDefinition);
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
  }
}