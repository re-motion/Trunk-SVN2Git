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
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.DataManagement
{
  [TestFixture]
  public class ObjectEndPointModificationTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private ObjectEndPoint _endPointMock;
    private IEndPoint _oldEndPointMock;
    private IEndPoint _newEndPointMock;
    private ObjectEndPointModification _modification;
    private RelationEndPointID _id;

    public override void SetUp ()
    {
      base.SetUp();
      _mockRepository = new MockRepository();
      _id = new RelationEndPointID (
          DomainObjectIDs.Computer1,
          MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Computer), "Employee"));

      _endPointMock = _mockRepository.CreateMock<ObjectEndPoint> (ClientTransactionMock, _id, DomainObjectIDs.Employee3);
      _oldEndPointMock = _mockRepository.CreateMock<IEndPoint>();
      _newEndPointMock = _mockRepository.CreateMock<IEndPoint>();

      _modification = new ObjectEndPointModification (_endPointMock, _oldEndPointMock, _newEndPointMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPointMock, _modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, _modification.OldEndPoint);
      Assert.AreSame (_newEndPointMock, _modification.NewEndPoint);
    }

    [Test]
    public void Initialization_FromEndPoint ()
    {
      RelationEndPoint endPoint = new ObjectEndPoint (ClientTransactionMock, _id, DomainObjectIDs.Employee3);
      RelationEndPointModification modification = endPoint.CreateModification (_oldEndPointMock, _newEndPointMock);
      Assert.IsInstanceOfType (typeof (ObjectEndPointModification), modification);
      Assert.AreSame (endPoint, modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, modification.OldEndPoint);
      Assert.AreSame (_newEndPointMock, modification.NewEndPoint);
    }

    [Test]
    public void BeginInvokesBeginRelationChange_OnDomainObject ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      DomainObject otherDomainObject = Order.GetObject (DomainObjectIDs.Order2);
      DomainObjectEventReceiver eventReceiver = new DomainObjectEventReceiver (domainObject);
      
      Expect.Call (_endPointMock.GetDomainObject()).Return (domainObject);
      Expect.Call (_oldEndPointMock.GetDomainObject ()).Return (otherDomainObject);
      Expect.Call (_newEndPointMock.GetDomainObject ()).Return (otherDomainObject);

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
    public void EndInvokesEndRelationChange_OnDomainObject ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      DomainObjectEventReceiver eventReceiver = new DomainObjectEventReceiver (domainObject);

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
      _endPointMock.NotifyClientTransactionOfBeginRelationChange (_oldEndPointMock, _newEndPointMock);

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
      ObjectEndPointModification modificationMock = _mockRepository.CreateMock<ObjectEndPointModification> (_endPointMock, _oldEndPointMock, _newEndPointMock);

      modificationMock.NotifyClientTransactionOfBegin ();
      modificationMock.Begin ();
      modificationMock.Perform ();
      modificationMock.NotifyClientTransactionOfEnd ();
      modificationMock.End();

      _mockRepository.ReplayAll();

      modificationMock.ExecuteAllSteps();

      _mockRepository.VerifyAll();
    }
  }
}
