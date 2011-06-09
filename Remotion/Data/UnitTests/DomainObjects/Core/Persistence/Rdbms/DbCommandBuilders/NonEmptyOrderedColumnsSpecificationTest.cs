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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class NonEmptyOrderedColumnsSpecificationTest
  {
    private SimpleColumnDefinition _column1;
    private SimpleColumnDefinition _column2;
    private SimpleColumnDefinition _column3;
    private NonEmptyOrderedColumnsSpecification _specification;
    private ISqlDialect _sqlDialectStub;

    [SetUp]
    public void SetUp ()
    {
      _column1 = new SimpleColumnDefinition ("Column1", typeof (string), "varchar", true, false);
      _column2 = new SimpleColumnDefinition ("Column2", typeof (string), "varchar", true, false);
      _column3 = new SimpleColumnDefinition ("Column3", typeof (string), "varchar", true, false);
      _specification =
          new NonEmptyOrderedColumnsSpecification (
              new[]
              {
                  Tuple.Create (_column1, SortOrder.Ascending), Tuple.Create (_column2, SortOrder.Descending),
                  Tuple.Create (_column3, SortOrder.Ascending)
              });
      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Column1")).Return ("[Column1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Column2")).Return ("[Column2]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Column3")).Return ("[Column3]");
    }

    [Test]
    public void AppendOrderByClause_StringBuilderEmpty ()
    {
      var sb = new StringBuilder();

      _specification.AppendOrderByClause (sb, _sqlDialectStub);

      Assert.That (sb.ToString(), Is.EqualTo (" ORDER BY [Column1] ASC, [Column2] DESC, [Column3] ASC"));
    }

    [Test]
    public void AppendOrderByClause_StringBuilderNotEmpty ()
    {
      var sb = new StringBuilder ("Test");

      _specification.AppendOrderByClause (sb, _sqlDialectStub);

      Assert.That (sb.ToString (), Is.EqualTo ("Test ORDER BY [Column1] ASC, [Column2] DESC, [Column3] ASC"));
    }

    [Test]
    public void UnionWithSelectedColumns ()
    {
      var selectedColumns = MockRepository.GenerateStrictMock<ISelectedColumnsSpecification>();

      selectedColumns
        .Expect (mock => mock.Union (Arg<IEnumerable<SimpleColumnDefinition>>.List.Equal (new[] { _column1, _column2, _column3 })))
        .Return (selectedColumns);
      selectedColumns.Replay();

      var result = _specification.UnionWithSelectedColumns (selectedColumns);

      Assert.That (result, Is.SameAs (selectedColumns));
      selectedColumns.VerifyAllExpectations();
    }
  }
}