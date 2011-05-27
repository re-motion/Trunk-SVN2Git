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
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects
{
  [Serializable]
  public class ClientTransactionMock : ClientTransaction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ClientTransactionMock () : this (new RootClientTransactionComponentFactory())
    {
    }

    protected ClientTransactionMock (IClientTransactionComponentFactory componentFactory) : base (componentFactory)
    {
    }

    public IClientTransactionListener TransactionEventSink
    {
      get { return (IClientTransactionListener) PrivateInvoke.GetNonPublicProperty (this, typeof (ClientTransaction), "TransactionEventSink"); }
    }

    public new DomainObject GetObject (ObjectID id, bool includeDeleted)
    {
      return base.GetObject (id, includeDeleted);
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

    public IEnumerable<DomainObject> GetEnlistedObjects<T>()
      where T : DomainObject
    {
      return GetEnlistedDomainObjects().OfType<T>().Cast<DomainObject>();
    }

    public new DataManager DataManager
    {
      get { return (DataManager) base.DataManager; }
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

    public new void Delete (DomainObject domainObject)
    {
      base.Delete (domainObject);
    }
  }
}
