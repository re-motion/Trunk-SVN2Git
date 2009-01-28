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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.BindableDomainObjectMixinTests
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
      IBusinessObject businessObject = (IBusinessObject) RepositoryAccessor.NewObject (typeof (BindableDomainObjectWithProperties), ParamList.Empty);
      Assert.IsNotNull (businessObject.GetProperty ("RequiredPropertyNotInMapping"));
      Assert.AreEqual (true, businessObject.GetProperty ("RequiredPropertyNotInMapping"));
    }
  }
}
