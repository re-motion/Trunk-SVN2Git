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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime
{
  /// <summary>
  /// Implements <see cref="IObjectInitializationContextProvider"/> by returning instances of <see cref="ObjectInitializationContext"/>.
  /// </summary>
  [Serializable]
  public class ObjectInitializationContextProvider : IObjectInitializationContextProvider
  {
    private readonly IEnlistedDomainObjectManager _enlistedDomainObjectManager;
    private readonly IDataManager _dataManager;

    public ObjectInitializationContextProvider (IEnlistedDomainObjectManager enlistedDomainObjectManager, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("enlistedDomainObjectManager", enlistedDomainObjectManager);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      _enlistedDomainObjectManager = enlistedDomainObjectManager;
      _dataManager = dataManager;
    }

    public IEnlistedDomainObjectManager EnlistedDomainObjectManager
    {
      get { return _enlistedDomainObjectManager; }
    }

    public IDataManager DataManager
    {
      get { return _dataManager; }
    }

    public IObjectInitializationContext CreateContext (ObjectID objectID, ClientTransaction bindingTransaction)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      return new ObjectInitializationContext (objectID, _enlistedDomainObjectManager, _dataManager, bindingTransaction);
    }
  }
}