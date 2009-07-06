namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> Defines when the <see cref="BocRowEditModeColumnDefinition"/> will be shown in the <see cref="BocList"/>. </summary>
  public enum BocRowEditColumnDefinitionShow
  {
    /// <summary> The column is always shown, but inactive if the <see cref="BocList"/> is read-only. </summary>
    Always,
    /// <summary> The column is only shown if the <see cref="BocList"/> is in edit-mode. </summary>
    EditMode
  }
}