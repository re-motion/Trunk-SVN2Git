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
  public class NonNotifyingBidirectionalRelationModificationTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IRelationEndPointModification _modificationMock1;
    private IRelationEndPointModification _modificationMock2;
    private IRelationEndPointModification _modificationMock3;
    private NonNotifyingBidirectionalRelationModification _collection;

    public override void SetUp ()
    {
      base.SetUp ();
      _mockRepository = new MockRepository ();

      _modificationMock1 = _mockRepository.StrictMock<IRelationEndPointModification> ();
      _modificationMock2 = _mockRepository.StrictMock<IRelationEndPointModification> ();
      _modificationMock3 = _mockRepository.StrictMock<IRelationEndPointModification> ();

      _collection = new NonNotifyingBidirectionalRelationModification (_modificationMock1, _modificationMock2, _modificationMock3);
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
    public void ExecuteAllSteps ()
    {
      _modificationMock1.Expect (mock => mock.Perform());
      _modificationMock2.Expect (mock => mock.Perform ());
      _modificationMock3.Expect (mock => mock.Perform ());

      _mockRepository.ReplayAll ();

      _collection.ExecuteAllSteps ();

      _mockRepository.VerifyAll ();
    }
  }
}
