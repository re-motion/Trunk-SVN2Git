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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class NotifyingBidirectionalRelationModificationTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IRelationEndPointModification _modificationMock1;
    private IRelationEndPointModification _modificationMock2;
    private IRelationEndPointModification _modificationMock3;
    private NotifyingBidirectionalRelationModification _collection;

    public override void SetUp ()
    {
      base.SetUp ();
      _mockRepository = new MockRepository ();
    
      _modificationMock1 = _mockRepository.StrictMock<IRelationEndPointModification> ();
      _modificationMock2 = _mockRepository.StrictMock<IRelationEndPointModification> ();
      _modificationMock3 = _mockRepository.StrictMock<IRelationEndPointModification> ();

      _collection = new NotifyingBidirectionalRelationModification (_modificationMock1, _modificationMock2, _modificationMock3);
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
      _modificationMock1.Expect (mock => mock.NotifyClientTransactionOfBegin ());
      _modificationMock2.Expect (mock => mock.NotifyClientTransactionOfBegin ());
      _modificationMock3.Expect (mock => mock.NotifyClientTransactionOfBegin ());

      _modificationMock1.Expect (mock => mock.Begin ());
      _modificationMock2.Expect (mock => mock.Begin ());
      _modificationMock3.Expect (mock => mock.Begin ());

      _modificationMock1.Expect (mock => mock.Perform());
      _modificationMock2.Expect (mock => mock.Perform ());
      _modificationMock3.Expect (mock => mock.Perform ());

      _modificationMock1.Expect (mock => mock.NotifyClientTransactionOfEnd ());
      _modificationMock2.Expect (mock => mock.NotifyClientTransactionOfEnd ());
      _modificationMock3.Expect (mock => mock.NotifyClientTransactionOfEnd ());

      _modificationMock1.Expect (mock => mock.End ());
      _modificationMock2.Expect (mock => mock.End ());
      _modificationMock3.Expect (mock => mock.End ());

      _mockRepository.ReplayAll ();

      _collection.ExecuteAllSteps ();

      _mockRepository.VerifyAll ();
    }
  }
}
