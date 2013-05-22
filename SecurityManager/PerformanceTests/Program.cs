using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Logging;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.PerformanceTests
{
  class Program
  {
    static void Main (string[] args)
    {
      var defaultServiceLocator = new DefaultServiceLocator();

      //defaultServiceLocator.Register (typeof (Remotion.Data.DomainObjects.IClientTransactionExtensionFactory), typeof (Remotion.Data.DomainObjects.UberProfIntegration.LinqToSqlExtensionFactory), LifetimeKind.Singleton);
      //defaultServiceLocator.Register (typeof (Remotion.Data.DomainObjects.Tracing.IPersistenceExtensionFactory), typeof (Remotion.Data.DomainObjects.UberProfIntegration.LinqToSqlExtensionFactory), LifetimeKind.Singleton);

      ServiceLocator.SetLocatorProvider (() => defaultServiceLocator);

      LogManager.Initialize();

      SecurityService provider = new SecurityService("SecurityManager", new NameValueCollection());
      var context =
          new SimpleSecurityContext (
             "ActaNova.Federal.Domain.File, ActaNova.Federal.Domain",
              "ServiceUser",
              string.Empty,
              "SystemTenant",
              false,
              new Dictionary<string, EnumWrapper> { { "CommonFileState", EnumWrapper.Get ("Work|ActaNova.Domain.CommonFile+CommonFileStateType, ActaNova.Domain") } },
              new EnumWrapper[0]);
      ISecurityPrincipal user = new SecurityPrincipal ("ServiceUser", null, null, null);
        //new SecurityService().GetAccess (ClientTransaction.CreateRootTransaction(), context, user);
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction();
      //using (StopwatchScope.CreateScope ("{elapsed:ms}"))
      {
        provider.GetAccess (context, user);
      }
      Console.WriteLine ("Init done");
      Console.ReadKey();

      Stopwatch stopwatch = Stopwatch.StartNew();
      int dummy = 0;
      int count = 10;
      for (int i = 0; i < count; i++)
      {
        dummy += provider.GetAccess (context, user).Length;
      }
      stopwatch.Stop();
      Trace.Write (dummy);
      Console.WriteLine ("Time taken: {0}ms", ((decimal)stopwatch.ElapsedMilliseconds)/count);
      Console.ReadKey();
    }
  }
}
