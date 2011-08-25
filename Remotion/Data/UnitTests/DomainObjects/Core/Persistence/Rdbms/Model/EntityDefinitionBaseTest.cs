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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class EntityDefinitionBaseTest
  {
    private TestableEntityDefinitionBase _entityDefinition;

    private ColumnDefinition _column1;
    private ColumnDefinition _column2;
    private ColumnDefinition _column3;

    private SimpleStoragePropertyDefinition _timestampProperty;
    private ObjectIDStoragePropertyDefinition _objectIDProperty;
    private SimpleStoragePropertyDefinition _property1;
    private SimpleStoragePropertyDefinition _property2;
    private SimpleStoragePropertyDefinition _property3;

    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private IIndexDefinition[] _indexes;
    private EntityNameDefinition[] _synonyms;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID");
      
      _column1 = ColumnDefinitionObjectMother.CreateColumn ("Column1");
      _column2 = ColumnDefinitionObjectMother.CreateColumn ("Column2");
      _column3 = ColumnDefinitionObjectMother.CreateColumn ("Column3");

      _timestampProperty = SimpleStoragePropertyDefinitionObjectMother.TimestampProperty;
      _objectIDProperty = ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      _property1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column1");
      _property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column2");
      _property3 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column3");
      
      _indexes = new[] { MockRepository.GenerateStub<IIndexDefinition>() };
      _synonyms = new[] { new EntityNameDefinition (null, "Test") };

      _entityDefinition = new TestableEntityDefinitionBase (
          _storageProviderDefinition,
          new EntityNameDefinition ("Schema", "Test"),
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new[] { _column1, _column2, _column3 },
          _objectIDProperty,
          _timestampProperty,
          new[] { _property1, _property2, _property3 },
          _indexes,
          _synonyms);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_entityDefinition.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));
      Assert.That (_entityDefinition.ViewName, Is.EqualTo (new EntityNameDefinition ("Schema", "Test")));

      Assert.That (_entityDefinition.ObjectIDProperty, Is.SameAs (_objectIDProperty));
      Assert.That (_entityDefinition.TimestampProperty, Is.SameAs (_timestampProperty));
      Assert.That (_entityDefinition.DataProperties, Is.EqualTo (new[] { _property1, _property2, _property3 }));

      Assert.That (_entityDefinition.Indexes, Is.EqualTo (_indexes));
      Assert.That (_entityDefinition.Synonyms, Is.EqualTo (_synonyms));
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var entityDefinition = new TestableEntityDefinitionBase (
          _storageProviderDefinition,
          null,
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new ColumnDefinition[0],
          _objectIDProperty,
          _timestampProperty,
          new[] { _property1, _property2, _property3 },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]
          );
      Assert.That (entityDefinition.ViewName, Is.Null);
    }

    [Test]
    public void StorageProviderID ()
    {
      Assert.That (_entityDefinition.StorageProviderID, Is.EqualTo ("SPID"));
    }

    [Test]
    public void GetAllProperties ()
    {
      var result = _entityDefinition.GetAllProperties ();

      Assert.That (
          result,
          Is.EqualTo (
              new IRdbmsStoragePropertyDefinition[]
              {
                  _objectIDProperty, 
                  _timestampProperty,
                  _property1, 
                  _property2, 
                  _property3
              }));
    }

    [Test]
    public void GetAllColumns ()
    {
      var result = _entityDefinition.GetAllColumns();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  ColumnDefinitionObjectMother.IDColumn, 
                  ColumnDefinitionObjectMother.ClassIDColumn,
                  ColumnDefinitionObjectMother.TimestampColumn,
                  _column1, _column2, _column3
              }));
    }


    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _entityDefinition).IsNull, Is.False);
    }
  }
}