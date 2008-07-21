/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DomainObjects.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.DataManagement
{
  [TestFixture]
  public class CollectionEndPointModificationTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private CollectionEndPoint _endPointMock;
    private IEndPoint _oldEndPointMock;
    private IEndPoint _newEndPointMock;
    private CollectionEndPointModification _modification;
    private RelationEndPointID _id;
    private CollectionEndPointChangeAgent _changeAgentMock;

    public override void SetUp ()
    {
      base.SetUp();
      _mockRepository = new MockRepository();
      _id = new RelationEndPointID (
          DomainObjectIDs.Order1,
          MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Order), "OrderItems"));

      _endPointMock = _mockRepository.CreateMock<CollectionEndPoint> (ClientTransactionMock, _id, new DomainObjectCollection());
      _oldEndPointMock = _mockRepository.Stub<IEndPoint>();
      _newEndPointMock = _mockRepository.Stub<IEndPoint>();
      _changeAgentMock = _mockRepository.CreateMock<CollectionEndPointChangeAgent>(new DomainObjectCollection(), _oldEndPointMock, _newEndPointMock,
          CollectionEndPointChangeAgent.OperationType.Add, 0);

      _modification = new CollectionEndPointModification (_endPointMock, _changeAgentMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPointMock, _modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, _modification.OldEndPoint);
      Assert.AreSame (_newEndPointMock, _modification.NewEndPoint);
      Assert.AreSame (_changeAgentMock, _modification.ChangeAgent);
    }

    [Test]
    public void Initialization_FromEndPoint_Add ()
    {
      RelationEndPoint endPoint = new CollectionEndPoint (ClientTransactionMock, _id, new DomainObjectCollection());
      CollectionEndPointModification modification = (CollectionEndPointModification) endPoint.CreateModification (RelationEndPoint.CreateNullRelationEndPoint (_id.Definition), _newEndPointMock);
      Assert.AreSame (endPoint, modification.AffectedEndPoint);
      Assert.AreSame (_newEndPointMock, modification.NewEndPoint);
      Assert.AreSame (modification.NewEndPoint, modification.ChangeAgent.NewEndPoint);
      Assert.IsTrue (modification.OldEndPoint.IsNull);
      Assert.AreSame (modification.OldEndPoint, modification.ChangeAgent.OldEndPoint);
      Assert.AreEqual (CollectionEndPointChangeAgent.OperationType.Add, modification.ChangeAgent.Operation);
    }

    [Test]
    public void Initialization_FromEndPoint_Remove ()
    {
      RelationEndPoint endPoint = new CollectionEndPoint (ClientTransactionMock, _id, new DomainObjectCollection ());
      CollectionEndPointModification modification = (CollectionEndPointModification) endPoint.CreateModification (_oldEndPointMock, RelationEndPoint.CreateNullRelationEndPoint (_id.Definition));
      Assert.AreSame (endPoint, modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, modification.OldEndPoint);
      Assert.AreSame (modification.OldEndPoint, modification.ChangeAgent.OldEndPoint);
      Assert.IsTrue (modification.NewEndPoint.IsNull);
      Assert.AreSame (modification.NewEndPoint, modification.ChangeAgent.NewEndPoint);
      Assert.AreEqual (CollectionEndPointChangeAgent.OperationType.Remove, modification.ChangeAgent.Operation);
    }

    [Test]
    public void Initialization_FromEndPoint_Insert ()
    {
      CollectionEndPoint endPoint = new CollectionEndPoint (ClientTransactionMock, _id, new DomainObjectCollection ());
      CollectionEndPointModification modification = (CollectionEndPointModification) endPoint.CreateInsertModification (_oldEndPointMock, _newEndPointMock, 3);
      Assert.AreSame (endPoint, modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, modification.OldEndPoint);
      Assert.AreSame (modification.OldEndPoint, modification.ChangeAgent.OldEndPoint);
      Assert.AreSame (_newEndPointMock, modification.NewEndPoint);
      Assert.AreSame (modification.NewEndPoint, modification.ChangeAgent.NewEndPoint);
      Assert.AreEqual (CollectionEndPointChangeAgent.OperationType.Insert, modification.ChangeAgent.Operation);
      Assert.AreEqual (3, PrivateInvoke.GetNonPublicField (modification.ChangeAgent, "_collectionIndex"));
    }

    [Test]
    public void Initialization_FromEndPoint_Replace ()
    {
      CollectionEndPoint endPoint = new CollectionEndPoint (ClientTransactionMock, _id, new DomainObjectCollection ());
      CollectionEndPointModification modification = (CollectionEndPointModification) endPoint.CreateReplaceModification (_oldEndPointMock, _newEndPointMock);
      Assert.AreSame (endPoint, modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, modification.OldEndPoint);
      Assert.AreSame (modification.OldEndPoint, modification.ChangeAgent.OldEndPoint);
      Assert.AreSame (_newEndPointMock, modification.NewEndPoint);
      Assert.AreSame (modification.NewEndPoint, modification.ChangeAgent.NewEndPoint);
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
      _endPointMock.NotifyClientTransactionOfBeginRelationChange (_oldEndPointMock, _newEndPointMock);

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
      CollectionEndPointModification modificationMock = _mockRepository.CreateMock<CollectionEndPointModification> (_endPointMock, _changeAgentMock);

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
