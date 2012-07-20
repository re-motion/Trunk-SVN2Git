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
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.TestDomain
{
  [Serializable]
  public abstract class TestDomainBase : DomainObject
  {
    public static event EventHandler StaticLoadHandler;
    public event EventHandler ProtectedLoaded;
    public static event EventHandler StaticInitializationHandler;

    public static TestDomainBase GetObject (ObjectID id)
    {
      return GetObject<TestDomainBase> (id);
    }

    public static TestDomainBase GetObject (ObjectID id, bool includeDeleted)
    {
      return GetObject<TestDomainBase> (id, includeDeleted);
    }

    public static TestDomainBase TryGetObject (ObjectID id)
    {
      return TryGetObject<TestDomainBase> (id);
    }

    [NonSerialized]
    private IUnloadEventReceiver _unloadEventReceiver;
    [NonSerialized]
    private ILoadEventReceiver _loadEventReceiver;

    [NonSerialized]
    public bool CtorCalled;
    [NonSerialized]
    public ClientTransaction CtorTx;

    [NonSerialized]
    public bool OnReferenceInitializingCalledBeforeCtor;
    [NonSerialized]
    public bool OnReferenceInitializingCalled;
    [NonSerialized]
    public ClientTransaction OnReferenceInitializingTx;
    [NonSerialized]
    public ObjectID OnReferenceInitializingID;
    [NonSerialized]
    public ClientTransaction OnReferenceInitializingBindingTransaction;

    [NonSerialized]
    public bool OnLoadedCalled;
    [NonSerialized]
    public ClientTransaction OnLoadedTx;
    [NonSerialized]
    public LoadMode OnLoadedLoadMode;
    [NonSerialized]
    public int OnLoadedCallCount;

    [NonSerialized]
    public bool OnUnloadingCalled;
    [NonSerialized]
    public ClientTransaction OnUnloadingTx;
    [NonSerialized]
    public DateTime OnUnloadingDateTime;
    [NonSerialized]
    public StateType UnloadingState;

    [NonSerialized]
    public bool OnUnloadedCalled;
    [NonSerialized]
    public ClientTransaction OnUnloadedTx;
    [NonSerialized]
    public DateTime OnUnloadedDateTime;
    [NonSerialized]
    public StateType UnloadedState;

    protected TestDomainBase()
    {
      CtorCalled = true;
      CtorTx = ClientTransaction.Current;
      OnReferenceInitializingCalledBeforeCtor = OnReferenceInitializingCalled;
    }

    protected TestDomainBase (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }

    [StorageClassNone]
    public DataContainer InternalDataContainer
    {
      get 
      {
        var transaction = HasBindingTransaction ? GetBindingTransaction () : ClientTransaction.Current;
        return GetInternalDataContainerForTransaction (transaction);
      }
    }

    public DataContainer GetInternalDataContainerForTransaction (ClientTransaction transaction)
    {
      var dataManager = (DataManager) PrivateInvoke.GetNonPublicProperty (transaction, "DataManager");
      return dataManager.GetDataContainerWithLazyLoad (ID, true);
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
    public bool NeedsLoadModeDataContainerOnly
    {
      get { return (bool) PrivateInvoke.GetNonPublicField (this, typeof (DomainObject), "_needsLoadModeDataContainerOnly"); }
    }

    protected override void OnReferenceInitializing ()
    {
      base.OnReferenceInitializing ();

      OnReferenceInitializingCalled = true;
      OnReferenceInitializingTx = ClientTransaction.Current;
      OnReferenceInitializingID = ID;
      OnReferenceInitializingBindingTransaction = HasBindingTransaction ? GetBindingTransaction() : null;

      if (StaticInitializationHandler != null)
        StaticInitializationHandler (this, EventArgs.Empty);
    }

    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded (loadMode);
 
      OnLoadedCalled = true;
      OnLoadedTx = ClientTransaction.Current;
      OnLoadedLoadMode = loadMode;
      ++OnLoadedCallCount;
      
      if (ProtectedLoaded != null)
        ProtectedLoaded (this, EventArgs.Empty);
      if (StaticLoadHandler != null)
        StaticLoadHandler (this, EventArgs.Empty);

      if (_loadEventReceiver != null)
        _loadEventReceiver.OnLoaded (this);
    }

    protected override void OnUnloading ()
    {
      base.OnUnloading ();
      OnUnloadingCalled = true;
      OnUnloadingTx = ClientTransaction.Current;

      OnUnloadingDateTime = DateTime.Now;
      while (DateTime.Now == OnUnloadingDateTime)
      {
      }

      UnloadingState = State;
      if (_unloadEventReceiver != null)
        _unloadEventReceiver.OnUnloading (this);
    }

    protected override void OnUnloaded ()
    {
      base.OnUnloading ();
      OnUnloadedCalled = true;
      OnUnloadedTx = ClientTransaction.Current;

      OnUnloadedDateTime = DateTime.Now;
      while (DateTime.Now == OnUnloadedDateTime)
      {
      }

      UnloadedState = State;
      if (_unloadEventReceiver != null)
        _unloadEventReceiver.OnUnloaded (this);
    }

    public void SetUnloadEventReceiver (IUnloadEventReceiver unloadEventReceiver)
    {
      _unloadEventReceiver = unloadEventReceiver;
    }

    public void SetLoadEventReceiver (ILoadEventReceiver loadEventReceiver)
    {
      _loadEventReceiver = loadEventReceiver;
    }
  }
}