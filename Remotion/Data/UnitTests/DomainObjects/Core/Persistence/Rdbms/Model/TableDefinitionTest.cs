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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class TableDefinitionTest
  {
    private TableDefinition _tableDefintion;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private PrimaryKeyConstraintDefinition[] _constraints;
    private IIndexDefinition[] _indexes;
    private EntityNameDefinition[] _synonyms;
    private SimpleColumnDefinition _column;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID");
      _column = ColumnDefinitionObjectMother.CreateColumn("COL1");
      _constraints = new[] { new PrimaryKeyConstraintDefinition ("PK_Table", true, new[] { ColumnDefinitionObjectMother.ObjectIDColumn }) };
      _indexes = new[] { MockRepository.GenerateStub<IIndexDefinition>() };
      _synonyms = new[] { new EntityNameDefinition (null, "Test") };

      _tableDefintion = new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          new EntityNameDefinition (null, "TestView"),
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new[] { _column },
          _constraints,
          _indexes,
          _synonyms);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_tableDefintion.TableName.EntityName, Is.EqualTo ("Test"));
      Assert.That (_tableDefintion.StorageProviderID, Is.EqualTo ("SPID"));
      Assert.That (_tableDefintion.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));

      Assert.That (_tableDefintion.ObjectIDColumn, Is.SameAs (ColumnDefinitionObjectMother.ObjectIDColumn));
      Assert.That (_tableDefintion.ClassIDColumn, Is.SameAs (ColumnDefinitionObjectMother.ClassIDColumn));
      Assert.That (_tableDefintion.TimestampColumn, Is.SameAs (ColumnDefinitionObjectMother.TimestampColumn));
      Assert.That (_tableDefintion.DataColumns, Is.EqualTo (new[] { _column }));

      Assert.That (_tableDefintion.LegacyEntityName, Is.EqualTo ("Test"));
      Assert.That (_tableDefintion.LegacyViewName, Is.EqualTo ("TestView"));
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var tableDefinition = new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          null,
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new[] { _column },
          _constraints,
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      Assert.That (tableDefinition.ViewName, Is.Null);
    }

    [Test]
    public void GetAllColumns ()
    {
      var result = _tableDefintion.GetAllColumns();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  ColumnDefinitionObjectMother.ObjectIDColumn, ColumnDefinitionObjectMother.ClassIDColumn, ColumnDefinitionObjectMother.TimestampColumn,
                  _column
              }));
    }

    [Test]
    public void Constraints ()
    {
      var result = _tableDefintion.Constraints;

      Assert.That (result, Is.EqualTo (_constraints));
    }

    [Test]
    public void Indexes ()
    {
      var result = _tableDefintion.Indexes;

      Assert.That (result, Is.EqualTo (_indexes));
    }

    [Test]
    public void Synonyms ()
    {
      var result = _tableDefintion.Synonyms;

      Assert.That (result, Is.EqualTo (_synonyms));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_tableDefintion.IsNull, Is.False);
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IEntityDefinitionVisitor>();

      visitorMock.Expect (mock => mock.VisitTableDefinition (_tableDefintion));
      visitorMock.Replay();

      _tableDefintion.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }
  }
}