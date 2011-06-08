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
  public class EmptyOrderedColumnsSpecificationTest
  {
    private EmptyOrderedColumnsSpecification _specification;

    [SetUp]
    public void SetUp ()
    {
      _specification = EmptyOrderedColumnsSpecification.Instance;
    }

    [Test]
    public void AppendOrderByClause_StringBuilderEmpty ()
    {
      var stringBuilder = new StringBuilder();

      _specification.AppendOrderByClause (stringBuilder, MockRepository.GenerateStub<ISqlDialect>());

      Assert.That (stringBuilder.ToString(), Is.Empty);
    }

    [Test]
    public void AppendOrderByClause_StringBuilderNotEmpty ()
    {
      var stringBuilder = new StringBuilder ("Test");

      _specification.AppendOrderByClause (stringBuilder, MockRepository.GenerateStub<ISqlDialect> ());

      Assert.That (stringBuilder.ToString (), Is.EqualTo("Test"));
    }

    [Test]
    public void UnionWithSelectedColumns ()
    {
      var selectedColumnsStub = MockRepository.GenerateStub<ISelectedColumnsSpecification>();

      var result = _specification.UnionWithSelectedColumns (selectedColumnsStub);

      Assert.That (result, Is.SameAs (selectedColumnsStub));
    }
  }
}