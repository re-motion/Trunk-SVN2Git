using System;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using Remotion.Development.Web.ResourceHosting;
using Remotion.ServiceLocation;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public class Global : HttpApplication
  {
    private static ResourceVirtualPathProvider _resourceVirtualPathProvider;

    protected void Application_Start (object sender, EventArgs e)
    {
      RegisterResourceVirtualPathProvider();
      SetRenderingFeatures(RenderingFeatures.WithDiagnosticMetadata);
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
              new ResourcePathMapping ("Remotion.Web", @"..\..\Web\Core\res")
          },
          FileExtensionHandlerMapping.Default);
      _resourceVirtualPathProvider.Register();
    }

    private void SetRenderingFeatures (IRenderingFeatures renderingFeatures)
    {
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => renderingFeatures);
      ServiceLocator.SetLocatorProvider (() => serviceLocator);
    }
  }
}