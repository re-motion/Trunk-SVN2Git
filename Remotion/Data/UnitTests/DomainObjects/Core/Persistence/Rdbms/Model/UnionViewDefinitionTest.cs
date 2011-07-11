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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class UnionViewDefinitionTest
  {
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private UnionViewDefinition _unionViewDefinition;
    private SimpleColumnDefinition _column1;
    private SimpleColumnDefinition _column2;
    private SimpleColumnDefinition _column3;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private IIndexDefinition[] _indexes;
    private EntityNameDefinition[] _synonyms;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID");
      _column1 = ColumnDefinitionObjectMother.CreateColumn("Column1");
      _column2 = ColumnDefinitionObjectMother.CreateColumn("Column2");
      _column3 = ColumnDefinitionObjectMother.CreateColumn("Column3");
      _indexes = new[] { MockRepository.GenerateStub<IIndexDefinition>() };
      _synonyms = new[] { new EntityNameDefinition (null, "Test") };

      _tableDefinition1 = TableDefinitionObjectMother.Create (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Table1"),
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          _column1);
      _tableDefinition2 = TableDefinitionObjectMother.Create (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Table2"),
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          _column2,
          _column3);
      _unionViewDefinition = new UnionViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          new[] { _tableDefinition1, _tableDefinition2 },
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new[] { _column1, _column2, _column3 },
          _indexes,
          _synonyms);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_unionViewDefinition.ViewName.EntityName, Is.EqualTo ("Test"));
      Assert.That (_unionViewDefinition.UnionedEntities, Is.EqualTo (new[] { _tableDefinition1, _tableDefinition2 }));
      Assert.That (_unionViewDefinition.StorageProviderID, Is.EqualTo ("SPID"));
      Assert.That (_unionViewDefinition.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));

      Assert.That (_unionViewDefinition.ObjectIDColumn, Is.SameAs (ColumnDefinitionObjectMother.ObjectIDColumn));
      Assert.That (_unionViewDefinition.ClassIDColumn, Is.SameAs (ColumnDefinitionObjectMother.ClassIDColumn));
      Assert.That (_unionViewDefinition.TimestampColumn, Is.SameAs (ColumnDefinitionObjectMother.TimestampColumn));
      Assert.That (_unionViewDefinition.DataColumns, Is.EqualTo (new[] { _column1, _column2, _column3 }));

      Assert.That (_unionViewDefinition.LegacyEntityName, Is.Null);
      Assert.That (_unionViewDefinition.LegacyViewName, Is.EqualTo ("Test"));
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          _storageProviderDefinition,
          null,
          new[] { _tableDefinition1, _tableDefinition2 },
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      Assert.That (unionViewDefinition.ViewName, Is.Null);
    }

    [Test]
    public void Initialization_WithUnionedUnionEntity ()
    {
      new UnionViewDefinition (
          _storageProviderDefinition,
          null,
          new[] { _unionViewDefinition },
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemTypeException), ExpectedMessage =
        "The unioned entities must either be a TableDefinitions or UnionViewDefinitions.\r\nParameter name: unionedEntities")]
    public void Initialization_WithInvalidUnionedEntity ()
    {
      var filterViewDefinition = new FilterViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "ViewName"),
          _tableDefinition1,
          new[] { "x" },
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      new UnionViewDefinition (
          _storageProviderDefinition,
          null,
          new[] { filterViewDefinition },
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void GetAllColumns ()
    {
      var result = _unionViewDefinition.GetAllColumns();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  ColumnDefinitionObjectMother.ObjectIDColumn, ColumnDefinitionObjectMother.ClassIDColumn,
                  ColumnDefinitionObjectMother.TimestampColumn,
                  _column1, _column2, _column3
              }));
    }

    [Test]
    public void Indexes ()
    {
      var result = _unionViewDefinition.Indexes;

      Assert.That (result, Is.EqualTo (_indexes));
    }

    [Test]
    public void Synonyms ()
    {
      var result = _unionViewDefinition.Synonyms;

      Assert.That (result, Is.EqualTo (_synonyms));
    }

    [Test]
    public void CreateFullColumnList ()
    {
      var column4 = ColumnDefinitionObjectMother.CreateColumn("Test");
      var availableColumns = new[]
                             {
                                 ColumnDefinitionObjectMother.ObjectIDColumn, ColumnDefinitionObjectMother.ClassIDColumn,
                                 ColumnDefinitionObjectMother.TimestampColumn, _column3, column4, _column1
                             };

      var result = _unionViewDefinition.CreateFullColumnList (availableColumns).ToArray();

      Assert.That (result.Length, Is.EqualTo (6));
      Assert.That (result[0], Is.SameAs (ColumnDefinitionObjectMother.ObjectIDColumn));
      Assert.That (result[1], Is.SameAs (ColumnDefinitionObjectMother.ClassIDColumn));
      Assert.That (result[2], Is.SameAs (ColumnDefinitionObjectMother.TimestampColumn));
      Assert.That (result[3], Is.SameAs (_column1));
      Assert.That (result[4], Is.Null);
      Assert.That (result[5], Is.SameAs (_column3));
    }

    [Test]
    public void CreateFullColumnList_ChecksByContentNotByReference ()
    {
      var column1WithDifferentReference = new SimpleColumnDefinition (
          _column1.Name, _column1.PropertyType, _column1.StorageType, _column1.IsNullable, false);
      var availableColumns = new[]
                             {
                                 ColumnDefinitionObjectMother.ObjectIDColumn, ColumnDefinitionObjectMother.ClassIDColumn,
                                 ColumnDefinitionObjectMother.TimestampColumn, column1WithDifferentReference, _column2, _column3
                             };

      var result = _unionViewDefinition.CreateFullColumnList (availableColumns).ToArray();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  ColumnDefinitionObjectMother.ObjectIDColumn, ColumnDefinitionObjectMother.ClassIDColumn,
                  ColumnDefinitionObjectMother.TimestampColumn,
                  column1WithDifferentReference, _column2, _column3
              }));
    }

    [Test]
    public void CreateFullColumnList_OneColumnNotFound ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          new[] { _tableDefinition1, _tableDefinition2 },
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new[] { _column1, _column2 },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      var availableColumns = new[]
                             {
                                 ColumnDefinitionObjectMother.ObjectIDColumn, ColumnDefinitionObjectMother.ClassIDColumn,
                                 ColumnDefinitionObjectMother.TimestampColumn, _column1
                             };

      var result = unionViewDefinition.CreateFullColumnList (availableColumns).ToArray();

      Assert.That (result.Length, Is.EqualTo (5));
      Assert.That (result[0], Is.SameAs (ColumnDefinitionObjectMother.ObjectIDColumn));
      Assert.That (result[1], Is.SameAs (ColumnDefinitionObjectMother.ClassIDColumn));
      Assert.That (result[2], Is.SameAs (ColumnDefinitionObjectMother.TimestampColumn));
      Assert.That (result[3], Is.SameAs (_column1));
      Assert.That (result[4], Is.Null);
    }

    [Test]
    public void GetAllTables ()
    {
      var result = _unionViewDefinition.GetAllTables().ToArray();

      Assert.That (result, Is.EqualTo (new[] { _tableDefinition1, _tableDefinition2 }));
    }

    [Test]
    public void GetAllTables_IndirectTables ()
    {
      var tableDefinition3 = TableDefinitionObjectMother.Create (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Table3"),
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn);
      var baseUnionDefinition = new UnionViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "UnionView"),
          new IEntityDefinition[] { _unionViewDefinition, tableDefinition3 },
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      var result = baseUnionDefinition.GetAllTables().ToArray();

      Assert.That (result, Is.EqualTo (new[] { _tableDefinition1, _tableDefinition2, tableDefinition3 }));
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IEntityDefinitionVisitor>();

      visitorMock.Expect (mock => mock.VisitUnionViewDefinition (_unionViewDefinition));
      visitorMock.Replay();

      _unionViewDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_unionViewDefinition.IsNull, Is.False);
    }
  }
}