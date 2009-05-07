using System;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.Security;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Renderers
{
  /// <summary>
  /// Abstract base class for column renderers that can handle derived classes of <see cref="BocCommandEnabledColumnDefinition"/>.
  /// Defines common utility methods.
  /// </summary>
  /// <typeparam name="TBocColumnDefinition">The column definition class which the deriving class can handle.</typeparam>
  public abstract class BocCommandEnabledColumnRenderer<TBocColumnDefinition> : BocColumnRenderer<TBocColumnDefinition>
    where TBocColumnDefinition : BocCommandEnabledColumnDefinition
  {
    protected BocCommandEnabledColumnRenderer (BocList list, HtmlTextWriter writer, TBocColumnDefinition column)
        : base(list, writer, column)
    {
    }

    protected EditableRow GetEditableRow (bool isEditedRow, int originalRowIndex)
    {
      EditableRow editableRow = null;
      if (isEditedRow)
        editableRow = List.EditModeController._rows[0];
      else if (List.IsListEditModeActive)
        editableRow = List.EditModeController._rows[originalRowIndex];
      return editableRow;
    }

    protected void RenderCellIcon (IBusinessObject businessObject)
    {
      IconInfo icon = BusinessObjectBoundWebControl.GetIcon (
          businessObject,
          businessObject.BusinessObjectClass.BusinessObjectProvider);

      if (icon != null)
      {
        RenderIcon (icon, null);
        Writer.Write (c_whiteSpace);
      }
    }

    protected void RenderValueColumnCellText (string contents)
    {
      contents = HtmlUtility.HtmlEncode (contents);
      if (StringUtility.IsNullOrEmpty (contents))
        contents = c_whiteSpace;
      Writer.Write (contents);
    }

    protected bool RenderBeginTagDataCellCommand (
        IBusinessObject businessObject,
        int originalRowIndex)
    {
      BocListItemCommand command = Column.Command;
      if (command == null)
        return false;

      bool isReadOnly = List.IsReadOnly;
      bool isActive = command.Show == CommandShow.Always
                      || isReadOnly && command.Show == CommandShow.ReadOnly
                      || !isReadOnly && command.Show == CommandShow.EditMode;

      bool isCommandAllowed = (command.Type != CommandType.None) && !List.IsRowEditModeActive;
      bool isCommandEnabled = (command.CommandState == null) || command.CommandState.IsEnabled (List, businessObject, Column);
      
      if (isActive && isCommandAllowed && isCommandEnabled)
      {
        if (WcagHelper.Instance.IsWaiConformanceLevelARequired () && command.Type != CommandType.Href)
          return false;
      }

      string objectID = null;
      IBusinessObjectWithIdentity businessObjectWithIdentity = businessObject as IBusinessObjectWithIdentity;
      if (businessObjectWithIdentity != null)
        objectID = businessObjectWithIdentity.UniqueIdentifier;

      string argument = List.GetListItemCommandArgument (ColumnIndex, originalRowIndex);
      string postBackEvent = List.Page.ClientScript.GetPostBackEventReference (List, argument) + ";";
      string onClick = List.HasClientScript ? c_onCommandClickScript : string.Empty;
      command.RenderBegin (Writer, postBackEvent, onClick, originalRowIndex, objectID, businessObject as ISecurableObject);

      return true;
    }

    protected void RenderEndTagDataCellCommand ()
    {
      Column.Command.RenderEnd (Writer);
    }
  }

}
