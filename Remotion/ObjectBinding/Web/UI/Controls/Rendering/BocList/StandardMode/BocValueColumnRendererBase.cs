// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.Utilities;
using System.Web;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode
{
  /// <summary>
  /// Abstract base class for column renderers that handle classes derived from <see cref="BocValueColumnDefinition"/>.
  /// Defines <see cref="RenderCellContents"/> as template method and common utility methods.
  /// </summary>
  /// <typeparam name="TBocColumnDefinition">The column definition class that the derived class can handle.</typeparam>
  public abstract class BocValueColumnRendererBase<TBocColumnDefinition> : BocCommandEnabledColumnRendererBase<TBocColumnDefinition>
      where TBocColumnDefinition: BocValueColumnDefinition
  {
    protected BocValueColumnRendererBase (
        HttpContextBase context, HtmlTextWriter writer, IBocList list, TBocColumnDefinition columnDefintion, CssClassContainer cssClasses)
        : base (context, writer, list, columnDefintion, cssClasses)
    {
    }

    /// <summary>
    /// Renders a table cell for a <see cref="BocValueColumnDefinition"/>. This is a template method using 
    /// <see cref="BocCommandEnabledColumnRendererBase{TBocColumnDefinition}.RenderCellIcon"/>
    /// and <see cref="RenderCellText"/>, which have to be defined in deriving classes.
    /// </summary>
    protected override void RenderCellContents (BocListDataRowRenderEventArgs dataRowRenderEventArgs, int rowIndex, bool showIcon)
    {
      ArgumentUtility.CheckNotNull ("dataRowRenderEventArgs", dataRowRenderEventArgs);

      int originalRowIndex = dataRowRenderEventArgs.ListIndex;
      IBusinessObject businessObject = dataRowRenderEventArgs.BusinessObject;

      IEditableRow editableRow = List.EditModeController.GetEditableRow (originalRowIndex);

      bool hasEditModeControl = editableRow != null && editableRow.HasEditControl (ColumnIndex);
      bool showEditModeControl = hasEditModeControl
                                 && !editableRow.GetEditControl (ColumnIndex).IsReadOnly;

      string valueColumnText = null;
      if (!showEditModeControl)
        valueColumnText = Column.GetStringValue (businessObject);

      bool enforceWidth = RenderCropSpanBeginTag (showEditModeControl, valueColumnText);
      bool isCommandEnabled = RenderBeginTag (originalRowIndex, businessObject, valueColumnText);

      if (!hasEditModeControl)
      {
        if (showIcon)
          RenderCellIcon (businessObject);

        RenderOtherIcons (businessObject);
      }
      RenderCellText (businessObject, showEditModeControl, editableRow);

      RenderEndTag (isCommandEnabled);
      RenderCropSpanEndTag (enforceWidth);
    }


    protected abstract void RenderCellText (IBusinessObject businessObject, bool showEditModeControl, IEditableRow editableRow);

    /// <summary>
    /// Used by <see cref="RenderCellContents"/> to render icons in addition to the <paramref name="businessObject"/>'s icon.
    /// Deriving classes should override this empty implementation if they wish to add other icons.
    /// Should not be used by other clients.
    /// </summary>
    /// <param name="businessObject">Can be used to derive the icon to render.</param>
    protected virtual void RenderOtherIcons (IBusinessObject businessObject)
    {
    }

    /// <summary>
    /// If the column width must be enforced, this method renders a &lt;span&gt; block container that crops any overflowing content.
    /// </summary>
    /// <param name="showEditModeControl">Specifies if the cell contains edit mode controls.</param>
    /// <param name="spanTitle">Specifies the text to be written to the 'title' attribute.</param>
    /// <returns><see langword="true"/> if the crop span begin tag has been rendered, <see langword="false"/> otherwise.</returns>
    private bool RenderCropSpanBeginTag (bool showEditModeControl, string spanTitle)
    {
      bool enforceWidth =
          Column.EnforceWidth
          && !Column.Width.IsEmpty
          && Column.Width.Type != UnitType.Percentage
          && !showEditModeControl;

      if (enforceWidth)
      {
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Column.Width.ToString());
        Writer.AddStyleAttribute ("overflow", "hidden");
        Writer.AddStyleAttribute ("white-space", "nowrap");
        Writer.AddStyleAttribute ("display", "block");
        Writer.AddAttribute (HtmlTextWriterAttribute.Title, spanTitle);
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      return enforceWidth;
    }

    /// <summary>
    /// Renders the end tag to the crop span element if a begin tag has been rendered.
    /// </summary>
    /// <param name="enforceWidth">Specifies if a corresponding begin tag has been rendered.</param>
    private void RenderCropSpanEndTag (bool enforceWidth)
    {
      if (enforceWidth)
        Writer.RenderEndTag();
    }


    private bool RenderBeginTag (int originalRowIndex, IBusinessObject businessObject, string valueColumnText)
    {
      bool isCommandEnabled = false;
      if (!StringUtility.IsNullOrEmpty (valueColumnText))
        isCommandEnabled = RenderBeginTagDataCellCommand (businessObject, originalRowIndex);
      if (!isCommandEnabled)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Content);
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      return isCommandEnabled;
    }

    private void RenderEndTag (bool isCommandEnabled)
    {
      if (isCommandEnabled)
        RenderEndTagDataCellCommand();
      else
        Writer.RenderEndTag();
    }
  }
}
