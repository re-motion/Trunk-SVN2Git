// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class DelegatingDataManagerTest : StandardMappingTest
  {
    [Test]
    public void DelegatingMembers ()
    {
      var objectID = DomainObjectIDs.Order1;
      var dataContainer = DataContainer.CreateNew (objectID);
      var relationEndPointID = RelationEndPointID.Create (objectID, typeof (Order), "OrderTicket");
      var virtualEndPoint = MockRepository.GenerateStub<IVirtualEndPoint>();
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      var persistableData = new PersistableData (domainObject, StateType.Unchanged, dataContainer, new IRelationEndPoint[0]);
      var dataManagementCommand = MockRepository.GenerateStub<IDataManagementCommand> ();

      CheckDelegation (dm => dm.GetOrCreateVirtualEndPoint (relationEndPointID), virtualEndPoint);
      CheckDelegation (dm => dm.GetRelationEndPointWithLazyLoad (relationEndPointID), virtualEndPoint);
      CheckDelegation (dm => dm.GetRelationEndPointWithoutLoading (relationEndPointID), virtualEndPoint);
      CheckDelegation (dm => dm.GetDataContainerWithoutLoading (objectID), dataContainer);
      CheckDelegation (dm => dm.RegisterDataContainer (dataContainer));
      CheckDelegation (dm => dm.Discard (dataContainer));
      CheckDelegation (dm => dm.DataContainers, MockRepository.GenerateStub<IDataContainerMapReadOnlyView> ());
      CheckDelegation (dm => dm.RelationEndPoints, MockRepository.GenerateStub<IRelationEndPointMapReadOnlyView> ());
      CheckDelegation (dm => dm.DomainObjectStateCache, new DomainObjectStateCache (ClientTransaction.CreateRootTransaction()));
      CheckDelegation (dm => dm.GetDataContainerWithLazyLoad (objectID), dataContainer);
      CheckDelegation (dm => dm.GetDataContainersWithLazyLoad (new[] { objectID }, true), new[] { dataContainer });
      CheckDelegation (dm => dm.GetLoadedDataByObjectState (StateType.Unchanged), new[] { persistableData });
      CheckDelegation (dm => dm.GetNewChangedDeletedData (), new[] { persistableData });
      CheckDelegation (dm => dm.GetOppositeRelationEndPoints (dataContainer), new[] { virtualEndPoint });
      CheckDelegation (dm => dm.HasRelationChanged (dataContainer), true);
      CheckDelegation (dm => dm.Commit());
      CheckDelegation (dm => dm.Rollback ());
      CheckDelegation (dm => dm.CreateDeleteCommand (domainObject), dataManagementCommand);
      CheckDelegation (dm => dm.CreateUnloadCommand (new[] { objectID }), dataManagementCommand);
      CheckDelegation (dm => dm.CreateUnloadVirtualEndPointsCommand (new[] { relationEndPointID }), dataManagementCommand);
    }

    private void CheckDelegation<TR> (Func<IDataManager, TR> func, TR fakeResult)
    {
      var innerMock = MockRepository.GenerateStrictMock<IDataManager>();
      var delegatingDataManager = new DelegatingDataManager();
      delegatingDataManager.InnerDataManager = innerMock;

      innerMock.Expect (mock => func (mock)).Return (fakeResult);
      innerMock.Replay();

      var result = func (delegatingDataManager);

      innerMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (fakeResult));

      delegatingDataManager.InnerDataManager = null;
      Assert.That (
          () => func (delegatingDataManager),
          Throws.InvalidOperationException.With.Message.EqualTo ("InnerDataManager property must be set before it can be used."));
    }

    private void CheckDelegation (Action<IDataManager> action)
    {
      var innerMock = MockRepository.GenerateStrictMock<IDataManager> ();
      var delegatingDataManager = new DelegatingDataManager ();
      delegatingDataManager.InnerDataManager = innerMock;

      innerMock.Expect (action);
      innerMock.Replay ();

      action (delegatingDataManager);

      innerMock.VerifyAllExpectations ();

      delegatingDataManager.InnerDataManager = null;
      Assert.That (
          () => action (delegatingDataManager),
          Throws.InvalidOperationException.With.Message.EqualTo ("InnerDataManager property must be set before it can be used."));
    }
  }
}