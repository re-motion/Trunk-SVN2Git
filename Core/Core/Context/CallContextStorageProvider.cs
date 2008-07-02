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
using System.Runtime.Remoting.Messaging;
using Remotion.BridgeInterfaces;
using Remotion.Utilities;

namespace Remotion.Context
{
  public class CallContextStorageProvider : ICallContextStorageProvider
  {
    public object GetData (string key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return CallContext.GetData (key);
    }

    public void SetData (string key, object value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      CallContext.SetData (key, value);
    }

    public void FreeData (string key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      CallContext.FreeNamedDataSlot (key);
    }
  }
}