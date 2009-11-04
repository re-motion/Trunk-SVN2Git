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
