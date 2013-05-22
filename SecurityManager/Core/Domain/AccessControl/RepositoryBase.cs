// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 

using System;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public abstract class RepositoryBase<TData>
      where TData : RepositoryBase<TData>.RevisionBasedData
  {
    public abstract class RevisionBasedData
    {
      private readonly int _revision;

      protected RevisionBasedData (int revision)
      {
        _revision = revision;
      }

      public int Revision
      {
        get { return _revision; }
      }
    }

    private readonly IRevisionProvider _revisionProvider;
    private readonly object _syncRoot = new object();
    private volatile TData _cachedData;

    protected RepositoryBase (IRevisionProvider revisionProvider)
    {
      ArgumentUtility.CheckNotNull ("revisionProvider", revisionProvider);

      _revisionProvider = revisionProvider;
    }

    protected abstract TData LoadData (int revision);

    protected TData GetCachedData ()
    {
      var currentRevision = _revisionProvider.GetRevision();
      if (_cachedData == null || _cachedData.Revision < currentRevision)
      {
        lock (_syncRoot)
        {
          if (_cachedData == null || _cachedData.Revision < currentRevision)
            _cachedData = LoadData (currentRevision);
        }
      }
      return _cachedData;
    }
  }
}