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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class SelectedColumnsSpecificationTest
  {
    private SimpleColumnDefinition _column1;
    private SimpleColumnDefinition _column2;
    private SimpleColumnDefinition _column3;
    private SelectedColumnsSpecification _specification;
    private ISqlDialect _sqlDialectStub;

    [SetUp]
    public void SetUp ()
    {
      _column1 = new SimpleColumnDefinition ("Column1", typeof (string), "varchar", true, false);
      _column2 = new SimpleColumnDefinition ("Column2", typeof (string), "varchar", true, false);
      _column3 = new SimpleColumnDefinition ("Column3", typeof (string), "varchar", true, false);
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

      Assert.That (sb.ToString(), Is.EqualTo (" [Column1], [Column2], [Column3]"));
    }

    [Test]
    public void AppendProjection_StringBuilderIsNotEmpty ()
    {
      var sb = new StringBuilder ("OtherString");

      _specification.AppendProjection (sb, _sqlDialectStub);

      Assert.That (sb.ToString (), Is.EqualTo ("OtherString [Column1], [Column2], [Column3]"));
    }

    [Test]
    public void Union ()
    {
      var column4 = new SimpleColumnDefinition ("Column4", typeof (string), "varchar", true, false);
      var column5 = new SimpleColumnDefinition ("Column5", typeof (string), "varchar", true, false);

      var result = (SelectedColumnsSpecification) _specification.Union (new[] { column4, column5 });

      Assert.That (result.SelectedColumns, Is.EqualTo (new[] { _column1, _column2, _column3, column4, column5 }));
    }
  }
}