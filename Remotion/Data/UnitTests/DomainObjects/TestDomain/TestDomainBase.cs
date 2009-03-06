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
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.UnitTesting;

#pragma warning disable 0618

namespace Remotion.Data.UnitTests.DomainObjects.TestDomain
{
  [Serializable]
  public abstract class TestDomainBase : DomainObject
  {
    public static TestDomainBase GetObject (ObjectID id)
    {
      return GetObject<TestDomainBase> (id);
    }

    public static TestDomainBase GetObject (ObjectID id, bool includeDeleted)
    {
      return GetObject<TestDomainBase> (id, includeDeleted);
    }

    protected TestDomainBase()
    {
    }

    protected TestDomainBase (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }

    [StorageClassNone]
    public DataContainer InternalDataContainer
    {
      get { return GetInternalDataContainerForTransaction (GetBindingTransaction() ?? ClientTransaction.Current); }
    }

    public DataContainer GetInternalDataContainerForTransaction (ClientTransaction transaction)
    {
      return (DataContainer) PrivateInvoke.InvokeNonPublicMethod (transaction, typeof (ClientTransaction), "GetDataContainer", this);
    }

    public DomainObject GetRelatedObject (string propertyName)
    {
      return (DomainObject) Properties[propertyName].GetValueWithoutTypeCheck ();
    }

    public DomainObjectCollection GetRelatedObjects (string propertyName)
    {
      return (DomainObjectCollection) Properties[propertyName].GetValueWithoutTypeCheck ();
    }

    public DomainObject GetOriginalRelatedObject (string propertyName)
    {
      return (DomainObject) Properties[propertyName].GetOriginalValueWithoutTypeCheck ();
    }

    public DomainObjectCollection GetOriginalRelatedObjects (string propertyName)
    {
      return (DomainObjectCollection) Properties[propertyName].GetOriginalValueWithoutTypeCheck ();
    }

    public void SetRelatedObject (string propertyName, DomainObject newRelatedObject)
    {
      Properties[propertyName].SetValueWithoutTypeCheck (newRelatedObject);
    }

    public new void Delete ()
    {
      base.Delete();
    }

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }

    [StorageClassNone]
    public DomainObjectEventManager EventManager
    {
      get { return (DomainObjectEventManager) PrivateInvoke.GetNonPublicProperty (this, typeof (DomainObject), "EventManager"); }
    }
  }
}
#pragma warning restore 0618
