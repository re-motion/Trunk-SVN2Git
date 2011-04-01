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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class VirtualObjectEndPointOppositeTrackingTest : ClientTransactionBaseTest
  {
    private Employee _employee1;
    private VirtualObjectEndPoint _virtualObjectEndPoint;

    private Computer _computer1;
    private Computer _computer2;

    private RealObjectEndPoint _computer1EndPoint;
    private RealObjectEndPoint _computer2EndPoint;

    public override void SetUp ()
    {
      base.SetUp ();

      _employee1 = Employee.NewObject ();
      
      _computer1 = Computer.NewObject ();
      _computer2 = Computer.NewObject ();

      _employee1.Computer = _computer1;
      
      ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope();

      _employee1.Computer.EnsureDataAvailable();
      _virtualObjectEndPoint = GetEndPoint<VirtualObjectEndPoint> (RelationEndPointID.Create (_employee1, o => o.Computer));

      _computer1EndPoint = GetEndPoint<RealObjectEndPoint> (RelationEndPointID.Create (_computer1, oi => oi.Employee));
      _computer2EndPoint = GetEndPoint<RealObjectEndPoint> (RelationEndPointID.Create (_computer2, oi => oi.Employee));
    }

    [Test]
    public void StateAfterLoading ()
    {
      CheckOriginalData (_computer1);
      CheckOriginalOppositeEndPoint (_computer1EndPoint);

      CheckCurrentData (_computer1);
      CheckCurrentOppositeEndPoint (_computer1EndPoint);
    }

    [Test]
    public void Replace ()
    {
      _employee1.Computer = _computer2;

      CheckOriginalData (_computer1);
      CheckOriginalOppositeEndPoint (_computer1EndPoint);

      CheckCurrentData (_computer2);
      CheckCurrentOppositeEndPoint (_computer2EndPoint);

      ClientTransaction.Current.Rollback();

      CheckOriginalData (_computer1);
      CheckOriginalOppositeEndPoint (_computer1EndPoint);

      CheckCurrentData (_computer1);
      CheckCurrentOppositeEndPoint (_computer1EndPoint);

      _employee1.Computer = _computer2;
      ClientTransaction.Current.Commit();

      CheckOriginalData (_computer2);
      CheckOriginalOppositeEndPoint (_computer2EndPoint);

      CheckCurrentData (_computer2);
      CheckCurrentOppositeEndPoint (_computer2EndPoint);
    }

    [Test]
    public void Replace_ViaFKProperty ()
    {
      _computer2.Employee = _employee1;

      CheckOriginalData (_computer1);
      CheckOriginalOppositeEndPoint (_computer1EndPoint);

      CheckCurrentData (_computer2);
      CheckCurrentOppositeEndPoint (_computer2EndPoint);
    }

    [Test]
    public void Remove ()
    {
      _employee1.Computer = null;

      CheckOriginalData (_computer1);
      CheckOriginalOppositeEndPoint (_computer1EndPoint);

      CheckCurrentData (null);
      CheckCurrentOppositeEndPoint (null);

      ClientTransaction.Current.Rollback();

      CheckOriginalData (_computer1);
      CheckOriginalOppositeEndPoint (_computer1EndPoint);

      CheckCurrentData (_computer1);
      CheckCurrentOppositeEndPoint (_computer1EndPoint);

      _employee1.Computer = null;

      ClientTransaction.Current.Commit();

      CheckOriginalData (null);
      CheckOriginalOppositeEndPoint (null);

      CheckCurrentData (null);
      CheckCurrentOppositeEndPoint (null);
    }

    [Test]
    public void Remove_ViaFKProperty ()
    {
      _computer1.Employee = null;

      CheckOriginalData (_computer1);
      CheckOriginalOppositeEndPoint (_computer1EndPoint);

      CheckCurrentData (null);
      CheckCurrentOppositeEndPoint (null);
    }

    [Test]
    public void Delete_FKSide ()
    {
      _computer1.Delete();

      CheckOriginalData (_computer1);
      CheckOriginalOppositeEndPoint (_computer1EndPoint);

      CheckCurrentData (null);
      CheckCurrentOppositeEndPoint (null);

      ClientTransaction.Current.Rollback ();

      CheckOriginalData (_computer1);
      CheckOriginalOppositeEndPoint (_computer1EndPoint);

      CheckCurrentData (_computer1);
      CheckCurrentOppositeEndPoint (_computer1EndPoint);

      _computer1.Delete();

      ClientTransaction.Current.Commit ();

      CheckOriginalData (null);
      CheckOriginalOppositeEndPoint (null);

      CheckCurrentData (null);
      CheckCurrentOppositeEndPoint (null);
    }

    [Test]
    public void Delete_VirtualObjectSide ()
    {
      _employee1.Delete ();

      CheckOriginalData (_computer1);
      CheckOriginalOppositeEndPoint (_computer1EndPoint);

      CheckCurrentData (null);
      CheckCurrentOppositeEndPoint (null);

      ClientTransaction.Current.Rollback ();

      CheckOriginalData (_computer1);
      CheckOriginalOppositeEndPoint (_computer1EndPoint);

      CheckCurrentData (_computer1);
      CheckCurrentOppositeEndPoint (_computer1EndPoint);

      _employee1.Delete ();

      ClientTransaction.Current.Commit ();

      CheckOriginalOppositeEndPoint (null);
      CheckCurrentOppositeEndPoint (null);
    }
    
    private T GetEndPoint<T> (RelationEndPointID endPointID) where T : IRelationEndPoint
    {
      var relationEndPointID = endPointID;
      return (T) ClientTransactionTestHelper.GetDataManager (ClientTransaction.Current).GetRelationEndPointWithLazyLoad (relationEndPointID);
    }

    private void CheckOriginalData (Computer expected)
    {
      Assert.That (_employee1.Properties[typeof (Employee), "Computer"].GetOriginalValue<Computer> (), Is.SameAs (expected));
    }

    private void CheckCurrentData (Computer expected)
    {
      Assert.That (_employee1.Computer, Is.SameAs (expected));
    }

    private void CheckOriginalOppositeEndPoint (RealObjectEndPoint expected)
    {
      var loadState = (CompleteVirtualObjectEndPointLoadState) VirtualObjectEndPointTestHelper.GetLoadState (_virtualObjectEndPoint);
      var dataKeeper = (VirtualObjectEndPointDataKeeper) loadState.DataKeeper;
      Assert.That (dataKeeper.OriginalOppositeEndPoint, Is.SameAs (expected));
    }

    private void CheckCurrentOppositeEndPoint (RealObjectEndPoint expected)
    {
      var loadState = (CompleteVirtualObjectEndPointLoadState) VirtualObjectEndPointTestHelper.GetLoadState (_virtualObjectEndPoint);
      var dataKeeper = (VirtualObjectEndPointDataKeeper) loadState.DataKeeper;
      Assert.That (dataKeeper.CurrentOppositeEndPoint, Is.SameAs (expected));
    }

  }
}