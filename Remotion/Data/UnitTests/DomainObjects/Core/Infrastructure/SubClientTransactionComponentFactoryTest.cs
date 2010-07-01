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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class SubClientTransactionComponentFactoryTest : StandardMappingTest
  {
    private ClientTransactionMock _parentTransaction;
    private SubClientTransactionComponentFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();

      _parentTransaction = new ClientTransactionMock ();
      _factory = new SubClientTransactionComponentFactory (_parentTransaction);
    }
    [Test]
    public void CreateListeners_IncludesSubTransactionListener ()
    {
      var listeners = _factory.CreateListeners (new ClientTransactionMock ());

      var listener = listeners.OfType<SubClientTransactionListener> ().SingleOrDefault ();
      Assert.That (listener, Is.Not.Null);
    }

    [Test]
    public void CreateDataManager_CollectionEndPointChangeDetectionStrategy ()
    {
      var dataManager = _factory.CreateDataManager (new ClientTransactionMock ());
      Assert.That (
          dataManager.RelationEndPointMap.CollectionEndPointChangeDetectionStrategy,
          Is.InstanceOfType (typeof (SubCollectionEndPointChangeDetectionStrategy)));
    }

    [Test]
    public void CreateDataManager_ObjectsInvalidOrDeletedInParentTransaction_AreAutomaticallyMarkedInvalid ()
    {
      var objectInvalidInParent = _parentTransaction.Execute (() => Order.NewObject());
      var objectDeletedInParent = _parentTransaction.GetObject (DomainObjectIDs.Order2, false);
      var objectLoadedInParent = _parentTransaction.GetObject (DomainObjectIDs.Order3, false);

      _parentTransaction.Delete (objectInvalidInParent);
      _parentTransaction.Delete (objectDeletedInParent);

      Assert.That (objectInvalidInParent.TransactionContext[_parentTransaction].State, Is.EqualTo (StateType.Invalid));
      Assert.That (objectDeletedInParent.TransactionContext[_parentTransaction].State, Is.EqualTo (StateType.Deleted));
      Assert.That (objectLoadedInParent.TransactionContext[_parentTransaction].State, Is.EqualTo (StateType.Unchanged));

      var dataManager = _factory.CreateDataManager (new ClientTransactionMock ());

      Assert.That (dataManager.IsInvalid (objectInvalidInParent.ID), Is.True);
      Assert.That (dataManager.IsInvalid (objectDeletedInParent.ID), Is.True);
      Assert.That (dataManager.IsInvalid (objectLoadedInParent.ID), Is.False);
    }

    [Test]
    public void CreateEnlistedObjectManager ()
    {
      var manager = _factory.CreateEnlistedObjectManager ();
      Assert.That (manager, Is.TypeOf (typeof (DelegatingEnlistedDomainObjectManager)));
      Assert.That (((DelegatingEnlistedDomainObjectManager) manager).TargetTransaction, Is.SameAs (_parentTransaction));
    }
  }
}