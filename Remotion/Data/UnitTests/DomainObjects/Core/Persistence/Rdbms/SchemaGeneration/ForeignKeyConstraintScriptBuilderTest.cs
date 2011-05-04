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
  public class ForeignKeyConstraintScriptBuilderTest : SchemaGenerationTestBase
  {
    private IForeignKeyConstraintScriptElementFactory _factoryStub;
    private ForeignKeyConstraintScriptBuilder _builder;
    private TableDefinition _tableDefinition1;
    private IScriptElement _fakeElement1;
    private IScriptElement _fakeElement2;
    private IScriptElement _fakeElement3;
    private EntityNameDefinition _tableName;
    private ForeignKeyConstraintDefinition _constraint1;
    private ForeignKeyConstraintDefinition _constraint2;
    private ForeignKeyConstraintDefinition _constraint3;
    private TableDefinition _tableDefinition2;

    public override void SetUp ()
    {
      base.SetUp();

      _factoryStub = MockRepository.GenerateStub<IForeignKeyConstraintScriptElementFactory>();

      _builder = new ForeignKeyConstraintScriptBuilder (_factoryStub);

      _tableName = new EntityNameDefinition (null, "Table");
      _constraint1 = new ForeignKeyConstraintDefinition ("FK1", _tableName, new SimpleColumnDefinition[0], new SimpleColumnDefinition[0]);
      _constraint2 = new ForeignKeyConstraintDefinition ("FK2", _tableName, new SimpleColumnDefinition[0], new SimpleColumnDefinition[0]);
      _constraint3 = new ForeignKeyConstraintDefinition ("FK3", _tableName, new SimpleColumnDefinition[0], new SimpleColumnDefinition[0]);

      _tableDefinition1 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          _tableName,
          null,
          new SimpleColumnDefinition[0],
          new[] { _constraint1 },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      _tableDefinition2 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          _tableName,
          null,
          new SimpleColumnDefinition[0],
          new[] { _constraint2, _constraint3 },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

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
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, 
        Is.EqualTo ("-- Create foreign key constraints for tables that were created above"));
      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop foreign keys of all tables"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneTableDefinitionAdded ()
    {
      _factoryStub.Stub (mock => mock.GetCreateElement (_constraint1, _tableName)).Return (_fakeElement1);
      _factoryStub.Stub (mock => mock.GetDropElement (_constraint1, _tableName)).Return (_fakeElement2);

      _builder.AddEntityDefinition (_tableDefinition1);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, 
        Is.EqualTo ("-- Create foreign key constraints for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop foreign keys of all tables"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralTableDefinitionsAdded ()
    {
      _factoryStub.Stub (mock => mock.GetCreateElement (_constraint1, _tableName)).Return (_fakeElement1);
      _factoryStub.Stub (mock => mock.GetDropElement (_constraint1, _tableName)).Return (_fakeElement3);
      _factoryStub.Stub (mock => mock.GetCreateElement (_constraint2, _tableName)).Return (_fakeElement2);
      _factoryStub.Stub (mock => mock.GetDropElement (_constraint2, _tableName)).Return (_fakeElement2);
      _factoryStub.Stub (mock => mock.GetCreateElement (_constraint3, _tableName)).Return (_fakeElement3);
      _factoryStub.Stub (mock => mock.GetDropElement (_constraint3, _tableName)).Return (_fakeElement1);

      _builder.AddEntityDefinition (_tableDefinition1);
      _builder.AddEntityDefinition (_tableDefinition2);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, 
        Is.EqualTo ("-- Create foreign key constraints for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (createScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (createScriptResult.Elements[3], Is.SameAs (_fakeElement3));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop foreign keys of all tables"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement3));
      Assert.That (dropScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (dropScriptResult.Elements[3], Is.SameAs (_fakeElement1));
    }

    [Test]
    public void GetCreateScript_GetDropScript_FilterViewDefinitionAdded ()
    {
      var entityDefinition = new FilterViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "FilterView"),
          _tableDefinition1,
          new[] { "ClassID" },
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      _builder.AddEntityDefinition (entityDefinition);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement,
        Is.EqualTo ("-- Create foreign key constraints for tables that were created above"));
      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop foreign keys of all tables"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_UnionViewDefinitionAdded ()
    {
      var entityDefinition = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "UnionView"),
          new[] { _tableDefinition1 },
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      _builder.AddEntityDefinition (entityDefinition);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement,
        Is.EqualTo ("-- Create foreign key constraints for tables that were created above"));
      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop foreign keys of all tables"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_NullEntityDefinitionAdded ()
    {
      var entityDefinition = new NullEntityDefinition (SchemaGenerationFirstStorageProviderDefinition);
      _builder.AddEntityDefinition (entityDefinition);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement,
        Is.EqualTo ("-- Create foreign key constraints for tables that were created above"));
      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop foreign keys of all tables"));
    }
  }
}