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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class TableScriptBuilderTest : SchemaGenerationTestBase
  {
    private IScriptElementFactory<TableDefinition> _elementFactory;
    private TableScriptBuilder _builder;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private TableDefinition _tableDefinition3;
    private IScriptElement _fakeElement1;
    private IScriptElement _fakeElement2;
    private IScriptElement _fakeElement3;

    public override void SetUp ()
    {
      base.SetUp();

      _elementFactory = MockRepository.GenerateStrictMock<IScriptElementFactory<TableDefinition>>();
      _builder = new TableScriptBuilder (_elementFactory);

      _tableDefinition1 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Table1"),
          null,
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      _tableDefinition2 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Table2"),
          null,
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      _tableDefinition3 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Table3"),
          null,
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      _fakeElement1 = MockRepository.GenerateStub<IScriptElement>();
      _fakeElement2 = MockRepository.GenerateStub<IScriptElement>();
      _fakeElement3 = MockRepository.GenerateStub<IScriptElement>();
    }

    [Test]
    public void GetCreateScript_GetDropScript_NoEntitiesAdded ()
    {
      _elementFactory.Replay();

      Assert.That (_builder.GetCreateScript().Elements, Is.Empty);
      Assert.That (_builder.GetDropScript().Elements, Is.Empty);
      _elementFactory.VerifyAllExpectations();
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneTableDefinitionAdded ()
    {
      _elementFactory.Expect (mock => mock.GetCreateElement (_tableDefinition1)).Return (_fakeElement1);
      _elementFactory.Expect (mock => mock.GetDropElement (_tableDefinition1)).Return (_fakeElement2);
      _elementFactory.Replay();

      _builder.AddEntityDefinition (_tableDefinition1);

      Assert.That (_builder.GetCreateScript().Elements, Is.EqualTo (new[] { _fakeElement1 }));
      Assert.That (_builder.GetDropScript().Elements, Is.EqualTo (new[] { _fakeElement2 }));
      _elementFactory.VerifyAllExpectations();
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralTableDefinitionsAdded ()
    {
      _elementFactory.Expect (mock => mock.GetCreateElement (_tableDefinition1)).Return (_fakeElement1);
      _elementFactory.Expect (mock => mock.GetDropElement (_tableDefinition1)).Return (_fakeElement3);
      _elementFactory.Expect (mock => mock.GetCreateElement (_tableDefinition2)).Return (_fakeElement2);
      _elementFactory.Expect (mock => mock.GetDropElement (_tableDefinition2)).Return (_fakeElement2);
      _elementFactory.Expect (mock => mock.GetCreateElement (_tableDefinition3)).Return (_fakeElement3);
      _elementFactory.Expect (mock => mock.GetDropElement (_tableDefinition3)).Return (_fakeElement1);
      _elementFactory.Replay();

      _builder.AddEntityDefinition (_tableDefinition1);
      _builder.AddEntityDefinition (_tableDefinition2);
      _builder.AddEntityDefinition (_tableDefinition3);

      Assert.That (_builder.GetCreateScript().Elements, Is.EqualTo (new[] { _fakeElement1, _fakeElement2, _fakeElement3 }));
      Assert.That (_builder.GetDropScript().Elements, Is.EqualTo (new[] { _fakeElement3, _fakeElement2, _fakeElement1 }));
      _elementFactory.VerifyAllExpectations();
    }

    [Test]
    public void GetCreateScript_GetDropScript_FilterViewDefinitionAdded ()
    {
      _elementFactory.Replay ();

      var entityDefinition = new FilterViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "FilterView"),
          _tableDefinition1,
          new[] { "ClassID" },
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      _builder.AddEntityDefinition (entityDefinition);
      
      Assert.That (_builder.GetCreateScript().Elements, Is.Empty);
      Assert.That (_builder.GetDropScript().Elements, Is.Empty);
      _elementFactory.VerifyAllExpectations();
    }

    [Test]
    public void GetCreateScript_GetDropScript_UnionViewDefinitionAdded ()
    {
      _elementFactory.Replay ();

      var entityDefinition = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "UnionView"),
          new[] { _tableDefinition1 },
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      _builder.AddEntityDefinition (entityDefinition);

      Assert.That (_builder.GetCreateScript().Elements, Is.Empty);
      Assert.That (_builder.GetDropScript().Elements, Is.Empty);
      _elementFactory.VerifyAllExpectations();
    }

    [Test]
    public void GetCreateScript_GetDropScript_NullEntityDefinitionAdded ()
    {
      _elementFactory.Replay ();

      var entityDefinition = new NullEntityDefinition (SchemaGenerationFirstStorageProviderDefinition);
      _builder.AddEntityDefinition (entityDefinition);

      Assert.That (_builder.GetCreateScript ().Elements, Is.Empty);
      Assert.That (_builder.GetDropScript ().Elements, Is.Empty);
      _elementFactory.VerifyAllExpectations ();
    }
  }
}