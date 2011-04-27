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

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class SynonymScriptBuilderBaseTest : SchemaGenerationTestBase
  {
    private TestableSynonymBuilder _synonymBuilder;
    private TableDefinition _tableDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _synonymBuilder = new TestableSynonymBuilder ();

      _tableDefinition = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Order"),
          new EntityNameDefinition(null, "OrderView"),
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0], 
          new[]{new EntityNameDefinition(null, "SynonymName")});
    }

    [Test]
    public void AddEntityDefinition_TableDefinition ()
    {
      _synonymBuilder.AddEntityDefinition (_tableDefinition);

      var createTableScript = _synonymBuilder.GetCreateScript ();
      var dropTableScript = _synonymBuilder.GetDropScript ();

      Assert.AreEqual (createTableScript, "-- Create synonyms for tables that were created above\r\nCREATE SYNONYM [SynonymName] FOR [Order]");
      Assert.AreEqual (dropTableScript, "-- Drop all synonyms that will be created below\r\nDROP SYNONYM [SynonymName]");
    }

    [Test]
    public void AddEntityDefinition_FilterViewDefinition ()
    {
      var filterViewDefinition = new FilterViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "OrderView"),
          _tableDefinition,
          new[] { "ClassID" },
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new[] { new EntityNameDefinition (null, "SynonymName") });

      _synonymBuilder.AddEntityDefinition (filterViewDefinition);

      var createTableScript = _synonymBuilder.GetCreateScript ();
      var dropTableScript = _synonymBuilder.GetDropScript ();

      Assert.AreEqual (createTableScript, "-- Create synonyms for tables that were created above\r\nCREATE SYNONYM [SynonymName] FOR [OrderView]");
      Assert.AreEqual (dropTableScript, "-- Drop all synonyms that will be created below\r\nDROP SYNONYM [SynonymName]");
    }

    [Test]
    public void AddEntityDefinition_UnionViewDefinition ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          SchemaGenerationInternalStorageProviderDefinition,
          new EntityNameDefinition (null, "OrderView"),
          new[] { _tableDefinition },
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new[] { new EntityNameDefinition (null, "SynonymName") });

      _synonymBuilder.AddEntityDefinition (unionViewDefinition);

      var createTableScript = _synonymBuilder.GetCreateScript ();
      var dropTableScript = _synonymBuilder.GetDropScript ();

      Assert.AreEqual (createTableScript, "-- Create synonyms for tables that were created above\r\nCREATE SYNONYM [SynonymName] FOR [OrderView]");
      Assert.AreEqual (dropTableScript, "-- Drop all synonyms that will be created below\r\nDROP SYNONYM [SynonymName]");
    }

    [Test]
    public void GetCreateScript_GetDropScript_NoTableAdded ()
    {
      var createTableScript = _synonymBuilder.GetCreateScript ();
      var dropTableScript = _synonymBuilder.GetDropScript ();

      Assert.That (createTableScript, Is.EqualTo ("-- Create synonyms for tables that were created above\r\n"));
      Assert.That (dropTableScript, Is.EqualTo ("-- Drop all synonyms that will be created below\r\n"));
    }

  }
}