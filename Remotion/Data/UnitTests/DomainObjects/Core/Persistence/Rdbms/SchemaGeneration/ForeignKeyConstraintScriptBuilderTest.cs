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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class ForeignKeyConstraintScriptBuilderTest : SchemaGenerationTestBase
  {
    private IScriptElementFactory<Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition>> _factoryStub;
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

      _factoryStub =
          MockRepository.GenerateStub<IScriptElementFactory<Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition>>>();

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
      Assert.That (_builder.GetCreateScript().Elements, Is.Empty);
      Assert.That (_builder.GetDropScript().Elements, Is.Empty);
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneTableDefinitionAdded ()
    {
      _factoryStub
        .Stub (mock => mock.GetCreateElement (new Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition> (_constraint1, _tableName)))
        .Return (_fakeElement1);
      _factoryStub
        .Stub (mock => mock.GetDropElement (new Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition> (_constraint1, _tableName)))
        .Return (_fakeElement2);

      _builder.AddEntityDefinition (_tableDefinition1);

      Assert.That (_builder.GetCreateScript().Elements, Is.EqualTo (new[] { _fakeElement1 }));
      Assert.That (_builder.GetDropScript().Elements, Is.EqualTo (new[] { _fakeElement2 }));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralTableDefinitionsAdded ()
    {
      _factoryStub
        .Stub (mock => mock.GetCreateElement (new Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition> (_constraint1, _tableName)))
        .Return (_fakeElement1);
      _factoryStub
        .Stub (mock => mock.GetDropElement (new Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition> (_constraint1, _tableName)))
        .Return (_fakeElement3);
      _factoryStub
        .Stub (mock => mock.GetCreateElement (new Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition> (_constraint2, _tableName)))
        .Return (_fakeElement2);
      _factoryStub
        .Stub (mock => mock.GetDropElement (new Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition> (_constraint2, _tableName)))
        .Return (_fakeElement2);
      _factoryStub
        .Stub (mock => mock.GetCreateElement (new Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition> (_constraint3, _tableName)))
        .Return (_fakeElement3);
      _factoryStub
        .Stub (mock => mock.GetDropElement (new Tuple<ForeignKeyConstraintDefinition, EntityNameDefinition> (_constraint3, _tableName)))
        .Return (_fakeElement1);

      _builder.AddEntityDefinition (_tableDefinition1);
      _builder.AddEntityDefinition (_tableDefinition2);

      Assert.That (_builder.GetCreateScript ().Elements, Is.EqualTo (new[] { _fakeElement1, _fakeElement2, _fakeElement3 }));
      Assert.That (_builder.GetDropScript ().Elements, Is.EqualTo (new[] { _fakeElement3, _fakeElement2, _fakeElement1 }));
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

      Assert.That (_builder.GetCreateScript().Elements, Is.Empty);
      Assert.That (_builder.GetDropScript().Elements, Is.Empty);
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

      Assert.That (_builder.GetCreateScript().Elements, Is.Empty);
      Assert.That (_builder.GetDropScript().Elements, Is.Empty);
    }

    [Test]
    public void GetCreateScript_GetDropScript_NullEntityDefinitionAdded ()
    {
      var entityDefinition = new NullEntityDefinition (SchemaGenerationFirstStorageProviderDefinition);
      _builder.AddEntityDefinition (entityDefinition);

      Assert.That (_builder.GetCreateScript().Elements, Is.Empty);
      Assert.That (_builder.GetDropScript().Elements, Is.Empty);
    }
  }
}