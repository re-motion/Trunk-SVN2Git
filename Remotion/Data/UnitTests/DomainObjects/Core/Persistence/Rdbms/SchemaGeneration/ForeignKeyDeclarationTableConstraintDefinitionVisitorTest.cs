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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class ForeignKeyDeclarationTableConstraintDefinitionVisitorTest
  {
    private ForeignKeyDeclarationTableConstraintDefinitionVisitor _visitor;
    private ISqlDialect _sqlDialectStub;
    private SimpleColumnDefinition _referencingColumn1;
    private SimpleColumnDefinition _referencingColumn2;
    private SimpleColumnDefinition _referencedColumn1;
    private SimpleColumnDefinition _referencedColumn2;
    private ForeignKeyConstraintDefinition _foreignKeyConstraintDefinition1;
    private ForeignKeyConstraintDefinition _foreignKeyConstraintDefinition2;

    [SetUp]
    public void SetUp ()
    {
      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect> ();
      _visitor = new ForeignKeyDeclarationTableConstraintDefinitionVisitor (_sqlDialectStub);

      _referencingColumn1 = new SimpleColumnDefinition ("ID1", typeof (int), "integer", false, false);
      _referencingColumn2 = new SimpleColumnDefinition ("ID2", typeof (int), "integer", false, false);
      _referencedColumn1 = new SimpleColumnDefinition ("FKID1", typeof (int), "integer", false, true);
      _referencedColumn2 = new SimpleColumnDefinition ("FKID2", typeof (int), "integer", false, true);
      
      _foreignKeyConstraintDefinition1 = new ForeignKeyConstraintDefinition (
          "FK1", "Table1", new[] { _referencingColumn1 }, new[] { _referencedColumn1 });

      _foreignKeyConstraintDefinition2 = new ForeignKeyConstraintDefinition (
          "FK2", "Table2", new[] { _referencingColumn1, _referencingColumn2 }, new[] { _referencedColumn1, _referencedColumn2 });
    }

    [Test]
    public void VisitForeignKeyConstraintDefinition_OneConstraint ()
    {
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("FKID1")).Return ("[FKID1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("ID1")).Return ("[ID1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("FK1")).Return ("[FK1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("dbo")).Return ("[dbo]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Table1")).Return ("[Table1]");
      _sqlDialectStub.Stub (stub => stub.ConstraintDelimiter).Return (",");

      _visitor.VisitForeignKeyConstraintDefinition (_foreignKeyConstraintDefinition1);

      var result = _visitor.GetConstraintStatement();

      Assert.That (result, Is.EqualTo (" CONSTRAINT [FK1] FOREIGN KEY ([FKID1]) REFERENCES [dbo].[Table1] ([ID1])"));
    }

    [Test]
    public void VisitForeignKeyConstraintDefinition_SeveralConstraintsWithSeveralColumns ()
    {
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("FKID1")).Return ("[FKID1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("FKID2")).Return ("[FKID2]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("ID1")).Return ("[ID1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("ID2")).Return ("[ID2]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("FK1")).Return ("[FK1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("FK2")).Return ("[FK2]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("dbo")).Return ("[dbo]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Table1")).Return ("[Table1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Table2")).Return ("[Table2]");
      _sqlDialectStub.Stub (stub => stub.ConstraintDelimiter).Return (",");

      _visitor.VisitForeignKeyConstraintDefinition (_foreignKeyConstraintDefinition1);
      _visitor.VisitForeignKeyConstraintDefinition (_foreignKeyConstraintDefinition2);

      var result = _visitor.GetConstraintStatement ();

      Assert.That (
          result,
          Is.EqualTo (
              " CONSTRAINT [FK1] FOREIGN KEY ([FKID1]) REFERENCES [dbo].[Table1] ([ID1]),\r\n " +
              " CONSTRAINT [FK2] FOREIGN KEY ([FKID1], [FKID2]) REFERENCES [dbo].[Table2] ([ID1], [ID2])"));
    }
  }
}