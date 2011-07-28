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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Implementation;
using Remotion.Linq;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005
{
  /// <summary>
  /// The <see cref="SqlStorageObjectFactory"/> is responsible to create SQL Server-specific storage provider instances.
  /// </summary>
  [ConcreteImplementation (typeof (SqlStorageObjectFactory))]
  public class SqlStorageObjectFactory : IRdbmsStorageObjectFactory
  {
    public StorageProvider CreateStorageProvider (
        IPersistenceListener persistenceListener, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("persistenceListener", persistenceListener);
      var rdbmsProviderDefinition =
          ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition> ("storageProviderDefinition", storageProviderDefinition);

      var storageNameProvider = CreateStorageNameProvider();
      var storagePropertyDefinitionFactory = CreateInfrastructureStoragePropertyDefinitionFactory (storageNameProvider);
      var commandFactory = CreateStorageProviderCommandFactory (rdbmsProviderDefinition, storageNameProvider, storagePropertyDefinitionFactory);
      return CreateStorageProvider (persistenceListener, rdbmsProviderDefinition, storageNameProvider, commandFactory);
    }

    protected virtual StorageProvider CreateStorageProvider (
        IPersistenceListener persistenceListener,
        RdbmsProviderDefinition rdbmsProviderDefinition,
        IStorageNameProvider storageNameProvider,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> commandFactory)
    {
      ArgumentUtility.CheckNotNull ("persistenceListener", persistenceListener);
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("commandFactory", commandFactory);

      return ObjectFactory.Create<SqlProvider> (ParamList.Create (rdbmsProviderDefinition, storageNameProvider, persistenceListener, commandFactory));
    }

    public virtual TypeConversionProvider CreateTypeConversionProvider ()
    {
      return TypeConversionProvider.Create();
    }

    public virtual TypeProvider CreateTypeProvider ()
    {
      return new TypeProvider();
    }

    public virtual IPersistenceModelLoader CreatePersistenceModelLoader (
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder,
        StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageNameProvider = CreateStorageNameProvider();
      var columnDefinitionResolver = CreateColumnDefinitionResolver();
      var infrastructureStoragePropertyDefinitionFactory = CreateInfrastructureStoragePropertyDefinitionFactory (storageNameProvider);
      var dataStoragePropertyDefinitionFactory = CreateDataStoragePropertyDefinitionFactory (storageNameProvider, storageProviderDefinitionFinder);
      var foreignKeyConstraintDefintiionFactory = CreateForeignKeyConstraintDefinitionsFactory (
          storageNameProvider, columnDefinitionResolver, infrastructureStoragePropertyDefinitionFactory, storageProviderDefinitionFinder);
      var entityDefinitionFactory = CreateEntityDefinitionFactory (
          infrastructureStoragePropertyDefinitionFactory,
          foreignKeyConstraintDefintiionFactory,
          columnDefinitionResolver,
          storageNameProvider,
          storageProviderDefinition);

      return new RdbmsPersistenceModelLoader (
          storageProviderDefinition,
          entityDefinitionFactory,
          dataStoragePropertyDefinitionFactory,
          storageNameProvider,
          CreateRdbmsPersistenceModelProvider());
    }

    public virtual IQueryExecutor CreateLinqQueryExecutor (
        ClassDefinition startingClassDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      ArgumentUtility.CheckNotNull ("startingClassDefinition", startingClassDefinition);
      ArgumentUtility.CheckNotNull ("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull ("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);

      var generator = new UniqueIdentifierGenerator();
      var storageNameProvider = CreateStorageNameProvider();
      var resolver = new MappingResolver (
          new StorageSpecificExpressionResolver (storageNameProvider, CreateRdbmsPersistenceModelProvider()), storageNameProvider);
      var sqlPreparationStage = ObjectFactory.Create<DefaultSqlPreparationStage> (
          ParamList.Create (methodCallTransformerProvider, resultOperatorHandlerRegistry, generator));
      var mappingResolutionStage = ObjectFactory.Create<DefaultMappingResolutionStage> (ParamList.Create (resolver, generator));
      var sqlGenerationStage = ObjectFactory.Create<DefaultSqlGenerationStage> (ParamList.Empty);
      var sqlStorageTypeCalculator = ObjectFactory.Create<SqlStorageTypeCalculator> (ParamList.Empty);

      var ctorParameters = ParamList.Create (
          startingClassDefinition, sqlPreparationStage, mappingResolutionStage, sqlGenerationStage, sqlStorageTypeCalculator);
      return ObjectFactory.Create<DomainObjectQueryExecutor> (ctorParameters);
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

    protected virtual IEntityDefinitionFactory CreateEntityDefinitionFactory (
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
        IForeignKeyConstraintDefinitionFactory foreignKeyConstraintDefinitionFactory,
        IColumnDefinitionResolver columnDefinitionResolver,
        IStorageNameProvider storageNameProvider,
        StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);
      ArgumentUtility.CheckNotNull ("foreignKeyConstraintDefinitionFactory", foreignKeyConstraintDefinitionFactory);
      ArgumentUtility.CheckNotNull ("columnDefinitionResolver", columnDefinitionResolver);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new EntityDefinitionFactory (
          infrastructureStoragePropertyDefinitionProvider,
          foreignKeyConstraintDefinitionFactory,
          columnDefinitionResolver,
          storageNameProvider,
          storageProviderDefinition);
    }

    protected virtual IColumnDefinitionResolver CreateColumnDefinitionResolver ()
    {
      return new ColumnDefinitionResolver();
    }

    protected virtual IInfrastructureStoragePropertyDefinitionProvider CreateInfrastructureStoragePropertyDefinitionFactory (
        IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);

      return new InfrastructureStoragePropertyDefinitionProvider (new SqlStorageTypeCalculator(), storageNameProvider);
    }

    protected virtual IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (
        IStorageNameProvider storageNameProvider, IStorageProviderDefinitionFinder providerDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("providerDefinitionFinder", providerDefinitionFinder);

      return new DataStoragePropertyDefinitionFactory (new SqlStorageTypeCalculator(), storageNameProvider, providerDefinitionFinder);
    }

    protected virtual IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (
        IStorageNameProvider storageNameProvider,
        IColumnDefinitionResolver columnDefinitionResolver,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("columnDefinitionResolver", columnDefinitionResolver);
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);

      return new ForeignKeyConstraintDefinitionFactory (
          storageNameProvider, columnDefinitionResolver, infrastructureStoragePropertyDefinitionProvider, storageProviderDefinitionFinder);
    }

    protected virtual TableScriptBuilder CreateTableBuilder ()
    {
      return new TableScriptBuilder (new SqlTableScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    protected virtual ViewScriptBuilder CreateViewBuilder ()
    {
      return new ViewScriptBuilder (
          new SqlTableViewScriptElementFactory(),
          new SqlUnionViewScriptElementFactory(),
          new SqlFilterViewScriptElementFactory(),
          new SqlCommentScriptElementFactory());
    }

    protected virtual ForeignKeyConstraintScriptBuilder CreateConstraintBuilder ()
    {
      return new ForeignKeyConstraintScriptBuilder (new SqlForeignKeyConstraintScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    protected virtual IndexScriptBuilder CreateIndexBuilder ()
    {
      return new IndexScriptBuilder (
          new SqlIndexScriptElementFactory (
              new SqlIndexDefinitionScriptElementFactory(),
              new SqlPrimaryXmlIndexDefinitionScriptElementFactory(),
              new SqlSecondaryXmlIndexDefinitionScriptElementFactory()),
          new SqlCommentScriptElementFactory());
    }

    protected virtual SynonymScriptBuilder CreateSynonymBuilder ()
    {
      return new SynonymScriptBuilder (
          new SqlSynonymScriptElementFactory(),
          new SqlSynonymScriptElementFactory(),
          new SqlSynonymScriptElementFactory(),
          new SqlCommentScriptElementFactory());
    }

    protected virtual IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> CreateStorageProviderCommandFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageNameProvider storageNameProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);

      var valueConverter = CreateValueConverter (storageProviderDefinition, storageNameProvider, storageProviderDefinition.TypeConversionProvider);
      var dbCommandBuilderFactory = CreateDbCommandBuilderFactory (valueConverter, storageNameProvider);

      return new RdbmsProviderCommandFactory (
          dbCommandBuilderFactory,
          CreateRdbmsPersistenceModelProvider(),
          infrastructureStoragePropertyDefinitionProvider);
    }

    protected virtual SqlDbCommandBuilderFactory CreateDbCommandBuilderFactory (
        IValueConverter valueConverter, IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull ("valueConverter", valueConverter);

      return new SqlDbCommandBuilderFactory (SqlDialect.Instance, valueConverter);
    }

    protected virtual IValueConverter CreateValueConverter (
        StorageProviderDefinition storageProviderDefinition, IStorageNameProvider storageNameProvider, TypeConversionProvider typeConversionProvider)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("typeConversionProvider", typeConversionProvider);

      return new ValueConverter ((RdbmsProviderDefinition) storageProviderDefinition, storageNameProvider, typeConversionProvider);
    }

    protected virtual IRdbmsPersistenceModelProvider CreateRdbmsPersistenceModelProvider ()
    {
      return new RdbmsPersistenceModelProvider();
    }
  }
}