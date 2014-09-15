using System;
using System.Web;
using Remotion.Development.Web.ResourceHosting;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public class Global : HttpApplication
  {
    private static ResourceVirtualPathProvider _resourceVirtualPathProvider;

    protected void Application_Start (object sender, EventArgs e)
    {
      RegisterResourceVirtualPathProvider();
    }

    protected void Application_BeginRequest (Object sender, EventArgs e)
    {
      _resourceVirtualPathProvider.HandleBeginRequest();
    }

    private static void RegisterResourceVirtualPathProvider ()
    {
      _resourceVirtualPathProvider = new ResourceVirtualPathProvider (
          new[]
          {
              new ResourcePathMapping ("Remotion.Web", @"..\..\Web\Core\res"),
          },
          FileExtensionHandlerMapping.Default);
      _resourceVirtualPathProvider.Register();
    }
  }
}