// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the cells containing the row selector controls.
  /// </summary>
  public class BocSelectorColumnRenderer : IBocSelectorColumnRenderer
  {
    private const int c_titleRowIndex = -1;
    protected const string c_whiteSpace = "&nbsp;";

    private readonly BocListCssClassDefinition _cssClasses;

    public BocSelectorColumnRenderer (BocListCssClassDefinition cssClasses)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);

      _cssClasses = cssClasses;
    }

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public void RenderDataCell (BocListRenderingContext renderingContext, int originalRowIndex, string selectorControlID, bool isChecked, string cssClassTableCell)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNullOrEmpty ("selectorControlID", selectorControlID);
      ArgumentUtility.CheckNotNullOrEmpty ("cssClassTableCell", cssClassTableCell);

      if (!renderingContext.Control.IsSelectionEnabled)
        return;

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderSelectorControl (renderingContext, selectorControlID, originalRowIndex, isChecked, false);
      renderingContext.Writer.RenderEndTag();
    }

    public void RenderTitleCell (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      if (!renderingContext.Control.IsSelectionEnabled)
        return;

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.TitleCell);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Th);
      if (renderingContext.Control.Selection == RowSelection.Multiple)
      {
        string selectorControlName = renderingContext.Control.GetSelectAllControlClientID();
        bool isChecked = (renderingContext.Control.SelectorControlCheckedState.Contains (c_titleRowIndex));
        RenderSelectorControl (renderingContext, selectorControlName, c_titleRowIndex, isChecked, true);
      }
      else
        renderingContext.Writer.Write (c_whiteSpace);
      renderingContext.Writer.RenderEndTag();
    }

    /// <summary> Renders a check-box or a radio-button used for row selection. </summary>
    /// <param name="renderingContext">The <see cref="BocListRenderingContext"/>.</param>
    /// <param name="id"> The <see cref="string"/> rendered into the <c>id</c> and <c>name</c> attributes. </param>
    /// <param name="value"> The value of the check-box or radio-button. </param>
    /// <param name="isChecked"> 
    ///   <see langword="true"/> if the check-box or radio-button is checked. 
    /// </param>
    /// <param name="isSelectAllSelectorControl"> 
    ///   <see langword="true"/> if the rendered check-box or radio-button is in the title row.
    /// </param>
    private void RenderSelectorControl (BocListRenderingContext renderingContext, string id, int value, bool isChecked, bool isSelectAllSelectorControl)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      if (renderingContext.Control.Selection == RowSelection.SingleRadioButton)
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Type, "radio");
      else
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Type, "checkbox");
      
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, id);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Name, id);

      if (isChecked)
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Checked, "checked");
      if (renderingContext.Control.EditModeController.IsRowEditModeActive)
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Disabled, "disabled");

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Value, value.ToString());

      if (isSelectAllSelectorControl)
        AddSelectAllSelectorAttributes (renderingContext);
      else
        AddRowSelectorAttributes (renderingContext);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Input);
      renderingContext.Writer.RenderEndTag();
    }

    private void AddRowSelectorAttributes (BocListRenderingContext renderingContext)
    {
      string alternateText = renderingContext.Control.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.SelectRowAlternateText);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Alt, alternateText);

      if (renderingContext.Control.HasClientScript)
      {
        const string script = "BocList_OnSelectionSelectorControlClick();";
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
    }

    private void AddSelectAllSelectorAttributes (BocListRenderingContext renderingContext)
    {
      string alternateText = renderingContext.Control.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.SelectAllRowsAlternateText);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Alt, alternateText);

      int count = 0;
      if (renderingContext.Control.IsPagingEnabled)
        count = renderingContext.Control.PageSize.Value;
      else if (!renderingContext.Control.IsEmptyList)
        count = renderingContext.Control.Value.Count;

      if (renderingContext.Control.HasClientScript)
      {
        string script = "BocList_OnSelectAllSelectorControlClick ("
                        + "document.getElementById ('" + renderingContext.Control.ClientID + "'), "
                        + "this , '"
                        + renderingContext.Control.GetSelectorControlClientId (null) + "', "
                        + count + ", "
                        + "document.getElementById ('" + renderingContext.Control.ListMenu.ClientID + "'));";
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
    }
  }
}