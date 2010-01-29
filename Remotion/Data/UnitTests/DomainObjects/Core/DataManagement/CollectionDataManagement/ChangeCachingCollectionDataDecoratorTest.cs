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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class ChangeCachingCollectionDataDecoratorTest : StandardMappingTest
  {
    private IDomainObjectCollectionData _wrappedDataStub;
    private DomainObjectCollection _originalData;
    private ChangeCachingCollectionDataDecorator _decorator;
    private ICollectionEndPointChangeDetectionStrategy _strategyMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _wrappedDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData> ();
      _originalData = new DomainObjectCollection ();

      _decorator = new ChangeCachingCollectionDataDecorator (_wrappedDataStub, _originalData);

      _strategyMock = new MockRepository().StrictMock<ICollectionEndPointChangeDetectionStrategy> ();
    }

    [Test]
    public void HasChanged_UsesStrategy ()
    {
      _strategyMock.Expect (mock => mock.HasDataChanged (_decorator, _originalData)).Return (true);
      _strategyMock.Replay ();

      var result = _decorator.HasChanged (_strategyMock);
      
      _strategyMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChanged_CachesData ()
    {
      _strategyMock.Expect (mock => mock.HasDataChanged (_decorator, _originalData)).Return (true).Repeat.Once();
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
    public void InvalidateCache ()
    {
      StubStrategyMock (_decorator, _originalData);

      _decorator.HasChanged (_strategyMock);
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      _decorator.InvalidateCache ();
      
      Assert.That (_decorator.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void InvalidateCache_InvalidatedCacheLeadsToReEvaluation ()
    {
      using (_strategyMock.GetMockRepository ().Ordered ())
      {
        _strategyMock.Expect (mock => mock.HasDataChanged (_decorator, _originalData)).Return (true);
        _strategyMock.Expect (mock => mock.HasDataChanged (_decorator, _originalData)).Return (false);
      }
      _strategyMock.Replay ();

      var result1 = _decorator.HasChanged (_strategyMock);
      _decorator.InvalidateCache ();

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
    public void Insert_InvalidatesCache ()
    {
      WarmUpCache ();
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      var fake = DomainObjectMother.CreateFakeObject<Order> ();
      _decorator.Insert (0, fake);

      _wrappedDataStub.AssertWasCalled (stub => stub.Insert (0, fake));
      Assert.That (_decorator.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Remove_Object_InvalidatesCache_IfTrue ()
    {
      WarmUpCache ();
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      var fake = DomainObjectMother.CreateFakeObject<Order> ();
      _wrappedDataStub.Stub (stub => stub.Remove (fake)).Return (true);

      _decorator.Remove (fake);

      _wrappedDataStub.AssertWasCalled (stub => stub.Remove (fake));
      Assert.That (_decorator.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Remove_Object_LeavesCache_IfFalse ()
    {
      WarmUpCache ();
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      var fake = DomainObjectMother.CreateFakeObject<Order> ();
      _wrappedDataStub.Stub (stub => stub.Remove (fake)).Return (false);

      _decorator.Remove (fake);

      _wrappedDataStub.AssertWasCalled (stub => stub.Remove (fake));
      Assert.That (_decorator.IsCacheUpToDate, Is.True);
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
    public void Replace_InvalidatesCache ()
    {
      WarmUpCache ();
      Assert.That (_decorator.IsCacheUpToDate, Is.True);

      var fake = DomainObjectMother.CreateFakeObject<Order> ();
      _decorator.Replace (0, fake);

      _wrappedDataStub.AssertWasCalled (stub => stub.Replace (0, fake));
      Assert.That (_decorator.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Serializable ()
    {
      var fake = DomainObjectMother.CreateFakeObject<Order> ();

      var wrappedData = new DomainObjectCollectionData (new[] { fake });
      var originalData = new DomainObjectCollection ();
      var decorator = new ChangeCachingCollectionDataDecorator (wrappedData, originalData);

      WarmUpCache (decorator, originalData);

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
      WarmUpCache(_decorator, _originalData);
    }

    private void WarmUpCache (ChangeCachingCollectionDataDecorator decorator, DomainObjectCollection originalData)
    {
      StubStrategyMock (decorator, originalData);

      decorator.HasChanged (_strategyMock);
      Assert.That (decorator.IsCacheUpToDate, Is.True);
    }

    private void StubStrategyMock (ChangeCachingCollectionDataDecorator decorator, DomainObjectCollection originalData)
    {
      _strategyMock.Stub (mock => mock.HasDataChanged (decorator, originalData)).Return (false);
      _strategyMock.Replay ();
    }

  }
}