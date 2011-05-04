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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Development.UnitTesting.Data.SqlClient;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  public class SchemaGenerationTestBase : DatabaseTest
  {
    public SchemaGenerationTestBase ()
        : base (new DatabaseAgent (SchemaGenerationConnectionString1), "Dummy.sql")
    {
    }

    [TestFixtureSetUp]
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
    }

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      DomainObjectsConfiguration.SetCurrent (SchemaGenerationConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent (SchemaGenerationConfiguration.Instance.GetMappingConfiguration());
    }

    [TearDown]
    public override void TearDown ()
    {
      base.TearDown();
    }

    protected MappingConfiguration MappingConfiguration
    {
      get { return SchemaGenerationConfiguration.Instance.GetMappingConfiguration(); }
    }

    protected StorageConfiguration StorageConfiguration
    {
      get { return SchemaGenerationConfiguration.Instance.GetPersistenceConfiguration(); }
    }

    protected RdbmsProviderDefinition SchemaGenerationFirstStorageProviderDefinition
    {
      get { return (RdbmsProviderDefinition) DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[SchemaGenerationFirstStorageProviderID]; }
    }

    protected RdbmsProviderDefinition SchemaGenerationSecondStorageProviderDefinition
    {
      get
      {
        return
            (RdbmsProviderDefinition) DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[SchemaGenerationSecondStorageProviderID];
      }
    }

    protected RdbmsProviderDefinition SchemaGenerationThirdStorageProviderDefinition
    {
      get { return (RdbmsProviderDefinition) DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[SchemaGenerationThirdStorageProviderID]; }
    }

    protected RdbmsProviderDefinition SchemaGenerationInternalStorageProviderDefinition
    {
      get
      {
        return
            (RdbmsProviderDefinition) DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[SchemaGenerationInternalStorageProviderID];
      }
    }

    protected virtual TableScriptBuilder CreateTableBuilder ()
    {
      return new TableScriptBuilder (new SqlTableScriptElementFactory());
    }

    protected virtual ViewScriptBuilder CreateViewBuilder ()
    {
      return new ViewScriptBuilder (
          new SqlTableViewScriptElementFactory(), new SqlUnionViewScriptElementFactory(), new SqlFilterViewScriptElementFactory());
    }

    protected virtual ViewScriptBuilder CreateExtendedViewBuilder ()
    {
      return new ViewScriptBuilder (
          new ExtendedSqlTableViewScriptElementFactory(),
          new ExtendedSqlUnionViewScriptElementFactory(),
          new ExtendedSqlFilterViewScriptElementFactory());
    }

    protected virtual ForeignKeyConstraintScriptBuilder CreateConstraintBuilder ()
    {
      return new ForeignKeyConstraintScriptBuilder (new SqlForeignKeyConstraintScriptElementFactory());
    }
    
    protected virtual IndexScriptBuilder CreateIndexBuilder ()
    {
      return
          new IndexScriptBuilder (
              new SqlIndexScriptElementFactory (
                  new SqlIndexDefinitionScriptElementFactory(),
                  new SqlPrimaryXmlIndexDefinitionScriptElementFactory(),
                  new SqlSecondaryXmlIndexDefinitionScriptElementFactory()));
    }

    protected virtual SynonymScriptBuilder CreateSynonymBuilder ()
    {
      return new SynonymScriptBuilder (
          new SqlSynonymScriptElementFactory(), new SqlSynonymScriptElementFactory(), new SqlSynonymScriptElementFactory());
    }
  }
}