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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands
{
  [TestFixture]
  public class UnregisterEndPointsCommandTest : StandardMappingTest
  {
    private RelationEndPointMap _map;
    private IRealObjectEndPoint _realEndPoint;
    private IVirtualEndPoint _virtualEndPoint;

    private UnregisterEndPointsCommand _command;
    private IVirtualEndPoint _oppositeEndPointMock;

    public override void SetUp ()
    {
      base.SetUp();

      _map = new RelationEndPointMap (
          ClientTransaction.CreateRootTransaction(),
          MockRepository.GenerateStub<IObjectLoader>(),
          MockRepository.GenerateStub<IRelationEndPointLazyLoader>(),
          MockRepository.GenerateStub<IRelationEndPointProvider>(),
          MockRepository.GenerateStub<IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper>>(),
          MockRepository.GenerateStub<IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper>>()
          );

      _realEndPoint = MockRepository.GenerateStub<IRealObjectEndPoint>();
      _realEndPoint.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer"));
      _realEndPoint.Stub (stub => stub.Definition).Return (_realEndPoint.ID.Definition);
      _realEndPoint.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.Customer1);

      _oppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint> ();
      _oppositeEndPointMock.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders"));
      
      _virtualEndPoint = MockRepository.GenerateStub<IVirtualEndPoint> ();
      _virtualEndPoint.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Customer2, typeof (Customer), "Orders"));

      RelationEndPointMapTestHelper.AddEndPoint (_map, _realEndPoint);
      RelationEndPointMapTestHelper.AddEndPoint (_map, _oppositeEndPointMock);
      RelationEndPointMapTestHelper.AddEndPoint (_map, _virtualEndPoint);

      _command = new UnregisterEndPointsCommand (new[] { _realEndPoint.ID, _virtualEndPoint.ID }, _map);
    }

    [Test]
    public void GetAllExceptions ()
    {
      Assert.That (_command.GetAllExceptions(), Is.Empty);
    }

    [Test]
    public void NotifyClientTransactionOfBegin_DoesNothing ()
    {
      _command.NotifyClientTransactionOfBegin();
    }

    [Test]
    public void Begin_DoesNothing ()
    {
      _command.Begin ();
    }

    [Test]
    public void Perform ()
    {
      Assert.That (_map[_realEndPoint.ID], Is.Not.Null);
      Assert.That (_map[_virtualEndPoint.ID], Is.Not.Null);
      Assert.That (_map[_oppositeEndPointMock.ID], Is.Not.Null);

      _oppositeEndPointMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (_realEndPoint));
      _oppositeEndPointMock.Stub (stub => stub.HasChanged).Return (false);
      _oppositeEndPointMock.Stub (stub => stub.CanBeCollected).Return (false);
      _oppositeEndPointMock.Replay();

      _command.Perform();

      Assert.That (_map[_realEndPoint.ID], Is.Null);
      Assert.That (_map[_virtualEndPoint.ID], Is.Null);
      Assert.That (_map[_oppositeEndPointMock.ID], Is.Not.Null);
      _oppositeEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void End_DoesNothing ()
    {
      _command.End ();
    }

    [Test]
    public void NotifyClientTransactionOfEnd_DoesNothing ()
    {
      _command.End ();
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var result = _command.ExpandToAllRelatedObjects();

      Assert.That (result.GetNestedCommands(), Is.EqualTo (new[] { _command }));
    }
  }
}