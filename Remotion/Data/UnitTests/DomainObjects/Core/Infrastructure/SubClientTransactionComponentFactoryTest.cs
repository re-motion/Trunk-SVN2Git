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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class SubClientTransactionComponentFactoryTest : StandardMappingTest
  {
    private ClientTransactionMock _parentTransaction;
    private IInvalidDomainObjectManager _parentInvalidDomainObjectManagerStub;
    private SubClientTransactionComponentFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();

      _parentTransaction = new ClientTransactionMock ();
      _parentInvalidDomainObjectManagerStub = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      _factory = new SubClientTransactionComponentFactory (_parentTransaction, _parentInvalidDomainObjectManagerStub);
    }

    [Test]
    public void CreateListeners_IncludesSubTransactionListener ()
    {
      var listeners = _factory.CreateListeners (new ClientTransactionMock ());

      var listener = listeners.OfType<SubClientTransactionListener> ().SingleOrDefault ();
      Assert.That (listener, Is.Not.Null);
      Assert.That (listener.ParentInvalidDomainObjectManager, Is.SameAs (_parentInvalidDomainObjectManagerStub));
    }

    [Test]
    public void CreateEnlistedObjectManager ()
    {
      var manager = _factory.CreateEnlistedObjectManager ();
      Assert.That (manager, Is.TypeOf (typeof (DelegatingEnlistedDomainObjectManager)));
      Assert.That (((DelegatingEnlistedDomainObjectManager) manager).TargetTransaction, Is.SameAs (_parentTransaction));
    }

    [Test]
    public void CreateInvalidDomainObjectManager ()
    {
      _parentInvalidDomainObjectManagerStub.Stub (stub => stub.InvalidObjectIDs).Return (new ObjectID[0]);

      var manager = _factory.CreateInvalidDomainObjectManager ();
      Assert.That (manager, Is.TypeOf (typeof (SubInvalidDomainObjectManager)));
      Assert.That (((SubInvalidDomainObjectManager) manager).ParentTransactionManager, Is.SameAs (_parentInvalidDomainObjectManagerStub));
    }

    [Test]
    public void CreateInvalidDomainObjectManager_AutomaticallyMarksInvalid_ObjectsInvalidOrDeletedInParentTransaction ()
    {
      var objectInvalidInParent = _parentTransaction.Execute (() => Order.NewObject ());
      var objectDeletedInParent = _parentTransaction.GetObject (DomainObjectIDs.Order2, false);
      var objectLoadedInParent = _parentTransaction.GetObject (DomainObjectIDs.Order3, false);

      _parentInvalidDomainObjectManagerStub.Stub (stub => stub.InvalidObjectIDs).Return (new[] { objectInvalidInParent.ID });
      _parentInvalidDomainObjectManagerStub.Stub (stub => stub.GetInvalidObjectReference (objectInvalidInParent.ID)).Return (objectInvalidInParent);
      
      _parentTransaction.Delete (objectDeletedInParent);

      var invalidOjectManager = _factory.CreateInvalidDomainObjectManager();

      Assert.That (invalidOjectManager.IsInvalid (objectInvalidInParent.ID), Is.True);
      Assert.That (invalidOjectManager.IsInvalid (objectDeletedInParent.ID), Is.True);
      Assert.That (invalidOjectManager.IsInvalid (objectLoadedInParent.ID), Is.False);
    }

    [Test]
    public void CreateDataManager ()
    {
      var invalidDomainObjectManagerStub = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var objectLoaderStub = MockRepository.GenerateStub<IObjectLoader> ();
      var dataManager = _factory.CreateDataManager (new ClientTransactionMock (), invalidDomainObjectManagerStub, objectLoaderStub);

      Assert.That (
          dataManager.RelationEndPointMap.CollectionEndPointChangeDetectionStrategy,
          Is.InstanceOfType (typeof (SubCollectionEndPointChangeDetectionStrategy)));
      Assert.That (PrivateInvoke.GetNonPublicField (dataManager, "_invalidDomainObjectManager"), Is.SameAs (invalidDomainObjectManagerStub));
      Assert.That (PrivateInvoke.GetNonPublicField (dataManager, "_objectLoader"), Is.SameAs (objectLoaderStub));
    }
  }
}