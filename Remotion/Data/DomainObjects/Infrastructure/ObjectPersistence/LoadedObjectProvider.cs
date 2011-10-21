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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Returns <see cref="AlreadyExistingLoadedObject"/> and <see cref="NullLoadedObject"/> instances for objects known by a given 
  /// <see cref="IDataContainerProvider"/>.
  /// </summary>
  public class LoadedObjectProvider : ILoadedObjectProvider
  {
    private readonly IDataContainerProvider _dataContainerProvider;
    private readonly IInvalidDomainObjectManager _invalidDomainObjectManager;

    public LoadedObjectProvider (IDataContainerProvider dataContainerProvider, IInvalidDomainObjectManager invalidDomainObjectManager)
    {
      ArgumentUtility.CheckNotNull ("dataContainerProvider", dataContainerProvider);
      ArgumentUtility.CheckNotNull ("invalidDomainObjectManager", invalidDomainObjectManager);
      
      _dataContainerProvider = dataContainerProvider;
      _invalidDomainObjectManager = invalidDomainObjectManager;
    }

    public ILoadedObject GetLoadedObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      if (_invalidDomainObjectManager.IsInvalid (objectID))
        return new InvalidLoadedObject (_invalidDomainObjectManager.GetInvalidObjectReference (objectID));

      var dataContainer = _dataContainerProvider.GetDataContainerWithoutLoading (objectID);
      return dataContainer != null ? new AlreadyExistingLoadedObject (dataContainer) : null;
    }
  }
}