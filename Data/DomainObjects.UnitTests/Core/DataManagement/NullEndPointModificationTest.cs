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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DataManagement
{
  [TestFixture]
  public class NullEndPointModificationTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private ObjectEndPoint _endPointMock;
    private IEndPoint _oldEndPointMock;
    private IEndPoint _newEndPointMock;
    private NullEndPointModification _modification;
    private RelationEndPointID _id;

    public override void SetUp ()
    {
      base.SetUp ();
      _mockRepository = new MockRepository ();
      _id = new RelationEndPointID (
          DomainObjectIDs.Computer1,
          ReflectionUtility.GetPropertyName (typeof (Computer), "Employee"));

      _endPointMock = _mockRepository.CreateMock<ObjectEndPoint> (ClientTransactionMock, _id, DomainObjectIDs.Employee3);
      _oldEndPointMock = _mockRepository.CreateMock<IEndPoint> ();
      _newEndPointMock = _mockRepository.CreateMock<IEndPoint> ();

      _modification = new NullEndPointModification (_endPointMock, _oldEndPointMock, _newEndPointMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPointMock, _modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, _modification.OldEndPoint);
      Assert.AreSame (_newEndPointMock, _modification.NewEndPoint);
    }

    [Test]
    public void Initialization_FromNullObjectEndPoint ()
    {
      RelationEndPoint endPoint = new NullObjectEndPoint (_id.Definition);
      RelationEndPointModification modification = endPoint.CreateModification (_oldEndPointMock, _newEndPointMock);
      Assert.IsInstanceOfType (typeof (NullEndPointModification), modification);
      Assert.AreSame (endPoint, modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, modification.OldEndPoint);
      Assert.AreSame (_newEndPointMock, modification.NewEndPoint);
    }

    [Test]
    public void Initialization_FromNullCollectionEndPoint ()
    {
      NullCollectionEndPoint endPoint = new NullCollectionEndPoint (_id.Definition);
      RelationEndPointModification modification = endPoint.CreateModification (_oldEndPointMock, _newEndPointMock);
      Assert.IsInstanceOfType (typeof (NullEndPointModification), modification);
      Assert.AreSame (endPoint, modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, modification.OldEndPoint);
      Assert.AreSame (_newEndPointMock, modification.NewEndPoint);

      modification = endPoint.CreateInsertModification (_oldEndPointMock, _newEndPointMock, 0);
      Assert.IsInstanceOfType (typeof (NullEndPointModification), modification);

      modification = endPoint.CreateReplaceModification (_oldEndPointMock, _newEndPointMock);
      Assert.IsInstanceOfType (typeof (NullEndPointModification), modification);
    }

    [Test]
    public void BeginDoesNothing ()
    {
      _mockRepository.ReplayAll ();

      _modification.Begin ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PerformDoesNothing ()
    {
      _mockRepository.ReplayAll ();

      _modification.Perform ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void EndDoesNothing ()
    {
      _mockRepository.ReplayAll ();

      _modification.End ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NotifyClientTransactionOfBeginDoesNothing ()
    {
      _mockRepository.ReplayAll ();

      _modification.NotifyClientTransactionOfBegin ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NotifyClientTransactionOfEndDoesNothing ()
    {
      _mockRepository.ReplayAll ();

      _modification.NotifyClientTransactionOfEnd ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExecuteAllSteps ()
    {
      NullEndPointModification modificationMock = _mockRepository.CreateMock<NullEndPointModification> (_endPointMock, _oldEndPointMock, _newEndPointMock);

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
