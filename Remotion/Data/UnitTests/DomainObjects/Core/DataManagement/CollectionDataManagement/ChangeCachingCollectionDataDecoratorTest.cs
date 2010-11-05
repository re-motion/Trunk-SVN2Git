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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class ChangeCachingCollectionDataDecoratorTest : StandardMappingTest
  {
    private Order _domainObject;

    private IDomainObjectCollectionData _wrappedDataStub;
    private ICollectionDataStateUpdateListener _stateUpdateListenerMock;

    private ChangeCachingCollectionDataDecorator _decorator;
    
    private ICollectionEndPointChangeDetectionStrategy _strategyMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainObject = DomainObjectMother.CreateFakeObject<Order> ();

      _wrappedDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData> ();
      _wrappedDataStub.Stub (stub => stub.GetEnumerator ()).Return (new List<DomainObject>().GetEnumerator());
      _stateUpdateListenerMock = MockRepository.GenerateMock<ICollectionDataStateUpdateListener> ();

      _decorator = new ChangeCachingCollectionDataDecorator (_wrappedDataStub, _stateUpdateListenerMock);

      _strategyMock = new MockRepository().StrictMock<ICollectionEndPointChangeDetectionStrategy> ();
    }

    [Test]
    public void OriginalData_IsReadOnly ()
    {
      Assert.That (_decorator.OriginalData.IsReadOnly, Is.True);
      Assert.That (_decorator.OriginalData, Is.TypeOf (typeof (ReadOnlyCollectionDataDecorator)));

      var originalData = (ReadOnlyCollectionDataDecorator) _decorator.OriginalData;
      Assert.That (originalData.IsGetDataStoreAllowed, Is.False);
    }

    [Test]
    public void OriginalData_PointsToActualData_AfterInitialization ()
    {
      var originalData = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<OriginalDomainObjectCollectionData> (
          (ReadOnlyCollectionDataDecorator) _decorator.OriginalData);

      var originalDataStore = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<IDomainObjectCollectionData> (originalData);
      Assert.That (originalDataStore, Is.SameAs (_decorator));
    }

    [Test]
    public void HasChanged_UsesStrategy ()
    {
      _strategyMock.Expect (mock => mock.HasDataChanged (Arg.Is (_decorator), Arg<IDomainObjectCollectionData>.Is.Anything))
               .Return (true)
               .WhenCalled (mi => CheckOriginalDataMatches (_decorator.OriginalData, (IDomainObjectCollectionData) mi.Arguments[1]))
               .Repeat.Once ();
      _strategyMock.Replay ();

      var result = _decorator.HasChanged (_strategyMock);
      
      _strategyMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChanged_RaisesStateChangedNotification ()
    {
      _strategyMock.Expect (mock => mock.HasDataChanged (Arg.Is (_decorator), Arg<IDomainObjectCollectionData>.Is.Anything))
          .Return (true)
          .Repeat.Once ();
      _strategyMock.Replay ();

      _decorator.HasChanged (_strategyMock);

      _strategyMock.VerifyAllExpectations ();

      _stateUpdateListenerMock.AssertWasCalled (mock => mock.StateUpdated (true));
    }

    [Test]
    public void HasChanged_CachesData ()
    {
      _strategyMock.Expect (mock => mock.HasDataChanged (Arg.Is (_decorator), Arg<IDomainObjectCollectionData>.Is.Anything))
          .Return (true)
          .Repeat.Once();
      _strategyMock.Replay ();
      Assert.That (_decorator.IsCacheUpToDate, Is.False);

      var result1 = _decorator.HasChanged (_strategyMock);
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      var result2 = _decorator.HasChanged (_strategyMock);
      var result3 = _decorator.HasChanged (_strategyMock);

      _strategyMock.VerifyAllExpectations ();
      Assert.That (result1, Is.True);
      Assert.That (result2, Is.True);
      Assert.That (result3, Is.True);
    }

    [Test]
    public void CallOnDataChanged ()
    {
      _decorator.HasChanged (_strategyMock);
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      CallOnDataChanged (_decorator);
      
      Assert.That (_decorator.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void OnDataChanged_RaisesStateNotification ()
    {
      CallOnDataChanged (_decorator);

      _stateUpdateListenerMock.AssertWasCalled (mock => mock.StateUpdated (null));
    }

    [Test]
    public void OnDataChanged_InvalidatedCacheLeadsToReEvaluation ()
    {
      using (_strategyMock.GetMockRepository ().Ordered ())
      {
        _strategyMock.Expect (mock => mock.HasDataChanged (Arg.Is (_decorator), Arg<IDomainObjectCollectionData>.Is.Anything)).Return (true);
        _strategyMock.Expect (mock => mock.HasDataChanged (Arg.Is (_decorator), Arg<IDomainObjectCollectionData>.Is.Anything)).Return (false);
      }
      _strategyMock.Replay ();

      var result1 = _decorator.HasChanged (_strategyMock);
      CallOnDataChanged (_decorator);

      var result2 = _decorator.HasChanged (_strategyMock);

      _strategyMock.VerifyAllExpectations ();

      Assert.That (result1, Is.True);
      Assert.That (result2, Is.False);
    }

    [Test]
    public void GetDataStore ()
    {
      Assert.That (_decorator.GetDataStore (), Is.SameAs (_decorator));
    }

    [Test]
    public void Clear_InvalidatesCache ()
    {
      WarmUpCache();
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      _decorator.Clear ();

      _wrappedDataStub.AssertWasCalled (stub => stub.Clear ());
      Assert.That (_decorator.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Clear_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification ((d, obj) => d.Clear());
    }

    [Test]
    public void Insert_InvalidatesCache ()
    {
      WarmUpCache ();
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      _decorator.Insert (0, _domainObject);

      _wrappedDataStub.AssertWasCalled (stub => stub.Insert (0, _domainObject));
      Assert.That (_decorator.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Insert_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification ((d, obj) => d.Insert (0, _domainObject));
    }

    [Test]
    public void Remove_Object_InvalidatesCache_IfTrue ()
    {
      WarmUpCache ();
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      _wrappedDataStub.Stub (stub => stub.Remove (_domainObject)).Return (true);

      _decorator.Remove (_domainObject);

      _wrappedDataStub.AssertWasCalled (stub => stub.Remove (_domainObject));
      Assert.That (_decorator.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Remove_Object_LeavesCache_IfFalse ()
    {
      WarmUpCache ();
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      _wrappedDataStub.Stub (stub => stub.Remove (_domainObject)).Return (false);

      _decorator.Remove (_domainObject);

      _wrappedDataStub.AssertWasCalled (stub => stub.Remove (_domainObject));
      Assert.That (_decorator.IsCacheUpToDate, Is.True);
    }

    [Test]
    public void Remove_Object_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification ((d, obj) => d.Remove (obj));
    }

    [Test]
    public void Remove_ID_InvalidatesCache_IfTrue ()
    {
      WarmUpCache ();
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      _wrappedDataStub.Stub (stub => stub.Remove (DomainObjectIDs.Order1)).Return (true);

      _decorator.Remove (DomainObjectIDs.Order1);

      _wrappedDataStub.AssertWasCalled (stub => stub.Remove (DomainObjectIDs.Order1));
      Assert.That (_decorator.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Remove_ID_LeavesCache_IfFalse ()
    {
      WarmUpCache ();
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      _wrappedDataStub.Stub (stub => stub.Remove (DomainObjectIDs.Order1)).Return (false);

      _decorator.Remove (DomainObjectIDs.Order1);

      _wrappedDataStub.AssertWasCalled (stub => stub.Remove (DomainObjectIDs.Order1));
      Assert.That (_decorator.IsCacheUpToDate, Is.True);
    }

    [Test]
    public void Remove_ID_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification ((d, obj) => d.Remove (obj.ID));
    }

    [Test]
    public void Replace_InvalidatesCache ()
    {
      WarmUpCache ();
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      _decorator.Replace (0, _domainObject);

      _wrappedDataStub.AssertWasCalled (stub => stub.Replace (0, _domainObject));
      Assert.That (_decorator.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Replace_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification ((d, obj) => d.Replace (0, _domainObject));
    }

    [Test]
    public void Commit_ReversOriginalObjects_ToCurrentObjects ()
    {
      var wrappedData = new DomainObjectCollectionData ();
      var decorator = new ChangeCachingCollectionDataDecorator (wrappedData, _stateUpdateListenerMock);
      decorator.Add (_domainObject);
      Assert.That (decorator.OriginalData.ToArray (), Is.Empty);

      decorator.Commit ();

      Assert.That (decorator.OriginalData.ToArray (), Is.EqualTo (new[] { _domainObject }));
    }

    [Test]
    public void Commit_SetsFlagUnchanged ()
    {
      var wrappedData = new DomainObjectCollectionData ();
      var decorator = new ChangeCachingCollectionDataDecorator (wrappedData, _stateUpdateListenerMock);
      decorator.Add (_domainObject);
      _strategyMock.Replay ();

      decorator.Commit ();

      Assert.That (decorator.HasChanged (_strategyMock), Is.False);
      _strategyMock.AssertWasNotCalled (
          mock => mock.HasDataChanged (Arg<IDomainObjectCollectionData>.Is.Anything, Arg<IDomainObjectCollectionData>.Is.Anything));
    }

    [Test]
    public void Commit_RaisesNotification ()
    {
      _decorator.Commit ();

      _stateUpdateListenerMock.AssertWasCalled (mock => mock.StateUpdated (false));
    }

    [Test]
    public void Serializable ()
    {
      var wrappedData = new DomainObjectCollectionData (new[] { _domainObject });
      var stateUpdateListenerStub = new FakeCollectionDataStateUpdateListener();
      var decorator = new ChangeCachingCollectionDataDecorator (wrappedData, stateUpdateListenerStub);

      WarmUpCache (decorator);

      Assert.That (decorator.Count, Is.EqualTo (1));
      Assert.That (decorator.IsCacheUpToDate, Is.True);
      Assert.That (decorator.HasChanged (_strategyMock), Is.False);

      var deserializedDecorator = Serializer.SerializeAndDeserialize (decorator);

      Assert.That (deserializedDecorator.Count, Is.EqualTo (1));
      Assert.That (deserializedDecorator.IsCacheUpToDate, Is.True);
      Assert.That (deserializedDecorator.HasChanged (_strategyMock), Is.False);
    }

    private void WarmUpCache ()
    {
      WarmUpCache (_decorator);
    }

    private void WarmUpCache (ChangeCachingCollectionDataDecorator decorator)
    {
      StubStrategyMock (decorator);

      decorator.HasChanged (_strategyMock);
      Assert.That (decorator.IsCacheUpToDate, Is.True);
    }

    private void StubStrategyMock (ChangeCachingCollectionDataDecorator decorator)
    {
      _strategyMock.Stub (mock => mock.HasDataChanged (Arg.Is (decorator), Arg<IDomainObjectCollectionData>.Is.Anything)).Return (false);
      _strategyMock.Replay ();
    }

    private void CallOnDataChanged (ChangeCachingCollectionDataDecorator decorator)
    {
      PrivateInvoke.InvokeNonPublicMethod (decorator, "OnDataChanged");
    }

    private void CheckOriginalValuesCopiedBeforeModification (Action<ChangeCachingCollectionDataDecorator, DomainObject> action)
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();

      var wrappedData = new DomainObjectCollectionData (new[] { domainObject1, domainObject2 });
      var decorator = new ChangeCachingCollectionDataDecorator (wrappedData, _stateUpdateListenerMock);

      action (decorator, domainObject1);

      Assert.That (decorator.OriginalData.ToArray (), Is.EqualTo (new[] { domainObject1, domainObject2 }));
    }

    private void CheckOriginalDataMatches (IDomainObjectCollectionData expected, IDomainObjectCollectionData actual)
    {
      var expectedInner = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<OriginalDomainObjectCollectionData> (
          (ReadOnlyCollectionDataDecorator) expected);
      var actualInner = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<OriginalDomainObjectCollectionData> (
          (ReadOnlyCollectionDataDecorator) actual);
      Assert.That (actualInner, Is.SameAs (expectedInner));
    }
  }
}