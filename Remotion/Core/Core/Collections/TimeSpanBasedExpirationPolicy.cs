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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// The <see cref="TimeSpanBasedExpirationPolicy{TValue}"/> handles values which can be expire based on <see cref="TimeSpan"/> periods.
  /// </summary>
  public class TimeSpanBasedExpirationPolicy<TValue> : IExpirationPolicy<TValue> where TValue : class
  {
    private readonly Dictionary<TValue, DateTime> _expirationTimes;
    private readonly TimeSpan _period;
    private readonly IUtcNowProvider _utcNowProvider;
    private DateTime _nextScan;

    public TimeSpanBasedExpirationPolicy (TimeSpan period, IUtcNowProvider utcNowProvider)
    {
      ArgumentUtility.CheckNotNull ("period", period);
      ArgumentUtility.CheckNotNull ("utcNowProvider", utcNowProvider);

      _expirationTimes = new Dictionary<TValue, DateTime> (new ReferenceEqualityComparer<TValue>());
      _period = period;
      _nextScan = utcNowProvider.UtcNow + period;
      _utcNowProvider = utcNowProvider;
    }

    public DateTime NextScan
    {
      get { return _nextScan; }
    }

    public Dictionary<TValue, DateTime> ExpirationTimes
    {
      get { return _expirationTimes; }
    }

    public void ItemAdded (TValue value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      _expirationTimes.Add (value, _utcNowProvider.UtcNow + _period);
    }

    public void ItemRemoved (TValue value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      _expirationTimes.Remove (value);
    }

    public void ItemsScanned ()
    {
      _nextScan = _utcNowProvider.UtcNow + _period;
    }

    public bool IsExpired (TValue value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      return _expirationTimes[value] <= _utcNowProvider.UtcNow;
    }

    public bool ShouldScanForExpiredItems ()
    {
      return _nextScan <= _utcNowProvider.UtcNow;
    }
  }
}