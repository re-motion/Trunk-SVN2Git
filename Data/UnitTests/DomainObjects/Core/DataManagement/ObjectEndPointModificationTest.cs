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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
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

      _endPointMock = _mockRepository.StrictMock<ObjectEndPoint> (ClientTransactionMock, _id, DomainObjectIDs.Employee3);
      _oldEndPointMock = _mockRepository.StrictMock<IEndPoint>();
      _newEndPointMock = _mockRepository.StrictMock<IEndPoint>();

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
      ObjectEndPointModification modificationMock = _mockRepository.StrictMock<ObjectEndPointModification> (_endPointMock, _oldEndPointMock, _newEndPointMock);

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
