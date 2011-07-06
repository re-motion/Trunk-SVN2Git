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
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class InlineEntityDefinitionVisitorTest : StandardMappingTest
  {
    private TableDefinition _tableDefinition;
    private FilterViewDefinition _filterViewDefinition;
    private UnionViewDefinition _unionViewDefinition;
    private NullEntityDefinition _nullEntityDefinition;
    private object _fakeResult;

    public override void SetUp ()
    {
      base.SetUp();

      _tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));
      _filterViewDefinition = FilterViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "FilterView"), _tableDefinition);
      _unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "UnionView"), new[] { _tableDefinition });
      _nullEntityDefinition = new NullEntityDefinition (TestDomainStorageProviderDefinition);

      _fakeResult = new object();
    }

    [Test]
    public void Visit_NonGeneric_TableDefinition ()
    {
      var count = 0;
      InlineEntityDefinitionVisitor.Visit (
          _tableDefinition,
          (td, continuation) =>
          {
            count++;
            Assert.That (td.TableName.EntityName, Is.EqualTo ("Table"));
          },
          (td, continuation) => { throw new InvalidOperationException(); },
          (td, continuation) => { throw new InvalidOperationException(); },
          (td, continuation) => { throw new InvalidOperationException(); });

      Assert.That (count, Is.EqualTo (1));
    }

    [Test]
    public void Visit_NonGeneric_FilterViewDefinition ()
    {
      var count = 0;
      InlineEntityDefinitionVisitor.Visit (
          _filterViewDefinition,
          (td, continuation) => { throw new InvalidOperationException(); },
          (fv, continuation) =>
          {
            count++;
            Assert.That (fv.ViewName.EntityName, Is.EqualTo ("FilterView"));
          },
          (uv, continuation) => { throw new InvalidOperationException(); },
          (ne, continuation) => { throw new InvalidOperationException(); });

      Assert.That (count, Is.EqualTo (1));
    }

    [Test]
    public void Visit_NonGeneric_FilterViewDefinition_WithContinuation ()
    {
      var filterCount = 0;
      var tableCount = 0;

      InlineEntityDefinitionVisitor.Visit (
          _filterViewDefinition,
          (td, continuation) =>
          {
            tableCount++;
            Assert.That (td.TableName.EntityName, Is.EqualTo ("Table"));
          },
          (fv, continuation) =>
          {
            filterCount++;
            Assert.That (fv.ViewName.EntityName, Is.EqualTo ("FilterView"));
            continuation (fv.BaseEntity);
          },
          (uv, continuation) => { throw new InvalidOperationException(); },
          (ne, continuation) => { throw new InvalidOperationException(); });

      Assert.That (filterCount, Is.EqualTo (1));
      Assert.That (tableCount, Is.EqualTo (1));
    }

    [Test]
    public void Visit_NonGeneric_UnionViewDefinition ()
    {
      var count = 0;
      InlineEntityDefinitionVisitor.Visit (
          _unionViewDefinition,
          (td, continuation) => { throw new InvalidOperationException(); },
          (fv, continuation) => { throw new InvalidOperationException(); },
          (uv, continuation) =>
          {
            count++;
            Assert.That (uv.ViewName.EntityName, Is.EqualTo ("UnionView"));
          },
          (ne, continuation) => { throw new InvalidOperationException(); });

      Assert.That (count, Is.EqualTo (1));
    }

    [Test]
    public void Visit_NonGeneric_NullEntityDefinition ()
    {
      var count = 0;
      InlineEntityDefinitionVisitor.Visit (
          _nullEntityDefinition,
          (td, continuation) => { throw new InvalidOperationException(); },
          (fv, continuation) => { throw new InvalidOperationException(); },
          (uv, continuation) => { throw new InvalidOperationException(); },
          (ne, continuation) => { count++; });

      Assert.That (count, Is.EqualTo (1));
    }

    [Test]
    public void Visit_Generic_TableDefinition ()
    {
      var count = 0;
      var result = InlineEntityDefinitionVisitor.Visit<object> (
          _tableDefinition,
          (td, continuation) =>
          {
            count++;
            Assert.That (td.TableName.EntityName, Is.EqualTo ("Table"));
            return _fakeResult;
          },
          (td, continuation) => { throw new InvalidOperationException(); },
          (td, continuation) => { throw new InvalidOperationException(); },
          (td, continuation) => { throw new InvalidOperationException(); });

      Assert.That (count, Is.EqualTo (1));
      Assert.That (result, Is.SameAs (_fakeResult));
    }

    [Test]
    public void Visit_Generic_FilterViewDefinition ()
    {
      var count = 0;
      var result = InlineEntityDefinitionVisitor.Visit<object> (
          _filterViewDefinition,
          (td, continuation) => { throw new InvalidOperationException(); },
          (fv, continuation) =>
          {
            count++;
            Assert.That (fv.ViewName.EntityName, Is.EqualTo ("FilterView"));
            return _fakeResult;
          },
          (uv, continuation) => { throw new InvalidOperationException(); },
          (ne, continuation) => { throw new InvalidOperationException(); });

      Assert.That (count, Is.EqualTo (1));
      Assert.That (result, Is.SameAs (_fakeResult));
    }

    [Test]
    public void Visit_Generic_FilterViewDefinition_WithContinuation ()
    {
      var tableCount = 0;
      var filterCount = 0;
      var result = InlineEntityDefinitionVisitor.Visit<object> (
          _filterViewDefinition,
          (td, continuation) =>
          {
            tableCount++;
            Assert.That (td.TableName.EntityName, Is.EqualTo ("Table"));
            return _fakeResult;
          },
          (fv, continuation) =>
          {
            filterCount++;
            Assert.That (fv.ViewName.EntityName, Is.EqualTo ("FilterView"));
            return continuation(fv.BaseEntity);
          },
          (uv, continuation) => { throw new InvalidOperationException (); },
          (ne, continuation) => { throw new InvalidOperationException (); });

      Assert.That (filterCount, Is.EqualTo (1));
      Assert.That (tableCount, Is.EqualTo (1));
      Assert.That (result, Is.SameAs (_fakeResult));
    }

    [Test]
    public void Visit_Generic_UnionViewDefinition ()
    {
      var count = 0;
      var result = InlineEntityDefinitionVisitor.Visit<object> (
          _unionViewDefinition,
          (td, continuation) => { throw new InvalidOperationException(); },
          (fv, continuation) => { throw new InvalidOperationException(); },
          (uv, continuation) =>
          {
            count++;
            Assert.That (uv.ViewName.EntityName, Is.EqualTo ("UnionView"));
            return _fakeResult;
          },
          (ne, continuation) => { throw new InvalidOperationException(); });

      Assert.That (count, Is.EqualTo (1));
      Assert.That (result, Is.SameAs (_fakeResult));
    }

    [Test]
    public void Visit_Generic_NullEntityDefinition ()
    {
      var count = 0;
      var result = InlineEntityDefinitionVisitor.Visit<object> (
          _nullEntityDefinition,
          (td, continuation) => { throw new InvalidOperationException(); },
          (fv, continuation) => { throw new InvalidOperationException(); },
          (uv, continuation) => { throw new InvalidOperationException(); },
          (ne, continuation) =>
          {
            count++;
            return _fakeResult;
          });

      Assert.That (count, Is.EqualTo (1));
      Assert.That (result, Is.SameAs (_fakeResult));
    }
  }
}