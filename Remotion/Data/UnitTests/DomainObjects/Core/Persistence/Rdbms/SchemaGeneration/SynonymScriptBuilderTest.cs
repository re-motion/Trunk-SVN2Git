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
  public class SynonymScriptBuilderTest : SchemaGenerationTestBase
  {
    private SynonymScriptBuilder _builder;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private UnionViewDefinition _unionViewDefinition1;
    private UnionViewDefinition _unionViewDefinition2;
    private FilterViewDefinition _filterViewDefinition1;
    private FilterViewDefinition _filterViewDefinition2;
    private IScriptElement _fakeElement1;
    private IScriptElement _fakeElement2;
    private IScriptElement _fakeElement3;
    private ISynonymScriptElementFactory<TableDefinition> _tableViewElementFactoryStub;
    private ISynonymScriptElementFactory<UnionViewDefinition> _unionViewElementFactoryStub;
    private ISynonymScriptElementFactory<FilterViewDefinition> _filterViewElementFactoryStub;
    private EntityNameDefinition _synonym1;
    private EntityNameDefinition _synonym2;
    private EntityNameDefinition _synonym3;

    public override void SetUp ()
    {
      base.SetUp();

      _tableViewElementFactoryStub = MockRepository.GenerateStub<ISynonymScriptElementFactory<TableDefinition>>();
      _unionViewElementFactoryStub = MockRepository.GenerateStub<ISynonymScriptElementFactory<UnionViewDefinition>>();
      _filterViewElementFactoryStub = MockRepository.GenerateStub<ISynonymScriptElementFactory<FilterViewDefinition>>();

      _builder = new SynonymScriptBuilder (_tableViewElementFactoryStub, _unionViewElementFactoryStub, _filterViewElementFactoryStub);

      _synonym1 = new EntityNameDefinition (null, "Synonym1");
      _synonym2 = new EntityNameDefinition (null, "Synonym2");
      _synonym3 = new EntityNameDefinition (null, "Synonym3");

      _tableDefinition1 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Table1"),
          new EntityNameDefinition (null, "TableView1"),
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new[] { _synonym1 });
      _tableDefinition2 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Table2"),
          new EntityNameDefinition (null, "TableView2"),
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new[] { _synonym2, _synonym3 });
      _unionViewDefinition1 = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "UnionView1"),
          new[] { _tableDefinition1 },
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new[] { _synonym1 });
      _unionViewDefinition2 = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "UnionView2"),
          new[] { _tableDefinition2 },
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new[] { _synonym2, _synonym3 });
      _filterViewDefinition1 = new FilterViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "FilterView1"),
          _tableDefinition1,
          new[] { "ClassID" },
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new[] { _synonym1 });
      _filterViewDefinition2 = new FilterViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "FilterView2"),
          _tableDefinition2,
          new[] { "ClassID" },
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new[] { _synonym2, _synonym3 });

      _fakeElement1 = MockRepository.GenerateStub<IScriptElement>();
      _fakeElement2 = MockRepository.GenerateStub<IScriptElement>();
      _fakeElement3 = MockRepository.GenerateStub<IScriptElement>();
    }

    [Test]
    public void GetCreateScript_GetDropScript_NoEntitiesAdded ()
    {
      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create synonyms for tables that were created above"));
      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all synonyms"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneTableDefinitionAdded ()
    {
      _tableViewElementFactoryStub
          .Stub (stub => stub.GetCreateElement (_tableDefinition1, _synonym1)).Return (_fakeElement1);
      _tableViewElementFactoryStub
          .Stub (stub => stub.GetDropElement (_tableDefinition1, _synonym1)).Return (_fakeElement2);

      _builder.AddEntityDefinition (_tableDefinition1);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create synonyms for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all synonyms"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralTableDefinitionsAdded ()
    {
      _tableViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_tableDefinition1, _synonym1)).Return (_fakeElement1);
      _tableViewElementFactoryStub.Stub (stub => stub.GetDropElement (_tableDefinition1, _synonym1)).Return (_fakeElement3);
      _tableViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_tableDefinition2, _synonym2)).Return (_fakeElement2);
      _tableViewElementFactoryStub.Stub (stub => stub.GetDropElement (_tableDefinition2, _synonym2)).Return (_fakeElement2);
      _tableViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_tableDefinition2, _synonym3)).Return (_fakeElement3);
      _tableViewElementFactoryStub.Stub (stub => stub.GetDropElement (_tableDefinition2, _synonym3)).Return (_fakeElement1);

      _builder.AddEntityDefinition (_tableDefinition1);
      _builder.AddEntityDefinition (_tableDefinition2);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create synonyms for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (createScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (createScriptResult.Elements[3], Is.SameAs (_fakeElement3));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all synonyms"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement3));
      Assert.That (dropScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (dropScriptResult.Elements[3], Is.SameAs (_fakeElement1));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneUnionViewDefinitionAdded ()
    {
      _unionViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_unionViewDefinition1, _synonym1)).Return (_fakeElement1);
      _unionViewElementFactoryStub.Stub (stub => stub.GetDropElement (_unionViewDefinition1, _synonym1)).Return (_fakeElement2);

      _builder.AddEntityDefinition (_unionViewDefinition1);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create synonyms for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all synonyms"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralUnionViewDefinitionsAdded ()
    {
      _unionViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_unionViewDefinition1, _synonym1)).Return (_fakeElement1);
      _unionViewElementFactoryStub.Stub (stub => stub.GetDropElement (_unionViewDefinition1, _synonym1)).Return (_fakeElement3);
      _unionViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_unionViewDefinition2, _synonym2)).Return (_fakeElement2);
      _unionViewElementFactoryStub.Stub (stub => stub.GetDropElement (_unionViewDefinition2, _synonym2)).Return (_fakeElement2);
      _unionViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_unionViewDefinition2, _synonym3)).Return (_fakeElement3);
      _unionViewElementFactoryStub.Stub (stub => stub.GetDropElement (_unionViewDefinition2, _synonym3)).Return (_fakeElement1);

      _builder.AddEntityDefinition (_unionViewDefinition1);
      _builder.AddEntityDefinition (_unionViewDefinition2);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create synonyms for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (createScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (createScriptResult.Elements[3], Is.SameAs (_fakeElement3));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all synonyms"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement3));
      Assert.That (dropScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (dropScriptResult.Elements[3], Is.SameAs (_fakeElement1));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneFilterViewDefinitionAdded ()
    {
      _filterViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_filterViewDefinition1, _synonym1)).Return (_fakeElement1);
      _filterViewElementFactoryStub.Stub (stub => stub.GetDropElement (_filterViewDefinition1, _synonym1)).Return (_fakeElement2);

      _builder.AddEntityDefinition (_filterViewDefinition1);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create synonyms for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all synonyms"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralFilterViewDefinitionsAdded ()
    {
      _filterViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_filterViewDefinition1, _synonym1)).Return (_fakeElement1);
      _filterViewElementFactoryStub.Stub (stub => stub.GetDropElement (_filterViewDefinition1, _synonym1)).Return (_fakeElement3);
      _filterViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_filterViewDefinition2, _synonym2)).Return (_fakeElement2);
      _filterViewElementFactoryStub.Stub (stub => stub.GetDropElement (_filterViewDefinition2, _synonym2)).Return (_fakeElement2);
      _filterViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_filterViewDefinition2, _synonym3)).Return (_fakeElement3);
      _filterViewElementFactoryStub.Stub (stub => stub.GetDropElement (_filterViewDefinition2, _synonym3)).Return (_fakeElement1);

      _builder.AddEntityDefinition (_filterViewDefinition1);
      _builder.AddEntityDefinition (_filterViewDefinition2);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create synonyms for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (createScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (createScriptResult.Elements[3], Is.SameAs (_fakeElement3));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all synonyms"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement3));
      Assert.That (dropScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (dropScriptResult.Elements[3], Is.SameAs (_fakeElement1));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralEntityDefinitionsAdded ()
    {
      _tableViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_tableDefinition1, _synonym1)).Return (_fakeElement1);
      _tableViewElementFactoryStub.Stub (stub => stub.GetDropElement (_tableDefinition1, _synonym1)).Return (_fakeElement3);
      _unionViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_unionViewDefinition1, _synonym1)).Return (_fakeElement2);
      _unionViewElementFactoryStub.Stub (stub => stub.GetDropElement (_unionViewDefinition1, _synonym1)).Return (_fakeElement2);
      _filterViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_filterViewDefinition1, _synonym1)).Return (_fakeElement3);
      _filterViewElementFactoryStub.Stub (stub => stub.GetDropElement (_filterViewDefinition1, _synonym1)).Return (_fakeElement1);

      _builder.AddEntityDefinition (_tableDefinition1);
      _builder.AddEntityDefinition (_unionViewDefinition1);
      _builder.AddEntityDefinition (_filterViewDefinition1);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create synonyms for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (createScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (createScriptResult.Elements[3], Is.SameAs (_fakeElement3));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all synonyms"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement3));
      Assert.That (dropScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (dropScriptResult.Elements[3], Is.SameAs (_fakeElement1));
    }

    [Test]
    public void GetCreateScript_GetDropScript_NullEntityDefinitionAdded ()
    {
      var entityDefinition = new NullEntityDefinition (SchemaGenerationFirstStorageProviderDefinition);
      _builder.AddEntityDefinition (entityDefinition);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create synonyms for tables that were created above"));
      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all synonyms"));
    }
  }
}