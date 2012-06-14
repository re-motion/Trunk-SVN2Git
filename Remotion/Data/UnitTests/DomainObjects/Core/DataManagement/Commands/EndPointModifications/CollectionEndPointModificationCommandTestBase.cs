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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  public abstract class CollectionEndPointModificationCommandTestBase : ClientTransactionBaseTest
  {
    private CollectionEndPoint _collectionEndPoint;
    private IDomainObjectCollectionData _collectionDataMock;
    private IRelationEndPointProvider _endPointProviderStub;
    private Customer _domainObject;
    private DomainObjectCollectionEventReceiver _collectionEventReceiver;
    private RelationEndPointID _relationEndPointID;
    private Order _order1;
    private Order _orderWithoutOrderItem;
    private ClientTransactionEventSinkWithMock _transactionEventSinkWithMock;

    public CollectionEndPoint CollectionEndPoint
    {
      get { return _collectionEndPoint; }
    }

    public IDomainObjectCollectionData CollectionDataMock
    {
      get { return _collectionDataMock; }
    }

    public IRelationEndPointProvider EndPointProviderStub
    {
      get { return _endPointProviderStub; }
    }

    public Customer DomainObject
    {
      get { return _domainObject; }
    }

    public DomainObjectCollectionEventReceiver CollectionEventReceiver
    {
      get { return _collectionEventReceiver; }
    }

    public RelationEndPointID RelationEndPointID
    {
      get { return _relationEndPointID; }
    }

    public ClientTransactionEventSinkWithMock TransactionEventSinkWithMock
    {
      get { return _transactionEventSinkWithMock; }
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _domainObject = Customer.GetObject (DomainObjectIDs.Customer1);

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

      _relationEndPointID = RelationEndPointID.Create(DomainObject.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      _collectionEndPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (_relationEndPointID, new[] { _order1, _orderWithoutOrderItem });
      _collectionEventReceiver = new DomainObjectCollectionEventReceiver (_collectionEndPoint.Collection);

      _collectionDataMock = new MockRepository ().StrictMock<IDomainObjectCollectionData> ();
      CollectionDataMock.Replay ();

      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();
      _transactionEventSinkWithMock = ClientTransactionEventSinkWithMock.CreateWithStrictMock(_collectionEndPoint.ClientTransaction);
    }
  }
}
