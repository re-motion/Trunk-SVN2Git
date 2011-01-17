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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Implementation;
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

    public virtual StorageProvider CreateStorageProvider (IPersistenceListener persistenceListener, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("persistenceListener", persistenceListener);
      var rdbmsProviderDefinition = ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition> ("storageProviderDefinition", storageProviderDefinition);

      return (StorageProvider) ObjectFactory.Create (StorageProviderType, ParamList.Create (rdbmsProviderDefinition, persistenceListener));
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

      var storageNameCalculator = CreateStorageNameCalculator();
      var columnDefinitionFactory = CreateColumnDefinitionFactory (storageNameCalculator, storageProviderDefinitionFinder);
      var columnDefinitionResolver = CreateColumnDefinitionResolver();
      var foreignKeyConstraintDefintiionFactory = CreateForeignKeyConstraintDefinitionsFactory (
          storageNameCalculator, columnDefinitionResolver, columnDefinitionFactory, storageProviderDefinitionFinder);
      var entityDefinitionFactory = CreateEntityDefinitionFactory (
          columnDefinitionFactory, foreignKeyConstraintDefintiionFactory, columnDefinitionResolver, storageNameCalculator, storageProviderDefinition);

      return new RdbmsPersistenceModelLoader (entityDefinitionFactory, columnDefinitionFactory, storageProviderDefinition);
    }

    public virtual FileBuilderBase CreateSchemaFileBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new FileBuilder (storageProviderDefinition);
    }

    protected virtual IEntityDefinitionFactory CreateEntityDefinitionFactory (
        IColumnDefinitionFactory columnDefinitionFactory,
        IForeignKeyConstraintDefinitionFactory foreignKeyConstraintDefinitionFactory,
        IColumnDefinitionResolver columnDefinitionResolver,
        IStorageNameCalculator storageNameCalculator,
        StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("columnDefinitionFactory", columnDefinitionFactory);
      ArgumentUtility.CheckNotNull ("foreignKeyConstraintDefinitionFactory", foreignKeyConstraintDefinitionFactory);
      ArgumentUtility.CheckNotNull ("columnDefinitionResolver", columnDefinitionResolver);
      ArgumentUtility.CheckNotNull ("storageNameCalculator", storageNameCalculator);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      
      return new EntityDefinitionFactory (
          columnDefinitionFactory, foreignKeyConstraintDefinitionFactory, columnDefinitionResolver, storageNameCalculator, storageProviderDefinition);
    }

    protected virtual IColumnDefinitionResolver CreateColumnDefinitionResolver ()
    {
      return new ColumnDefinitionResolver();
    }

    // TODO Review 3607: Rename to CreateStorageNameResolver
    // TODO Review 3607: Pull up to interface IRdbmsStorageObjectFactory
    protected virtual IStorageNameCalculator CreateStorageNameCalculator ()
    {
      return new StorageNameCalculator();
    }

    protected virtual IColumnDefinitionFactory CreateColumnDefinitionFactory (
        IStorageNameCalculator storageNameCalculator, IStorageProviderDefinitionFinder providerDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageNameCalculator", storageNameCalculator);
      ArgumentUtility.CheckNotNull ("providerDefinitionFinder", providerDefinitionFinder);

      return new ColumnDefinitionFactory (new SqlStorageTypeCalculator (providerDefinitionFinder), storageNameCalculator, providerDefinitionFinder);
    }

    protected virtual IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (
        IStorageNameCalculator storageNameCalculator,
        IColumnDefinitionResolver columnDefinitionResolver,
        IColumnDefinitionFactory columnDefinitionFactory,
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageNameCalculator", storageNameCalculator);
      ArgumentUtility.CheckNotNull ("columnDefinitionResolver", columnDefinitionResolver);
      ArgumentUtility.CheckNotNull ("columnDefinitionFactory", columnDefinitionFactory);
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);
      
      return new ForeignKeyConstraintDefinitionFactory (
          storageNameCalculator, columnDefinitionResolver, columnDefinitionFactory, storageProviderDefinitionFinder);
    }
  }
}