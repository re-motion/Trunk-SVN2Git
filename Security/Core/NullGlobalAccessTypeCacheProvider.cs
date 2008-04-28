using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using Remotion.Collections;
using Remotion.Configuration;
using System.Runtime.Serialization;
using Remotion.Security.Configuration;

namespace Remotion.Security
{
  /// <summary>
  /// Represents a nullable <see cref="IGlobalAccessTypeCacheProvider"/> according to the "Null Object Pattern".
  /// </summary>
  [Serializable]
  public class NullGlobalAccessTypeCacheProvider : ExtendedProviderBase, IGlobalAccessTypeCacheProvider
  {
    private readonly NullCache<Tuple<ISecurityContext, string>, AccessType[]> _cache = new NullCache<Tuple<ISecurityContext, string>, AccessType[]>();

    public NullGlobalAccessTypeCacheProvider()
        : this ("Null", new NameValueCollection())
    {
    }


    public NullGlobalAccessTypeCacheProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    protected NullGlobalAccessTypeCacheProvider (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }

    public ICache<Tuple<ISecurityContext, string>, AccessType[]> GetCache()
    {
      return _cache;
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }

    protected override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      if (this == SecurityConfiguration.Current.GlobalAccessTypeCacheProviders[Name])
        GlobalAccessTypeCacheProviderObjectReference.DoGetObjectDataForWellKnownProvider (this, info, context);
      else
        base.GetObjectData (info, context);
    }
  }
}