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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class VirtualObjectEndPointDataKeeperFactoryTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;
    private VirtualObjectEndPointDataKeeperFactory _factory;
    private RelationEndPointID _endPointID;

    public override void SetUp ()
    {
      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _factory = new VirtualObjectEndPointDataKeeperFactory (_clientTransaction);
      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
    }

    [Test]
    public void Create ()
    {
      var result = _factory.Create (_endPointID);

      Assert.That (result, Is.TypeOf (typeof (VirtualObjectEndPointDataKeeper)));
      Assert.That (((VirtualObjectEndPointDataKeeper) result).EndPointID, Is.SameAs (_endPointID));
      
      var updateListener = ((VirtualObjectEndPointDataKeeper) result).UpdateListener;
      Assert.That (updateListener, Is.TypeOf (typeof (VirtualEndPointStateUpdateListener)));
      Assert.That (((VirtualEndPointStateUpdateListener) updateListener).ClientTransaction, Is.SameAs (_clientTransaction));
      Assert.That (((VirtualEndPointStateUpdateListener) updateListener).EndPointID, Is.SameAs (_endPointID));
    }
  }
}