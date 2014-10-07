using System;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Diagnostic metadata attribute names used by various renderers.
  /// </summary>
  public static class DiagnosticMetadataAttributes
  {
    public static readonly string CommandName = "data-commandname";
    public static readonly string DisplayName = "data-displayname";
    public static readonly string IsReadOnly = "data-is-readonly";
    public static readonly string ItemID = "data-item-id";
    public static readonly string TriggersNavigation = "data-triggers-nav";
    public static readonly string TriggersPostBack = "data-triggers-pb";
  }
}