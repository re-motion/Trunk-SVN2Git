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

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class ViewScriptBuilderBaseTest : SchemaGenerationTestBase
  {
    private ISqlDialect _sqlDialectStub;
    private TestableViewBuilder _viewBuilder;
    private TableDefinition _tableDefinition;
    private UnionViewDefinition _unionViewDefinition;
    private FilterViewDefinition _filterViewDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _viewBuilder = new TestableViewBuilder (_sqlDialectStub);

      _tableDefinition = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Order"),
          new EntityNameDefinition (null, "OrderView"),
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);
      _unionViewDefinition = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "OrderView"),
          new[] { _tableDefinition },
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);
      _filterViewDefinition = new FilterViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "OrderView"),
          _tableDefinition,
          new[] { "ClassID" },
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);
    }

    [Test]
    public void AddEntityDefinition_TableDefinition_Once ()
    {
      _viewBuilder.AddEntityDefinition (_tableDefinition);

      var createViewScript = _viewBuilder.GetCreateScript();
      var dropViewScript = _viewBuilder.GetDropScript();

      Assert.That (createViewScript, Is.EqualTo ("-- Create a view for every class\r\nCREATE VIEW [OrderView]"));
      Assert.That (dropViewScript, Is.EqualTo ("-- Drop all views that will be created below\r\nDROP VIEW [OrderView]"));
    }

    [Test]
    public void AddEntityDefinition_TableDefinition_Twice ()
    {
      _viewBuilder.AddEntityDefinition (_tableDefinition);
      _viewBuilder.AddEntityDefinition (_tableDefinition);

      var createViewScript = _viewBuilder.GetCreateScript();
      var dropViewScript = _viewBuilder.GetDropScript();

      Assert.That (createViewScript, Is.EqualTo ("-- Create a view for every class\r\nCREATE VIEW [OrderView]\r\nCREATE VIEW [OrderView]"));
      Assert.That (dropViewScript, Is.EqualTo ("-- Drop all views that will be created below\r\nDROP VIEW [OrderView]\r\nDROP VIEW [OrderView]"));
    }

    [Test]
    public void AddEntityDefinition_UnionViewDefinition_Once ()
    {
      _viewBuilder.AddEntityDefinition (_unionViewDefinition);

      var createViewScript = _viewBuilder.GetCreateScript();
      var dropViewScript = _viewBuilder.GetDropScript();

      Assert.That (createViewScript, Is.EqualTo ("-- Create a view for every class\r\nCREATE VIEW [OrderView]"));
      Assert.That (dropViewScript, Is.EqualTo ("-- Drop all views that will be created below\r\nDROP VIEW [OrderView]"));
    }

    [Test]
    public void AddEntityDefinition_UnionViewDefinition_Twice ()
    {
      _viewBuilder.AddEntityDefinition (_unionViewDefinition);
      _viewBuilder.AddEntityDefinition (_unionViewDefinition);

      var createViewScript = _viewBuilder.GetCreateScript();
      var dropViewScript = _viewBuilder.GetDropScript();

      Assert.That (createViewScript, Is.EqualTo ("-- Create a view for every class\r\nCREATE VIEW [OrderView]\r\nCREATE VIEW [OrderView]"));
      Assert.That (dropViewScript, Is.EqualTo ("-- Drop all views that will be created below\r\nDROP VIEW [OrderView]\r\nDROP VIEW [OrderView]"));
    }

    [Test]
    public void AddEntityDefinition_FilterViewDefinition_Once ()
    {
      _viewBuilder.AddEntityDefinition (_filterViewDefinition);

      var createViewScript = _viewBuilder.GetCreateScript();
      var dropViewScript = _viewBuilder.GetDropScript();

      Assert.That (createViewScript, Is.EqualTo ("-- Create a view for every class\r\nCREATE VIEW [OrderView]"));
      Assert.That (dropViewScript, Is.EqualTo ("-- Drop all views that will be created below\r\nDROP VIEW [OrderView]"));
    }

    [Test]
    public void AddEntityDefinition_FilterViewDefinition_Twice ()
    {
      _viewBuilder.AddEntityDefinition (_filterViewDefinition);
      _viewBuilder.AddEntityDefinition (_filterViewDefinition);

      var createViewScript = _viewBuilder.GetCreateScript();
      var dropViewScript = _viewBuilder.GetDropScript();

      Assert.That (createViewScript, Is.EqualTo ("-- Create a view for every class\r\nCREATE VIEW [OrderView]\r\nCREATE VIEW [OrderView]"));
      Assert.That (dropViewScript, Is.EqualTo ("-- Drop all views that will be created below\r\nDROP VIEW [OrderView]\r\nDROP VIEW [OrderView]"));
    }

    [Test]
    public void AddEntityDefinition_NullEntityDefintion ()
    {
      var nullEntityDefinition = new NullEntityDefinition (SchemaGenerationFirstStorageProviderDefinition);

      _viewBuilder.AddEntityDefinition (nullEntityDefinition);

      var createViewScript = _viewBuilder.GetCreateScript();
      var dropViewScript = _viewBuilder.GetDropScript();

      Assert.That (createViewScript, Is.EqualTo ("-- Create a view for every class\r\n"));
      Assert.That (dropViewScript, Is.EqualTo ("-- Drop all views that will be created below\r\n"));
    }
  }
}