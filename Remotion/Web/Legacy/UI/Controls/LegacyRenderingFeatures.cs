using System;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Legacy.UI.Controls
{
  /// <summary>
  /// <see cref="IRenderingFeatures"/> implementations for the legacy rendering infrastructure.
  /// </summary>
  public static class LegacyRenderingFeatures
  {
    public static readonly IRenderingFeatures ForLegacy = RenderingFeatures.Default;
  }
}