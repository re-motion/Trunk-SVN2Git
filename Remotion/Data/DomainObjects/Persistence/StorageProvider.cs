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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  public abstract class StorageProvider : IDisposable
  {
    private StorageProviderDefinition _definition;
    private bool _disposed;
    private readonly IPersistenceTracer _persistenceTracer;

    protected StorageProvider (StorageProviderDefinition definition, IPersistenceTracer persistenceTracer)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      ArgumentUtility.CheckNotNull ("persistenceTracer", persistenceTracer);

      _definition = definition;
      _persistenceTracer = persistenceTracer;
    }

    ~StorageProvider ()
    {
      Dispose (false);
    }

    public void Dispose ()
    {
      Dispose (true);
      GC.SuppressFinalize (this);
    }

    protected virtual void Dispose (bool disposing)
    {
      if (disposing)
        _definition = null;

      _disposed = true;
    }

    public abstract DataContainer LoadDataContainer (ObjectID id);
    
    public abstract DataContainerCollection LoadDataContainers (IEnumerable<ObjectID> ids);

    public abstract DataContainerCollection LoadDataContainersByRelatedID (
        ClassDefinition classDefinition,
        string propertyName,
        ObjectID relatedID);

    public abstract void Save (DataContainerCollection dataContainers);
    public abstract void SetTimestamp (DataContainerCollection dataContainers);
    public abstract void BeginTransaction ();
    public abstract void Commit ();
    public abstract void Rollback ();
    public abstract ObjectID CreateNewObjectID (ClassDefinition classDefinition);
    public abstract DataContainer[] ExecuteCollectionQuery (IQuery query);
    public abstract object ExecuteScalarQuery (IQuery query);

    public string ID
    {
      get
      {
        CheckDisposed();
        return _definition.Name;
      }
    }

    protected virtual void CheckQuery (IQuery query, QueryType expectedQueryType, string argumentName)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("query", query);

      if (query.StorageProviderID != ID)
      {
        throw CreateArgumentException (
            "query",
            "The StorageProviderID '{0}' of the provided query '{1}' does not match with this StorageProvider's ID '{2}'.",
            query.StorageProviderID,
            query.ID,
            ID);
      }

      if (query.QueryType != expectedQueryType)
        throw CreateArgumentException (argumentName, "Expected query type is '{0}', but was '{1}'.", expectedQueryType, query.QueryType);
    }

    public StorageProviderDefinition Definition
    {
      get
      {
        CheckDisposed();
        return _definition;
      }
    }

    protected object GetFieldValue (DataContainer dataContainer, string propertyName, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckValidEnumValue ("valueAccess", valueAccess);

      return dataContainer.PropertyValues[propertyName].GetValueWithoutEvents (valueAccess);
    }

    protected bool IsDisposed
    {
      get { return _disposed; }
    }

    public TypeConversionProvider TypeConversionProvider
    {
      get { return _definition.TypeConversionProvider; }
    }

    public IPersistenceTracer PersistenceTracer
    {
      get { return _persistenceTracer; }
    }

    protected void CheckDisposed ()
    {
      if (_disposed)
        throw new ObjectDisposedException ("StorageProvider", "A disposed StorageProvider cannot be accessed.");
    }

    protected ArgumentException CreateArgumentException (string argumentName, string formatString, params object[] args)
    {
      return new ArgumentException (string.Format (formatString, args), argumentName);
    }
  }
}
