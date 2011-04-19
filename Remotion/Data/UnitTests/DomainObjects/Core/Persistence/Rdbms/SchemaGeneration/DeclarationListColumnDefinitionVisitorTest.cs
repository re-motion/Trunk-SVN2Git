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
  public class DeclarationListColumnDefinitionVisitorTest
  {
    private DeclarationListColumnDefinitionVisitor _visitor;
    private ISqlDialect _sqlDialectStub;
    private SimpleColumnDefinition _column1;
    private SimpleColumnDefinition _column2;

    [SetUp]
    public void SetUp ()
    {
      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _visitor = new DeclarationListColumnDefinitionVisitor (_sqlDialectStub);
      _column1 = new SimpleColumnDefinition ("C1", typeof (int), "integer", true, false);
      _column2 = new SimpleColumnDefinition ("C2", typeof (int), "integer", true, false);
    }

    [Test]
    public void GetDeclarationList ()
    {
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("C1")).Return ("[C1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("C2")).Return ("[C2]");

      var result = DeclarationListColumnDefinitionVisitor.GetDeclarationList (new[] { _column1, _column2 }, _sqlDialectStub);

      Assert.That (result, Is.EqualTo ("  [C1] integer NULL,\r\n  [C2] integer NULL"));
    }

    [Test]
    public void VisitSimpleColumnDefinition_Nullable ()
    {
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("C1")).Return ("[C1]");

      _visitor.VisitSimpleColumnDefinition (_column1);
      var result = _visitor.GetDeclarationList ();

      Assert.That (result, Is.EqualTo ("  [C1] integer NULL"));
    }

    [Test]
    public void VisitSimpleColumnDefinition_NotNullable ()
    {
      var column = new SimpleColumnDefinition ("C1", typeof (int), "integer", false, false);

      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("C1")).Return ("[C1]");

      _visitor.VisitSimpleColumnDefinition (column);
      var result = _visitor.GetDeclarationList ();

      Assert.That (result, Is.EqualTo ("  [C1] integer NOT NULL"));
    }

    [Test]
    public void VisitSqlIndexedColumnDefinition ()
    {
      var innerColumn = new SimpleColumnDefinition ("C1", typeof (int), "integer", false, false);
      var indexedColumn = new SqlIndexedColumnDefinition (innerColumn);

      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("C1")).Return ("[C1]");

      _visitor.VisitSqlIndexedColumnDefinition (indexedColumn);
      var result = _visitor.GetDeclarationList();

      Assert.That (result, Is.EqualTo ("  [C1] integer NOT NULL"));
    }

    [Test]
    public void VisitIDColumnDefinition ()
    {
      var objectIDColumn = new SimpleColumnDefinition ("C1ID", typeof (int), "integer", false, false);
      var classIDColumn = new SimpleColumnDefinition ("C1ClassID", typeof (int), "integer", false, false);
      var column = new IDColumnDefinition (objectIDColumn, classIDColumn);

      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("C1ID")).Return ("[C1ID]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("C1ClassID")).Return ("[C1ClassID]");
      
      _visitor.VisitIDColumnDefinition (column);
      var result = _visitor.GetDeclarationList ();

      Assert.That (result, Is.EqualTo ("  [C1ID] integer NOT NULL,\r\n  [C1ClassID] integer NOT NULL"));
    }

    [Test]
    public void VisitIDColumnDefinition_ClassIDColumnIsNull ()
    {
      var objectIDColumn = new SimpleColumnDefinition ("C1ID", typeof (int), "integer", false, false);
      var column = new IDColumnDefinition (objectIDColumn, null);

      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("C1ID")).Return ("[C1ID]");

      _visitor.VisitIDColumnDefinition (column);
      var result = _visitor.GetDeclarationList ();

      Assert.That (result, Is.EqualTo ("  [C1ID] integer NOT NULL"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot declare a non-existing column.")]
    public void VisitNullColumnDefinition ()
    {
      var column = new NullColumnDefinition ();

      _visitor.VisitNullColumnDefinition (column);
    }
  }
}