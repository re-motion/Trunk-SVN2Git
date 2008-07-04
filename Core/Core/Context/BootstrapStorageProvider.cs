/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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