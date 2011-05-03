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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlIndexScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private ISqlIndexDefinitionScriptElementFactory<SqlIndexDefinition> _indexDefinitionElementFactoryStub;
    private ISqlIndexDefinitionScriptElementFactory<SqlPrimaryXmlIndexDefinition> _primaryIndexDefinitionElementFactoryStub;
    private ISqlIndexDefinitionScriptElementFactory<SqlSecondaryXmlIndexDefinition> _secondaryIndexDefinitionElementFactoryStub;
    private SqlIndexDefinition _indexDefinition;
    private SqlPrimaryXmlIndexDefinition _primaryIndexDefinition;
    private SqlSecondaryXmlIndexDefinition _secondaryIndexDefinition;
    private SqlIndexScriptElementFactory _factory;
    private IScriptElement _fakeScriptElement;

    public override void SetUp ()
    {
      base.SetUp();

      _indexDefinitionElementFactoryStub = MockRepository.GenerateStub<ISqlIndexDefinitionScriptElementFactory<SqlIndexDefinition>>();
      _primaryIndexDefinitionElementFactoryStub = MockRepository.GenerateStub<ISqlIndexDefinitionScriptElementFactory<SqlPrimaryXmlIndexDefinition>> ();
      _secondaryIndexDefinitionElementFactoryStub =
          MockRepository.GenerateStub<ISqlIndexDefinitionScriptElementFactory<SqlSecondaryXmlIndexDefinition>>();

      _factory = new SqlIndexScriptElementFactory (
          _indexDefinitionElementFactoryStub, _primaryIndexDefinitionElementFactoryStub, _secondaryIndexDefinitionElementFactoryStub);

      var simpleColumn = new SimpleColumnDefinition ("Column", typeof (int), "integer", false, false);
      var indexedColumn = new SqlIndexedColumnDefinition (simpleColumn, IndexOrder.Desc);

      _indexDefinition = new SqlIndexDefinition ("Index1", new EntityNameDefinition (null, "Table"), new[] { indexedColumn });
      _primaryIndexDefinition = new SqlPrimaryXmlIndexDefinition ("Index2", new EntityNameDefinition (null, "Table"), simpleColumn);
      _secondaryIndexDefinition = new SqlSecondaryXmlIndexDefinition (
          "Index3", new EntityNameDefinition (null, "Table"), simpleColumn, "PrimaryIndexName", SqlSecondaryXmlIndexKind.Property);

      _fakeScriptElement = MockRepository.GenerateStub<IScriptElement>();
    }

    [Test]
    public void GetCreateElement_IndexDefinition ()
    {
      _indexDefinitionElementFactoryStub
          .Expect (stub => stub.GetCreateElement (_indexDefinition))
          .Return (_fakeScriptElement);
      
      var result = _factory.GetCreateElement (_indexDefinition);

      Assert.That (result, Is.SameAs (_fakeScriptElement));
    }

    [Test]
    public void GetCreateElement_PrimaryIndexDefinition ()
    {
      _primaryIndexDefinitionElementFactoryStub
          .Expect (stub => stub.GetCreateElement (_primaryIndexDefinition))
          .Return (_fakeScriptElement);

      var result = _factory.GetCreateElement (_primaryIndexDefinition);

      Assert.That (result, Is.SameAs (_fakeScriptElement));
    }

    [Test]
    public void GetCreateElement_SecondaryIndexDefinition ()
    {
      _secondaryIndexDefinitionElementFactoryStub
          .Expect (stub => stub.GetCreateElement (_secondaryIndexDefinition))
          .Return (_fakeScriptElement);

      var result = _factory.GetCreateElement (_secondaryIndexDefinition);

      Assert.That (result, Is.SameAs (_fakeScriptElement));
    }

    [Test]
    public void GetDropElement_IndexDefinition ()
    {
      _indexDefinitionElementFactoryStub
          .Expect (stub => stub.GetDropElement (_indexDefinition))
          .Return (_fakeScriptElement);

      var result = _factory.GetDropElement (_indexDefinition);

      Assert.That (result, Is.SameAs (_fakeScriptElement));
    }

    [Test]
    public void GetDropElement_PrimaryIndexDefinition ()
    {
      _primaryIndexDefinitionElementFactoryStub
          .Expect (stub => stub.GetDropElement (_primaryIndexDefinition))
          .Return (_fakeScriptElement);

      var result = _factory.GetDropElement (_primaryIndexDefinition);

      Assert.That (result, Is.SameAs (_fakeScriptElement));
    }

    [Test]
    public void GetDropElement_SecondaryIndexDefinition ()
    {
      _secondaryIndexDefinitionElementFactoryStub
          .Expect (stub => stub.GetDropElement (_secondaryIndexDefinition))
          .Return (_fakeScriptElement);

      var result = _factory.GetDropElement (_secondaryIndexDefinition);

      Assert.That (result, Is.SameAs (_fakeScriptElement));
    }
  }
}