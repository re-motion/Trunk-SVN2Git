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
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
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

    public DomainObject LoadObject (ObjectID id, IDataContainerLifetimeManager lifetimeManager)
    {
      ++NumberOfCallsToLoadObject;
      return _decorated.LoadObject (id, lifetimeManager);
    }

    public DomainObject[] LoadObjects (IEnumerable<ObjectID> idsToBeLoaded, bool throwOnNotFound, IDataContainerLifetimeManager lifetimeManager)
    {
      return _decorated.LoadObjects (idsToBeLoaded, throwOnNotFound, lifetimeManager);
    }

    public DomainObject GetOrLoadRelatedObject (RelationEndPointID relationEndPointID, IDataContainerLifetimeManager lifetimeManager, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ++NumberOfCallsToLoadRelatedObject;
      return _decorated.GetOrLoadRelatedObject (relationEndPointID, lifetimeManager, alreadyLoadedObjectDataProvider);
    }

    public DomainObject[] GetOrLoadRelatedObjects (RelationEndPointID relationEndPointID, IDataContainerLifetimeManager lifetimeManager, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      return _decorated.GetOrLoadRelatedObjects (relationEndPointID, lifetimeManager, alreadyLoadedObjectDataProvider);
    }

    public T[] GetOrLoadCollectionQueryResult<T> (IQuery query, IDataContainerLifetimeManager lifetimeManager, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider, IDataManager dataManager) where T: DomainObject
    {
      return _decorated.GetOrLoadCollectionQueryResult<T> (query, lifetimeManager, alreadyLoadedObjectDataProvider, dataManager);
    }
  }
}