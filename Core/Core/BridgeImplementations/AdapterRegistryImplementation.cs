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
using System.Collections.Generic;
using Remotion.BridgeInterfaces;
using Remotion.Utilities;

namespace Remotion.BridgeImplementations
{
  public class AdapterRegistryImplementation : IAdapterRegistryImplementation
  {
    private readonly Dictionary<Type, IAdapter> _registry = new Dictionary<Type, IAdapter> ();

    public AdapterRegistryImplementation()
    {
    }

    public void SetAdapter (Type adapterType, IAdapter value)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("adapterType", adapterType, typeof (IAdapter));
      ArgumentUtility.CheckType ("value", value, adapterType);

      _registry[adapterType] = value;
    }

    public T GetAdapter<T>() where T : class, IAdapter
    {
      if (_registry.ContainsKey (typeof (T)))
        return (T) _registry[typeof (T)];
      else
        return null;
    }

  }
}
