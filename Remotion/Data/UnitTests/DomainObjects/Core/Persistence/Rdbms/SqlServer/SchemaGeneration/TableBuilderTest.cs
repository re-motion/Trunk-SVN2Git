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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGenerationTestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class TableBuilderTest : SchemaGenerationTestBase
  {
    private SqlTableBuilder _tableBuilder;

    public override void SetUp ()
    {
      base.SetUp();

      _tableBuilder = new SqlTableBuilder();
    }

    [Test]
    public void AddToCreateTableScript ()
    {
      string expectedStatement =
          "CREATE TABLE [dbo].[Ceo]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n"
          + "  [Name] nvarchar (100) NOT NULL,\r\n"
          + "  [CompanyID] uniqueidentifier NULL,\r\n"
          + "  [CompanyIDClassID] varchar (100) NULL,\r\n"
          + "  CONSTRAINT [PK_Ceo] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (
          (TableDefinition) MappingConfiguration.ClassDefinitions[typeof (Ceo)].StorageEntityDefinition, stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString());
    }

    [Test]
    public void AddToDropTableScript ()
    {
      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer' AND TABLE_SCHEMA = 'dbo')\r\n"
                              + "  DROP TABLE [dbo].[Customer]\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToDropTableScript (
          (TableDefinition) MappingConfiguration.ClassDefinitions[typeof (Customer)].StorageEntityDefinition, stringBuilder);

      Assert.AreEqual (expectedScript, stringBuilder.ToString());
    }

    [Test]
    public void IntegrationTest ()
    {
      _tableBuilder.AddTable ((IEntityDefinition) MappingConfiguration.ClassDefinitions[typeof (Customer)].StorageEntityDefinition);
      _tableBuilder.AddTable ((IEntityDefinition) MappingConfiguration.ClassDefinitions[typeof (Order)].StorageEntityDefinition);

      string expectedCreateTableScript = "CREATE TABLE [dbo].[Customer]\r\n"
                                         + "(\r\n"
                                         + "  [ID] uniqueidentifier NOT NULL,\r\n"
                                         + "  [ClassID] varchar (100) NOT NULL,\r\n"
                                         + "  [Timestamp] rowversion NOT NULL,\r\n"
                                         + "  [Name] nvarchar (100) NOT NULL,\r\n"
                                         + "  [PhoneNumber] nvarchar (100) NULL,\r\n"
                                         + "  [AddressID] uniqueidentifier NULL,\r\n"
                                         + "  [CustomerType] int NOT NULL,\r\n"
                                         + "  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,\r\n"
                                         + "  [PrimaryOfficialID] varchar (255) NULL,\r\n"
                                         + "  [LicenseCode] nvarchar (max) NULL,\r\n"
                                         + "  CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID])\r\n"
                                         + ")\r\n\r\n"
                                         + "CREATE TABLE [dbo].[Order]\r\n"
                                         + "(\r\n"
                                         + "  [ID] uniqueidentifier NOT NULL,\r\n"
                                         + "  [ClassID] varchar (100) NOT NULL,\r\n"
                                         + "  [Timestamp] rowversion NOT NULL,\r\n"
                                         + "  [Number] int NOT NULL,\r\n"
                                         + "  [Priority] int NOT NULL,\r\n"
                                         + "  [CustomerID] uniqueidentifier NULL,\r\n"
                                         + "  [CustomerIDClassID] varchar (100) NULL,\r\n"
                                         + "  [OfficialID] varchar (255) NULL,\r\n"
                                         + "  CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([ID])\r\n"
                                         + ")\r\n";

      Assert.AreEqual (expectedCreateTableScript, _tableBuilder.GetCreateTableScript());

      string expectedDropTableScript =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP TABLE [dbo].[Customer]\r\n\r\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP TABLE [dbo].[Order]\r\n";

      Assert.AreEqual (expectedDropTableScript, _tableBuilder.GetDropTableScript());
    }

  }
}