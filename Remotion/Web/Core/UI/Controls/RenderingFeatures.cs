using System;

namespace Remotion.Web.UI.Controls
{
  // Todo RM-6297: Unit tests?

  /// <summary>
  /// <see cref="IRenderingFeatures"/> implementations which are directly supported by the framework.
  /// </summary>
  public static class RenderingFeatures
  {
    public static readonly IRenderingFeatures Default = new DefaultRenderingFeatures();
    public static readonly IRenderingFeatures WithDiagnosticMetadata = new WithDiagnosticMetadataRenderingFeatures();
    public static readonly IRenderingFeatures ForLegacy = Default;
  }
}