// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Linq;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Security.TestDomain
{
  public class StubStorageFactory : IRdbmsStorageObjectFactory
  {
    public StorageProvider CreateStorageProvider (IPersistenceExtension persistenceExtension, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("persistenceExtension", persistenceExtension);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new StubStorageProvider (storageProviderDefinition, persistenceExtension);
    }

    public IPersistenceModelLoader CreatePersistenceModelLoader (
        StorageProviderDefinition storageProviderDefinition, 
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new SqlStorageObjectFactory ().CreatePersistenceModelLoader (storageProviderDefinition, storageProviderDefinitionFinder);
    }

    public IQueryExecutor CreateLinqQueryExecutor (
        ClassDefinition startingClassDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      throw new NotSupportedException ("Linq queries are not supported by the stub storage provider.");
    }

    public IScriptBuilder CreateSchemaScriptBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new CompositeScriptBuilder (
          storageProviderDefinition,
          CreateTableBuilder(),
          CreateViewBuilder(),
          CreateConstraintBuilder(),
          CreateIndexBuilder(),
          CreateSynonymBuilder());
    }

    public IStorageNameProvider CreateStorageNameProvider ()
    {
      return new ReflectionBasedStorageNameProvider();
    }

    private TableScriptBuilder CreateTableBuilder ()
    {
      return new TableScriptBuilder (new SqlTableScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    private ViewScriptBuilder CreateViewBuilder ()
    {
      return new ViewScriptBuilder (
          new SqlTableViewScriptElementFactory(),
          new SqlUnionViewScriptElementFactory(),
          new SqlFilterViewScriptElementFactory(),
          new SqlEmptyViewScriptElementFactory(),
          new SqlCommentScriptElementFactory());
    }

    private ForeignKeyConstraintScriptBuilder CreateConstraintBuilder ()
    {
      return new ForeignKeyConstraintScriptBuilder (new SqlForeignKeyConstraintScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    private IndexScriptBuilder CreateIndexBuilder ()
    {
      return
          new IndexScriptBuilder (
              new SqlIndexScriptElementFactory (
                  new SqlIndexDefinitionScriptElementFactory(),
                  new SqlPrimaryXmlIndexDefinitionScriptElementFactory(),
                  new SqlSecondaryXmlIndexDefinitionScriptElementFactory()),
              new SqlCommentScriptElementFactory());
    }

    private SynonymScriptBuilder CreateSynonymBuilder ()
    {
      var sqlSynonymScriptElementFactory = new SqlSynonymScriptElementFactory();
      return new SynonymScriptBuilder (
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          new SqlCommentScriptElementFactory());
    }
  }
}