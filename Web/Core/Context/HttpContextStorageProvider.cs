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
using Remotion.Context;
using System.Web;
using Remotion.Utilities;

namespace Remotion.Web.Context
{
  public class HttpContextStorageProvider : ISafeContextStorageProvider
  {
    private readonly CallContextStorageProvider _fallbackProvider = new CallContextStorageProvider ();

    public object GetData (string key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      if (HttpContext.Current != null)
        return HttpContext.Current.Items[key];
      else
        return _fallbackProvider.GetData (key);
    }

    public void SetData (string key, object value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      if (HttpContext.Current != null)
        HttpContext.Current.Items[key] = value;
      else 
        _fallbackProvider.SetData (key, value);
    }

    public void FreeData (string key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      if (HttpContext.Current != null)
        HttpContext.Current.Items.Remove (key);
      else
        _fallbackProvider.FreeData (key);
    }
  }
}