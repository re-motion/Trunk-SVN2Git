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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  public abstract class CollectionEndPointModificationTestBase : ClientTransactionBaseTest
  {
    private CollectionEndPoint _collectionEndPoint;
    private IDomainObjectCollectionData _collectionDataMock;
    private Customer _domainObject;
    private DomainObjectCollectionEventReceiver _collectionEventReceiver;
    private RelationEndPointID _relationEndPointID;

    public CollectionEndPoint CollectionEndPoint
    {
      get { return _collectionEndPoint; }
    }

    public IDomainObjectCollectionData CollectionDataMock
    {
      get { return _collectionDataMock; }
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

    public override void SetUp ()
    {
      base.SetUp ();

      _domainObject = Customer.GetObject (DomainObjectIDs.Customer1);

      _relationEndPointID = new RelationEndPointID (DomainObject.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      _collectionEndPoint = new CollectionEndPoint (ClientTransactionMock, RelationEndPointID, new DomainObject[0]);
      _collectionEventReceiver = new DomainObjectCollectionEventReceiver (_collectionEndPoint.OppositeDomainObjects);

      _collectionDataMock = new MockRepository ().StrictMock<IDomainObjectCollectionData> ();
      CollectionDataMock.Replay ();
    }
  }
}
