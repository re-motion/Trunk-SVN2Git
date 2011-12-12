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
using System;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Enlistment
{
  /// <summary>
  /// Implements the <see cref="IEnlistedObjectManager{TKey,TObject}"/> interface by delegating to a given <see cref="TargetManager"/>. 
  /// Every object registered with this manager is actually registered in the <see cref="TargetManager"/>.
  /// </summary>
  [Serializable]
  public class DelegatingEnlistedObjectManager<TKey, TObject> : IEnlistedObjectManager<TKey, TObject>
  {
    private readonly IEnlistedObjectManager<TKey, TObject> _targetManager;

    public DelegatingEnlistedObjectManager (IEnlistedObjectManager<TKey, TObject> targetManager)
    {
      ArgumentUtility.CheckNotNull ("targetManager", targetManager);
      _targetManager = targetManager;
    }

    public IEnlistedObjectManager<TKey, TObject> TargetManager
    {
      get { return _targetManager; }
    }

    public int EnlistedObjectCount
    {
      get { return _targetManager.EnlistedObjectCount; }
    }

    public IEnumerable<TObject> GetEnlistedObjects ()
    {
      return _targetManager.GetEnlistedObjects ();
    }

    public TObject GetEnlistedObject (TKey key)
    {
      return _targetManager.GetEnlistedObject (key);
    }

    public bool EnlistObject (TObject instance)
    {
      return _targetManager.EnlistObject (instance);
    }

    public bool IsEnlisted (TObject instance)
    {
      return _targetManager.IsEnlisted (instance);
    }
  }
}