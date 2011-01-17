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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public class UnitTestStorageObjectFactoryStub : IStorageObjectFactory
  {
    public UnitTestStorageObjectFactoryStub ()
    {
    }

    public StorageProvider CreateStorageProvider (IPersistenceListener persistenceListener, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("persistenceListener", persistenceListener);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var providerDefiniton = ArgumentUtility.CheckNotNullAndType<UnitTestStorageProviderStubDefinition> (
          "storageProviderDefinition", storageProviderDefinition);

      return new UnitTestStorageProviderStub (providerDefiniton, persistenceListener);
    }

    public TypeConversionProvider CreateTypeConversionProvider ()
    {
      return TypeConversionProvider.Create();
    }

    public TypeProvider CreateTypeProvider ()
    {
      return new TypeProvider();
    }

    public IPersistenceModelLoader CreatePersistenceModelLoader (
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageNameCalculator = new ReflectionBasedStorageNameProvider();
      var columnDefinitionFactory = new ColumnDefinitionFactory (
          new SqlStorageTypeCalculator (storageProviderDefinitionFinder), storageNameCalculator, storageProviderDefinitionFinder);
      var columnDefinitionResolver = new ColumnDefinitionResolver();
      var foreignKeyConstraintDefinitionFactory = new ForeignKeyConstraintDefinitionFactory (
          storageNameCalculator, columnDefinitionResolver, columnDefinitionFactory, storageProviderDefinitionFinder);
      var entityDefinitionFactory = new EntityDefinitionFactory (
          columnDefinitionFactory,
          foreignKeyConstraintDefinitionFactory,
          columnDefinitionResolver,
          storageNameCalculator,
          storageProviderDefinition);

      return new RdbmsPersistenceModelLoader (entityDefinitionFactory, columnDefinitionFactory, storageProviderDefinition);
    }
  }
}