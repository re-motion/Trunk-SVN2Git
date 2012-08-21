// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Implements <see cref="ILoadedObjectDataRegistrationListener"/> by distributing the events to <see cref="IClientTransactionEventSink"/> and 
  /// <see cref="ITransactionHierarchyManager"/> implementations.
  /// </summary>
  [Serializable]
  public class LoadedObjectDataRegistrationListener : ILoadedObjectDataRegistrationListener 
  {
    private readonly IClientTransactionEventSink _eventSink;
    private readonly ITransactionHierarchyManager _hierarchyManager;

    public LoadedObjectDataRegistrationListener (
        IClientTransactionEventSink eventSink, ITransactionHierarchyManager hierarchyManager)
    {
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);
      ArgumentUtility.CheckNotNull ("hierarchyManager", hierarchyManager);

      _eventSink = eventSink;
      _hierarchyManager = hierarchyManager;
    }

    public IClientTransactionEventSink EventSink
    {
      get { return _eventSink; }
    }

    public ITransactionHierarchyManager HierarchyManager
    {
      get { return _hierarchyManager; }
    }

    public void OnBeforeObjectRegistration (ReadOnlyCollection<ObjectID> loadedObjectIDs)
    {
      ArgumentUtility.CheckNotNull ("loadedObjectIDs", loadedObjectIDs);

      _hierarchyManager.OnBeforeObjectRegistration (loadedObjectIDs);
      _eventSink.RaiseEvent ((tx, l) => l.ObjectsLoading (tx, loadedObjectIDs));
    }

    public void OnAfterObjectRegistration (ReadOnlyCollection<ObjectID> objectIDsToBeLoaded, ReadOnlyCollection<DomainObject> actuallyLoadedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("objectIDsToBeLoaded", objectIDsToBeLoaded);
      ArgumentUtility.CheckNotNull ("actuallyLoadedDomainObjects", actuallyLoadedDomainObjects);

      try
      {
        if (actuallyLoadedDomainObjects.Count > 0)
          _eventSink.RaiseEvent ((tx, l) => l.ObjectsLoaded (tx, actuallyLoadedDomainObjects));
      }
      finally
      {
        _hierarchyManager.OnAfterObjectRegistration (objectIDsToBeLoaded);
      }
    }

    public void OnObjectsNotFound (ReadOnlyCollection<ObjectID> notFoundObjectIDs)
    {
      ArgumentUtility.CheckNotNull ("notFoundObjectIDs", notFoundObjectIDs);

      _eventSink.RaiseEvent ((tx, l) => l.ObjectsNotFound (tx, notFoundObjectIDs));
    }
  }
}