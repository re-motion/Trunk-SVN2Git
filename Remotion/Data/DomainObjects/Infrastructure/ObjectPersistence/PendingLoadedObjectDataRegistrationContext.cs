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
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Tracks objects that still need to be registered during an ongoing object loading operation. This is used by <see cref="FetchEnabledObjectLoader"/>
  /// in order to ensure that all fetch requests are performed before any OnLoaded events are raised.
  /// </summary>
  public class PendingLoadedObjectDataRegistrationContext
  {
    private readonly Dictionary<ObjectID, ILoadedObjectData> _objectsPendingRegistration = new Dictionary<ObjectID, ILoadedObjectData>();

    public ReadOnlyCollectionDecorator<ILoadedObjectData> ObjectsPendingRegistration
    {
      get { return _objectsPendingRegistration.Values.AsReadOnly(); }
    }

    public void AddObjectsPendingRegistration (IEnumerable<ILoadedObjectData> pendingObjects)
    {
      ArgumentUtility.CheckNotNull ("pendingObjects", pendingObjects);

      foreach (var pendingObject in pendingObjects)
      {
        if (!_objectsPendingRegistration.ContainsKey (pendingObject.ObjectID))
          _objectsPendingRegistration.Add (pendingObject.ObjectID, pendingObject);
      }
    }
  }
}