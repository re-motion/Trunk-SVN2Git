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