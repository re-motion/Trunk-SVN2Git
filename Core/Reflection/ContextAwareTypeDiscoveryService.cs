using System;
using System.ComponentModel.Design;
using System.Runtime.Remoting.Messaging;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  public static class ContextAwareTypeDiscoveryService
  {
    private static readonly string s_defaultServiceKey = typeof (ContextAwareTypeDiscoveryService).FullName + ".DefaultService";

    public static readonly CallContextSingleton<ITypeDiscoveryService> DefaultService =
        new CallContextSingleton<ITypeDiscoveryService> (s_defaultServiceKey, CreateDefaultService);

    private static ITypeDiscoveryService CreateDefaultService ()
    {
      return new AssemblyFinderTypeDiscoveryService (new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, false));
    }

    public static ITypeDiscoveryService GetInstance ()
    {
      if (DesignerUtility.IsDesignMode)
        return (ITypeDiscoveryService) DesignerUtility.DesignerHost.GetService (typeof (ITypeDiscoveryService));
      else
        return DefaultService.Current;
    }
  }
}