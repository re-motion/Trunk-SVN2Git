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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class NullVirtualObjectEndPointTest : ClientTransactionBaseTest
  {
    private IRelationEndPointDefinition _definition;
    private NullVirtualObjectEndPoint _nullEndPoint;
    private IRealObjectEndPoint _oppositeEndPointStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _definition = DomainObjectIDs.Order1.ClassDefinition.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      _nullEndPoint = new NullVirtualObjectEndPoint(ClientTransactionMock, _definition);

      _oppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void MarkDataIncomplete ()
    {
      _nullEndPoint.MarkDataIncomplete();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void RegisterOriginalOppositeEndPoint ()
    {
      _nullEndPoint.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
    }
    
    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      _nullEndPoint.UnregisterOriginalOppositeEndPoint (_oppositeEndPointStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _nullEndPoint.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);
    }
    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _nullEndPoint.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);
    }
  }
}