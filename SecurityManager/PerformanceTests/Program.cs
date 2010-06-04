using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Remotion.Security;
using Remotion.Security.Configuration;

namespace Remotion.SeucrityManager.PerformanceTests
{
  class Program
  {
    static void Main (string[] args)
    {
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
      Thread.Sleep (2000);
      Stopwatch stopwatch = Stopwatch.StartNew();

      int dummy = 0;
      for (int i = 0; i < 10; i++)
        dummy += provider.GetAccess (context, user).Length;
      stopwatch.Stop();
      Trace.Write (dummy);
      Console.WriteLine ("Time taken: {0}ms", stopwatch.ElapsedMilliseconds/10.0);
    }
  }
}
