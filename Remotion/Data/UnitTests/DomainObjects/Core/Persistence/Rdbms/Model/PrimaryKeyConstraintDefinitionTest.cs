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
  public class PrimaryKeyConstraintDefinitionTest
  {
    private PrimaryKeyConstraintDefinition _constraint;
    private SimpleColumnDefinition _column1;
    private SimpleColumnDefinition _column2;

    [SetUp]
    public void SetUp ()
    {
      _column1 = new SimpleColumnDefinition ("COL1", typeof (string), "varchar", false);
      _column2 = new SimpleColumnDefinition ("COL2", typeof (int), "integer", true);
      _constraint = new PrimaryKeyConstraintDefinition ("Test", true, new IColumnDefinition[] { _column1, _column2 });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_constraint.ConstraintName, Is.EqualTo ("Test"));
      Assert.That (_constraint.Clustered, Is.True);
      Assert.That (_constraint.Columns, Is.EqualTo (new IColumnDefinition[] { _column1, _column2 }));
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ITableConstraintDefinitionVisitor> ();

      visitorMock.Expect (mock => mock.VisitPrimaryKeyConstraintDefinition (_constraint));
      visitorMock.Replay ();

      _constraint.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }
  }
}