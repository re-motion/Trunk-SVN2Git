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
      var rdbmsProviderDefinition = ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition> (
          "storageProviderDefinition", storageProviderDefinition);

      var commandFactory = CreateStorageProviderCommandFactory (rdbmsProviderDefinition);
      return CreateStorageProvider (persistenceExtension, rdbmsProviderDefinition, commandFactory);
    }

    public virtual IPersistenceModelLoader CreatePersistenceModelLoader (
        StorageProviderDefinition storageProviderDefinition, IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      var rdmsStorageProviderDefinition = ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition> (
          "storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);

      var storageNameProvider = CreateStorageNameProvider (rdmsStorageProviderDefinition);
      var dataStoragePropertyDefinitionFactory = CreateDataStoragePropertyDefinitionFactory (
          rdmsStorageProviderDefinition, storageProviderDefinitionFinder);
      var entityDefinitionFactory = CreateEntityDefinitionFactory (rdmsStorageProviderDefinition);

      return new RdbmsPersistenceModelLoader (
          entityDefinitionFactory,
          dataStoragePropertyDefinitionFactory,
          storageNameProvider,
          CreateRdbmsPersistenceModelProvider (rdmsStorageProviderDefinition));
    }

    public virtual IInfrastructureStoragePropertyDefinitionProvider CreateInfrastructureStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider (storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);

      return new InfrastructureStoragePropertyDefinitionProvider (storageTypeInformationProvider, storageNameProvider);
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

    public virtual IRdbmsStorageEntityDefinitionFactory CreateEntityDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);
      var infrastructureStoragePropertyDefinitionProvider = CreateInfrastructureStoragePropertyDefinitionFactory (storageProviderDefinition);
      var foreignKeyConstraintDefinitionFactory = CreateForeignKeyConstraintDefinitionsFactory (storageProviderDefinition);
      var storagePropertyDefinitionResolver = CreateStoragePropertyDefinitionResolver (storageProviderDefinition);

      return new RdbmsStorageEntityDefinitionFactory (
          infrastructureStoragePropertyDefinitionProvider,
          foreignKeyConstraintDefinitionFactory,
          storagePropertyDefinitionResolver,
          storageNameProvider,
          storageProviderDefinition);
    }

    public virtual IDomainObjectQueryGenerator CreateDomainObjectQueryGenerator (
        StorageProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      var rdmsStorageProviderDefinition = ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition> (
          "storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull ("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider (rdmsStorageProviderDefinition);
      var sqlQueryGenerator = CreateSqlQueryGenerator (rdmsStorageProviderDefinition, methodCallTransformerProvider, resultOperatorHandlerRegistry);

      var typeConversionProvider = TypeConversionProvider.Create();
      return
          ObjectFactory.Create<DomainObjectQueryGenerator> (
              ParamList.Create (sqlQueryGenerator, typeConversionProvider, storageTypeInformationProvider));
    }

    public virtual ISqlQueryGenerator CreateSqlQueryGenerator (
        RdbmsProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull ("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider (storageProviderDefinition);

      var generator = new UniqueIdentifierGenerator();
      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);
      var resolver =
          new MappingResolver (
              new StorageSpecificExpressionResolver (CreateRdbmsPersistenceModelProvider(storageProviderDefinition), storageNameProvider, storageTypeInformationProvider));
      var sqlPreparationStage = ObjectFactory.Create<DefaultSqlPreparationStage> (
          ParamList.Create (methodCallTransformerProvider, resultOperatorHandlerRegistry, generator));
      var mappingResolutionStage = ObjectFactory.Create<DefaultMappingResolutionStage> (ParamList.Create (resolver, generator));
      var sqlGenerationStage = ObjectFactory.Create<DefaultSqlGenerationStage> (ParamList.Empty);

      return new SqlQueryGenerator (sqlPreparationStage, mappingResolutionStage, sqlGenerationStage);
    }

    public ISqlDialect CreateSqlDialect (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new SqlDialect();
    }

    public virtual IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition, IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var valueStoragePropertyDefinitionFactory = CreateValueStoragePropertyDefinitionFactory (storageProviderDefinition);
      var relationStoragePropertyDefinitionFactory = CreateRelationStoragePropertyDefinitionFactory (
          storageProviderDefinition, storageProviderDefinitionFinder);

      return new DataStoragePropertyDefinitionFactory (valueStoragePropertyDefinitionFactory, relationStoragePropertyDefinitionFactory);
    }

    public virtual IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> CreateStorageProviderCommandFactory (
        RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var rdbmsProviderDefinition = ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition> (
          "storageProviderDefinition", storageProviderDefinition);
      var infrastructureStoragePropertyDefinitionProvider = CreateInfrastructureStoragePropertyDefinitionFactory (storageProviderDefinition);
      var storageTypeInformationProvider = CreateStorageTypeInformationProvider (storageProviderDefinition);
      var storageProviderDefinitionFinder = CreateStorageProviderDefinitionFinder(storageProviderDefinition);
      var dataStoragePropertyDefinitionFactory = CreateDataStoragePropertyDefinitionFactory (rdbmsProviderDefinition, storageProviderDefinitionFinder);

      var dbCommandBuilderFactory = CreateDbCommandBuilderFactory (storageProviderDefinition);
      var rdbmsPersistenceModelProvider = CreateRdbmsPersistenceModelProvider(storageProviderDefinition);
      return new RdbmsProviderCommandFactory (
          rdbmsProviderDefinition,
          dbCommandBuilderFactory,
          rdbmsPersistenceModelProvider,
          new ObjectReaderFactory (rdbmsPersistenceModelProvider, infrastructureStoragePropertyDefinitionProvider, storageTypeInformationProvider),
          new TableDefinitionFinder (rdbmsPersistenceModelProvider),
          dataStoragePropertyDefinitionFactory);
    }

    public virtual IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition, IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);

      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);
      var storageTypeInformationProvider = CreateStorageTypeInformationProvider (storageProviderDefinition);

      return new RelationStoragePropertyDefinitionFactory (
          storageProviderDefinition, false, storageNameProvider, storageTypeInformationProvider, storageProviderDefinitionFinder);
    }

    public virtual IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider (storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider (storageProviderDefinition);

      return new ValueStoragePropertyDefinitionFactory (storageTypeInformationProvider, storageNameProvider);
    }

    public virtual IStoragePropertyDefinitionResolver CreateStoragePropertyDefinitionResolver (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new StoragePropertyDefinitionResolver (CreateRdbmsPersistenceModelProvider (storageProviderDefinition));
    }

    public virtual IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (
        RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageNameProvider = CreateStorageNameProvider (storageProviderDefinition);
      var infrastructureStoragePropertyDefinitionProvider = CreateInfrastructureStoragePropertyDefinitionFactory (storageProviderDefinition);

      var persistenceModelProvider = CreateRdbmsPersistenceModelProvider (storageProviderDefinition);
      return new ForeignKeyConstraintDefinitionFactory (
          storageNameProvider, persistenceModelProvider, infrastructureStoragePropertyDefinitionProvider);
    }

    public virtual IStorageNameProvider CreateStorageNameProvider (RdbmsProviderDefinition storageProviderDefiniton)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefiniton", storageProviderDefiniton);

      return new ReflectionBasedStorageNameProvider();
    }

    public virtual IStorageProviderDefinitionFinder CreateStorageProviderDefinitionFinder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new StorageEntityBasedStorageProviderDefinitionFinder();
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

    public virtual IDbCommandBuilderFactory CreateDbCommandBuilderFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var sqlDialect = CreateSqlDialect (storageProviderDefinition);
      return new SqlDbCommandBuilderFactory (sqlDialect);
    }

    public virtual IStorageTypeInformationProvider CreateStorageTypeInformationProvider (RdbmsProviderDefinition rdmsStorageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("rdmsStorageProviderDefinition", rdmsStorageProviderDefinition);

      return new SqlStorageTypeInformationProvider();
    }

    public virtual IRdbmsPersistenceModelProvider CreateRdbmsPersistenceModelProvider (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new RdbmsPersistenceModelProvider();
    }

    protected virtual StorageProvider CreateStorageProvider (
        IPersistenceExtension persistenceExtension,
        StorageProviderDefinition storageProviderDefinition,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> commandFactory)
    {
      ArgumentUtility.CheckNotNull ("persistenceExtension", persistenceExtension);
      ArgumentUtility.CheckNotNull ("commandFactory", commandFactory);
      var rdbmsProviderDefinition = ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition> (
          "storageProviderDefinition", storageProviderDefinition);

      return
          ObjectFactory.Create<RdbmsProvider> (
              ParamList.Create (
                  rdbmsProviderDefinition,
                  persistenceExtension,
                  commandFactory,
                  (Func<IDbConnection>) (() => new SqlConnection())));
    }
  }
}