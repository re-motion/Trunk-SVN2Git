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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class RelationEndPointValueCheckerTest : ClientTransactionBaseTest
  {
    [Test]
    public void CheckClientTransaction_MatchingTransactions ()
    {
      var owningObject = DomainObjectMother.CreateObjectInTransaction<Order> (ClientTransactionMock);
      var relatedObject = DomainObjectMother.CreateObjectInTransaction<OrderItem> (ClientTransactionMock);

      var endPointStub = CreateRelationEndPointStub (ClientTransactionMock, owningObject);

      CallCheckClientTransaction (endPointStub, relatedObject);
    }

    [Test]
    public void CheckClientTransaction_NullObject ()
    {
      var owningObject = DomainObjectMother.CreateObjectInTransaction<Order> (ClientTransactionMock);

      var endPointStub = CreateRelationEndPointStub (ClientTransactionMock, owningObject);

      CallCheckClientTransaction (endPointStub, null);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage =
        @"Cannot xx DomainObject 'OrderItem\|.*@|System.Guid' from/to collection of property "
        + @"'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order\|.*\|System.Guid'. The objects do not belong "
        + @"to the same ClientTransaction.", MatchType = MessageMatch.Regex)]
    public void CheckClientTransaction_Differ_NoObjectInBindingTransaction ()
    {
      var owningObject = DomainObjectMother.CreateObjectInTransaction<Order> (ClientTransactionMock);
      var relatedObject = DomainObjectMother.CreateObjectInTransaction<OrderItem> (ClientTransaction.CreateRootTransaction ());

      var endPointStub = CreateRelationEndPointStub (ClientTransactionMock, owningObject);

      CallCheckClientTransaction (endPointStub, relatedObject);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage =
        @"Cannot xx DomainObject 'OrderItem\|.*@|System.Guid' from/to collection of property "
        + @"'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order\|.*\|System.Guid'. The objects do not belong "
        + @"to the same ClientTransaction. The OrderItem object is bound to a BindingClientTransaction.", MatchType = MessageMatch.Regex)]
    public void CheckClientTransaction_Differ_RelatedObjectInBindingTransaction ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();

      var owningObject = DomainObjectMother.CreateObjectInTransaction<Order> (ClientTransactionMock);
      var relatedObject = DomainObjectMother.CreateObjectInTransaction<OrderItem> (bindingTransaction);

      var endPointStub = CreateRelationEndPointStub (ClientTransactionMock, owningObject);

      CallCheckClientTransaction (endPointStub, relatedObject);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage =
        @"Cannot xx DomainObject 'OrderItem\|.*@|System.Guid' from/to collection of property "
        + @"'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order\|.*\|System.Guid'. The objects do not belong "
        + @"to the same ClientTransaction. The Order object owning the property is bound to a BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void CheckClientTransaction_Differ_OwningObjectInBindingTransaction ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();

      var owningObject = DomainObjectMother.CreateObjectInTransaction<Order> (bindingTransaction);
      var relatedObject = DomainObjectMother.CreateObjectInTransaction<OrderItem> (ClientTransaction.CreateRootTransaction ());

      var endPointStub = CreateRelationEndPointStub (bindingTransaction, owningObject);

      CallCheckClientTransaction (endPointStub, relatedObject);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage =
        @"Cannot xx DomainObject 'OrderItem\|.*@|System.Guid' from/to collection of property "
        + @"'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order\|.*\|System.Guid'. The objects do not belong "
        + @"to the same ClientTransaction. The OrderItem object is bound to a BindingClientTransaction. The Order object owning the property is "
        + @"also bound, but to a different BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void CheckClientTransaction_Differ_BothObjectsInBindingTransactions ()
    {
      var bindingTransaction1 = ClientTransaction.CreateBindingTransaction ();
      var bindingTransaction2 = ClientTransaction.CreateBindingTransaction ();
      var owningObject = DomainObjectMother.CreateObjectInTransaction<Order> (bindingTransaction1);
      var relatedObject = DomainObjectMother.CreateObjectInTransaction<OrderItem> (bindingTransaction2);

      var endPointStub = CreateRelationEndPointStub (bindingTransaction1, owningObject);

      CallCheckClientTransaction (endPointStub, relatedObject);
    }

    private void CallCheckClientTransaction (IRelationEndPoint endPoint, DomainObject relatedObject)
    {
      RelationEndPointValueChecker.CheckClientTransaction (
          endPoint, 
          relatedObject, 
          "Cannot xx DomainObject '{0}' from/to collection of property '{1}' of DomainObject '{2}'.");
    }

    private IRelationEndPoint CreateRelationEndPointStub (ClientTransaction transaction, Order owningObject)
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (owningObject.ID, "OrderItems");

      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.ClientTransaction).Return (transaction);
      endPointStub.Stub (stub => stub.ID).Return (id);
      endPointStub.Stub (stub => stub.Definition).Return (id.Definition);
      endPointStub.Stub (stub => stub.ObjectID).Return (owningObject.ID);
      endPointStub.Stub (stub => stub.GetDomainObjectReference()).Return (owningObject);
      
      return endPointStub;
    }
  }
}