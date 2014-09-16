using System;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Diagnostic metadata attribute names used by various renderers.
  /// </summary>
  public static class DiagnosticMetadataAttributes
  {
    public static readonly string DisplayName = "data-displayname";
    public static readonly string HasAutoPostBack = "data-autopostback";
    public static readonly string TriggersNavigation = "data-triggers-nav";
    public static readonly string IsBound = "data-is-bound";
    public static readonly string BoundType = "data-bound-type";
    public static readonly string BoundProperty = "data-bound-property";
  }
}