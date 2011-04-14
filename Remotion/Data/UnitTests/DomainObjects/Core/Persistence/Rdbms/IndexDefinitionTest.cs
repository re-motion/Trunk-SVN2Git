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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class IndexDefinitionTest
  {
    private SimpleColumnDefinition[] _includedColumns;
    private SimpleColumnDefinition[] _columns;
    private EntityNameDefinition _objectName;
    private IndexDefinition _indexDefinition;

    [SetUp]
    public void SetUp ()
    {
      _objectName = new EntityNameDefinition ("objectSchema", "objectName");
      _columns = new[] { new SimpleColumnDefinition ("TestColumn1", typeof (string), "varchar", true, false) };
      _includedColumns = new[] { new SimpleColumnDefinition ("TestColumn2", typeof (string), "varchar", true, false) };

      _indexDefinition = new IndexDefinition ("IndexName", _objectName, _columns, _includedColumns, true, true, true, true);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_indexDefinition.IndexName, Is.EqualTo("IndexName"));
      Assert.That (_indexDefinition.ObjectName, Is.SameAs (_objectName));
      Assert.That (_indexDefinition.Columns, Is.EqualTo (_columns));
      Assert.That (_indexDefinition.IncludedColumns, Is.EqualTo (_includedColumns));
      Assert.That (_indexDefinition.IsClustered, Is.True);
      Assert.That (_indexDefinition.IsUnique, Is.True);
      Assert.That (_indexDefinition.IgnoreDupKey, Is.True);
      Assert.That (_indexDefinition.IsUnique, Is.True);
    }

    [Test]
    public void Initialization_NoIncludedColumns ()
    {
      _indexDefinition = new IndexDefinition ("IndexName", _objectName, _columns, null, true, true, true, true);

      Assert.That (_indexDefinition.IncludedColumns, Is.Null);
    }

    [Test]
    public void Accept_IndexDefinitionVisitor ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IIndexDefinitionVisitor>();
      visitorMock.Replay();

      _indexDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }

    [Test]
    public void Accept_SqlIndexDefinitionVisitor ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ISqlIndexDefinitionVisitor> ();
      visitorMock.Expect (mock => mock.VisitIndexDefinition (_indexDefinition));
      visitorMock.Replay ();

      _indexDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }
  }
}