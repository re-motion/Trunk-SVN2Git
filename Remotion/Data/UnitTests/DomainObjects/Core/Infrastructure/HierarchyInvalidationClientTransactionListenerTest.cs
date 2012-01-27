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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class HierarchyInvalidationClientTransactionListenerTest : StandardMappingTest
  {
    private ClientTransaction _rootTransaction;

    private HierarchyInvalidationClientTransactionListener _listener;

    public override void SetUp ()
    {
      base.SetUp();

      _rootTransaction = ClientTransaction.CreateRootTransaction ();

      _listener = new HierarchyInvalidationClientTransactionListener ();
    }

    [Test]
    public void DataContainerMapRegistering_MarksNewObjectsInvalid ()
    {
      var middleTransaction = _rootTransaction.CreateSubTransaction ();
      var subTransaction = middleTransaction.CreateSubTransaction ();

      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var domainObject = LifetimeService.GetObjectReference (subTransaction, dataContainer.ID);
      dataContainer.SetDomainObject (domainObject);

      _listener.DataContainerMapRegistering (subTransaction, dataContainer);

      Assert.That (_rootTransaction.IsInvalid (DomainObjectIDs.Order1), Is.True);
      Assert.That (ClientTransactionTestHelper.CallGetInvalidObjectReference (_rootTransaction, DomainObjectIDs.Order1), Is.SameAs (domainObject));
      
      Assert.That (middleTransaction.IsInvalid (DomainObjectIDs.Order1), Is.True);
      Assert.That (ClientTransactionTestHelper.CallGetInvalidObjectReference (middleTransaction, DomainObjectIDs.Order1), Is.SameAs (domainObject));

      Assert.That (subTransaction.IsInvalid (DomainObjectIDs.Order1), Is.False);
    }

    [Test]
    public void DataContainerMapRegistering_NewObjects_NoParentTransaction ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var domainObject = LifetimeService.GetObjectReference (_rootTransaction, dataContainer.ID);
      dataContainer.SetDomainObject (domainObject);

      _listener.DataContainerMapRegistering (_rootTransaction, dataContainer);

      Assert.That (_rootTransaction.IsInvalid (DomainObjectIDs.Order1), Is.False);
    }

    [Test]
    public void DataContainerMapRegistering_IgnoresNonNewObjects ()
    {
      var middleTransaction = _rootTransaction.CreateSubTransaction ();
      var subTransaction = middleTransaction.CreateSubTransaction ();

      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      var domainObject = LifetimeService.GetObjectReference (subTransaction, dataContainer.ID);
      dataContainer.SetDomainObject (domainObject);

      _listener.DataContainerMapRegistering (subTransaction, dataContainer);

      Assert.That (_rootTransaction.IsInvalid (DomainObjectIDs.Order1), Is.False);
      Assert.That (middleTransaction.IsInvalid (DomainObjectIDs.Order1), Is.False);
      Assert.That (subTransaction.IsInvalid (DomainObjectIDs.Order1), Is.False);
    }

    [Test]
    public void DataContainerMapUnregistering_MarksNewObjectsInvalidInSubtransactions ()
    {
      var middleTopTransaction = _rootTransaction.CreateSubTransaction ();

      var domainObject = middleTopTransaction.Execute (() => Order.NewObject());
      var dataContainer = DataManagementService.GetDataManager (middleTopTransaction).GetDataContainerWithoutLoading (domainObject.ID);
      Assert.That (dataContainer, Is.Not.Null);

      var middleBottomTransaction = middleTopTransaction.CreateSubTransaction ();
      var subTransaction = middleBottomTransaction.CreateSubTransaction ();

      _listener.DataContainerMapUnregistering (middleTopTransaction, dataContainer);

      Assert.That (middleBottomTransaction.IsInvalid (domainObject.ID), Is.True);
      Assert.That (ClientTransactionTestHelper.CallGetInvalidObjectReference (middleBottomTransaction, domainObject.ID), Is.SameAs (domainObject));
      Assert.That (subTransaction.IsInvalid (domainObject.ID), Is.True);
      Assert.That (ClientTransactionTestHelper.CallGetInvalidObjectReference (subTransaction, domainObject.ID), Is.SameAs (domainObject));
    }

    [Test]
    public void DataContainerMapUnregistering_IgnoresNonNewObjects ()
    {
      var middleTopTransaction = _rootTransaction.CreateSubTransaction ();

      var domainObject = middleTopTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      var dataContainer = DataManagementService.GetDataManager (middleTopTransaction).GetDataContainerWithoutLoading (domainObject.ID);
      Assert.That (dataContainer, Is.Not.Null);

      var middleBottomTransaction = middleTopTransaction.CreateSubTransaction ();
      var subTransaction = middleBottomTransaction.CreateSubTransaction ();

      _listener.DataContainerMapUnregistering (middleTopTransaction, dataContainer);

      Assert.That (middleBottomTransaction.IsInvalid (DomainObjectIDs.Order1), Is.False);
      Assert.That (subTransaction.IsInvalid (DomainObjectIDs.Order1), Is.False);
    }

    [Test]
    public void Serialization ()
    {
      Serializer.SerializeAndDeserialize (_listener);
    }
  }
}