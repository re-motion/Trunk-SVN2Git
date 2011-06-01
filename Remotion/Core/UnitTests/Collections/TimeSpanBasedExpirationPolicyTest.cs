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

using System;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class TimeSpanBasedExpirationPolicyTest
  {
    private IUtcNowProvider _utcProviderStub;
    private TimeSpanBasedExpirationPolicy<string> _policy;

    [SetUp]
    public void SetUp ()
    {
      _utcProviderStub = MockRepository.GenerateStub<IUtcNowProvider>();
      _policy = new TimeSpanBasedExpirationPolicy<string> (TimeSpan.FromTicks(1), _utcProviderStub);
    }

    [Test]
    public void ItemsScanned ()
    {
      Assert.That (_policy.NextScan.Ticks, Is.EqualTo (1));

      _utcProviderStub.Stub (stub => stub.UtcNow).Return (new DateTime (1));

      _policy.ItemsScanned();

      Assert.That (_policy.NextScan.Ticks, Is.EqualTo (2));
    }

    [Test]
    public void IsExpired_True ()
    {
      var result = _policy.IsExpired ("Test", new DateTime(0));

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsExpired_False ()
    {
      var result = _policy.IsExpired ("Test", new DateTime (2));

      Assert.That (result, Is.False);
    }

    [Test]
    public void ShouldScanForExpiredItems_False ()
    {
      Assert.That (_policy.ShouldScanForExpiredItems(), Is.False);
    }

    [Test]
    public void ShouldScanForExpiredItems_True ()
    {
      _utcProviderStub.Stub (stub => stub.UtcNow).Return (new DateTime (2));

      Assert.That (_policy.ShouldScanForExpiredItems(), Is.True);
    }

    [Test]
    public void GetExpirationInfo ()
    {
      _utcProviderStub.Stub (stub => stub.UtcNow).Return (new DateTime (5));

      var result = _policy.GetExpirationInfo ("Test");

      Assert.That (result.Ticks, Is.EqualTo (6));
    }
  }
}