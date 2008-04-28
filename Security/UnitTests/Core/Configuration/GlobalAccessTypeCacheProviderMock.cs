using System;
using System.Collections.Specialized;
using Remotion.Collections;
using Remotion.Configuration;

namespace Remotion.Security.UnitTests.Core.Configuration
{
  public class GlobalAccessTypeCacheProviderMock : ExtendedProviderBase, IGlobalAccessTypeCacheProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public GlobalAccessTypeCacheProviderMock (string name, NameValueCollection config)
        : base (name, config)
    {
    }
    
     // methods and properties

    public ICache<Tuple<ISecurityContext, string>, AccessType[]> GetCache ()
    {
      throw new NotImplementedException();
    }

    public bool IsNull
    {
      get { return false; }
    }
  }
}