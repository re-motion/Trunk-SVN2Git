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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class EndPointExtensionsTest : ClientTransactionBaseTest
  {
    [Test]
    public void GetDomainObject ()
    {
      var endPointStub = MockRepository.GenerateStub<IEndPoint> ();
      endPointStub.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointStub.Stub (stub => stub.ClientTransaction).Return (ClientTransactionMock);

      var domainObject = endPointStub.GetDomainObject ();

      Assert.That (domainObject, Is.SameAs (Order.GetObject (DomainObjectIDs.Order1)));
    }

    [Test]
    public void GetDomainObject_Null ()
    {
      var endPointStub = MockRepository.GenerateStub<IEndPoint> ();
      endPointStub.Stub (stub => stub.ObjectID).Return (null);
      endPointStub.Stub (stub => stub.ClientTransaction).Return (ClientTransactionMock);

      var domainObject = endPointStub.GetDomainObject ();

      Assert.That (domainObject, Is.Null);
    }

    [Test]
    public void GetDomainObject_Deleted ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.Delete ();

      Assert.That (order1.State, Is.EqualTo (StateType.Deleted));

      var endPointStub = MockRepository.GenerateStub<IEndPoint> ();
      endPointStub.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointStub.Stub (stub => stub.ClientTransaction).Return (ClientTransactionMock);

      var domainObject = endPointStub.GetDomainObject ();

      Assert.That (domainObject, Is.SameAs (order1));
    }

    [Test]
    public void GetEndPointWithOppositeDefinition_Object ()
    {
      var id = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (id, null);

      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var oppositeEndPoint = endPoint.GetEndPointWithOppositeDefinition<ICollectionEndPoint> (customer);

      var oppositeID = new RelationEndPointID (customer.ID, endPoint.Definition.GetOppositeEndPointDefinition());
      Assert.That (oppositeEndPoint, Is.SameAs (ClientTransactionMock.DataManager.RelationEndPointMap[oppositeID]));
    }

    [Test]
    public void GetEndPointWithOppositeDefinition_Object_Null ()
    {
      var id = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (id, null);

      var oppositeEndPoint = endPoint.GetEndPointWithOppositeDefinition<ICollectionEndPoint> ((DomainObject) null);

      Assert.That (oppositeEndPoint, Is.InstanceOfType (typeof (NullCollectionEndPoint)));
      var expectedID = new RelationEndPointID (null, endPoint.Definition.GetOppositeEndPointDefinition ());
      Assert.That (oppositeEndPoint.ID, Is.EqualTo (expectedID));
    }

    [Test]
    public void GetEndPointWithOppositeDefinition_ID ()
    {
      var id = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (id, null);

      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var oppositeEndPoint = endPoint.GetEndPointWithOppositeDefinition<ICollectionEndPoint> (customer.ID);

      var oppositeID = new RelationEndPointID (customer.ID, endPoint.Definition.GetOppositeEndPointDefinition ());
      Assert.That (oppositeEndPoint, Is.SameAs (ClientTransactionMock.DataManager.RelationEndPointMap[oppositeID]));
    }

    [Test]
    public void GetEndPointWithOppositeDefinition_ID_Null ()
    {
      var id = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (id, null);

      var oppositeEndPoint = endPoint.GetEndPointWithOppositeDefinition<ICollectionEndPoint> ((ObjectID) null);

      Assert.That (oppositeEndPoint, Is.InstanceOfType (typeof (NullCollectionEndPoint)));
      var expectedID = new RelationEndPointID (null, endPoint.Definition.GetOppositeEndPointDefinition ());
      Assert.That (oppositeEndPoint.ID, Is.EqualTo (expectedID));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The opposite end point 'null/Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders' is of type "
        + "'Remotion.Data.DomainObjects.DataManagement.NullCollectionEndPoint', not of type 'Remotion.Data.DomainObjects.DataManagement.IObjectEndPoint'.")]
    public void GetEndPointWithOppositeDefinition_ID_InvalidType ()
    {
      var id = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (id, null);

      endPoint.GetEndPointWithOppositeDefinition<IObjectEndPoint> ((ObjectID) null);
    }
  }
}