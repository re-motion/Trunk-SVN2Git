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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
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
    private readonly Type _storageProviderType;

    public SqlStorageObjectFactory ()
        : this (typeof (SqlProvider))
    {
    }

    protected SqlStorageObjectFactory (Type storageProviderType)
    {
      ArgumentUtility.CheckNotNull ("storageProviderType", storageProviderType);

      _storageProviderType = storageProviderType;
    }

    public Type StorageProviderType
    {
      get { return _storageProviderType; }
    }

    public virtual StorageProvider CreateStorageProvider (
        IPersistenceListener persistenceListener, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("persistenceListener", persistenceListener);
      var rdbmsProviderDefinition = ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition> (
          "storageProviderDefinition", storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider();

      return
          (StorageProvider)
          ObjectFactory.Create (StorageProviderType, ParamList.Create (rdbmsProviderDefinition, storageNameProvider, persistenceListener));
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
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageNameProvider = CreateStorageNameProvider();
      var columnDefinitionFactory = CreateColumnDefinitionFactory (storageNameProvider, storageProviderDefinitionFinder);
      var columnDefinitionResolver = CreateColumnDefinitionResolver();
      var foreignKeyConstraintDefintiionFactory = CreateForeignKeyConstraintDefinitionsFactory (
          storageNameProvider, columnDefinitionResolver, columnDefinitionFactory, storageProviderDefinitionFinder);
      var entityDefinitionFactory = CreateEntityDefinitionFactory (
          columnDefinitionFactory, foreignKeyConstraintDefintiionFactory, columnDefinitionResolver, storageNameProvider, storageProviderDefinition);

      return new RdbmsPersistenceModelLoader (entityDefinitionFactory, columnDefinitionFactory, storageProviderDefinition, storageNameProvider);
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
      var resolver = new MappingResolver (new StorageSpecificExpressionResolver (storageNameProvider), storageNameProvider);
      var sqlPreparationStage = ObjectFactory.Create<DefaultSqlPreparationStage> (
          ParamList.Create (methodCallTransformerProvider, resultOperatorHandlerRegistry, generator));
      var mappingResolutionStage = ObjectFactory.Create<DefaultMappingResolutionStage> (ParamList.Create (resolver, generator));
      var sqlGenerationStage = ObjectFactory.Create<DefaultSqlGenerationStage> (ParamList.Empty);

      var ctorParameters = ParamList.Create (startingClassDefinition, sqlPreparationStage, mappingResolutionStage, sqlGenerationStage);
      return ObjectFactory.Create<DomainObjectQueryExecutor> (ctorParameters);
    }

    public virtual ScriptBuilderBase CreateSchemaScriptBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new ScriptBuilder (storageProviderDefinition);
    }

    public virtual IStorageNameProvider CreateStorageNameProvider ()
    {
      return new ReflectionBasedStorageNameProvider();
    }

    protected virtual IEntityDefinitionFactory CreateEntityDefinitionFactory (
        IColumnDefinitionFactory columnDefinitionFactory,
        IForeignKeyConstraintDefinitionFactory foreignKeyConstraintDefinitionFactory,
        IColumnDefinitionResolver columnDefinitionResolver,
        IStorageNameProvider storageNameProvider,
        StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("columnDefinitionFactory", columnDefinitionFactory);
      ArgumentUtility.CheckNotNull ("foreignKeyConstraintDefinitionFactory", foreignKeyConstraintDefinitionFactory);
      ArgumentUtility.CheckNotNull ("columnDefinitionResolver", columnDefinitionResolver);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new EntityDefinitionFactory (
          columnDefinitionFactory, foreignKeyConstraintDefinitionFactory, columnDefinitionResolver, storageNameProvider, storageProviderDefinition);
    }

    protected virtual IColumnDefinitionResolver CreateColumnDefinitionResolver ()
    {
      return new ColumnDefinitionResolver();
    }

    protected virtual IColumnDefinitionFactory CreateColumnDefinitionFactory (
        IStorageNameProvider storageNameProvider, IStorageProviderDefinitionFinder providerDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("providerDefinitionFinder", providerDefinitionFinder);

      return new ColumnDefinitionFactory (new SqlStorageTypeCalculator (providerDefinitionFinder), storageNameProvider, providerDefinitionFinder);
    }

    protected virtual IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (
        IStorageNameProvider storageNameProvider,
        IColumnDefinitionResolver columnDefinitionResolver,
        IColumnDefinitionFactory columnDefinitionFactory,
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("columnDefinitionResolver", columnDefinitionResolver);
      ArgumentUtility.CheckNotNull ("columnDefinitionFactory", columnDefinitionFactory);
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);

      return new ForeignKeyConstraintDefinitionFactory (
          storageNameProvider, columnDefinitionResolver, columnDefinitionFactory, storageProviderDefinitionFinder);
    }
  }
}