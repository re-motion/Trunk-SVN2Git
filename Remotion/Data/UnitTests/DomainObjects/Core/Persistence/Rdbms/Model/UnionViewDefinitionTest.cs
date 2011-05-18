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
      _column1 = new SimpleColumnDefinition ("Column1", typeof (string), "varchar", true, false);
      _column2 = new SimpleColumnDefinition ("Column2", typeof (string), "varchar", true, false);
      _column3 = new SimpleColumnDefinition ("Column3", typeof (string), "varchar", true, false);
      _indexes = new[] { MockRepository.GenerateStub<IIndexDefinition>() };
      _synonyms = new[] { new EntityNameDefinition (null, "Test") };

      _tableDefinition1 = new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Table1"),
          new EntityNameDefinition (null, "View1"),
          new[] { _column1 },
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      _tableDefinition2 = new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Table2"),
          new EntityNameDefinition (null, "View2"),
          new[] { _column2, _column3 },
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      _unionViewDefinition = new UnionViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          new[] { _tableDefinition1, _tableDefinition2 },
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
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          _storageProviderDefinition,
          null,
          new[] { _tableDefinition1, _tableDefinition2 },
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
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      new UnionViewDefinition (
          _storageProviderDefinition,
          null,
          new[] { filterViewDefinition },
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void LegacyEntityName ()
    {
      Assert.That (_unionViewDefinition.LegacyEntityName, Is.Null);
    }

    [Test]
    public void LegacyViewName ()
    {
      Assert.That (_unionViewDefinition.LegacyViewName, Is.EqualTo ("Test"));
    }

    [Test]
    public void Columns ()
    {
      var result = _unionViewDefinition.Columns;

      Assert.That (result, Is.EqualTo (new[] { _column1, _column2, _column3 }));
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
      var column4 = new SimpleColumnDefinition ("Test", typeof (int), "integer", false, false);
      var availableColumns = new[] { _column3, column4, _column1 };

      var result = _unionViewDefinition.CreateFullColumnList (availableColumns).ToArray();

      Assert.That (result.Length, Is.EqualTo (3));
      Assert.That (result[0], Is.SameAs (_column1));
      Assert.That (result[1], Is.Null);
      Assert.That (result[2], Is.SameAs (_column3));
    }

    [Test]
    public void CreateFullColumnList_ChecksByContentNotByReference ()
    {
      var column1WithDifferentReference = new SimpleColumnDefinition (
          _column1.Name, _column1.PropertyType, _column1.StorageType, _column1.IsNullable, false);
      var availableColumns = new[] { column1WithDifferentReference, _column2, _column3 };

      var result = _unionViewDefinition.CreateFullColumnList (availableColumns).ToArray();

      Assert.That (result, Is.EqualTo (new[] { column1WithDifferentReference, _column2, _column3 }));
    }

    [Test]
    public void CreateFullColumnList_OneColumnNotFound ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          new[] { _tableDefinition1, _tableDefinition2 },
          new[] { _column1, _column2 },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      var availableColumns = new[] { _column1 };

      var result = unionViewDefinition.CreateFullColumnList (availableColumns).ToArray();

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0], Is.SameAs(_column1));
      Assert.That (result[1], Is.Null);
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
      var tableDefinition3 = new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Table3"),
          new EntityNameDefinition (null, "View"),
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      var baseUnionDefinition = new UnionViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "UnionView"),
          new IEntityDefinition[] { _unionViewDefinition, tableDefinition3 },
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