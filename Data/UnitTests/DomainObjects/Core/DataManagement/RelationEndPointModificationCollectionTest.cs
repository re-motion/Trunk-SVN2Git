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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class RelationEndPointModificationCollectionTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private ObjectEndPoint _endPointMock;
    private IEndPoint _oldEndPointMock;
    private IEndPoint _newEndPointMock;
    private RelationEndPointModification _modificationMock1;
    private RelationEndPointModification _modificationMock2;
    private RelationEndPointModification _modificationMock3;
    private RelationEndPointModificationCollection _collection;
    private RelationEndPointID _id;

    public override void SetUp ()
    {
      base.SetUp ();
      _mockRepository = new MockRepository ();
      _id = new RelationEndPointID (
          DomainObjectIDs.Computer1,
          MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Computer), "Employee"));

      _endPointMock = _mockRepository.CreateMock<ObjectEndPoint> (ClientTransactionMock, _id, DomainObjectIDs.Employee3);
      _oldEndPointMock = _mockRepository.CreateMock<IEndPoint> ();
      _newEndPointMock = _mockRepository.CreateMock<IEndPoint> ();

      _modificationMock1 = _mockRepository.CreateMock<RelationEndPointModification> (_endPointMock, _oldEndPointMock, _newEndPointMock);
      _modificationMock2 = _mockRepository.CreateMock<RelationEndPointModification> (_endPointMock, _oldEndPointMock, _newEndPointMock);
      _modificationMock3 = _mockRepository.CreateMock<RelationEndPointModification> (_endPointMock, _oldEndPointMock, _newEndPointMock);

      _collection = new RelationEndPointModificationCollection (_modificationMock1, _modificationMock2, _modificationMock3);
    }

    [Test]
    public void Begin ()
    {
      _modificationMock1.Begin ();
      _modificationMock2.Begin ();
      _modificationMock3.Begin ();

      _mockRepository.ReplayAll();

      _collection.Begin();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Perform ()
    {
      _modificationMock1.Perform ();
      _modificationMock2.Perform ();
      _modificationMock3.Perform ();

      _mockRepository.ReplayAll ();

      _collection.Perform ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void End ()
    {
      _modificationMock1.End ();
      _modificationMock2.End ();
      _modificationMock3.End ();

      _mockRepository.ReplayAll ();

      _collection.End ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      _modificationMock1.NotifyClientTransactionOfBegin ();
      _modificationMock2.NotifyClientTransactionOfBegin ();
      _modificationMock3.NotifyClientTransactionOfBegin ();

      _mockRepository.ReplayAll ();

      _collection.NotifyClientTransactionOfBegin ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      _modificationMock1.NotifyClientTransactionOfEnd ();
      _modificationMock2.NotifyClientTransactionOfEnd ();
      _modificationMock3.NotifyClientTransactionOfEnd ();

      _mockRepository.ReplayAll ();

      _collection.NotifyClientTransactionOfEnd ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExecuteAllSteps ()
    {
      _modificationMock1.NotifyClientTransactionOfBegin ();
      _modificationMock2.NotifyClientTransactionOfBegin ();
      _modificationMock3.NotifyClientTransactionOfBegin ();

      _modificationMock1.Begin ();
      _modificationMock2.Begin ();
      _modificationMock3.Begin ();

      _modificationMock1.Perform();
      _modificationMock2.Perform ();
      _modificationMock3.Perform ();

      _modificationMock1.NotifyClientTransactionOfEnd ();
      _modificationMock2.NotifyClientTransactionOfEnd ();
      _modificationMock3.NotifyClientTransactionOfEnd ();

      _modificationMock1.End ();
      _modificationMock2.End ();
      _modificationMock3.End ();

      _mockRepository.ReplayAll ();

      _collection.ExecuteAllSteps ();

      _mockRepository.VerifyAll ();
    }
  }
}
