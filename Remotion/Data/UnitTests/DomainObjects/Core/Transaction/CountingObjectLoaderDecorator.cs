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

    public DomainObject LoadObject (ObjectID id)
    {
      ++NumberOfCallsToLoadObject;
      return _decorated.LoadObject (id);
    }

    public DomainObject[] LoadObjects (IList<ObjectID> idsToBeLoaded, bool throwOnNotFound)
    {
      return _decorated.LoadObjects (idsToBeLoaded, throwOnNotFound);
    }

    public DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
    {
      ++NumberOfCallsToLoadRelatedObject;
      return _decorated.LoadRelatedObject (relationEndPointID);
    }

    public DomainObject[] LoadRelatedObjects (RelationEndPointID relationEndPointID)
    {
      return _decorated.LoadRelatedObjects (relationEndPointID);
    }

    public T[] LoadCollectionQueryResult<T> (IQuery query) where T: DomainObject
    {
      return _decorated.LoadCollectionQueryResult<T> (query);
    }

    public ClientTransaction ClientTransaction
    {
      get { return _decorated.ClientTransaction; }
    }
  }
}