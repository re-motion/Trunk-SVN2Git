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
using System.Configuration.Provider;
using System.Runtime.Serialization;
using Remotion.Security.Configuration;
using Remotion.Utilities;

namespace Remotion.Security
{
  [Serializable]
  internal class GlobalAccessTypeCacheProviderObjectReference : ISerializable, IObjectReference
  {
    internal static void DoGetObjectDataForWellKnownProvider (ProviderBase provider, SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("info", info);

      info.SetType (typeof (GlobalAccessTypeCacheProviderObjectReference));
      info.AddValue ("Name", provider.Name);
    }

    private readonly string _providerName;

    public GlobalAccessTypeCacheProviderObjectReference (SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _providerName = info.GetString ("Name");
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException ("This method should never be called.");
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      ProviderBase realObject = SecurityConfiguration.Current.GlobalAccessTypeCacheProviders[_providerName];
      if (realObject == null)
      {
        string message = string.Format ("No GlobalAccessTypeCacheProvider named '0' is registered in the current security configuration.", _providerName);
        throw new SerializationException (message);
      }
      else
        return realObject;
    }
  }
}
