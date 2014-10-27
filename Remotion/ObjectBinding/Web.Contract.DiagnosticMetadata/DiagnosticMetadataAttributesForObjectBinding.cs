using System;

namespace Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata
{
  /// <summary>
  /// Diagnostic metadata attribute names used by various renderers.
  /// </summary>
  public static class DiagnosticMetadataAttributesForObjectBinding
  {
    public static readonly string DisplayName = "data-displayname";
    public static readonly string IsBound = "data-is-bound";
    public static readonly string BoundType = "data-bound-type";
    public static readonly string BoundProperty = "data-bound-property";

    public static readonly string BocBooleanValueIsTriState = "data-bocbooleanvalue-is-tristate";

    public static readonly string BocEnumValueStyle = "data-bocenumvalue-style";

    public static readonly string BocListNumberOfPages = "data-boclist-number-of-pages";
    public static readonly string BocListRowIndex = "data-boclist-row-index";
    public static readonly string BocListCellIndex = "data-boclist-cell-index";
    public static readonly string BocListWellKnownEditCell = "data-boclist-wellknown-cell-edit";
    public static readonly string BocListWellKnownRowDropDownMenuCell = "data-boclist-wellknown-cell-dropdownmenu";
    public static readonly string BocListCellContents = "data-boclist-cell-contents";
  }
}