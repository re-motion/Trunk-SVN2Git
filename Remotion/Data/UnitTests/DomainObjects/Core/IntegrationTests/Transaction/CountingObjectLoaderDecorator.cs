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
  }
}