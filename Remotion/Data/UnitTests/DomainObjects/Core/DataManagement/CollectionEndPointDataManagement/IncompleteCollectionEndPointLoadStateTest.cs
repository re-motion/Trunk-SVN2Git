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
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement
{
  [TestFixture]
  public class IncompleteCollectionEndPointLoadStateTest : StandardMappingTest
  {
    private ICollectionEndPoint _collectionEndPointMock;
    private IRelationEndPointLazyLoader _lazyLoaderMock;

    private IncompleteCollectionEndPointLoadState _loadState;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      _lazyLoaderMock = MockRepository.GenerateStrictMock<IRelationEndPointLazyLoader> ();

      _loadState = new IncompleteCollectionEndPointLoadState (_collectionEndPointMock, _lazyLoaderMock);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _lazyLoaderMock.Expect (mock => mock.LoadLazyCollectionEndPoint (_collectionEndPointMock));
      _lazyLoaderMock.Replay();
      _collectionEndPointMock.Replay();
      
      _loadState.EnsureDataComplete();

      _lazyLoaderMock.VerifyAllExpectations();
      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void GetOriginalOppositeObjects ()
    {
      CheckOperationDelegatesToCompleteState (s => s.GetOriginalOppositeObjects(), new DomainObjectCollection());
    }

    [Test]
    public void GetOppositeRelationEndPoints ()
    {
      var dataManager = MockRepository.GenerateStub<IDataManager>();
      CheckOperationDelegatesToCompleteState (s => s.GetOppositeRelationEndPoints (dataManager), new IRelationEndPoint[0]);
    }

    [Test]
    public void CreateSetOppositeCollectionCommand ()
    {
      var domainObjectCollection = new DomainObjectCollection ();

      CheckOperationDelegatesToCompleteState (
          s => s.CreateSetOppositeCollectionCommand (domainObjectCollection),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var relatedObject = DomainObjectMother.CreateFakeObject<Order>();
      CheckOperationDelegatesToCompleteState (
          s => s.CreateRemoveCommand (relatedObject),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      CheckOperationDelegatesToCompleteState (s => s.CreateDeleteCommand (), MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateInsertCommand ()
    {
      var relatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      CheckOperationDelegatesToCompleteState (
          s => s.CreateInsertCommand (relatedObject, 0),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateAddCommand ()
    {
      var relatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      CheckOperationDelegatesToCompleteState (
          s => s.CreateAddCommand (relatedObject),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      var relatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      CheckOperationDelegatesToCompleteState (
          s => s.CreateReplaceCommand (0, relatedObject),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void SetValueFrom ()
    {
      var fakeSourceEndPoint = MockRepository.GenerateStub<ICollectionEndPoint> ();
      CheckOperationDelegatesToCompleteState (s => s.SetValueFrom (fakeSourceEndPoint));
    }

    [Test]
    public void CheckMandatory ()
    {
      CheckOperationDelegatesToCompleteState (s => s.CheckMandatory());
    }

    private void CheckOperationDelegatesToCompleteState (Action<ICollectionEndPointLoadState> operation)
    {
      var newStateMock = MockRepository.GenerateStrictMock<ICollectionEndPointLoadState> ();
      using (_collectionEndPointMock.GetMockRepository ().Ordered ())
      {
        _collectionEndPointMock.Expect (mock => mock.EnsureDataComplete ());
        _collectionEndPointMock.Expect (mock => mock.GetState ()).Return (newStateMock);
      }
      _collectionEndPointMock.Replay ();

      newStateMock.Expect (operation);
      newStateMock.Replay ();

      operation (_loadState);

      _collectionEndPointMock.VerifyAllExpectations ();
      newStateMock.VerifyAllExpectations ();
    }

    private void CheckOperationDelegatesToCompleteState<T> (Func<ICollectionEndPointLoadState, T> operation, T fakeResult)
    {
      var newStateMock = MockRepository.GenerateStrictMock<ICollectionEndPointLoadState> ();
      using (_collectionEndPointMock.GetMockRepository ().Ordered ())
      {
        _collectionEndPointMock.Expect (mock => mock.EnsureDataComplete ());
        _collectionEndPointMock.Expect (mock => mock.GetState ()).Return (newStateMock);
      }
      _collectionEndPointMock.Replay ();

      newStateMock.Expect (mock => operation (mock)).Return (fakeResult);
      newStateMock.Replay ();

      var result = operation (_loadState);

      _collectionEndPointMock.VerifyAllExpectations ();
      newStateMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (fakeResult));
    }
  }
}