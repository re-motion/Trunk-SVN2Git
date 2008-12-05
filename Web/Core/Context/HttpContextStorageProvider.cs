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
