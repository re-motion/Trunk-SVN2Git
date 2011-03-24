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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class ObjectEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _endPointID;
    private IRelationEndPointLazyLoader _lazyLoaderStub;
    private IRelationEndPointProvider _endPointProviderStub;
    private TestableObjectEndPoint _endPoint;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      _lazyLoaderStub = MockRepository.GenerateStub<IRelationEndPointLazyLoader>();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      
      _endPoint = new TestableObjectEndPoint (ClientTransactionMock, _endPointID, _lazyLoaderStub, _endPointProviderStub, DomainObjectIDs.OrderTicket1);
    }

    [Test]
    public void GetOppositeObject ()
    {
      var oppositeObject = _endPoint.GetOppositeObject (true);
      Assert.That (OrderTicket.GetObject (_endPoint.OppositeObjectID), Is.SameAs (oppositeObject));
    }


    [Test]
    public void GetOppositeObject_Null ()
    {
      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, null);

      var oppositeObject = _endPoint.GetOppositeObject (false);
      Assert.That (oppositeObject, Is.Null);
    }

    [Test]
    public void GetOppositeObject_Deleted ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.Delete ();
      Assert.That (order1.State, Is.EqualTo (StateType.Deleted));

      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, order1.ID);

      Assert.That (_endPoint.GetOppositeObject (true), Is.SameAs (order1));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetOppositeObject_Deleted_NoDeleted ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.Delete ();
      Assert.That (order1.State, Is.EqualTo (StateType.Deleted));

      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, order1.ID);

      _endPoint.GetOppositeObject (false);
    }

    [Test]
    public void GetOppositeObject_Invalid_IncludeDeleted ()
    {
      var oppositeObject = Order.NewObject ();

      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Invalid));

      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, oppositeObject.ID);

      Assert.That (_endPoint.GetOppositeObject (true), Is.SameAs (oppositeObject));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void GetOppositeObject_Invalid_ExcludeDeleted ()
    {
      var oppositeObject = Order.NewObject ();

      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Invalid));

      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, oppositeObject.ID);
      
      _endPoint.GetOppositeObject (false);
    }

    [Test]
    public void GetOriginalOppositeObject ()
    {
      _endPoint.SetOppositeObjectID (DomainObjectIDs.OrderTicket4);

      Assert.That (_endPoint.GetOriginalOppositeObject (), Is.SameAs (OrderTicket.GetObject (DomainObjectIDs.OrderTicket1)));
    }

    [Test]
    public void GetOriginalOppositeObject_Null ()
    {
      var endPoint = new TestableObjectEndPoint (ClientTransactionMock, _endPointID, _lazyLoaderStub, _endPointProviderStub, null);

      Assert.That (endPoint.GetOriginalOppositeObject (), Is.Null);
    }

    [Test]
    public void GetOriginalOppositeObject_Deleted ()
    {
      var originalOppositeObject = (OrderTicket) _endPoint.GetOppositeObject (true);
      originalOppositeObject.Delete ();

      Assert.That (originalOppositeObject.State, Is.EqualTo (StateType.Deleted));
      Assert.That (_endPoint.GetOriginalOppositeObject (), Is.SameAs (originalOppositeObject));
    }

    [Test]
    public void IsDataComplete_True ()
    {
      Assert.That (_endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void EnsureDataComplete_DoesNothing ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      _endPoint.EnsureDataComplete ();
    }
    
    [Test]
    public void SetValueFrom_SetsOppositeObjectID_IfIDsDiffer ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, DomainObjectIDs.OrderTicket2);
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket2));

      _endPoint.ExpectSetOppositeObjectIDValueFrom();

      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket2));
      Assert.That (_endPoint.HasChanged, Is.True);
    }

    [Test]
    public void SetValueFrom_LeavesOppositeObjectID_IfIDsEqual ()
    {
      var sourceID = RelationEndPointID.Create (DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, DomainObjectIDs.OrderTicket1);
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));

      _endPoint.ExpectNotSetOppositeObjectIDValueFrom ();
      
      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (_endPoint.HasChanged, Is.False);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfEndPointWasTouched ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, _endPoint.OppositeObjectID);

      _endPoint.Touch ();
      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.HasChanged, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfSourceWasTouched ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, _endPoint.OppositeObjectID);

      source.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.HasChanged, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfDataWasChanged ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, DomainObjectIDs.Order2);
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));

      Assert.That (_endPoint.HasBeenTouched, Is.False);
      Assert.That (source.HasBeenTouched, Is.False);

      _endPoint.ExpectSetOppositeObjectIDValueFrom ();

      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.HasChanged, Is.True);
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_FalseIfNothingHappened ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, _endPoint.OppositeObjectID);

      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot set this end point's value from "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order'; "
        + "the end points do not have the same end point definition.\r\nParameter name: source")]
    public void SetValueFrom_InvalidDefinition ()
    {
      var otherID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      ObjectEndPoint source = RelationEndPointObjectMother.CreateRealObjectEndPoint (otherID);

      _endPoint.SetValueFrom (source);
    }
    
    [Test]
    public void CreateRemoveCommand ()
    {
      var relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> (_endPoint.OppositeObjectID);
      var result = (TestableObjectEndPoint.TestSetCommand) _endPoint.CreateRemoveCommand (relatedObject);

      Assert.That (result.NewRelatedObject, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "Cannot remove object 'OrderTicket|6768db2b-9c66-4e2f-bba2-89c56718ff2b|System.Guid' from object end point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' - it currently holds object "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'.")]
    public void CreateRemoveCommand_InvalidID ()
    {
      var orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket4);
      _endPoint.CreateRemoveCommand (orderTicket);
    }

    [Test]
    public void GetOppositeRelationEndPointID_NullEndPoint ()
    {
      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, null);

      var oppositeEndPointID = _endPoint.GetOppositeRelationEndPointID ();

      var expectedID = RelationEndPointID.Create (null, _endPoint.Definition.GetOppositeEndPointDefinition ());
      Assert.That (oppositeEndPointID, Is.EqualTo (expectedID));
    }

    [Test]
    public void GetOppositeRelationEndPointID_UnidirectionalEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      var endPoint = RelationEndPointObjectMother.CreateRealObjectEndPoint (endPointID);
      Assert.That (endPoint.Definition.GetOppositeEndPointDefinition ().IsAnonymous, Is.True);

      var oppositeEndPointID = endPoint.GetOppositeRelationEndPointID ();

      Assert.That (oppositeEndPointID, Is.Null);
    }

    [Test]
    public void GetOppositeRelationEndPointID_NonNullEndPoint ()
    {
      var oppositeEndPointID = _endPoint.GetOppositeRelationEndPointID ();

      var expectedID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      Assert.That (oppositeEndPointID, Is.EqualTo (expectedID));
    }

    [Test]
    public void GetOppositeRelationEndPointIDs_UnidirectionalEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      var endPoint = RelationEndPointObjectMother.CreateRealObjectEndPoint (endPointID);

      Assert.That (endPoint.Definition.GetOppositeEndPointDefinition ().IsAnonymous, Is.True);

      var oppositeEndPointIDs = endPoint.GetOppositeRelationEndPointIDs ().ToArray ();

      Assert.That (oppositeEndPointIDs, Is.Empty);
    }

    [Test]
    public void GetOppositeRelationEndPointIDs_BidirectionalEndPoint ()
    {
      var oppositeEndPointIDs = _endPoint.GetOppositeRelationEndPointIDs ().ToArray ();

      var expectedID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      Assert.That (oppositeEndPointIDs, Is.EqualTo (new[] { expectedID }));
    }
    
  }
}
