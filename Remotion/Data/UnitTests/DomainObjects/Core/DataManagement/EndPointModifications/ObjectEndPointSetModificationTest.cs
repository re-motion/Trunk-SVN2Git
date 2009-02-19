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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetModificationTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private ObjectEndPoint _endPointMock;
    private ObjectEndPointSetModification _modification;
    private RelationEndPointID _id;
    private Employee _oldRelatedObject;
    private Employee _newRelatedObject;

    public override void SetUp ()
    {
      base.SetUp();
      _mockRepository = new MockRepository();
      _id = new RelationEndPointID (
          DomainObjectIDs.Computer1,
          MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Computer), "Employee"));

      _oldRelatedObject = Employee.GetObject (DomainObjectIDs.Employee3);
      _newRelatedObject = Employee.GetObject (DomainObjectIDs.Employee4);

      _endPointMock = _mockRepository.StrictMock<ObjectEndPoint> (ClientTransactionMock, _id, _oldRelatedObject.ID);

      _endPointMock.Expect (mock => mock.IsNull).Return (false);
      _endPointMock.Replay ();
      _modification = new ObjectEndPointSetModification (_endPointMock, _newRelatedObject);
      _endPointMock.BackToRecord();
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPointMock, _modification.ModifiedEndPoint);
      Assert.AreSame (_oldRelatedObject, _modification.OldRelatedObject);
      Assert.AreSame (_newRelatedObject, _modification.NewRelatedObject);
    }

    [Test]
    public void Initialization_FromEndPoint ()
    {
      var endPoint = new ObjectEndPoint (ClientTransactionMock, _id, DomainObjectIDs.Employee3);
      RelationEndPointModification modification = endPoint.CreateSetModification (_newRelatedObject);
      Assert.IsInstanceOfType (typeof (ObjectEndPointSetModification), modification);
      Assert.AreSame (endPoint, modification.ModifiedEndPoint);
      Assert.AreSame (_oldRelatedObject, modification.OldRelatedObject);
      Assert.AreSame (_newRelatedObject, modification.NewRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModification is needed.\r\n" 
        + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullObjectEndPoint (_id.Definition);
      new ObjectEndPointSetModification (endPoint, _newRelatedObject);
    }

    [Test]
    public void BeginInvokesBeginRelationChange_OnDomainObject ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      var eventReceiver = new DomainObjectEventReceiver (domainObject);
      
      Expect.Call (_endPointMock.GetDomainObject()).Return (domainObject);

      _mockRepository.ReplayAll();

      _modification.Begin();

      _mockRepository.VerifyAll();

      Assert.IsTrue (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void Perform_InvokesPerformRelationChange ()
    {
      Assert.That (_endPointMock.OppositeObjectID, Is.EqualTo (_oldRelatedObject.ID));
      _modification.Perform ();
      Assert.That (_endPointMock.OppositeObjectID, Is.EqualTo (_newRelatedObject.ID));
    }

    [Test]
    public void Perform_TouchesEndPoint ()
    {
      var endPoint = new ObjectEndPoint(ClientTransactionMock, _id, DomainObjectIDs.Employee3);
      Assert.That (endPoint.HasBeenTouched, Is.False);

      var modification = new ObjectEndPointSetModification (endPoint, _newRelatedObject);

      modification.Perform ();

      Assert.That (endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void EndInvokesEndRelationChange_OnDomainObject ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      Expect.Call (_endPointMock.GetDomainObject ()).Return (domainObject);

      _mockRepository.ReplayAll();

      _modification.End();

      _mockRepository.VerifyAll();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      _endPointMock.NotifyClientTransactionOfBeginRelationChange (_oldRelatedObject, _newRelatedObject);

      _mockRepository.ReplayAll();

      _modification.NotifyClientTransactionOfBegin();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      _endPointMock.NotifyClientTransactionOfEndRelationChange ();

      _mockRepository.ReplayAll ();

      _modification.NotifyClientTransactionOfEnd ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExecuteAllSteps ()
    {
      _endPointMock.Expect (mock => mock.IsNull).Return (false);
      _endPointMock.Replay ();

      var modificationMock = _mockRepository.StrictMock<ObjectEndPointSetModification> (_endPointMock, _newRelatedObject);

      modificationMock.NotifyClientTransactionOfBegin ();
      modificationMock.Begin ();
      modificationMock.Perform ();
      modificationMock.NotifyClientTransactionOfEnd ();
      modificationMock.End();

      _mockRepository.ReplayAll();

      modificationMock.ExecuteAllSteps();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CreateBidirectionalModification_SetSame_Unidirectional ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var unidirectionalEndPoint = (ObjectEndPoint) 
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (client, parentClientEndPointDefinition);
      Assert.That (unidirectionalEndPoint.OppositeEndPointDefinition.IsAnonymous, Is.True);

      var setSameModification = new ObjectEndPointSetModification (unidirectionalEndPoint, unidirectionalEndPoint.GetOppositeObject (true));
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
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (order, orderTicketEndPointDefinition);

      var oppositeEndPoint = 
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (bidirectionalEndPoint.GetOppositeObject(true),
          bidirectionalEndPoint.OppositeEndPointDefinition);
      var setSameModification = new ObjectEndPointSetModification (bidirectionalEndPoint, bidirectionalEndPoint.GetOppositeObject (true));

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