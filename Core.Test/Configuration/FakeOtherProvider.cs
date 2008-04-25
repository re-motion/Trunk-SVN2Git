using System;
using System.Collections.Specialized;
using Remotion.Configuration;

namespace Remotion.UnitTests.Configuration
{
  public class FakeOtherProvider : ExtendedProviderBase
  {
    public FakeOtherProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }
  }
}