// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.BridgeInterfaces;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Context
{
  /// <summary>
  /// Implements <see cref="ISafeContextStorageProvider"/> for bootstrapping of <see cref="SafeContext"/>. This provider should not be
  /// used for any other purpose because it does not store its data in a thread-local way.
  /// </summary>
  public class BootstrapStorageProvider : IBootstrapStorageProvider
  {
    private readonly SimpleDataStore<string, object> _data = new SimpleDataStore<string, object> ();
    
    public object GetData (string key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _data.GetValueOrDefault (key);
    }

    public void SetData (string key, object value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      _data[key] = value;
    }

    public void FreeData (string key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      _data.Remove (key);
    }
  }
}
