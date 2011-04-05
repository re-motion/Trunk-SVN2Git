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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class PrimaryKeyDeclarationTableConstraintDefinitionVisitorTest
  {
    private ISqlDialect _sqlDialectStub;
    private PrimaryKeyDeclarationTableConstraintDefinitionVisitor _visitor;

    [SetUp]
    public void SetUp ()
    {
      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect> ();
      _visitor = new PrimaryKeyDeclarationTableConstraintDefinitionVisitor (_sqlDialectStub);
    }

    [Test]
    public void VisitPrimaryKeyConstraintDefinition ()
    {
      var column1 = new SimpleColumnDefinition ("COL1", typeof (ObjectID), "uniqueidentifier", false, false);
      var column2 = new SimpleColumnDefinition ("COL2", typeof (string), "varchar", true, false);
      var primaryKeyConstraint = new PrimaryKeyConstraintDefinition ("PK_Test", true, new[] { column1, column2 });

      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("PK_Test")).Return ("[PK_Test]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("COL1")).Return ("[COL1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("COL2")).Return ("[COL2]");

      _visitor.VisitPrimaryKeyConstraintDefinition (primaryKeyConstraint);

      Assert.That (_visitor.GetConstraintStatement(), Is.EqualTo ("CONSTRAINT [PK_Test] PRIMARY KEY CLUSTERED ([COL1], [COL2])"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Only one primary key constraint is allowed.")]
    public void VisitPrimaryKeyConstraintDefinition_Twice ()
    {
      var primaryKeyConstraint = new PrimaryKeyConstraintDefinition ("PK_Test", true, new SimpleColumnDefinition[0]);

      _visitor.VisitPrimaryKeyConstraintDefinition (primaryKeyConstraint);
      _visitor.VisitPrimaryKeyConstraintDefinition (primaryKeyConstraint);
    }
  }
}