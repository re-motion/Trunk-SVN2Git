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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class SubPersistenceStrategyTest : ClientTransactionBaseTest
  {
    private IParentTransactionOperations _parentTransactionOperationsMock;
    private SubPersistenceStrategy _persistenceStrategy;

    public override void SetUp ()
    {
      base.SetUp ();

      _parentTransactionOperationsMock = MockRepository.GenerateStrictMock<IParentTransactionOperations> ();
      var parentTransactionContextStub = MockRepository.GenerateStub<IParentTransactionContext> ();
      parentTransactionContextStub.Stub (stub => stub.AccessParentTransaction()).Return (_parentTransactionOperationsMock);
      _persistenceStrategy = new SubPersistenceStrategy (parentTransactionContextStub);
    }

    [Test]
    public void PersistData_NewDataContainer ()
    {
      var instance = DomainObjectMother.CreateFakeObject<Order> ();
      var dataContainer = DataContainer.CreateNew (instance.ID);
      dataContainer.PropertyValues[GetPropertyIdentifier (typeof (Order), "OrderNumber")].Value = 12;
      dataContainer.SetDomainObject (instance);

      var persistableData = new PersistableData (instance, StateType.New, dataContainer, new IRelationEndPoint[0]);

      using (_parentTransactionOperationsMock.GetMockRepository ().Ordered ())
      {
        _parentTransactionOperationsMock.Stub (mock => mock.IsInvalid (instance.ID)).Return (true);
        _parentTransactionOperationsMock.Expect (mock => mock.MarkNotInvalid (instance.ID));
        _parentTransactionOperationsMock.Stub (stub => stub.GetDataContainerWithoutLoading (instance.ID)).Return (null);
        _parentTransactionOperationsMock
            .Expect (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything))
            .WhenCalled (
                mi =>
                {
                  var dc = (DataContainer) mi.Arguments[0];
                  Assert.That (dc.ID, Is.EqualTo (instance.ID));
                  Assert.That (dc.State, Is.EqualTo (StateType.New));
                  Assert.That (dc.HasDomainObject, Is.True);
                  Assert.That (dc.DomainObject, Is.SameAs (instance));
                  Assert.That (dc.PropertyValues[GetPropertyIdentifier (typeof (Order), "OrderNumber")].Value, Is.EqualTo (12));
                }
            );
        _parentTransactionOperationsMock.Expect (mock => mock.Dispose());
      }

      _parentTransactionOperationsMock.Replay();

      _persistenceStrategy.PersistData (Array.AsReadOnly (new[] { persistableData }));

      _parentTransactionOperationsMock.VerifyAllExpectations();
    }
  }
}