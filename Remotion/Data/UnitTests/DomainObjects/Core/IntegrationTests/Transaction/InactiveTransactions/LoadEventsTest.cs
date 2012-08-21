// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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

using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions
{
  [TestFixture]
  public class LoadEventsTest : InactiveTransactionsTestBase
  {
    private Order _order;

    public override void SetUp ()
    {
      base.SetUp ();

      _order = (Order) LifetimeService.GetObjectReference (ActiveSubTransaction, DomainObjectIDs.Order1);

      InstallExtensionMock ();
    }

    [Test]
    public void ObjectsLoading_Loaded_RaisedInAllHierarchyLevels ()
    {
      using (ExtensionStrictMock.GetMockRepository ().Ordered ())
      {
        ExtensionStrictMock
            .Expect (
                mock => mock.ObjectsLoading (
                    Arg.Is (InactiveRootTransaction),
                    Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { _order.ID })))
            .WhenCalled (mi => Assert.That (InactiveRootTransaction.IsActive, Is.False));
        ExtensionStrictMock
            .Expect (
                mock => mock.ObjectsLoaded (
                    Arg.Is (InactiveRootTransaction),
                    Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _order })))
            .WhenCalled (mi => Assert.That (InactiveRootTransaction.IsActive, Is.False));

        ExtensionStrictMock
            .Expect (
                mock => mock.ObjectsLoading (
                    Arg.Is (InactiveMiddleTransaction),
                    Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { _order.ID })))
            .WhenCalled (mi => Assert.That (InactiveMiddleTransaction.IsActive, Is.False));
        ExtensionStrictMock
            .Expect (
                mock => mock.ObjectsLoaded (
                    Arg.Is (InactiveMiddleTransaction),
                    Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _order })))
            .WhenCalled (mi => Assert.That (InactiveMiddleTransaction.IsActive, Is.False));

        ExtensionStrictMock
            .Expect (
                mock => mock.ObjectsLoading (
                    Arg.Is (ActiveSubTransaction),
                    Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { _order.ID })))
            .WhenCalled (mi => Assert.That (ActiveSubTransaction.IsActive, Is.True));
        ExtensionStrictMock
            .Expect (
                mock => mock.ObjectsLoaded (
                    Arg.Is (ActiveSubTransaction),
                    Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _order })))
            .WhenCalled (mi => Assert.That (ActiveSubTransaction.IsActive, Is.True));
      }

      ActiveSubTransaction.Execute (() => _order.EnsureDataAvailable());

      ExtensionStrictMock.VerifyAllExpectations();
    }

  }
}