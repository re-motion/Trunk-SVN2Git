/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.DomainObjects.MixedDomains.SampleTypes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.MixedDomains
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

        using (new MixedObjectInstantiationScope (mixinInstance))
        {
          Order.GetObject (DomainObjectIDs.Order1);
        }

        Assert.IsTrue (mixinInstance.OnLoadedCalled);
        Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, mixinInstance.OnLoadedLoadMode);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
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

        Order order;
        using (new MixedObjectInstantiationScope (mixinInstance))
        {
          order = Order.GetObject (DomainObjectIDs.Order1);
        }

        mixinInstance.OnLoadedCalled = false;
        mixinInstance.OnLoadedCount = 0;

        ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();
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

        using (new MixedObjectInstantiationScope (mixinInstance))
        {
          Order.GetObject (DomainObjectIDs.Order1);
          Assert.IsTrue (mixinInstance.OnLoadedCalled);
          Assert.AreEqual (1, mixinInstance.OnLoadedCount);
          Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, mixinInstance.OnLoadedLoadMode);

          using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
          {
            Order.GetObject (DomainObjectIDs.Order1);
          }
        }

        Assert.AreEqual (2, mixinInstance.OnLoadedCount);
        Assert.AreEqual (LoadMode.DataContainerLoadedOnly, mixinInstance.OnLoadedLoadMode);
        Assert.IsFalse (mixinInstance.OnCreatedCalled);
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

        using (new MixedObjectInstantiationScope (mixinInstance))
        {
          Order.NewObject ();
        }

        Assert.IsFalse (mixinInstance.OnLoadedCalled);
        Assert.IsTrue (mixinInstance.OnCreatedCalled);
      }
    }
  }
}
