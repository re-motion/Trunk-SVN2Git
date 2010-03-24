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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains
{
  [TestFixture]
  public class DomainObjectMixinHookTest : ClientTransactionBaseTest
  {
    [Test]
    public void OnDomainObjectLoaded ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (HookedDomainObjectMixin)).EnterScope())
      {
        HookedDomainObjectMixin mixinInstance = new HookedDomainObjectMixin ();

        Assert.IsFalse (mixinInstance.OnLoadedCalled);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
        Assert.IsFalse (mixinInstance.OnDomainObjectReferenceInitializedCalled);

        using (new MixedObjectInstantiationScope (mixinInstance))
        {
          Order.GetObject (DomainObjectIDs.Order1);
        }

        Assert.IsTrue (mixinInstance.OnLoadedCalled);
        Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, mixinInstance.OnLoadedLoadMode);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
        Assert.IsTrue (mixinInstance.OnDomainObjectReferenceInitializedCalled);
      }
    }

    [Test]
    public void OnDomainObjectLoadedAfterEnlist ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (HookedDomainObjectMixin)).EnterScope())
      {
        HookedDomainObjectMixin mixinInstance = new HookedDomainObjectMixin ();

        Assert.IsFalse (mixinInstance.OnLoadedCalled);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
        Assert.IsFalse (mixinInstance.OnDomainObjectReferenceInitializedCalled);

        Order order;
        using (new MixedObjectInstantiationScope (mixinInstance))
        {
          order = Order.GetObject (DomainObjectIDs.Order1);
        }

        mixinInstance.OnLoadedCalled = false;
        mixinInstance.OnLoadedCount = 0;

        ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
        newTransaction.EnlistDomainObject (order);

        Assert.IsFalse (mixinInstance.OnLoadedCalled);

        using (newTransaction.EnterDiscardingScope ())
        {
          ++order.OrderNumber;
        }

        Assert.IsTrue (mixinInstance.OnLoadedCalled);
        Assert.AreEqual (LoadMode.DataContainerLoadedOnly, mixinInstance.OnLoadedLoadMode);
        Assert.AreEqual (1, mixinInstance.OnLoadedCount);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
        Assert.IsTrue (mixinInstance.OnDomainObjectReferenceInitializedCalled);
      }
    }

    [Test]
    public void OnDomainObjectLoadedInSubTransaction ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (HookedDomainObjectMixin)).EnterScope())
      {
        HookedDomainObjectMixin mixinInstance = new HookedDomainObjectMixin ();

        Assert.IsFalse (mixinInstance.OnLoadedCalled);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
        Assert.IsFalse (mixinInstance.OnDomainObjectReferenceInitializedCalled);

        using (new MixedObjectInstantiationScope (mixinInstance))
        {
          using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
          {
            Order.GetObject (DomainObjectIDs.Order1);
          }
        }

        Assert.IsTrue (mixinInstance.OnLoadedCalled);
        Assert.AreEqual (2, mixinInstance.OnLoadedCount);
        Assert.AreEqual (LoadMode.DataContainerLoadedOnly, mixinInstance.OnLoadedLoadMode);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
        Assert.IsTrue (mixinInstance.OnDomainObjectReferenceInitializedCalled);
      }
    }

    [Test]
    public void OnDomainObjectLoadedInParentAndSubTransaction ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (HookedDomainObjectMixin)).EnterScope())
      {
        HookedDomainObjectMixin mixinInstance = new HookedDomainObjectMixin ();

        Assert.IsFalse (mixinInstance.OnLoadedCalled);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
        Assert.IsFalse (mixinInstance.OnDomainObjectReferenceInitializedCalled);

        using (new MixedObjectInstantiationScope (mixinInstance))
        {
          Order.GetObject (DomainObjectIDs.Order1);
          Assert.IsTrue (mixinInstance.OnLoadedCalled);
          Assert.AreEqual (1, mixinInstance.OnLoadedCount);
          Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, mixinInstance.OnLoadedLoadMode);
          Assert.IsTrue (mixinInstance.OnDomainObjectReferenceInitializedCalled);
          Assert.AreEqual (1, mixinInstance.OnDomainObjectReferenceInitializedCount);

          using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
          {
            Order.GetObject (DomainObjectIDs.Order1);
          }
        }

        Assert.AreEqual (2, mixinInstance.OnLoadedCount);
        Assert.AreEqual (LoadMode.DataContainerLoadedOnly, mixinInstance.OnLoadedLoadMode);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
        Assert.IsTrue (mixinInstance.OnDomainObjectReferenceInitializedCalled);
        Assert.AreEqual (1, mixinInstance.OnDomainObjectReferenceInitializedCount);
      }
    }

    [Test]
    public void OnDomainObjectCreated ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (HookedDomainObjectMixin)).EnterScope())
      {
        HookedDomainObjectMixin mixinInstance = new HookedDomainObjectMixin ();

        Assert.IsFalse (mixinInstance.OnLoadedCalled);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
        Assert.IsFalse (mixinInstance.OnDomainObjectReferenceInitializedCalled);

        using (new MixedObjectInstantiationScope (mixinInstance))
        {
          Order.NewObject ();
        }

        Assert.IsFalse (mixinInstance.OnLoadedCalled);
        Assert.IsTrue (mixinInstance.OnCreatedCalled);
        Assert.IsTrue (mixinInstance.OnDomainObjectReferenceInitializedCalled);
      }
    }
  }
}
