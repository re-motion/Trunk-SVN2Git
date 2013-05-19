using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.ServiceLocation;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.PerformanceTests
{
  class Program
  {
    static void Main (string[] args)
    {
      //var defaultServiceLocator = new DefaultServiceLocator();

      //defaultServiceLocator.Register (typeof (Remotion.Data.DomainObjects.IClientTransactionExtensionFactory), typeof (Remotion.Data.DomainObjects.UberProfIntegration.LinqToSqlExtensionFactory), LifetimeKind.Singleton);
      //defaultServiceLocator.Register (typeof (Remotion.Data.DomainObjects.Tracing.IPersistenceExtensionFactory), typeof (Remotion.Data.DomainObjects.UberProfIntegration.LinqToSqlExtensionFactory), LifetimeKind.Singleton);

      //ServiceLocator.SetLocatorProvider (() => defaultServiceLocator);

      ISecurityProvider provider = SecurityConfiguration.Current.SecurityProvider;
      var context =
          new SimpleSecurityContext (
             "ActaNova.Federal.Domain.File, ActaNova.Federal.Domain",
              "TestBenutzer",
              string.Empty,
              "{00000001-0000-0000-0000-000000000000}",
              false,
              new Dictionary<string, EnumWrapper> { { "CommonFileState", EnumWrapper.Get ("Work|ActaNova.Domain.CommonFile+CommonFileStateType, ActaNova.Domain") } },
              new EnumWrapper[0]);
      ISecurityPrincipal user = new SecurityPrincipal ("TestBenutzer", null, null, null);
      provider.GetAccess (context, user);
      //Console.ReadKey();

      Stopwatch stopwatch = Stopwatch.StartNew();

      int dummy = 0;
      int count = 10;
      for (int i = 0; i < count; i++)
        dummy += provider.GetAccess (context, user).Length;
      stopwatch.Stop();
      Trace.Write (dummy);
      Console.WriteLine ("Time taken: {0}ms", ((decimal)stopwatch.ElapsedMilliseconds)/count);
      Console.ReadKey();
    }
  }
}
