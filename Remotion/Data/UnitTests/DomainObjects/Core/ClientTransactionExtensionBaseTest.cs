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
using Remotion.Data.DomainObjects.Queries;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class ClientTransactionExtensionBaseTest
  {
    private TestableClientTransactionExtensionBase _extension;

    [SetUp]
    public void SetUp ()
    {
      _extension = new TestableClientTransactionExtensionBase ("key");
    }

    [Test]
    public void Key ()
    {
      Assert.That (_extension.Key, Is.EqualTo ("key"));
    }

    [Test]
    public void EventMethods_Empty ()
    {
      CheckNopEvent (e => e.SubTransactionCreating (null));
      CheckNopEvent (e => e.SubTransactionCreated (null, null));
      CheckNopEvent (e => e.NewObjectCreating (null, null));
      CheckNopEvent (e => e.ObjectsLoading (null, null));
      CheckNopEvent (e => e.ObjectsLoaded (null, null));
      CheckNopEvent (e => e.ObjectsUnloading (null, null));
      CheckNopEvent (e => e.ObjectsUnloaded (null, null));
      CheckNopEvent (e => e.ObjectDeleting (null, null));
      CheckNopEvent (e => e.ObjectDeleted (null, null));
      CheckNopEvent (e => e.PropertyValueReading (null, null, null, ValueAccess.Current));
      CheckNopEvent (e => e.PropertyValueRead (null, null, null, null, ValueAccess.Current));
      CheckNopEvent (e => e.PropertyValueChanging (null, null, null, null, null));
      CheckNopEvent (e => e.PropertyValueChanged (null, null, null, null, null));
      CheckNopEvent (e => e.RelationReading (null, null, null, ValueAccess.Current));
      CheckNopEvent (e => e.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current));
      CheckNopEvent (e => e.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current));
      CheckNopEvent (e => e.RelationChanging (null, null, null, null, null));
      CheckNopEvent (e => e.RelationChanged (null, null, null));

      var fakeResult = new QueryResult<DomainObject> (MockRepository.GenerateStub<IQuery>(), new DomainObject[0]);
      CheckNopEvent (e => e.FilterQueryResult (null, fakeResult), fakeResult);

      CheckNopEvent (e => e.Committing (null, null));
      CheckNopEvent (e => e.Committed (null, null));
      CheckNopEvent (e => e.RollingBack (null, null));
      CheckNopEvent (e => e.RolledBack (null, null));
    }

    [Test]
    public void TryInstall_Success ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      var result = _extension.TryInstall (transaction);

      Assert.That (result, Is.True);
      Assert.That (transaction.Extensions, Has.Member (_extension));
    }

    [Test]
    public void TryInstall_NoSuccess ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();
      var otherExtensionWithSameName = MockRepository.GenerateStub<IClientTransactionExtension>();
      otherExtensionWithSameName.Stub (stub => stub.Key).Return (_extension.Key);

      transaction.Extensions.Add (otherExtensionWithSameName);

      var result = _extension.TryInstall (transaction);

      Assert.That (result, Is.False);
      Assert.That (transaction.Extensions, Has.Member (otherExtensionWithSameName));
      Assert.That (transaction.Extensions, Has.No.Member (_extension));
    }

    private void CheckNopEvent (Action<IClientTransactionExtension> action)
    {
      // expectation: no exception
      action (_extension);
    }

    private void CheckNopEvent<T> (Func<IClientTransactionExtension, T> func, T expectedResult)
    {
      var result = func (_extension);
      Assert.That (result, Is.EqualTo (expectedResult));
    }
  }
}