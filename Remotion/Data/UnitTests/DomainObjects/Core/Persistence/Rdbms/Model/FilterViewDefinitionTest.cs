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
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class FilterViewDefinitionTest
  {
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;
    private ColumnDefinition _column3;
    private TableDefinition _entityDefinition;
    private FilterViewDefinition _filterViewDefinition;

    [SetUp]
    public void SetUp ()
    {
      _column1 = new ColumnDefinition ("Column1", typeof (string).GetProperty ("Length"));
      _column2 = new ColumnDefinition ("Column2", typeof (ArrayList).GetProperty ("Count"));
      _column3 = new ColumnDefinition ("Column3", typeof (TableDefinition).GetProperty ("TableName"));
      _entityDefinition = new TableDefinition ("Table", new[] { _column1, _column2, _column3 });

      _filterViewDefinition = new FilterViewDefinition ("Test", _entityDefinition, "CLASSID", new[] { "Column1", "Column3" });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_filterViewDefinition.ClassID, Is.EqualTo ("CLASSID"));
      Assert.That (_filterViewDefinition.BaseEntity, Is.SameAs (_entityDefinition));
      Assert.That (_filterViewDefinition.LegacyEntityName, Is.Null);
      Assert.That (_filterViewDefinition.ViewName, Is.EqualTo ("Test"));
    }

    [Test]
    public void GetColumns ()
    {
      var result = _filterViewDefinition.GetColumns();

      Assert.That (result.SequenceEqual (new[] { _column1, _column3 }), Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
      ExpectedMessage = "Following column names could not be found: 'InvalidColumnName1, InvalidColumnName3'")]
    public void ColumnNamesNotFoundInBaseEntitiyDefinition ()
    {
      new FilterViewDefinition ("Test", _entityDefinition, "CLASSID", new[] { "Column1", "InvalidColumnName1", "Column2", "InvalidColumnName3" });
    }
  }
}