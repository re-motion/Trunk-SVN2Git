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
using System.Data;
using System.Data.SqlClient;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Linq;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005
{
  /// <summary>
  /// The <see cref="SqlStorageObjectFactory"/> is responsible to create SQL Server-specific storage provider instances.
  /// </summary>
  [ConcreteImplementation (typeof (SqlStorageObjectFactory))]
  public class SqlStorageObjectFactory : IRdbmsStorageObjectFactory
  {
    public StorageProvider CreateStorageProvider (IPersistenceExtension persistenceExtension, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("persistenceExtension", persistenceExtension);
      var rdbmsProviderDefinition =
          ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition> ("storageProviderDefinition", storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider();
      var storageNameProvider = CreateStorageNameProvider();
      var infrastructureStoragePropertyDefinitionFactory = CreateInfrastructureStoragePropertyDefinitionFactory (
          storageTypeInformationProvider,
          storageNameProvider);
      var storageProviderDefinitionFinder = new StorageEntityBasedStorageProviderDefinitionFinder();
      var dataStoragePropertyDefinitionFactory = CreateDataStoragePropertyDefinitionFactory (
          storageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider,
          storageProviderDefinitionFinder);
      var commandFactory = CreateStorageProviderCommandFactory (
          rdbmsProviderDefinition,
          storageNameProvider,
          infrastructureStoragePropertyDefinitionFactory,
          dataStoragePropertyDefinitionFactory,
          storageTypeInformationProvider);
      return CreateStorageProvider (persistenceExtension, rdbmsProviderDefinition, commandFactory);
    }

    // TODO 4896: Keep as is, move down to protected members, use Arg-typecheck as in CreateStorageProvider for ProviderDefinition
    protected virtual StorageProvider CreateStorageProvider (
        IPersistenceExtension persistenceExtension,
        RdbmsProviderDefinition rdbmsProviderDefinition,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> commandFactory)
    {
      ArgumentUtility.CheckNotNull ("persistenceExtension", persistenceExtension);
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);
      ArgumentUtility.CheckNotNull ("commandFactory", commandFactory);

      return
          ObjectFactory.Create<RdbmsProvider> (
              ParamList.Create (
                  rdbmsProviderDefinition,
                  persistenceExtension,
                  commandFactory,
                  (Func<IDbConnection>) (() => new SqlConnection())));
    }

    public virtual IPersistenceModelLoader CreatePersistenceModelLoader (
        StorageProviderDefinition storageProviderDefinition,
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider();
      var storageNameProvider = CreateStorageNameProvider();
      var storagePropertyDefinitionResolver = CreateStoragePropertyDefinitionResolver();
      var infrastructureStoragePropertyDefinitionFactory = CreateInfrastructureStoragePropertyDefinitionFactory (
          storageTypeInformationProvider, storageNameProvider);
      var dataStoragePropertyDefinitionFactory = CreateDataStoragePropertyDefinitionFactory (
          storageProviderDefinition, storageTypeInformationProvider, storageNameProvider, storageProviderDefinitionFinder);
      var foreignKeyConstraintDefintiionFactory = CreateForeignKeyConstraintDefinitionsFactory (
          storageNameProvider,
          infrastructureStoragePropertyDefinitionFactory);
      var entityDefinitionFactory = CreateEntityDefinitionFactory (
          infrastructureStoragePropertyDefinitionFactory,
          foreignKeyConstraintDefintiionFactory,
          storagePropertyDefinitionResolver,
          storageNameProvider,
          storageProviderDefinition);

      return new RdbmsPersistenceModelLoader (
          entityDefinitionFactory,
          dataStoragePropertyDefinitionFactory,
          storageNameProvider,
          CreateRdbmsPersistenceModelProvider());
    }

    //TODO 4900: Inline at usage point
    public virtual IQueryExecutor CreateLinqQueryExecutor (
        ClassDefinition startingClassDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      ArgumentUtility.CheckNotNull ("startingClassDefinition", startingClassDefinition);
      ArgumentUtility.CheckNotNull ("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull ("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);

      var queryGenerator = CreateDomainObjectQueryGenerator (methodCallTransformerProvider, resultOperatorHandlerRegistry);
      return new DomainObjectQueryExecutor (startingClassDefinition, queryGenerator);
    }

    public virtual IScriptBuilder CreateSchemaScriptBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var compositeScriptBuilder = new CompositeScriptBuilder (
          storageProviderDefinition,
          CreateTableBuilder(),
          CreateConstraintBuilder(),
          CreateViewBuilder(),
          CreateIndexBuilder(),
          CreateSynonymBuilder());

      return new SqlDatabaseSelectionScriptElementBuilder (compositeScriptBuilder, storageProviderDefinition.ConnectionString);
    }

    public virtual IStorageNameProvider CreateStorageNameProvider ()
    {
      return new ReflectionBasedStorageNameProvider();
    }

    // TODO 4896: Add public, non-virtual, interface variant without arguments (except storageProviderDefinition as RdbmsStorageProviderDefinition)
    protected virtual IRdbmsStorageEntityDefinitionFactory CreateEntityDefinitionFactory (
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
        IForeignKeyConstraintDefinitionFactory foreignKeyConstraintDefinitionFactory,
        IStoragePropertyDefinitionResolver storagePropertyDefinitionResolver,
        IStorageNameProvider storageNameProvider,
        StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);
      ArgumentUtility.CheckNotNull ("foreignKeyConstraintDefinitionFactory", foreignKeyConstraintDefinitionFactory);
      ArgumentUtility.CheckNotNull ("storagePropertyDefinitionResolver", storagePropertyDefinitionResolver);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new RdbmsStorageEntityDefinitionFactory (
          infrastructureStoragePropertyDefinitionProvider,
          foreignKeyConstraintDefinitionFactory,
          storagePropertyDefinitionResolver,
          storageNameProvider,
          storageProviderDefinition);
    }

    // TODO 4896: public, interface
    protected virtual IStoragePropertyDefinitionResolver CreateStoragePropertyDefinitionResolver ()
    {
      return new StoragePropertyDefinitionResolver (CreateRdbmsPersistenceModelProvider());
    }

    // TODO 4896: Add public, non-virtual, interface variant without arguments (except storageProviderDefinition as RdbmsStorageProviderDefinition)
    protected virtual IInfrastructureStoragePropertyDefinitionProvider CreateInfrastructureStoragePropertyDefinitionFactory (
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);

      return new InfrastructureStoragePropertyDefinitionProvider (storageTypeInformationProvider, storageNameProvider);
    }

    // TODO 4896: Add public, non-virtual, interface variant without arguments (except storageProviderDefinition as RdbmsStorageProviderDefinition)
    protected virtual IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (
        StorageProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider,
        IStorageProviderDefinitionFinder providerDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("providerDefinitionFinder", providerDefinitionFinder);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);

      var valueStoragePropertyDefinitionFactory = CreateValueStoragePropertyDefinitionFactory (storageNameProvider, storageTypeInformationProvider);
      var relationStoragePropertyDefinitionFactory = CreateRelationStoragePropertyDefinitionFactory (
          storageProviderDefinition, storageNameProvider, providerDefinitionFinder, storageTypeInformationProvider);

      return new DataStoragePropertyDefinitionFactory (valueStoragePropertyDefinitionFactory, relationStoragePropertyDefinitionFactory);
    }

    // TODO 4896: Add public, non-virtual, interface variant without arguments
    protected virtual IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory (
        IStorageNameProvider storageNameProvider,
        IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);

      return new ValueStoragePropertyDefinitionFactory (storageTypeInformationProvider, storageNameProvider);
    }

    // TODO 4896: Add public, non-virtual, interface variant without arguments (except storageProviderDefinition as RdbmsStorageProviderDefinition)
    protected virtual IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (
        StorageProviderDefinition storageProviderDefinition,
        IStorageNameProvider storageNameProvider,
        IStorageProviderDefinitionFinder providerDefinitionFinder,
        IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("providerDefinitionFinder", providerDefinitionFinder);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);

      return new RelationStoragePropertyDefinitionFactory (
          storageProviderDefinition, false, storageNameProvider, storageTypeInformationProvider, providerDefinitionFinder);
    }

    // TODO 4896: Add public, non-virtual, interface variant without arguments
    protected virtual IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (
        IStorageNameProvider storageNameProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);

      var persistenceModelProvider = CreateRdbmsPersistenceModelProvider();
      return new ForeignKeyConstraintDefinitionFactory (
          storageNameProvider, persistenceModelProvider, infrastructureStoragePropertyDefinitionProvider);
    }

    // TODO 4896: public, interface
    protected virtual TableScriptBuilder CreateTableBuilder ()
    {
      return new TableScriptBuilder (new SqlTableScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    // TODO 4896: public, interface
    protected virtual ViewScriptBuilder CreateViewBuilder ()
    {
      return new ViewScriptBuilder (
          new SqlTableViewScriptElementFactory(),
          new SqlUnionViewScriptElementFactory(),
          new SqlFilterViewScriptElementFactory(),
          new SqlEmptyViewScriptElementFactory(),
          new SqlCommentScriptElementFactory());
    }

    // TODO 4896: public, interface
    protected virtual ForeignKeyConstraintScriptBuilder CreateConstraintBuilder ()
    {
      return new ForeignKeyConstraintScriptBuilder (new SqlForeignKeyConstraintScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    // TODO 4896: public, interface
    protected virtual IndexScriptBuilder CreateIndexBuilder ()
    {
      return new IndexScriptBuilder (
          new SqlIndexScriptElementFactory (
              new SqlIndexDefinitionScriptElementFactory(),
              new SqlPrimaryXmlIndexDefinitionScriptElementFactory(),
              new SqlSecondaryXmlIndexDefinitionScriptElementFactory()),
          new SqlCommentScriptElementFactory());
    }

    // TODO 4896: public, interface
    protected virtual SynonymScriptBuilder CreateSynonymBuilder ()
    {
      var sqlSynonymScriptElementFactory = new SqlSynonymScriptElementFactory();
      return new SynonymScriptBuilder (
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          new SqlCommentScriptElementFactory());
    }

    // TODO 4896: Add public, non-virtual, interface variant without arguments (except storageProviderDefinition as RdbmsStorageProviderDefinition)
    protected virtual IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> CreateStorageProviderCommandFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageNameProvider storageNameProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
        IDataStoragePropertyDefinitionFactory dataStoragePropertyDefinitionFactory,
        IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull ("dataStoragePropertyDefinitionFactory", dataStoragePropertyDefinitionFactory);

      var dbCommandBuilderFactory = CreateDbCommandBuilderFactory();
      var rdbmsPersistenceModelProvider = CreateRdbmsPersistenceModelProvider();
      return new RdbmsProviderCommandFactory (
          storageProviderDefinition,
          dbCommandBuilderFactory,
          rdbmsPersistenceModelProvider,
          new ObjectReaderFactory (rdbmsPersistenceModelProvider, infrastructureStoragePropertyDefinitionProvider, storageTypeInformationProvider),
          new TableDefinitionFinder (rdbmsPersistenceModelProvider),
          dataStoragePropertyDefinitionFactory);
    }

    // TODO 4896: public, interface
    protected virtual SqlDbCommandBuilderFactory CreateDbCommandBuilderFactory ()
    {
      return new SqlDbCommandBuilderFactory (SqlDialect.Instance);
    }

    // TODO 4896: public, interface
    protected virtual SqlStorageTypeInformationProvider CreateStorageTypeInformationProvider ()
    {
      return new SqlStorageTypeInformationProvider();
    }

    // TODO 4896: public, interface
    protected virtual IRdbmsPersistenceModelProvider CreateRdbmsPersistenceModelProvider ()
    {
      return new RdbmsPersistenceModelProvider();
    }

    // TODO 4896: public, interface IStorageObjectFactory, use Arg-typecheck as in CreateStorageProvider for ProviderDefinition
    protected virtual IDomainObjectQueryGenerator CreateDomainObjectQueryGenerator (
        IMethodCallTransformerProvider methodCallTransformerProvider, ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      ArgumentUtility.CheckNotNull ("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull ("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider();
      var sqlQueryGenerator = CreateSqlQueryGenerator (methodCallTransformerProvider, resultOperatorHandlerRegistry, storageTypeInformationProvider);

      var typeConversionProvider = TypeConversionProvider.Create();
      return
          ObjectFactory.Create<DomainObjectQueryGenerator> (
              ParamList.Create (sqlQueryGenerator, typeConversionProvider, storageTypeInformationProvider));
    }

    // TODO 4896: Add public, non-virtual, interface variant without storageTypeInformationProvider parameter (instead call CreateStorageTypeInformationProvider)
    protected virtual ISqlQueryGenerator CreateSqlQueryGenerator (
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry,
        IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull ("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);

      var generator = new UniqueIdentifierGenerator();
      var storageNameProvider = CreateStorageNameProvider();
      var resolver = new MappingResolver (new StorageSpecificExpressionResolver (CreateRdbmsPersistenceModelProvider(), storageNameProvider, storageTypeInformationProvider));
      var sqlPreparationStage = ObjectFactory.Create<DefaultSqlPreparationStage> (
          ParamList.Create (methodCallTransformerProvider, resultOperatorHandlerRegistry, generator));
      var mappingResolutionStage = ObjectFactory.Create<DefaultMappingResolutionStage> (ParamList.Create (resolver, generator));
      var sqlGenerationStage = ObjectFactory.Create<DefaultSqlGenerationStage> (ParamList.Empty);

      return new SqlQueryGenerator (sqlPreparationStage, mappingResolutionStage, sqlGenerationStage);
    }
  }
}