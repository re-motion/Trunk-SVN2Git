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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  public partial class DomainObjectCollectionTest
  {
    [Test]
    public void EventRaiser_BeginAdd ()
    {
      var collectionMock = new MockRepository ().PartialMock<DomainObjectCollection> ();
      collectionMock.Replay ();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.BeginAdd (1, _customer1);

      collectionMock.AssertWasCalled (
          mock => PrivateInvoke.InvokeNonPublicMethod (
                      mock,
                      "OnAdding",
                      Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _customer1)));
    }

    [Test]
    public void EventRaiser_EndAdd ()
    {
      var collectionMock = new MockRepository ().PartialMock<DomainObjectCollection> ();
      collectionMock.Replay ();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.EndAdd (1, _customer1);

      collectionMock.AssertWasCalled (
          mock => PrivateInvoke.InvokeNonPublicMethod (
                      mock,
                      "OnAdded",
                      Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _customer1)));
    }

    [Test]
    public void EventRaiser_BeginRemove ()
    {
      var collectionMock = new MockRepository ().PartialMock<DomainObjectCollection> ();
      collectionMock.Replay ();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.BeginRemove (1, _customer1);

      collectionMock.AssertWasCalled (
          mock => PrivateInvoke.InvokeNonPublicMethod (
                      mock,
                      "OnRemoving",
                      Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _customer1)));
    }

    [Test]
    public void EventRaiser_EndRemove ()
    {
      var collectionMock = new MockRepository ().PartialMock<DomainObjectCollection> ();
      collectionMock.Replay ();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.EndRemove (1, _customer1);

      collectionMock.AssertWasCalled (
          mock => PrivateInvoke.InvokeNonPublicMethod (
                      mock,
                      "OnRemoved",
                      Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _customer1)));
    }

    [Test]
    public void EventRaiser_BeginDelete ()
    {
      var collectionMock = new MockRepository ().PartialMock<DomainObjectCollection> ();
      collectionMock.Replay ();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.BeginDelete ();

      collectionMock.AssertWasCalled (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnDeleting"));
    }

    [Test]
    public void EventRaiser_EndDelete ()
    {
      var collectionMock = new MockRepository ().PartialMock<DomainObjectCollection> ();
      collectionMock.Replay ();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.EndDelete ();

      collectionMock.AssertWasCalled (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnDeleted"));
    }

    [Test]
    public void EventRaiser_WithinCommit ()
    {

      var collectionMock = new MockRepository ().PartialMock<DomainObjectCollection> ();
      collectionMock.Replay ();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.WithinCommit();

      collectionMock.AssertWasCalled (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnCommit"));
    }

    [Test]
    public void EventRaiser_WithinRollback ()
    {

      var collectionMock = new MockRepository ().PartialMock<DomainObjectCollection> ();
      collectionMock.Replay ();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.WithinRollback ();

      collectionMock.AssertWasCalled (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnRollback"));
    }

    [Test]
    public void EventRaiser_WithinAddItemThroughSynchronization ()
    {

      var collectionMock = new MockRepository ().PartialMock<DomainObjectCollection> ();
      collectionMock.Replay ();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.WithinAddItemThroughSynchronization (_customer1);

      collectionMock.AssertWasCalled (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnAddItemThroughSynchronization", _customer1));
    }

    [Test]
    public void EventRaiser_WithinRemoveItemThroughSynchronization ()
    {

      var collectionMock = new MockRepository ().PartialMock<DomainObjectCollection> ();
      collectionMock.Replay ();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.WithinRemoveItemThroughSynchronization (_customer1);

      collectionMock.AssertWasCalled (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnRemoveItemThroughSynchronization", _customer1));
    }

    [Test]
    public void EventRaiser_WithinMarkComplete ()
    {

      var collectionMock = new MockRepository ().PartialMock<DomainObjectCollection> ();
      collectionMock.Replay ();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.WithinMarkComplete();

      collectionMock.AssertWasCalled (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnMarkComplete"));
    }

    [Test]
    public void EventRaiser_WithinMarkIncomplete ()
    {

      var collectionMock = new MockRepository ().PartialMock<DomainObjectCollection> ();
      collectionMock.Replay ();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.WithinMarkIncomplete ();

      collectionMock.AssertWasCalled (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnMarkIncomplete"));
    }
  }
}