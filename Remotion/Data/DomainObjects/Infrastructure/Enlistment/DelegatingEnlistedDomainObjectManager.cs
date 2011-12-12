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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Enlistment
{
  /// <summary>
  /// Implements the <see cref="IEnlistedDomainObjectManager"/> by delegating to a given <see cref="TargetManager"/>. Every object registered with
  /// this manager actually is registered in the <see cref="TargetManager"/>.
  /// </summary>
  [Serializable]
  public class DelegatingEnlistedDomainObjectManager : IEnlistedDomainObjectManager
  {
    private readonly IEnlistedDomainObjectManager _targetManager;

    public DelegatingEnlistedDomainObjectManager (IEnlistedDomainObjectManager targetManager)
    {
      ArgumentUtility.CheckNotNull ("targetManager", targetManager);
      _targetManager = targetManager;
    }

    public IEnlistedDomainObjectManager TargetManager
    {
      get { return _targetManager; }
    }

    public int EnlistedDomainObjectCount
    {
      get { return _targetManager.EnlistedDomainObjectCount; }
    }

    public IEnumerable<DomainObject> GetEnlistedDomainObjects ()
    {
      return _targetManager.GetEnlistedDomainObjects ();
    }

    public DomainObject GetEnlistedDomainObject (ObjectID objectID)
    {
      return _targetManager.GetEnlistedDomainObject (objectID);
    }

    public bool EnlistDomainObject (DomainObject domainObject)
    {
      return _targetManager.EnlistDomainObject (domainObject);
    }

    public bool IsEnlisted (DomainObject domainObject)
    {
      return _targetManager.IsEnlisted (domainObject);
    }
  }
}