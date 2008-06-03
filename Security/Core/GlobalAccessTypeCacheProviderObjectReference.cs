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
