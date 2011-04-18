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
  public class SynonymBuilderBaseTest : SchemaGenerationTestBase
  {
    private ISqlDialect _sqlDialectStub;
    private TestableSynonymBuilder _synonymBuilder;
    private TableDefinition _tableDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect> ();
      _synonymBuilder = new TestableSynonymBuilder (_sqlDialectStub);

      _tableDefinition = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Order"),
          new EntityNameDefinition(null, "OrderView"),
          new IColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0], 
          new[]{new EntityNameDefinition(null, "SynonymName")});
    }

    [Test]
    public void AddSynonyms_TableDefinition()
    {
      _synonymBuilder.AddSynonyms (_tableDefinition);

      var createTableScript = _synonymBuilder.GetCreateTableScript ();
      var dropTableScript = _synonymBuilder.GetDropTableScript ();

      Assert.AreEqual (createTableScript, "CREATE SYNONYM [SynonymName] FOR [OrderView]");
      Assert.AreEqual (dropTableScript, "DROP SYNONYM [SynonymName]");
    }

    [Test]
    public void AddSynonyms_TableDefinition_Wice ()
    {
      _synonymBuilder.AddSynonyms (_tableDefinition);
      _synonymBuilder.AddSynonyms (_tableDefinition);

      var createTableScript = _synonymBuilder.GetCreateTableScript ();
      var dropTableScript = _synonymBuilder.GetDropTableScript ();

      Assert.AreEqual (createTableScript, "CREATE SYNONYM [SynonymName] FOR [OrderView]\r\nCREATE SYNONYM [SynonymName] FOR [OrderView]");
      Assert.AreEqual (dropTableScript, "DROP SYNONYM [SynonymName]\r\nDROP SYNONYM [SynonymName]");
    }

    [Test]
    public void AddSynonyms_FilterViewDefinition ()
    {
      var filterViewDefinition = new FilterViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "OrderView"),
          _tableDefinition,
          new[] { "ClassID" },
          new IColumnDefinition[0],
          new IIndexDefinition[0],
          new[] { new EntityNameDefinition (null, "SynonymName") });

      _synonymBuilder.AddSynonyms (filterViewDefinition);

      var createTableScript = _synonymBuilder.GetCreateTableScript ();
      var dropTableScript = _synonymBuilder.GetDropTableScript ();

      Assert.AreEqual (createTableScript, "CREATE SYNONYM [SynonymName] FOR [OrderView]");
      Assert.AreEqual (dropTableScript, "DROP SYNONYM [SynonymName]");
    }

    [Test]
    public void AddSynonyms_UnionViewDefinition ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          SchemaGenerationInternalStorageProviderDefinition,
          new EntityNameDefinition (null, "OrderView"),
          new[] { _tableDefinition },
          new IColumnDefinition[0],
          new IIndexDefinition[0],
          new[] { new EntityNameDefinition (null, "SynonymName") });

      _synonymBuilder.AddSynonyms (unionViewDefinition);

      var createTableScript = _synonymBuilder.GetCreateTableScript ();
      var dropTableScript = _synonymBuilder.GetDropTableScript ();

      Assert.AreEqual (createTableScript, "CREATE SYNONYM [SynonymName] FOR [OrderView]");
      Assert.AreEqual (dropTableScript, "DROP SYNONYM [SynonymName]");
    }

    [Test]
    public void GetCreateTableScript_GetDropTableScript_NoTableAdded ()
    {
      var createTableScript = _synonymBuilder.GetCreateTableScript ();
      var dropTableScript = _synonymBuilder.GetDropTableScript ();

      Assert.IsEmpty (createTableScript);
      Assert.IsEmpty (dropTableScript);
    }

  }
}