// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetSameModificationTest : ObjectEndPointSetModificationBaseTest
  {
    protected override DomainObject OldRelatedObject
    {
      get { return Employee.GetObject (DomainObjectIDs.Employee3); }
    }

    protected override DomainObject NewRelatedObject
    {
      get { return Employee.GetObject (DomainObjectIDs.Employee3); }
    }

    protected override RelationEndPointID GetRelationEndPointID ()
    {
      return new RelationEndPointID (DomainObjectIDs.Computer1, typeof (Computer).FullName + ".Employee");
    }

    protected override ObjectEndPointSetModificationBase CreateModification (ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return new ObjectEndPointSetSameModification (endPoint);
    }

    protected override ObjectEndPointSetModificationBase CreateModificationMock (MockRepository repository, ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return repository.StrictMock<ObjectEndPointSetSameModification> (endPoint);
    }

    public override void Begin ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      MockRepository.ReplayAll ();
      Modification.Begin ();
      MockRepository.VerifyAll ();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    public override void End ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      MockRepository.ReplayAll ();
      Modification.End ();
      MockRepository.VerifyAll ();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void CreateBidirectionalModification_SetSame_Unidirectional ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var unidirectionalEndPoint = (ObjectEndPoint)
                                   ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
                                       client, parentClientEndPointDefinition);
      Assert.That (unidirectionalEndPoint.OppositeEndPointDefinition.IsAnonymous, Is.True);

      var setSameModification = new ObjectEndPointSetSameModification (unidirectionalEndPoint);
      var bidirectionalModification = setSameModification.CreateBidirectionalModification ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (NonNotifyingBidirectionalRelationModification)));

      var steps = bidirectionalModification.GetModificationSteps ();
      Assert.That (steps.Count, Is.EqualTo (1));

      Assert.That (steps[0], Is.SameAs (setSameModification));
    }

    [Test]
    public void CreateBidirectionalModification_SetSame_Bidirectional ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderTicketEndPointDefinition = order.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      var bidirectionalEndPoint = (ObjectEndPoint)
                                  ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
                                      order, orderTicketEndPointDefinition);

      var oppositeEndPoint =
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
              bidirectionalEndPoint.GetOppositeObject (true),
              bidirectionalEndPoint.OppositeEndPointDefinition);
      var setSameModification = new ObjectEndPointSetSameModification (bidirectionalEndPoint);

      var bidirectionalModification = setSameModification.CreateBidirectionalModification ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (NonNotifyingBidirectionalRelationModification)));

      var steps = bidirectionalModification.GetModificationSteps ();
      Assert.That (steps.Count, Is.EqualTo (2));

      Assert.That (steps[0], Is.SameAs (setSameModification));

      Assert.That (steps[1], Is.InstanceOfType (typeof (RelationEndPointTouchModification)));
      Assert.That (steps[1].ModifiedEndPoint, Is.SameAs (oppositeEndPoint));
    }
  }
}