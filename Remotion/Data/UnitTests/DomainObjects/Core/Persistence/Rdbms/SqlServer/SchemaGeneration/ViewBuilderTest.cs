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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class ViewBuilderTest : SchemaGenerationTestBase
  {
    private ViewBuilder _viewBuilder;
    private TableDefinition _tableDefinition1;
    private FilterViewDefinition _filterViewDefinition;
    private TableDefinition _tableDefinition2;
    private SimpleColumnDefinition _column1;
    private SimpleColumnDefinition _column2;
    private SimpleColumnDefinition _column3;

    public override void SetUp ()
    {
      base.SetUp();
      _viewBuilder = new ViewBuilder();

      _column1 = new SimpleColumnDefinition ("ID", typeof (int), "integer", false, true);
      _column2 = new SimpleColumnDefinition ("Name", typeof (string), "varchar(100)", true, false);
      _column3 = new SimpleColumnDefinition ("Test", typeof (string), "varchar(100)", true, false);

      _tableDefinition1 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition, "Order", "OrderView", new[]{_column1, _column2}, new ITableConstraintDefinition[0]);
      _tableDefinition2 = new TableDefinition (
         SchemaGenerationFirstStorageProviderDefinition, "Customer", "CustomerView", new[]{_column3}, new ITableConstraintDefinition[0]);
      _filterViewDefinition = new FilterViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition, "OrderView", _tableDefinition1, new[] { "ClassID" }, new[] { _column1, _column2 });
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreEqual (string.Empty, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void CreateViewSeparator ()
    {
      var result = _viewBuilder.CreateViewSeparator;

      Assert.That (result, Is.EqualTo ("GO\r\n\r\n"));
    }

    [Test]
    public void GetCreateViewScript_TableDefinition ()
    {
      _viewBuilder.AddView (_tableDefinition1);

      string expectedScript =
          "CREATE VIEW [dbo].[OrderView] ([ID], [Name])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [Name]\r\n"
          + "    FROM [dbo].[Order]\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void GetCreateViewScript_FilterViewDefinition ()
    {
      _viewBuilder.AddView (_filterViewDefinition);

      string expectedScript =
          "CREATE VIEW [dbo].[OrderView] ([ID], [Name])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [Name]\r\n"
          + "    FROM [dbo].[Order]\r\n"
          + "    WHERE [ClassID] IN ('ClassID')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void GetCreateViewScript_UnionViewDefinitionWithOneTable ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition, "OrderView", new[] { _tableDefinition1 }, new[]{_column1, _column2});

      _viewBuilder.AddView (unionViewDefinition);

      string expectedScript =
          "CREATE VIEW [dbo].[OrderView] ([ID], [Name])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [Name]\r\n"
          + "    FROM [dbo].[Order]\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void GetCreateViewScript_UnionViewDefinitionWithSeveralTables ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition, "OrderView", new[] { _tableDefinition1, _tableDefinition2 }, new[] { _column1, _column2, _column3 });

      _viewBuilder.AddView (unionViewDefinition);

      string expectedScript =
          "CREATE VIEW [dbo].[OrderView] ([ID], [Name], [Test])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [Name], NULL\r\n"
          + "    FROM [dbo].[Order]\r\n"
          + "  UNION ALL\r\n"
          + "  SELECT NULL, NULL, [Test]\r\n"
          + "    FROM [dbo].[Customer]\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript ());
    }


    [Test]
    public void GetDropViewScript_OneView ()
    {
      _viewBuilder.AddView (_tableDefinition1);

      var expectedScript =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[OrderView]\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetDropViewScript ());
    }

    [Test]
    public void GetDropViewScript_TwoViews ()
    {
      _viewBuilder.AddView (_tableDefinition1);
      _viewBuilder.AddView (_tableDefinition2);

      var expectedScript =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[OrderView]\r\n\r\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CustomerView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[CustomerView]\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetDropViewScript ());
    }
  
  }
}
