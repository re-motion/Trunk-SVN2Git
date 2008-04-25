using System;
using Remotion.Security.Configuration;

namespace Remotion.Security.UnitTests.Core.Configuration
{
  public class SecurityConfigurationMock : SecurityConfiguration
  {
    public new static void SetCurrent (SecurityConfiguration configuration)
    {
      SecurityConfiguration.SetCurrent (configuration);
    }

    public SecurityConfigurationMock()
    {
    }
  }
}