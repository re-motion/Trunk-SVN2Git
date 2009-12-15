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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  public abstract class ObjectEndPointSetModificationBaseTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private ObjectEndPoint _endPointMock;
    private ObjectEndPointSetModificationBase _modification;

    public override void SetUp ()
    {
      base.SetUp();
      _mockRepository = new MockRepository();

      _endPointMock = _mockRepository.StrictMock<ObjectEndPoint> (ClientTransactionMock, GetRelationEndPointID(), OldRelatedObject.ID);

      _endPointMock.Expect (mock => mock.IsNull).Return (false);
      _endPointMock.Replay();
      _modification = CreateModification(_endPointMock, NewRelatedObject);
      _endPointMock.BackToRecord();
    }

    protected abstract DomainObject OldRelatedObject { get; }
    protected abstract DomainObject NewRelatedObject { get; }

    protected abstract RelationEndPointID GetRelationEndPointID ();

    protected abstract ObjectEndPointSetModificationBase CreateModification (ObjectEndPoint endPoint, DomainObject newRelatedObject);
    protected abstract ObjectEndPointSetModificationBase CreateModificationMock (MockRepository repository, ObjectEndPoint endPoint, DomainObject newRelatedObject);

    public MockRepository MockRepository
    {
      get { return _mockRepository; }
    }

    public ObjectEndPoint EndPointMock
    {
      get { return _endPointMock; }
    }

    public ObjectEndPointSetModificationBase Modification
    {
      get { return _modification; }
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPointMock, _modification.ModifiedEndPoint);
      Assert.AreSame (OldRelatedObject, _modification.OldRelatedObject);
      Assert.AreSame (NewRelatedObject, _modification.NewRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModification is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullObjectEndPoint (GetRelationEndPointID().Definition);
      CreateModification (endPoint, NewRelatedObject);
    }

    [Test]
    public virtual void Begin ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      _endPointMock.Expect (mock => mock.ObjectID).Return (domainObject.ID).Repeat.Any ();

      _mockRepository.ReplayAll();

      _modification.Begin();

      _mockRepository.VerifyAll();

      Assert.IsTrue (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void Perform_InvokesPerformRelationChange ()
    {
      Assert.That (_endPointMock.OppositeObjectID, Is.EqualTo (OldRelatedObject.ID));
      _modification.Perform();
      Assert.That (_endPointMock.OppositeObjectID, Is.EqualTo (NewRelatedObject.ID));
    }

    [Test]
    public void Perform_TouchesEndPoint ()
    {
      var endPoint = new ObjectEndPoint (ClientTransactionMock, GetRelationEndPointID(), OldRelatedObject.ID);
      Assert.That (endPoint.HasBeenTouched, Is.False);

      var modification = CreateModification (endPoint, NewRelatedObject);

      modification.Perform();

      Assert.That (endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public virtual void End ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      _endPointMock.Expect (mock => mock.ObjectID).Return (domainObject.ID).Repeat.Any();

      _mockRepository.ReplayAll();

      _modification.End();

      _mockRepository.VerifyAll();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public virtual void NotifyClientTransactionOfBegin ()
    {
      _endPointMock.NotifyClientTransactionOfBeginRelationChange (OldRelatedObject, NewRelatedObject);

      _mockRepository.ReplayAll();

      _modification.NotifyClientTransactionOfBegin();

      _mockRepository.VerifyAll();
    }

    [Test]
    public virtual void NotifyClientTransactionOfEnd ()
    {
      _endPointMock.NotifyClientTransactionOfEndRelationChange();

      _mockRepository.ReplayAll();

      _modification.NotifyClientTransactionOfEnd();

      _mockRepository.VerifyAll();
    }
  }
}
