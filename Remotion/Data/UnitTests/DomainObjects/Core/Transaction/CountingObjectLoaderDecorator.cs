// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  public class CountingObjectLoaderDecorator : IObjectLoader
  {
    private readonly IObjectLoader _decorated;

    public CountingObjectLoaderDecorator (IObjectLoader decorated)
    {
      _decorated = decorated;
    }

    public int NumberOfCallsToLoadObject { get; set; }
    public int NumberOfCallsToLoadRelatedObject { get; set; }

    public DomainObject LoadObject (ObjectID id, IDataManager dataManager)
    {
      ++NumberOfCallsToLoadObject;
      return _decorated.LoadObject (id, dataManager);
    }

    public DomainObject[] LoadObjects (IList<ObjectID> idsToBeLoaded, bool throwOnNotFound, IDataManager dataManager)
    {
      return _decorated.LoadObjects (idsToBeLoaded, throwOnNotFound, dataManager);
    }

    public DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID, IDataManager dataManager)
    {
      ++NumberOfCallsToLoadRelatedObject;
      return _decorated.LoadRelatedObject (relationEndPointID, dataManager);
    }

    public DomainObject[] LoadRelatedObjects (RelationEndPointID relationEndPointID, IDataManager dataManager)
    {
      return _decorated.LoadRelatedObjects (relationEndPointID, dataManager);
    }

    public T[] LoadCollectionQueryResult<T> (IQuery query, IDataManager dataManager) where T: DomainObject
    {
      return _decorated.LoadCollectionQueryResult<T> (query, dataManager);
    }

    public ClientTransaction ClientTransaction
    {
      get { return _decorated.ClientTransaction; }
    }
  }
}