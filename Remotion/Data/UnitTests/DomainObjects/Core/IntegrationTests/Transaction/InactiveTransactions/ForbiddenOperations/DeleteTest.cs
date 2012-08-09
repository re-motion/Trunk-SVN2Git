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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions.ForbiddenOperations
{
  [TestFixture]
  public class DeleteTest : InactiveTransactionsTestBase
  {
    private ClassWithAllDataTypes _loadedClassWithAllDataTypes;
    private Order _orderNewInRootTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _loadedClassWithAllDataTypes = ActiveSubTransaction.Execute (() => ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1));
    }

    protected override void InitializeInactiveRootTransaction ()
    {
      base.InitializeInactiveRootTransaction ();
      _orderNewInRootTransaction = Order.NewObject();
    }

    [Test]
    public void DeleteInInactiveRootTransaction_IsForbidden_Loaded ()
    {
      CheckState (InactiveRootTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (InactiveMiddleTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (ActiveSubTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);

      CheckForbidden (() => InactiveRootTransaction.Execute (() => _loadedClassWithAllDataTypes.Delete()), "ObjectDeleting");

      CheckState (InactiveRootTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (InactiveMiddleTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (ActiveSubTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
    }

    [Test]
    public void DeleteInInactiveMiddleTransaction_IsForbidden_Loaded ()
    {
      CheckState (InactiveRootTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (InactiveMiddleTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (ActiveSubTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);

      CheckForbidden (() => InactiveMiddleTransaction.Execute (() => _loadedClassWithAllDataTypes.Delete ()), "ObjectDeleting");

      CheckState (InactiveRootTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (InactiveMiddleTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (ActiveSubTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
    }

    [Test]
    public void DeleteInInactiveRootTransaction_IsForbidden_New ()
    {
      CheckState (InactiveRootTransaction, _orderNewInRootTransaction, StateType.New);
      CheckState (InactiveMiddleTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);
      CheckState (ActiveSubTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);

      CheckForbidden (() => InactiveRootTransaction.Execute (() => _orderNewInRootTransaction.Delete ()), "ObjectDeleting");

      CheckState (InactiveRootTransaction, _orderNewInRootTransaction, StateType.New);
      CheckState (InactiveMiddleTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);
      CheckState (ActiveSubTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);
    }

    [Test]
    public void DeleteInInactiveMiddleTransaction_IsForbidden_New ()
    {
      CheckState (InactiveRootTransaction, _orderNewInRootTransaction, StateType.New);
      CheckState (InactiveMiddleTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);
      CheckState (ActiveSubTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);

      CheckForbidden (() => InactiveMiddleTransaction.Execute (() => _orderNewInRootTransaction.Delete ()), "ObjectDeleting");

      CheckState (InactiveRootTransaction, _orderNewInRootTransaction, StateType.New);
      CheckState (InactiveMiddleTransaction, _orderNewInRootTransaction, StateType.Unchanged);
      CheckState (ActiveSubTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);
    }
  }
}