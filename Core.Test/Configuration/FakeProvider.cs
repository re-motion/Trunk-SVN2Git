using System;
using System.Collections.Specialized;

namespace Remotion.UnitTests.Configuration
{
  public class FakeProvider : FakeProviderBase, IFakeProvider
  {
    public FakeProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }
  }
}