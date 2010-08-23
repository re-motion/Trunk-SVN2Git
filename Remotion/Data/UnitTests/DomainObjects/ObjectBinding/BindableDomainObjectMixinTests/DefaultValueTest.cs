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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.ObjectBinding;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class DefaultValueTest : ObjectBindingBaseTest
  {
    private SampleBindableMixinDomainObject _loadedObject;
    private SampleBindableMixinDomainObject _newObject;
    private IBusinessObject _loadedBusinessObject;
    private IBusinessObject _newBusinessOrder;

    private ClientTransactionScope _subTxScope;

    public override void SetUp ()
    {
      base.SetUp ();

      var objectID = SampleBindableMixinDomainObject.NewObject ().ID;

      var subTx = ClientTransactionMock.CreateSubTransaction ();
      _subTxScope = subTx.EnterDiscardingScope ();

      _loadedObject = SampleBindableMixinDomainObject.GetObject (objectID);
      _loadedBusinessObject = (IBusinessObject) _loadedObject;

      _newObject = SampleBindableMixinDomainObject.NewObject ();
      _newBusinessOrder = (IBusinessObject) _newObject;
    }

    public override void TearDown ()
    {
      _subTxScope.Leave ();
      base.TearDown ();
    }

    [Test]
    public void GetPropertyReturnsNonNullIfDefaultValue_OnExistingObject ()
    {
      Assert.IsNotNull (_loadedBusinessObject.GetProperty ("Int32"));
      Assert.AreEqual (_loadedObject.Int32, _loadedBusinessObject.GetProperty ("Int32"));
    }

    [Test]
    public void GetPropertyReturnsNullIfDefaultValue_OnNewObject ()
    {
      Assert.IsNull (_newBusinessOrder.GetProperty ("Int32"));
    }

    [Test]
    public void GetPropertyReturnsNonNullIfDefaultListValue_OnNewObject ()
    {
      Assert.IsNotNull (_newBusinessOrder.GetProperty ("List"));
    }

    [Test]
    public void GetPropertyReturnsNonNullIfDefaultValue_OnNewObjectInSubtransaction ()
    {
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.IsNotNull (_newBusinessOrder.GetProperty ("Int32"));
        Assert.AreEqual (_newObject.Int32, _newBusinessOrder.GetProperty ("Int32"));
      }
    }

    [Test]
    public void GetPropertyReturnsNonNullIfNonDefaultValue_OnExistingObject ()
    {
      _loadedObject.Int32 = _loadedObject.Int32;
      Assert.IsNotNull (_loadedBusinessObject.GetProperty ("Int32"));
    }

    [Test]
    public void GetPropertyReturnsNonNullIfNonDefaultValue_OnNewObject ()
    {
      _newObject.Int32 = _newObject.Int32;
      Assert.IsNotNull (_newBusinessOrder.GetProperty ("Int32"));
    }

    [Test]
    public void GetPropertyDefaultForNonMappingProperties ()
    {
      var businessObject = (IBusinessObject)
          LifetimeService.NewObject (ClientTransaction.Current, typeof (BindableDomainObjectWithProperties), ParamList.Empty);
      Assert.IsNotNull (businessObject.GetProperty ("RequiredPropertyNotInMapping"));
      Assert.AreEqual (true, businessObject.GetProperty ("RequiredPropertyNotInMapping"));
    }
  }
}
