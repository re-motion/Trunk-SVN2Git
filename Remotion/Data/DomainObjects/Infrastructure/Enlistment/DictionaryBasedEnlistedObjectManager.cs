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
using Remotion.Collections;

namespace Remotion.Data.DomainObjects.Infrastructure.Enlistment
{
  /// <summary>
  /// Manages the enlisted objects via a <see cref="Dictionary{TKey,TValue}"/>.
  /// </summary>
  [Serializable]
  public class DictionaryBasedEnlistedObjectManager<TKey, TObject> : IEnlistedObjectManager<TKey, TObject>
      where TObject : class
  {
    private readonly Func<TObject, TKey> _keyFunc;
    private readonly Dictionary<TKey, TObject> _enlistedObjects = new Dictionary<TKey, TObject> ();

    public DictionaryBasedEnlistedObjectManager (Func<TObject, TKey> keyFunc)
    {
      ArgumentUtility.CheckNotNull ("keyFunc", keyFunc);
      _keyFunc = keyFunc;
    }

    public int EnlistedObjectCount
    {
      get { return _enlistedObjects.Count; }
    }

    public IEnumerable<TObject> GetEnlistedObjects ()
    {
      return _enlistedObjects.Values;
    }

    public TObject GetEnlistedObject (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      return _enlistedObjects.GetValueOrDefault (key);
    }

    public bool EnlistObject (TObject instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      var key = _keyFunc (instance);
      var alreadyEnlistedObject = GetEnlistedObject (key);
      if (alreadyEnlistedObject != null && alreadyEnlistedObject != instance)
      {
        string message = string.Format ("An instance for object '{0}' already exists in this transaction.", key);
        throw new InvalidOperationException (message);
      }
      else if (alreadyEnlistedObject == null)
      {
        _enlistedObjects.Add (key, instance);
        return true;
      }
      else
      {
        return false;
      }
    }

    public bool IsEnlisted (TObject instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      var key = _keyFunc (instance);
      return GetEnlistedObject (key) == instance;
    }
  }
}