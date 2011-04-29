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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Synchronization
{
  [TestFixture]
  public class VirtualEndPointGarbageCollectionTest : ClientTransactionBaseTest
  {
    [Test]
    [Ignore ("TODO 3914")]
    public void UnloadLastFK_CausesCollectionEndPointToBeRemoved ()
    {
      SetDatabaseModifyable();

      var industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      industrialSector.Companies.EnsureDataComplete();

      var unsynchronizedCompanyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (industrialSector.ID);
      var unsynchronizedCompany = Company.GetObject (unsynchronizedCompanyID);

      var dataManager = ClientTransactionMock.DataManager;
      var virtualEndPointID = RelationEndPointID.Create (industrialSector, s => s.Companies);

      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID), Is.Not.Null);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadCollectionEndPointAndData (ClientTransactionMock, virtualEndPointID);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID), Is.Not.Null);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID).IsDataComplete, Is.False);

      UnloadService.UnloadData (ClientTransactionMock, unsynchronizedCompany.ID);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID), Is.Null);
    }

    [Test]
    public void UnloadUnsynchronizedFK_LeavesCompleteEmptyCollection ()
    {
      SetDatabaseModifyable ();

      var employee = Employee.GetObject (DomainObjectIDs.Employee3);
      employee.Subordinates.EnsureDataComplete ();
      Assert.That (employee.Subordinates, Is.Empty);

      var unsynchronizedSubordinateID =
          DomainObjectMother.CreateObjectAndSetRelationInOtherTransaction<Employee, Employee> (
            employee.ID,
            (subOrdinate, e) => subOrdinate.Supervisor = e);
      var unsynchronizedSubordinate = Employee.GetObject (unsynchronizedSubordinateID);

      var dataManager = ClientTransactionMock.DataManager;
      var virtualEndPointID = RelationEndPointID.Create (employee, o => o.Subordinates);

      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID), Is.Not.Null);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadData (ClientTransactionMock, unsynchronizedSubordinate.ID);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID), Is.Not.Null);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID).IsDataComplete, Is.True);
    }

    [Test]
    public void UnloadLastFK_CausesVirtualObjectEndPointToBeRemoved ()
    {
      SetDatabaseModifyable ();

      var employee = Employee.GetObject (DomainObjectIDs.Employee3);
      var virtualEndPointID = RelationEndPointID.Create (employee, e => e.Computer);
      ClientTransactionMock.EnsureDataComplete (virtualEndPointID);

      var unsynchronizedComputerID =
          DomainObjectMother.CreateObjectAndSetRelationInOtherTransaction<Computer, Employee> (
            employee.ID,
            (c, e) => c.Employee = e);
      var unsynchronizedComputer = Computer.GetObject (unsynchronizedComputerID);

      var dataManager = ClientTransactionMock.DataManager;

      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID), Is.Not.Null);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadData (ClientTransactionMock, employee.Computer.ID);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID), Is.Not.Null);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID).IsDataComplete, Is.False);

      UnloadService.UnloadData (ClientTransactionMock, unsynchronizedComputer.ID);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID), Is.Null);
    }

    [Test]
    public void UnloadUnsynchronizedFK_LeavesNullCompleteVirtualObjectEndPoint ()
    {
      SetDatabaseModifyable ();

      var employee = Employee.GetObject (DomainObjectIDs.Employee1);
      var virtualEndPointID = RelationEndPointID.Create (employee, e => e.Computer);
      ClientTransactionMock.EnsureDataComplete (virtualEndPointID);
      Assert.That (employee.Computer, Is.Null);

      var unsynchronizedComputerID =
          DomainObjectMother.CreateObjectAndSetRelationInOtherTransaction<Computer, Employee> (
            employee.ID,
            (c, e) => c.Employee = e);
      var unsynchronizedComputer = Computer.GetObject (unsynchronizedComputerID);

      var dataManager = ClientTransactionMock.DataManager;

      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID), Is.Not.Null);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadData (ClientTransactionMock, unsynchronizedComputer.ID);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID), Is.Not.Null);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (virtualEndPointID).IsDataComplete, Is.True);
    }

    protected ObjectID CreateCompanyAndSetIndustrialSectorInOtherTransaction (ObjectID industrialSectorID)
    {
      return DomainObjectMother.CreateObjectAndSetRelationInOtherTransaction<Company, IndustrialSector> (industrialSectorID, (c, s) =>
      {
        c.IndustrialSector = s;
        c.Ceo = Ceo.NewObject ();
      });
    }
  }
}