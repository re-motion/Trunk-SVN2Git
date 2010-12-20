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
  public class NameListColumnDefinitionVisitorTest
  {
    private NameListColumnDefinitionVisitor _visitorAllowingNulls;

    [SetUp]
    public void SetUp ()
    {
      _visitorAllowingNulls = new NameListColumnDefinitionVisitor (true);
    }

    [Test]
    public void VisitSimpleColumnDefinition ()
    {
      var column = new SimpleColumnDefinition ("C1", typeof (int), "integer", true);

      _visitorAllowingNulls.VisitSimpleColumnDefinition (column);
      var result = _visitorAllowingNulls.GetNameList ();

      Assert.That (result, Is.EqualTo ("[C1]"));
    }

    [Test]
    public void VisitSimpleColumnDefinition_SecondColumn ()
    {
      var column1 = new SimpleColumnDefinition ("C1", typeof (int), "integer", true);
      var column2 = new SimpleColumnDefinition ("C2", typeof (int), "integer", true);

      _visitorAllowingNulls.VisitSimpleColumnDefinition (column1);
      _visitorAllowingNulls.VisitSimpleColumnDefinition (column2);
      var result = _visitorAllowingNulls.GetNameList ();

      Assert.That (result, Is.EqualTo ("[C1], [C2]"));
    }
    
    [Test]
    public void VisitObjectIDWithClassIDColumnDefinition ()
    {
      var objectIDColumn = new SimpleColumnDefinition ("C1ID", typeof (int), "integer", false);
      var classIDColumn = new SimpleColumnDefinition ("C1ClassID", typeof (int), "integer", false);
      var column = new ObjectIDWithClassIDColumnDefinition (objectIDColumn, classIDColumn);

      _visitorAllowingNulls.VisitObjectIDWithClassIDColumnDefinition (column);
      var result = _visitorAllowingNulls.GetNameList ();

      Assert.That (result, Is.EqualTo ("[C1ID], [C1ClassID]"));
    }

    [Test]
    public void VisitNullColumnDefinition_AllowNull ()
    {
      var column = new NullColumnDefinition ();

      _visitorAllowingNulls.VisitNullColumnDefinition (column);
      var result = _visitorAllowingNulls.GetNameList ();

      Assert.That (result, Is.EqualTo ("NULL"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Null columns are not supported at this point.")]
    public void VisitNullColumnDefinition_DisallowNull ()
    {
      var column = new NullColumnDefinition ();

      var visitorNotAllowingNulls = new NameListColumnDefinitionVisitor (false);
      visitorNotAllowingNulls.VisitNullColumnDefinition (column);
    }
  }
}