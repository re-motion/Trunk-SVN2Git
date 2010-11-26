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
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// The <see cref="SqlStorageObjectFactory"/> is responsible to create SQL Server-specific storage provider instances.
  /// </summary>
  public class SqlStorageObjectFactory : IStorageObjectFactory
  {
    private readonly RdbmsProviderDefinition _storageProviderDefinition;
    private readonly IStoragePropertyDefinitionFactory _storagePropertyDefinitionFactory;
    private readonly Type _storageProviderType;

    public SqlStorageObjectFactory (RdbmsProviderDefinition storageProviderDefinition) : this(storageProviderDefinition, typeof(SqlProvider))
    {
    }

    protected SqlStorageObjectFactory (RdbmsProviderDefinition storageProviderDefinition, Type storageProviderType)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      _storageProviderDefinition = storageProviderDefinition;
      _storagePropertyDefinitionFactory = new ColumnDefinitionFactory (new SqlStorageTypeCalculator ());
      _storageProviderType = storageProviderType;
    }

    public Type StorageProviderType
    {
      get { return _storageProviderType; }
    }

    protected RdbmsProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    protected IStoragePropertyDefinitionFactory StoragePropertyDefinitionFactory
    {
      get { return _storagePropertyDefinitionFactory; }
    }

    public virtual StorageProvider CreateStorageProvider (IPersistenceListener persistenceListener)
    {
      ArgumentUtility.CheckNotNull ("persistenceListener", persistenceListener);

      return (StorageProvider) ObjectFactory.Create (StorageProviderType, ParamList.Create (StorageProviderDefinition, persistenceListener));
    }

    public virtual TypeConversionProvider CreateTypeConversionProvider ()
    {
      return TypeConversionProvider.Create();
    }

    public virtual TypeProvider CreateTypeProvider ()
    {
      return new TypeProvider();
    }

    public virtual IPersistenceModelLoader CreatePersistenceModelLoader ()
    {
      return new PersistenceModelLoader (StoragePropertyDefinitionFactory, StorageProviderDefinition);
    }
  }
}