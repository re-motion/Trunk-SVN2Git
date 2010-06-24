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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Enlistment
{
  /// <summary>
  /// Implements the <see cref="IEnlistedDomainObjectManager"/> by delegating to a given <see cref="TargetTransaction"/>. Every object registered with
  /// this manager actually is registered in the <see cref="TargetTransaction"/>.
  /// </summary>
  [Serializable]
  public class DelegatingEnlistedDomainObjectManager : IEnlistedDomainObjectManager
  {
    private readonly ClientTransaction _targetTransaction;

    public DelegatingEnlistedDomainObjectManager (ClientTransaction targetTransaction)
    {
      ArgumentUtility.CheckNotNull ("targetTransaction", targetTransaction);
      _targetTransaction = targetTransaction;
    }

    public ClientTransaction TargetTransaction
    {
      get { return _targetTransaction; }
    }

    public int EnlistedDomainObjectCount
    {
      get { return _targetTransaction.EnlistedDomainObjectCount; }
    }

    public IEnumerable<DomainObject> GetEnlistedDomainObjects ()
    {
      return _targetTransaction.GetEnlistedDomainObjects ();
    }

    public DomainObject GetEnlistedDomainObject (ObjectID objectID)
    {
      return _targetTransaction.GetEnlistedDomainObject (objectID);
    }

    public bool EnlistDomainObject (DomainObject domainObject)
    {
      return _targetTransaction.EnlistDomainObject (domainObject);
    }

    public bool IsEnlisted (DomainObject domainObject)
    {
      return _targetTransaction.IsEnlisted (domainObject);
    }
  }
}