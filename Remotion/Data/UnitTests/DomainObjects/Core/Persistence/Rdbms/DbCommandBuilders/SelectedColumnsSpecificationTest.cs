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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class SelectedColumnsSpecificationTest
  {
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;
    private ColumnDefinition _column3;
    private SelectedColumnsSpecification _specification;
    private ISqlDialect _sqlDialectStub;

    [SetUp]
    public void SetUp ()
    {
      _column1 = ColumnDefinitionObjectMother.CreateColumn("Column1");
      _column2 = ColumnDefinitionObjectMother.CreateColumn("Column2");
      _column3 = ColumnDefinitionObjectMother.CreateColumn("Column3");
      _specification = new SelectedColumnsSpecification (new[] { _column1, _column2, _column3 });
      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Column1")).Return ("[Column1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Column2")).Return ("[Column2]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Column3")).Return ("[Column3]");
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_specification.SelectedColumns, Is.EqualTo (new[] { _column1, _column2, _column3 }));
    }

    [Test]
    public void AppendProjection_StringBuilderIsEmpty ()
    {
      var sb = new StringBuilder();

      _specification.AppendProjection (sb, _sqlDialectStub);

      Assert.That (sb.ToString(), Is.EqualTo ("[Column1], [Column2], [Column3]"));
    }

    [Test]
    public void Union ()
    {
      var column4 = ColumnDefinitionObjectMother.CreateColumn("Column4");
      var column5 = ColumnDefinitionObjectMother.CreateColumn("Column5");

      var result = (SelectedColumnsSpecification) _specification.Union (new[] { column4, column5 });

      Assert.That (result.SelectedColumns, Is.EqualTo (new[] { _column1, _column2, _column3, column4, column5 }));
    }

    [Test]
    public void Union_DuplicatedColumns()
    {
      var column4 = ColumnDefinitionObjectMother.CreateColumn("Column4");
      
      var result = (SelectedColumnsSpecification) _specification.Union (new[] { column4, column4 });

      Assert.That (result.SelectedColumns, Is.EqualTo (new[] { _column1, _column2, _column3, column4 }));
    }
  }
}