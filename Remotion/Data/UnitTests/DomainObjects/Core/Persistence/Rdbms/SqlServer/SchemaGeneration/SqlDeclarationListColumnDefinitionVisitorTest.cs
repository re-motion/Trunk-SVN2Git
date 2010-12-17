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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlDeclarationListColumnDefinitionVisitorTest
  {
    private SqlDeclarationListColumnDefinitionVisitor _visitor;

    [SetUp]
    public void SetUp ()
    {
      _visitor = new SqlDeclarationListColumnDefinitionVisitor ();
    }

    [Test]
    public void VisitSimpleColumnDefinition_Nullable ()
    {
      var column = new SimpleColumnDefinition ("C1", typeof (int), "integer", true);

      _visitor.VisitSimpleColumnDefinition (column);
      var result = _visitor.GetDeclarationList ();

      Assert.That (result, Is.EqualTo ("  [C1] integer NULL,\r\n"));
    }

    [Test]
    public void VisitSimpleColumnDefinition_NotNullable ()
    {
      var column = new SimpleColumnDefinition ("C1", typeof (int), "integer", false);

      _visitor.VisitSimpleColumnDefinition (column);
      var result = _visitor.GetDeclarationList ();

      Assert.That (result, Is.EqualTo ("  [C1] integer NOT NULL,\r\n"));
    }

    [Test]
    public void VisitObjectIDWithClassIDColumnDefinition ()
    {
      var objectIDColumn = new SimpleColumnDefinition ("C1ID", typeof (int), "integer", false);
      var classIDColumn = new SimpleColumnDefinition ("C1ClassID", typeof (int), "integer", false);
      var column = new ObjectIDWithClassIDColumnDefinition (objectIDColumn, classIDColumn);

      _visitor.VisitObjectIDWithClassIDColumnDefinition (column);
      var result = _visitor.GetDeclarationList ();

      Assert.That (result, Is.EqualTo ("  [C1ID] integer NOT NULL,\r\n  [C1ClassID] integer NOT NULL,\r\n"));
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