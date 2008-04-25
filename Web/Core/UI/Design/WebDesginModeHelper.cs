using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using Remotion.Design;
using Remotion.Utilities;

namespace Remotion.Web.UI.Design
{
  /// <summary>
  /// Implementation of the <see cref="IDesignModeHelper"/> interface for environments implementing the <see cref="IWebApplication"/> designer service.
  /// </summary>
  public class WebDesginModeHelper: DesignModeHelperBase
  {
    public WebDesginModeHelper (IDesignerHost designerHost)
        : base (designerHost)
    {
    }

    public override string GetProjectPath()
    {
      return GetWebApplication().RootProjectItem.PhysicalPath;
    }

    public override System.Configuration.Configuration GetConfiguration()
    {
      return GetWebApplication().OpenWebConfiguration (true);
    }

    private IWebApplication GetWebApplication()
    {
      IWebApplication webApplication = (IWebApplication) DesignerHost.GetService (typeof (IWebApplication));
      Assertion.IsNotNull(webApplication, "The 'IServiceProvider' failed to return an 'IWebApplication' service.");

      return webApplication;
    }
  }
}