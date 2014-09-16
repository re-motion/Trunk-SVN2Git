using System;
using Remotion.ServiceLocation;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Default <see cref="IRenderingFeatures"/> implementation.
  /// </summary>
  [ImplementationFor (typeof (IRenderingFeatures), Lifetime = LifetimeKind.Singleton)]
  public class DefaultRenderingFeatures : IRenderingFeatures
  {
    public bool EnableDiagnosticMetadata
    {
      get { return false; }
    }
  }
}