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
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointChangeAgentModificationTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private CollectionEndPoint _endPointMock;
    private IEndPoint _oldEndPointStub;
    private IEndPoint _newEndPointStub;
    private Order _oldRelatedObject;
    private Order _newRelatedObject;
    private CollectionEndPointChangeAgentModification _modification;
    private RelationEndPointID _id;
    private CollectionEndPointChangeAgent _changeAgentMock;

    public override void SetUp ()
    {
      base.SetUp();
      _mockRepository = new MockRepository();
      _id = new RelationEndPointID (
          DomainObjectIDs.Order1,
          MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Order), "OrderItems"));

      _oldRelatedObject = Order.GetObject (DomainObjectIDs.Order1);
      _newRelatedObject = Order.GetObject (DomainObjectIDs.Order2);

      _oldEndPointStub = _mockRepository.Stub<IEndPoint> ();
      _newEndPointStub = _mockRepository.Stub<IEndPoint> ();
      _oldEndPointStub.Stub (stub => stub.GetDomainObject ()).Return (_oldRelatedObject);
      _newEndPointStub.Stub (stub => stub.GetDomainObject ()).Return (_newRelatedObject);
      _oldEndPointStub.Stub (stub => stub.IsNull).Return (false);
      _oldEndPointStub.Stub (stub => stub.ObjectID).Return (_oldRelatedObject.ID);
      _newEndPointStub.Stub (stub => stub.IsNull).Return (false);
      _oldEndPointStub.Replay ();
      _newEndPointStub.Replay ();

      _changeAgentMock = _mockRepository.StrictMock<CollectionEndPointChangeAgent> (new DomainObjectCollection (), _oldEndPointStub, _newEndPointStub,
                                                                                   CollectionEndPointChangeAgent.OperationType.Add, 0);

      _endPointMock = _mockRepository.StrictMock<CollectionEndPoint> (ClientTransactionMock, _id, new DomainObjectCollection (), ClientTransactionMock.DataManager.RelationEndPointMap);
      _endPointMock.Expect (mock => mock.IsNull).Return (false);
      _endPointMock.Replay ();
      _modification = new CollectionEndPointChangeAgentModification (_endPointMock, _changeAgentMock);
      _endPointMock.BackToRecord ();
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPointMock, _modification.ModifiedEndPoint);
      Assert.AreSame (_oldRelatedObject, _modification.OldRelatedObject);
      Assert.AreSame (_newRelatedObject, _modification.NewRelatedObject);
      Assert.AreSame (_changeAgentMock, _modification.ChangeAgent);
    }

    [Test]
    public void Initialization_FromEndPoint_Add ()
    {
      RelationEndPoint endPoint = new CollectionEndPoint (ClientTransactionMock, _id, new DomainObjectCollection (), ClientTransactionMock.DataManager.RelationEndPointMap);
      CollectionEndPointChangeAgentModification modification = (CollectionEndPointChangeAgentModification) endPoint.CreateModification (RelationEndPoint.CreateNullRelationEndPoint (_id.Definition), _newEndPointStub);
      Assert.AreSame (endPoint, modification.ModifiedEndPoint);
      Assert.AreSame (_newRelatedObject, modification.NewRelatedObject);
      Assert.That (modification.OldRelatedObject, Is.Null);
      Assert.AreEqual (CollectionEndPointChangeAgent.OperationType.Add, modification.ChangeAgent.Operation);
    }

    [Test]
    public void Initialization_FromEndPoint_Remove ()
    {
      RelationEndPoint endPoint = new CollectionEndPoint (ClientTransactionMock, _id, new DomainObjectCollection (), ClientTransactionMock.DataManager.RelationEndPointMap);
      CollectionEndPointChangeAgentModification modification = (CollectionEndPointChangeAgentModification) endPoint.CreateModification (_oldEndPointStub, RelationEndPoint.CreateNullRelationEndPoint (_id.Definition));
      Assert.AreSame (endPoint, modification.ModifiedEndPoint);
      Assert.AreSame (_oldRelatedObject, modification.OldRelatedObject);
      Assert.That (modification.NewRelatedObject, Is.Null);
      Assert.AreEqual (CollectionEndPointChangeAgent.OperationType.Remove, modification.ChangeAgent.Operation);
    }

    [Test]
    public void Initialization_FromEndPoint_Insert ()
    {
      CollectionEndPoint endPoint = new CollectionEndPoint (ClientTransactionMock, _id, new DomainObjectCollection (), ClientTransactionMock.DataManager.RelationEndPointMap);
      CollectionEndPointChangeAgentModification modification = (CollectionEndPointChangeAgentModification) endPoint.CreateInsertModification (_oldEndPointStub, _newEndPointStub, 3);
      Assert.AreSame (endPoint, modification.ModifiedEndPoint);
      Assert.AreSame (_oldRelatedObject, modification.OldRelatedObject);
      Assert.AreSame (_newRelatedObject, modification.NewRelatedObject);
      Assert.AreEqual (CollectionEndPointChangeAgent.OperationType.Insert, modification.ChangeAgent.Operation);
      Assert.AreEqual (3, PrivateInvoke.GetNonPublicField (modification.ChangeAgent, "_collectionIndex"));
    }

    [Test]
    public void Initialization_FromEndPoint_Replace ()
    {
      CollectionEndPoint endPoint = new CollectionEndPoint (ClientTransactionMock, _id, new DomainObjectCollection (), ClientTransactionMock.DataManager.RelationEndPointMap);
      CollectionEndPointChangeAgentModification modification = (CollectionEndPointChangeAgentModification) endPoint.CreateReplaceModification (_oldEndPointStub, _newEndPointStub);
      Assert.AreSame (endPoint, modification.ModifiedEndPoint);
      Assert.AreSame (_oldRelatedObject, modification.OldRelatedObject);
      Assert.AreSame (_newRelatedObject, modification.NewRelatedObject);
      Assert.AreEqual (CollectionEndPointChangeAgent.OperationType.Replace, modification.ChangeAgent.Operation);
    }

    [Test]
    public void BeginInvokesBeginRelationChange_OnChangeAgentAndDomainObject ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      DomainObjectEventReceiver eventReceiver = new DomainObjectEventReceiver (domainObject);

      using (_mockRepository.Ordered ())
      {
        _changeAgentMock.BeginRelationChange();
        Expect.Call (_endPointMock.GetDomainObject()).Return (domainObject);
      }

      _mockRepository.ReplayAll();

      _modification.Begin();

      _mockRepository.VerifyAll();

      Assert.IsTrue (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void PerformInvokesPerformRelationChange ()
    {
      _endPointMock.PerformRelationChange (_modification);

      _mockRepository.ReplayAll();

      _modification.Perform();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void EndInvokesEndRelationChange_OnChangeDelegateAndDomainObject ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      DomainObjectEventReceiver eventReceiver = new DomainObjectEventReceiver (domainObject);

      using (_mockRepository.Ordered ())
      {
        _changeAgentMock.EndRelationChange();
        Expect.Call (_endPointMock.GetDomainObject()).Return (domainObject);
      }

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

      _mockRepository.ReplayAll ();

      _modification.NotifyClientTransactionOfBegin ();

      _mockRepository.VerifyAll ();
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
      CollectionEndPointChangeAgentModification modificationMock = _mockRepository.StrictMock<CollectionEndPointChangeAgentModification> (_endPointMock, _changeAgentMock);

      modificationMock.NotifyClientTransactionOfBegin ();
      modificationMock.Begin ();
      modificationMock.Perform ();
      modificationMock.NotifyClientTransactionOfEnd ();
      modificationMock.End ();

      _mockRepository.ReplayAll ();

      modificationMock.ExecuteAllSteps ();

      _mockRepository.VerifyAll ();
    }
  }
}