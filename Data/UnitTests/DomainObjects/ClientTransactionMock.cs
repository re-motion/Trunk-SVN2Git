// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.UnitTests.DomainObjects
{
  [Serializable]
  public class ClientTransactionMock : RootClientTransaction
  {
    // types

    // static members and constants

    // member fields

    private int _numberOfCallsToLoadDataContainer;
    private int _numberOfCallsToLoadRelatedObject;
    private IQueryManager _queryManager;

    // construction and disposing

    public ClientTransactionMock ()
    {
      Initialize();
      _queryManager = base.QueryManager;
    }

    // methods and properties

    private void Initialize ()
    {
      _numberOfCallsToLoadDataContainer = 0;
      _numberOfCallsToLoadRelatedObject = 0;
    }

    protected override DomainObject LoadObject (ObjectID id)
    {
      _numberOfCallsToLoadDataContainer++;
      return base.LoadObject (id);
    }

    protected override DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
    {
      _numberOfCallsToLoadRelatedObject++;
      return base.LoadRelatedObject (relationEndPointID);
    }

    public new DomainObject GetObject (ObjectID id)
    {
      return base.GetObject (id);
    }

    public new DomainObject GetObject (ObjectID id, bool includeDeleted)
    {
      return base.GetObject (id, includeDeleted);
    }

    protected override ObjectList<T> GetObjects<T> (ObjectID[] objectIDs, bool throwOnNotFound)
    {
      return MockableGetObjects<T> (objectIDs, throwOnNotFound);
    }

    public virtual ObjectList<T> MockableGetObjects<T> (ObjectID[] objectIDs, bool throwOnNotFound) where T: DomainObject
    {
      return base.GetObjects<T> (objectIDs, throwOnNotFound);
    }

    public new DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
    {
      return base.GetRelatedObject (relationEndPointID);
    }

    public new DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
    {
      return base.GetOriginalRelatedObjects (relationEndPointID);
    }

    public new DomainObjectCollection GetRelatedObjects (RelationEndPointID relationEndPointID)
    {
      return base.GetRelatedObjects (relationEndPointID);
    }

    public new void SetRelatedObject (RelationEndPointID relationEndPointID, DomainObject newRelatedObject)
    {
      base.SetRelatedObject (relationEndPointID, newRelatedObject);
    }

    public new void SetClientTransaction (DataContainer dataContainer)
    {
      base.SetClientTransaction (dataContainer);
    }

    public int NumberOfCallsToLoadDataContainer
    {
      get { return _numberOfCallsToLoadDataContainer; }
    }

    public int NumberOfCallsToLoadRelatedObject
    {
      get { return _numberOfCallsToLoadRelatedObject; }
    }

    public IEnumerable<DomainObject> GetEnlistedObjects<T>()
      where T : DomainObject
    {
      foreach (T t in EnlistedDomainObjects.Where (o => o is T))
        yield return t;
    }

    public new DataManager DataManager
    {
      get { return base.DataManager; }
    }

    public new bool IsReadOnly
    {
      get { return base.IsReadOnly; }
      set { base.IsReadOnly = value; }
    }

    public new void AddListener (IClientTransactionListener listener)
    {
      base.AddListener (listener);
    }

    public override IQueryManager QueryManager
    {
      get { return _queryManager; }
    }

    public void SetQueryManager (IQueryManager queryManager)
    {
      _queryManager = queryManager;
    }
  }
}
