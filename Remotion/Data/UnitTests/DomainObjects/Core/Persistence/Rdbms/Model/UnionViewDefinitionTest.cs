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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class UnionViewDefinitionTest
  {
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private UnionViewDefinition _unionViewDefinition;
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;
    private ColumnDefinition _column3;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID", typeof (UnitTestStorageObjectFactoryStub));
      _column1 = new ColumnDefinition ("Column1", typeof(string), "varchar", true);
      _column2 = new ColumnDefinition ("Column2", typeof(string), "varchar", true);
      _column3 = new ColumnDefinition ("Column3", typeof(string), "varchar", true);

      _tableDefinition1 = new TableDefinition (_storageProviderDefinition, "Table1", "View1", new[] { _column1 });
      _tableDefinition2 = new TableDefinition (_storageProviderDefinition, "Table2", "View2", new[] { _column2, _column3 });
      _unionViewDefinition = new UnionViewDefinition (_storageProviderDefinition, "Test", new[] {_tableDefinition1, _tableDefinition2 } );
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_unionViewDefinition.ViewName, Is.EqualTo ("Test"));
      Assert.That (_unionViewDefinition.UnionedEntities, Is.EqualTo (new[] { _tableDefinition1, _tableDefinition2 }));
      Assert.That (_unionViewDefinition.StorageProviderID, Is.EqualTo ("SPID"));
      Assert.That (_unionViewDefinition.StorageProviderDefinition, Is.SameAs(_storageProviderDefinition));
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var unionViewDefinition = new UnionViewDefinition (_storageProviderDefinition, null, new[] { _tableDefinition1, _tableDefinition2 });
      Assert.That (unionViewDefinition.ViewName, Is.Null);
    }

    [Test]
    public void LegacyEntityName ()
    {
      Assert.That (_unionViewDefinition.LegacyEntityName, Is.Null);
    }

    [Test]
    public void LegacyViewName ()
    {
      Assert.That (_unionViewDefinition.LegacyViewName, Is.EqualTo("Test"));
    }

    [Test]
    public void GetColumns ()
    {
      var result = _unionViewDefinition.GetColumns();

      Assert.That (result, Is.EqualTo (new[] { _column1, _column2, _column3 }));
    }

    [Test]
    public void GetColumns_Distinct ()
    {
      var tableDefinition1 = new TableDefinition (_storageProviderDefinition, "Table1", "View1", new[] { _column1, _column2 });
      var tableDefinition2 = new TableDefinition (_storageProviderDefinition, "Table2", "View2", new[] { _column2, _column3 });
      _unionViewDefinition = new UnionViewDefinition (_storageProviderDefinition, "Test", new[] { tableDefinition1, tableDefinition2 });

      var result = _unionViewDefinition.GetColumns ();

      Assert.That (result, Is.EqualTo (new[] { _column1, _column2, _column3 }));
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IEntityDefinitionVisitor> ();

      var fakeResult = new TableDefinition (_storageProviderDefinition, "Test", "TestView", Enumerable.Empty<ColumnDefinition> ());

      visitorMock.Expect (mock => mock.VisitUnionViewDefinition (_unionViewDefinition)).Return (fakeResult);
      visitorMock.Replay ();

      _unionViewDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }

  }
}