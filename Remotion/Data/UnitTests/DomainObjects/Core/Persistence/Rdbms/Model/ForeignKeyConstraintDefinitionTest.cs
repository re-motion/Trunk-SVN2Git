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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ForeignKeyConstraintDefinitionTest
  {
    private ForeignKeyConstraintDefinition _constraint;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private TableDefinition _referencedTable;
    private SimpleColumnDefinition _referencingColumn;
    private SimpleColumnDefinition _referencedColumn;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider", typeof (UnitTestStorageObjectFactoryStub));
      _referencingColumn = new SimpleColumnDefinition ("COL1", typeof (string), "varchar", false);
      _referencedColumn = new SimpleColumnDefinition ("COL2", typeof (string), "varchar", false);

      _referencedTable = new TableDefinition (
          _storageProviderDefinition, "TableName", null, new IColumnDefinition[0], new ITableConstraintDefinition[0]);
      _constraint = new ForeignKeyConstraintDefinition ("Test", _referencedTable, new[] { _referencingColumn }, new[] { _referencedColumn });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_constraint.ConstraintName, Is.EqualTo ("Test"));
      Assert.That (_constraint.ReferencedTable, Is.SameAs (_referencedTable));
      Assert.That (_constraint.ReferencingColumns, Is.EqualTo (new[] { _referencingColumn }));
      Assert.That (_constraint.ReferencedColumns, Is.EqualTo (new[] { _referencedColumn }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The referencing- and referenced-columns have a different column count which is not allowed.")]
    public void Initialization_InvalidColumns ()
    {
      new ForeignKeyConstraintDefinition ("Test", _referencedTable, new[] { _referencingColumn }, new IColumnDefinition[0]);
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ITableConstraintDefinitionVisitor> ();

      visitorMock.Expect (mock => mock.VisitForeignKeyConstraintDefinition(_constraint));
      visitorMock.Replay ();

      _constraint.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }
  }
}