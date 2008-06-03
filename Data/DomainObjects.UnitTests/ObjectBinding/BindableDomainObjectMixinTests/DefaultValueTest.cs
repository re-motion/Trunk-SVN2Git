/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;
using Remotion.ObjectBinding;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class DefaultValueTest : ObjectBindingBaseTest
  {
    private Order _loadedOrder;
    private Order _newOrder;
    private IBusinessObject _loadedBusinessOrder;
    private IBusinessObject _newBusinessOrder;

    public override void SetUp ()
    {
      base.SetUp ();
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (BindableDomainObjectMixin)).EnterScope())
      {
        _loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
        _loadedBusinessOrder = (IBusinessObject) _loadedOrder;

        _newOrder = Order.NewObject ();
        _newBusinessOrder = (IBusinessObject) _newOrder;
      }
    }

    [Test]
    public void GetPropertyReturnsNonNullIfDefaultValue_OnExistingObject ()
    {
      Assert.IsNotNull (_loadedBusinessOrder.GetProperty ("OrderNumber"));
      Assert.AreEqual (_loadedOrder.OrderNumber, _loadedBusinessOrder.GetProperty ("OrderNumber"));
    }

    [Test]
    public void GetPropertyReturnsNullIfDefaultValue_OnNewObject ()
    {
      Assert.IsNull (_newBusinessOrder.GetProperty ("OrderNumber"));
    }

    [Test]
    public void GetPropertyReturnsNonNullIfDefaultListValue_OnNewObject ()
    {
      Assert.IsNotNull (_newBusinessOrder.GetProperty ("OrderItems"));
    }

    [Test]
    public void GetPropertyReturnsNonNullIfDefaultValue_OnNewObjectInSubtransaction ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.IsNotNull (_newBusinessOrder.GetProperty ("OrderNumber"));
        Assert.AreEqual (_newOrder.OrderNumber, _newBusinessOrder.GetProperty ("OrderNumber"));
      }
    }

    [Test]
    public void GetPropertyReturnsNonNullIfNonDefaultValue_OnExistingObject ()
    {
      _loadedOrder.OrderNumber = _loadedOrder.OrderNumber;
      Assert.IsNotNull (_loadedBusinessOrder.GetProperty ("OrderNumber"));
    }

    [Test]
    public void GetPropertyReturnsNonNullIfNonDefaultValue_OnNewObject ()
    {
      _newOrder.OrderNumber = _newOrder.OrderNumber;
      Assert.IsNotNull (_newBusinessOrder.GetProperty ("OrderNumber"));
    }

    [Test]
    public void GetPropertyDefaultForNonMappingProperties ()
    {
      IBusinessObject businessObject = (IBusinessObject) RepositoryAccessor.NewObject (typeof (BindableDomainObjectWithProperties)).With();
      Assert.IsNotNull (businessObject.GetProperty ("RequiredPropertyNotInMapping"));
      Assert.AreEqual (true, businessObject.GetProperty ("RequiredPropertyNotInMapping"));
    }
  }
}
