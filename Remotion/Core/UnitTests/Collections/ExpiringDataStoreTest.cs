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

//
using System;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class ExpiringDataStoreTest
  {
    private IExpirationPolicy<object, DateTime> _expirationPolicyMock;
    private ExpiringDataStore<string, object, DateTime> _dataStore;
    private object _fakeValue;
    private DateTime _fakeExpirationInfo;

    [SetUp]
    public void SetUp ()
    {
      _expirationPolicyMock = MockRepository.GenerateStrictMock<IExpirationPolicy<object, DateTime>>();
      _dataStore = new ExpiringDataStore<string, object, DateTime> (_expirationPolicyMock, new ReferenceEqualityComparer<string>());
      _fakeValue = new object();
      _fakeExpirationInfo = new DateTime (0);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_dataStore.IsNull, Is.False);
    }

    [Test]
    public void AddAndContainsKey_ShouldScanForExpiredItemsFalse ()
    {
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return(_fakeExpirationInfo);
      _expirationPolicyMock.Replay ();

      _dataStore.Add ("Test", _fakeValue);

      _expirationPolicyMock.VerifyAllExpectations ();
      Assert.That (_dataStore.ContainsKey ("Test"), Is.True);
    }

    [Test]
    public void AddAndContainsKey_ShouldScanForExpiredItemsTrue_NoValues ()
    {
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (true);
      _expirationPolicyMock.Expect (mock => mock.ItemsScanned ());
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Replay ();

      _dataStore.Add ("Test", _fakeValue);

      _expirationPolicyMock.VerifyAllExpectations ();
    }

    [Test]
    public void AddAndContainsKey_ShouldScanForExpiredItemsTrue_WithValues_NotExpired ()
    {
       _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo).Repeat.Any ();
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _dataStore.Add ("Test1", _fakeValue);
      _dataStore.Add ("Test2", _fakeValue);

      _expirationPolicyMock.BackToRecord ();
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (true);
      _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (false).Repeat.Twice ();
      _expirationPolicyMock.Expect (mock => mock.ItemsScanned ());
      _expirationPolicyMock.Replay ();

      _dataStore.Add ("Test", _fakeValue);

      _expirationPolicyMock.VerifyAllExpectations ();
    }

    [Test]
    public void AddAndContainsKey_ShouldScanForExpiredItemsTrue_WithValues_Expired ()
    {
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo).Repeat.Any ();
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _dataStore.Add ("Test1", _fakeValue);
      _dataStore.Add ("Test2", _fakeValue);

      _expirationPolicyMock.BackToRecord ();
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (true);
      _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (true).Repeat.Twice ();
      _expirationPolicyMock.Expect (mock => mock.ItemsScanned ());
      _expirationPolicyMock.Replay ();

      _dataStore.Add ("Test", _fakeValue);

      _expirationPolicyMock.VerifyAllExpectations ();
    }

    [Test]
    public void Remove_ShouldScanForExpiredItemsFalse_KeyDoesNotExist ()
    {
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Replay ();

      var result = _dataStore.Remove ("Test");

      _expirationPolicyMock.VerifyAllExpectations ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void Remove_ShouldScanForExpiredItemsFalse_KeyDoesExist ()
    {
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Replay ();
      _dataStore.Add ("Test", _fakeValue);

      var result = _dataStore.Remove ("Test");

      _expirationPolicyMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ShouldScanForExpiredItemsTrue_WithValues_NotExpired ()
    {
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo).Repeat.Any();
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _dataStore.Add ("Test1", _fakeValue);
      _dataStore.Add ("Test2", _fakeValue);

      _expirationPolicyMock.BackToRecord ();
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (true);
      _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (false).Repeat.Twice ();
      _expirationPolicyMock.Expect (mock => mock.ItemsScanned ());
      _expirationPolicyMock.Replay ();

      var result = _dataStore.Remove ("Test1");

      _expirationPolicyMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ShouldScanForExpiredItemsTrue_WithValues_Expired ()
    {
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo).Repeat.Any ();
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _dataStore.Add ("Test1", _fakeValue);
      _dataStore.Add ("Test2", _fakeValue);

      _expirationPolicyMock.BackToRecord ();
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (true);
      _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (true).Repeat.Twice ();
      _expirationPolicyMock.Expect (mock => mock.ItemsScanned ());
      _expirationPolicyMock.Replay ();

      var result = _dataStore.Remove ("Test1");

      _expirationPolicyMock.VerifyAllExpectations ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void ContainsKey_False ()
    {
      Assert.That (_dataStore.ContainsKey ("Test"), Is.False);
    }

    [Test]
    public void GetValue ()
    {
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Replay ();

      _dataStore.Add ("Test", _fakeValue);

      Assert.That (_dataStore["Test"], Is.SameAs (_fakeValue));
    }

    [Test]
    public void SetValue ()
    {
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Replay();

      Assert.That (_dataStore.ContainsKey ("Test"), Is.False);

      _dataStore["Test"] = _fakeValue;

      _expirationPolicyMock.VerifyAllExpectations();
      Assert.That (_dataStore.ContainsKey ("Test"), Is.True);
    }

    [Test]
    public void Clear_NoItems ()
    {
      _expirationPolicyMock.Replay ();

      _dataStore.Clear ();

      _expirationPolicyMock.VerifyAllExpectations ();
    }

    [Test]
    public void Clear_WithItems ()
    {
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo).Repeat.Any();
      _expirationPolicyMock.Replay ();
      _dataStore.Add ("Test1", _fakeValue);
      _dataStore.Add ("Test2", _fakeValue);
      Assert.That (_dataStore.ContainsKey ("Test1"), Is.True);
      Assert.That (_dataStore.ContainsKey ("Test2"), Is.True);

      _dataStore.Clear ();

      _expirationPolicyMock.VerifyAllExpectations ();
      Assert.That (_dataStore.ContainsKey ("Test1"), Is.False);
      Assert.That (_dataStore.ContainsKey ("Test2"), Is.False);
    }

    [Test]
    public void TryGetValue_KeyDoesNotExist ()
    {
      object value;

      var result = _dataStore.TryGetValue ("Test", out value);

      Assert.That (result, Is.False);
    }

    [Test]
    public void TryGetValue_KeyDoesExist_ShouldScanForExpiredItemsFalse_NotExpired ()
    {
      object value;
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Stub (stub => stub.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (false);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Replay ();
      _dataStore.Add ("Test", _fakeValue);

      var result = _dataStore.TryGetValue ("Test", out value);

      _expirationPolicyMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
      Assert.That (value, Is.SameAs (_fakeValue));
    }

    [Test]
    public void TryGetValue_KeyDoesExist_ShouldScanForExpiredItemsFalse_Expired ()
    {
      object value;
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Stub (stub => stub.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (true);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Replay ();
      _dataStore.Add ("Test", _fakeValue);

      var result = _dataStore.TryGetValue ("Test", out value);

      _expirationPolicyMock.VerifyAllExpectations ();
      Assert.That (result, Is.False);
      Assert.That (value, Is.Null);
    }

    [Test]
    public void TryGetValue_KeyDoesExist_ShouldScanForExpiredItemsTrue_NotExpired ()
    {
      object value;
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (true);
      _expirationPolicyMock.Stub (stub => stub.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (false);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Expect (mock => mock.ItemsScanned ());
      _expirationPolicyMock.Replay ();
      _dataStore.Add ("Test", _fakeValue);

      var result = _dataStore.TryGetValue ("Test", out value);

      _expirationPolicyMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
      Assert.That (value, Is.SameAs (_fakeValue));
    }

    [Test]
    public void TryGetValue_KeyDoesExist_ShouldScanForExpiredItemsTrue_Expired ()
    {
      object value;
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (true);
      _expirationPolicyMock.Stub (stub => stub.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (true);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Expect (mock => mock.ItemsScanned ());
      _expirationPolicyMock.Replay ();
      _dataStore.Add ("Test", _fakeValue);

      var result = _dataStore.TryGetValue ("Test", out value);

      _expirationPolicyMock.VerifyAllExpectations ();
      Assert.That (result, Is.False);
      Assert.That (value, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_KeyDoesExist_NotExpired ()
    {
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (false);
      _expirationPolicyMock.Replay ();
      _dataStore.Add ("Test", _fakeValue);

      var result = _dataStore.GetOrCreateValue ("Test", k => new object ());

      Assert.That (result, Is.SameAs (_fakeValue));
    }

    [Test]
    public void GetOrCreateValue_KeyDoesExist_Expired ()
    {
      var newValue = new object ();
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (newValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (true);
      _expirationPolicyMock.Replay ();
      _dataStore.Add ("Test", _fakeValue);

      var result = _dataStore.GetOrCreateValue ("Test", k => newValue);

      Assert.That (result, Is.SameAs (newValue));
    }

    [Test]
    public void GetOrCreateValue_KeyDoesNotExist ()
    {
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Replay ();

      var result = _dataStore.GetOrCreateValue ("Test", k => _fakeValue);

      Assert.That (result, Is.SameAs (_fakeValue));
    }

    [Test]
    public void GetValueOrDefault_KeyDoesNotExist ()
    {
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Replay ();

      var result = _dataStore.GetValueOrDefault ("Test");

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetValueOrDefault_KeyDoesExist_NotExpired ()
    {
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (false);
      _expirationPolicyMock.Replay ();
      _dataStore.Add ("Test", _fakeValue);

      var result = _dataStore.GetValueOrDefault ("Test");

      Assert.That (result, Is.SameAs (_fakeValue));
    }

    [Test]
    public void GetValueOrDefault_KeyDoesExist_Expired ()
    {
      _expirationPolicyMock.Stub (stub => stub.ShouldScanForExpiredItems ()).Return (false);
      _expirationPolicyMock.Expect (mock => mock.GetExpirationInfo (_fakeValue)).Return (_fakeExpirationInfo);
      _expirationPolicyMock.Expect (mock => mock.IsExpired (_fakeValue, _fakeExpirationInfo)).Return (true);
      _expirationPolicyMock.Replay ();
      _dataStore.Add ("Test", _fakeValue);

      var result = _dataStore.GetValueOrDefault ("Test");

      Assert.That (result, Is.Null);
    }
  }
}