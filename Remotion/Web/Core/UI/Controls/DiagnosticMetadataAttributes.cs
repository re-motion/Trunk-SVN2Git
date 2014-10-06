using System;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Diagnostic metadata attribute names used by various renderers.
  /// </summary>
  public static class DiagnosticMetadataAttributes
  {
    // TODO RM-6297: When would title be needed, i.e. title is usally an attribute
    public static readonly string CommandName = "data-commandname";
    public static readonly string DisplayName = "data-displayname";
    public static readonly string Title = "data-title";
    public static readonly string ItemID = "data-item-id";
    public static readonly string TriggersPostBack = "data-triggers-pb";
    public static readonly string TriggersNavigation = "data-triggers-nav";
    public static readonly string IsReadOnly = "data-is-readonly";

    // TODO RM-6297: Move BOC-revelant bits to ObjectBinding asssembly.
    public static readonly string IsBound = "data-is-bound";
    public static readonly string BoundType = "data-bound-type";
    public static readonly string BoundProperty = "data-bound-property";

    public static readonly string BocListRowIndex = "data-boclist-row-index";
    public static readonly string BocListCellIndex = "data-boclist-cell-index";
    public static readonly string BocListNumberOfPages = "data-boclist-number-of-pages";
    public static readonly string BocListCellContents = "data-boclist-cell-contents";
  }
}