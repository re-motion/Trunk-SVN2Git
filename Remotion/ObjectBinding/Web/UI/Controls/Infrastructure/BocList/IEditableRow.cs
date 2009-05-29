using System;
using System.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{
  public interface IEditableRow
  {
    void CreateControls (IBusinessObject value, BocColumnDefinition[] columns);
    void RemoveControls();
    bool HasEditControls ();
    bool HasEditControl (int columnIndex);
    void PrepareValidation ();
    bool Validate ();
    IBusinessObjectBoundEditableWebControl[] GetEditControlsAsArray();

    void RenderSimpleColumnCellEditModeControl (
        HtmlTextWriter writer, 
        BocSimpleColumnDefinition column,
        IBusinessObject businessObject,
        int columnIndex,
        EditModeValidator editModeValidator,
        bool showEditModeValidationMarkers,
        bool disableEditModeValidationMessages);

    IBusinessObjectBoundEditableWebControl GetEditControl (int index);
  }
}