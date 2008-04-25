using System;
using System.ComponentModel.Design;
using Remotion.Utilities;

namespace Remotion.Design
{
  /// <summary>
  /// Base implementation of the <see cref="IDesignModeHelper"/> interface.
  /// </summary>
  public abstract class DesignModeHelperBase : IDesignModeHelper
  {
    private readonly IDesignerHost _designerHost;

    protected DesignModeHelperBase (IDesignerHost designerHost)
    {
      ArgumentUtility.CheckNotNull ("designerHost", designerHost);

      _designerHost = designerHost;
    }

    public abstract string GetProjectPath ();

    public abstract System.Configuration.Configuration GetConfiguration ();

    public IDesignerHost DesignerHost
    {
      get { return _designerHost; }
    }
  }
}