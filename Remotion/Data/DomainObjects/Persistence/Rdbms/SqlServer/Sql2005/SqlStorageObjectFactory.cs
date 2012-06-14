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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
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
    public StorageProvider CreateStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
    {
      ArgumentUtility.CheckNotNull ("persistenceExtension", persistenceExtension);
      var rdbmsProviderDefinition = 
          ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition> ("storageProviderDefinition", storageProviderDefinition);

      var commandFactory = CreateStorageProviderCommandFactory (rdbmsProviderDefinition);
      return CreateStorageProvider (persistenceExtension, rdbmsProviderDefinition, commandFactory);
    }

    public virtual IPersistenceModelLoader CreatePersistenceModelLoader (
        StorageProviderDefinition storageProviderDefinition,
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      var rdmsStorageProviderDefinition =
          ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition> ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(rdmsStorageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider (rdmsStorageProviderDefinition);
      var persistenceModelProvider = CreateRdbmsPersistenceModelProvider (rdmsStorageProviderDefinition);
      var storagePropertyDefinitionResolver = CreateStoragePropertyDefinitionResolver (rdmsStorageProviderDefinition, persistenceModelProvider);

      var dataStoragePropertyDefinitionFactory = CreateDataStoragePropertyDefinitionFactory (
          rdmsStorageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider,
          storageProviderDefinitionFinder);

      var infrastructureStoragePropertyDefinitionFactory = CreateInfrastructureStoragePropertyDefinitionProvider (
          rdmsStorageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider);

      var foreignKeyConstraintDefinitionFactory = CreateForeignKeyConstraintDefinitionsFactory (
          rdmsStorageProviderDefinition,
          storageNameProvider,
          persistenceModelProvider,
          infrastructureStoragePropertyDefinitionFactory);

      var entityDefinitionFactory = CreateEntityDefinitionFactory (
          rdmsStorageProviderDefinition,
          infrastructureStoragePropertyDefinitionFactory,
          foreignKeyConstraintDefinitionFactory,
          storagePropertyDefinitionResolver,
          storageNameProvider);

      return new RdbmsPersistenceModelLoader (
          entityDefinitionFactory,
          dataStoragePropertyDefinitionFactory,
          storageNameProvider,
          persistenceModelProvider);
    }

    public virtual IDomainObjectQueryGenerator CreateDomainObjectQueryGenerator (
        StorageProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      var rdmsStorageProviderDefinition = 
          ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition> ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull ("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider (rdmsStorageProviderDefinition);
      var sqlQueryGenerator = CreateSqlQueryGenerator (rdmsStorageProviderDefinition, methodCallTransformerProvider, resultOperatorHandlerRegistry);
      var typeConversionProvider = TypeConversionProvider.Create();

      return ObjectFactory.Create<DomainObjectQueryGenerator> (
          ParamList.Create (sqlQueryGenerator, typeConversionProvider, storageTypeInformationProvider));
    }


    public virtual ISqlDialect CreateSqlDialect (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new SqlDialect();
    }


    public virtual IStorageTypeInformationProvider CreateStorageTypeInformationProvider (RdbmsProviderDefinition rdmsStorageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("rdmsStorageProviderDefinition", rdmsStorageProviderDefinition);

      return new SqlStorageTypeInformationProvider();
    }

    public virtual IStorageNameProvider CreateStorageNameProvider (RdbmsProviderDefinition storageProviderDefiniton)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefiniton", storageProviderDefiniton);

      return new ReflectionBasedStorageNameProvider();
    }

    public virtual IRdbmsPersistenceModelProvider CreateRdbmsPersistenceModelProvider (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new RdbmsPersistenceModelProvider();
    }


    public ISqlQueryGenerator CreateSqlQueryGenerator (
        RdbmsProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull ("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider (storageProviderDefinition);
      var persistenceModelProvider = CreateRdbmsPersistenceModelProvider (storageProviderDefinition);

      return CreateSqlQueryGenerator (
          storageProviderDefinition,
          methodCallTransformerProvider,
          resultOperatorHandlerRegistry,
          storageTypeInformationProvider,
          persistenceModelProvider);
    }

    public IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> CreateStorageProviderCommandFactory (
        RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider (storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider (storageProviderDefinition);
      var persistenceModelProvider = CreateRdbmsPersistenceModelProvider (storageProviderDefinition);
      var storageProviderDefinitionFinder = new StorageEntityBasedStorageProviderDefinitionFinder();

      var infrastructureStoragePropertyDefinitionProvider = CreateInfrastructureStoragePropertyDefinitionProvider (
          storageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider);

      var dataStoragePropertyDefinitionFactory = CreateDataStoragePropertyDefinitionFactory (
          storageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider,
          storageProviderDefinitionFinder);

      return CreateStorageProviderCommandFactory (
          storageProviderDefinition,
          storageNameProvider,
          persistenceModelProvider,
          infrastructureStoragePropertyDefinitionProvider,
          dataStoragePropertyDefinitionFactory,
          storageTypeInformationProvider);
    }

    public virtual IDbCommandBuilderFactory CreateDbCommandBuilderFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var sqlDialect = CreateSqlDialect (storageProviderDefinition);
      return new SqlDbCommandBuilderFactory (sqlDialect);
    }


    public IRdbmsStorageEntityDefinitionFactory CreateEntityDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var persistenceModelProvider = CreateRdbmsPersistenceModelProvider (storageProviderDefinition);
      var infrastructureStoragePropertyDefinitionProvider = CreateInfrastructureStoragePropertyDefinitionProvider (storageProviderDefinition);
      var foreignKeyConstraintDefinitionFactory = CreateForeignKeyConstraintDefinitionsFactory (storageProviderDefinition);
      var storagePropertyDefinitionResolver = CreateStoragePropertyDefinitionResolver (storageProviderDefinition, persistenceModelProvider);
      var storageNameProvider = CreateStorageNameProvider (storageProviderDefinition);

      return CreateEntityDefinitionFactory (
          storageProviderDefinition,
          infrastructureStoragePropertyDefinitionProvider,
          foreignKeyConstraintDefinitionFactory,
          storagePropertyDefinitionResolver,
          storageNameProvider);
    }

    public IInfrastructureStoragePropertyDefinitionProvider CreateInfrastructureStoragePropertyDefinitionProvider (
        RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider (storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider (storageProviderDefinition);

      return CreateInfrastructureStoragePropertyDefinitionProvider (storageProviderDefinition, storageTypeInformationProvider, storageNameProvider);
    }

    public IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider (storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider (storageProviderDefinition);
      var storageProviderDefinitionFinder = new StorageEntityBasedStorageProviderDefinitionFinder();

      return CreateDataStoragePropertyDefinitionFactory (
          storageProviderDefinition, storageTypeInformationProvider, storageNameProvider, storageProviderDefinitionFinder);
    }

    public IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);
      var storageTypeInformationProvider = CreateStorageTypeInformationProvider (storageProviderDefinition);
      var storageProviderDefinitionFinder = new StorageEntityBasedStorageProviderDefinitionFinder();

      return CreateRelationStoragePropertyDefinitionFactory (
          storageProviderDefinition, storageNameProvider, storageProviderDefinitionFinder, storageTypeInformationProvider);
    }

    public IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider (storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider (storageProviderDefinition);

      return CreateValueStoragePropertyDefinitionFactory (storageProviderDefinition, storageNameProvider, storageTypeInformationProvider);
    }

    public IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageNameProvider = CreateStorageNameProvider (storageProviderDefinition);
      var persistenceModelProvider = CreateRdbmsPersistenceModelProvider (storageProviderDefinition);
      var infrastructureStoragePropertyDefinitionProvider = CreateInfrastructureStoragePropertyDefinitionProvider (storageProviderDefinition);

      return CreateForeignKeyConstraintDefinitionsFactory (
          storageProviderDefinition,
          storageNameProvider,
          persistenceModelProvider,
          infrastructureStoragePropertyDefinitionProvider);
    }


    public virtual IScriptBuilder CreateSchemaScriptBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var compositeScriptBuilder = new CompositeScriptBuilder (
          storageProviderDefinition,
          CreateTableBuilder (storageProviderDefinition),
          CreateConstraintBuilder (storageProviderDefinition),
          CreateViewBuilder (storageProviderDefinition),
          CreateIndexBuilder (storageProviderDefinition),
          CreateSynonymBuilder (storageProviderDefinition));

      return new SqlDatabaseSelectionScriptElementBuilder (compositeScriptBuilder, storageProviderDefinition.ConnectionString);
    }

    public virtual TableScriptBuilder CreateTableBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new TableScriptBuilder (new SqlTableScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    public virtual ViewScriptBuilder CreateViewBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new ViewScriptBuilder (
          new SqlTableViewScriptElementFactory(),
          new SqlUnionViewScriptElementFactory(),
          new SqlFilterViewScriptElementFactory(),
          new SqlEmptyViewScriptElementFactory(),
          new SqlCommentScriptElementFactory());
    }

    public virtual ForeignKeyConstraintScriptBuilder CreateConstraintBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new ForeignKeyConstraintScriptBuilder (new SqlForeignKeyConstraintScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    public virtual IndexScriptBuilder CreateIndexBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new IndexScriptBuilder (
          new SqlIndexScriptElementFactory (
              new SqlIndexDefinitionScriptElementFactory(),
              new SqlPrimaryXmlIndexDefinitionScriptElementFactory(),
              new SqlSecondaryXmlIndexDefinitionScriptElementFactory()),
          new SqlCommentScriptElementFactory());
    }

    public virtual SynonymScriptBuilder CreateSynonymBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var sqlSynonymScriptElementFactory = new SqlSynonymScriptElementFactory();
      return new SynonymScriptBuilder (
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          new SqlCommentScriptElementFactory());
    }


    protected virtual StorageProvider CreateStorageProvider (
        IPersistenceExtension persistenceExtension,
        StorageProviderDefinition storageProviderDefinition,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> commandFactory)
    {
      ArgumentUtility.CheckNotNull ("persistenceExtension", persistenceExtension);
      ArgumentUtility.CheckNotNull ("commandFactory", commandFactory);
      var rdbmsProviderDefinition = 
          ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition> ("storageProviderDefinition", storageProviderDefinition);

      return ObjectFactory.Create<RdbmsProvider> (
          ParamList.Create (
              rdbmsProviderDefinition,
              persistenceExtension,
              commandFactory,
              (Func<IDbConnection>) (() => new SqlConnection())));
    }


    protected virtual IStoragePropertyDefinitionResolver CreateStoragePropertyDefinitionResolver (
        RdbmsProviderDefinition storageProviderDefinition,
        IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("persistenceModelProvider", persistenceModelProvider);

      return new StoragePropertyDefinitionResolver (persistenceModelProvider);
    }


    protected virtual IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> CreateStorageProviderCommandFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageNameProvider storageNameProvider,
        IRdbmsPersistenceModelProvider persistenceModelProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
        IDataStoragePropertyDefinitionFactory dataStoragePropertyDefinitionFactory,
        IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("persistenceModelProvider", persistenceModelProvider);
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull ("dataStoragePropertyDefinitionFactory", dataStoragePropertyDefinitionFactory);

      var dbCommandBuilderFactory = CreateDbCommandBuilderFactory (storageProviderDefinition);

      return new RdbmsProviderCommandFactory (
          storageProviderDefinition,
          dbCommandBuilderFactory,
          persistenceModelProvider,
          new ObjectReaderFactory (persistenceModelProvider, infrastructureStoragePropertyDefinitionProvider, storageTypeInformationProvider),
          new TableDefinitionFinder (persistenceModelProvider),
          dataStoragePropertyDefinitionFactory);
    }

    protected virtual ISqlQueryGenerator CreateSqlQueryGenerator (
        RdbmsProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull ("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull ("persistenceModelProvider", persistenceModelProvider);

      var generator = new UniqueIdentifierGenerator ();
      var storageNameProvider = CreateStorageNameProvider (storageProviderDefinition);
      var resolver = new MappingResolver (
          new StorageSpecificExpressionResolver (
              persistenceModelProvider,
              storageNameProvider,
              storageTypeInformationProvider));
      var sqlPreparationStage = ObjectFactory.Create<DefaultSqlPreparationStage> (
          ParamList.Create (methodCallTransformerProvider, resultOperatorHandlerRegistry, generator));
      var mappingResolutionStage = ObjectFactory.Create<DefaultMappingResolutionStage> (ParamList.Create (resolver, generator));
      var sqlGenerationStage = ObjectFactory.Create<DefaultSqlGenerationStage> (ParamList.Empty);

      return new SqlQueryGenerator (sqlPreparationStage, mappingResolutionStage, sqlGenerationStage);
    }


    protected virtual IRdbmsStorageEntityDefinitionFactory CreateEntityDefinitionFactory (
         RdbmsProviderDefinition storageProviderDefinition,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionFactory,
        IForeignKeyConstraintDefinitionFactory foreignKeyConstraintDefinitionFactory,
        IStoragePropertyDefinitionResolver storagePropertyDefinitionResolver,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionFactory", infrastructureStoragePropertyDefinitionFactory);
      ArgumentUtility.CheckNotNull ("foreignKeyConstraintDefinitionFactory", foreignKeyConstraintDefinitionFactory);
      ArgumentUtility.CheckNotNull ("storagePropertyDefinitionResolver", storagePropertyDefinitionResolver);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      
      return new RdbmsStorageEntityDefinitionFactory (
          infrastructureStoragePropertyDefinitionFactory,
          foreignKeyConstraintDefinitionFactory,
          storagePropertyDefinitionResolver,
          storageNameProvider,
          storageProviderDefinition);
    }

    protected virtual IInfrastructureStoragePropertyDefinitionProvider CreateInfrastructureStoragePropertyDefinitionProvider (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);

      return new InfrastructureStoragePropertyDefinitionProvider (storageTypeInformationProvider, storageNameProvider);
    }

    protected virtual IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider,
        IStorageProviderDefinitionFinder providerDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("providerDefinitionFinder", providerDefinitionFinder);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);

      var valueStoragePropertyDefinitionFactory = CreateValueStoragePropertyDefinitionFactory (
          storageProviderDefinition,
          storageNameProvider,
          storageTypeInformationProvider);

      var relationStoragePropertyDefinitionFactory = CreateRelationStoragePropertyDefinitionFactory (
          storageProviderDefinition,
          storageNameProvider,
          providerDefinitionFinder,
          storageTypeInformationProvider);

      return new DataStoragePropertyDefinitionFactory (valueStoragePropertyDefinitionFactory, relationStoragePropertyDefinitionFactory);
    }

    protected virtual IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageNameProvider storageNameProvider,
        IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);

      return new ValueStoragePropertyDefinitionFactory (storageTypeInformationProvider, storageNameProvider);
    }

    protected virtual IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageNameProvider storageNameProvider,
        IStorageProviderDefinitionFinder providerDefinitionFinder,
        IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("providerDefinitionFinder", providerDefinitionFinder);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);

      return new RelationStoragePropertyDefinitionFactory (
          storageProviderDefinition,
          false,
          storageNameProvider,
          storageTypeInformationProvider,
          providerDefinitionFinder);
    }

    protected virtual IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageNameProvider storageNameProvider,
        IRdbmsPersistenceModelProvider persistenceModelProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("persistenceModelProvider", persistenceModelProvider);
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);

      return new ForeignKeyConstraintDefinitionFactory (
          storageNameProvider,
          persistenceModelProvider,
          infrastructureStoragePropertyDefinitionProvider);
    }
  }
}