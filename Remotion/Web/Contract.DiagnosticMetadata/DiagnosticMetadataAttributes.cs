using System;

namespace Remotion.Web.Contract.DiagnosticMetadata
{
  /// <summary>
  /// Diagnostic metadata attribute names used by various renderers.
  /// </summary>
  public static class DiagnosticMetadataAttributes
  {
    public static readonly string CommandName = "data-commandname";
    public static readonly string ControlType = "data-control-type";
    public static readonly string IndexInCollection = "data-index";
    public static readonly string IsReadOnly = "data-is-readonly";
    public static readonly string ItemID = "data-item-id";
    public static readonly string Text = "data-text";
    public static readonly string TriggersNavigation = "data-triggers-navigation";
    public static readonly string TriggersPostBack = "data-triggers-postback";

    public static readonly string WebTreeViewWellKnownAnchor = "data-webtreeview-wellknown-anchor";
  }
}