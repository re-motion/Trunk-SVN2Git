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
using System.Web;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the cells containing the row selector controls.
  /// </summary>
  public class BocSelectorColumnQuirksModeRenderer : IBocSelectorColumnRenderer
  {
    private const int c_titleRowIndex = -1;
    protected const string c_whiteSpace = "&nbsp;";

    private readonly HttpContextBase _context;
    private readonly IBocList _list;
    private readonly CssClassContainer _cssClasses;

    public BocSelectorColumnQuirksModeRenderer (HttpContextBase context, IBocList list, CssClassContainer cssClasses)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);

      _context = context;
      _list = list;
      _cssClasses = cssClasses;
    }

    public HttpContextBase Context
    {
      get { return _context; }
    }

    public IBocList List
    {
      get { return _list; }
    }

    public CssClassContainer CssClasses
    {
      get { return _cssClasses; }
    }

    public void RenderDataCell (HtmlTextWriter writer, int originalRowIndex, string selectorControlID, bool isChecked, string cssClassTableCell)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNullOrEmpty ("selectorControlID", selectorControlID);
      ArgumentUtility.CheckNotNullOrEmpty ("cssClassTableCell", cssClassTableCell);

      if (!List.IsSelectionEnabled)
        return;

      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderSelectorControl (writer, selectorControlID, originalRowIndex.ToString(), isChecked, false);
      writer.RenderEndTag();
    }

    public void RenderTitleCell (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      if (!List.IsSelectionEnabled)
        return;

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.TitleCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Th);
      if (List.Selection == RowSelection.Multiple)
      {
        string selectorControlName = List.GetSelectAllControlClientID();
        bool isChecked = (List.SelectorControlCheckedState.Contains (c_titleRowIndex));
        RenderSelectorControl (writer, selectorControlName, c_titleRowIndex.ToString(), isChecked, true);
      }
      else
        writer.Write (c_whiteSpace);
      writer.RenderEndTag();
    }

    /// <summary> Renders a check-box or a radio-button used for row selection. </summary>
    /// <param name="writer">The <see cref="HtmlTextWriter"/>.</param>
    /// <param name="id"> The <see cref="string"/> rendered into the <c>id</c> and <c>name</c> attributes. </param>
    /// <param name="value"> The value of the check-box or radio-button. </param>
    /// <param name="isChecked"> 
    ///   <see langword="true"/> if the check-box or radio-button is checked. 
    /// </param>
    /// <param name="isSelectAllSelectorControl"> 
    ///   <see langword="true"/> if the rendered check-box or radio-button is in the title row.
    /// </param>
    private void RenderSelectorControl (HtmlTextWriter writer, string id, string value, bool isChecked, bool isSelectAllSelectorControl)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);

      if (List.Selection == RowSelection.SingleRadioButton)
        writer.AddAttribute (HtmlTextWriterAttribute.Type, "radio");
      else
        writer.AddAttribute (HtmlTextWriterAttribute.Type, "checkbox");
      writer.AddAttribute (HtmlTextWriterAttribute.Id, id);
      writer.AddAttribute (HtmlTextWriterAttribute.Name, id);

      if (isChecked)
        writer.AddAttribute (HtmlTextWriterAttribute.Checked, "checked");
      if (List.EditModeController.IsRowEditModeActive)
        writer.AddAttribute (HtmlTextWriterAttribute.Disabled, "disabled");

      writer.AddAttribute (HtmlTextWriterAttribute.Value, value);

      if (isSelectAllSelectorControl)
        AddSelectAllSelectorAttributes (writer);
      else
        AddRowSelectorAttributes (writer);

      writer.RenderBeginTag (HtmlTextWriterTag.Input);
      writer.RenderEndTag();
    }

    private void AddRowSelectorAttributes (HtmlTextWriter writer)
    {
      string alternateText = List.GetResourceManager().GetString (BocList.ResourceIdentifier.SelectRowAlternateText);
      writer.AddAttribute (HtmlTextWriterAttribute.Alt, alternateText);

      if (List.HasClientScript)
      {
        const string script = "BocList_OnSelectionSelectorControlClick();";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
    }

    private void AddSelectAllSelectorAttributes (HtmlTextWriter writer)
    {
      string alternateText = List.GetResourceManager().GetString (BocList.ResourceIdentifier.SelectAllRowsAlternateText);
      writer.AddAttribute (HtmlTextWriterAttribute.Alt, alternateText);

      int count = 0;
      if (List.IsPagingEnabled)
        count = List.PageSize.Value;
      else if (!List.IsEmptyList)
        count = List.Value.Count;

      if (List.HasClientScript)
      {
        string script = "BocList_OnSelectAllSelectorControlClick ("
                        + "document.getElementById ('" + List.ClientID + "'), "
                        + "this , '"
                        + List.GetSelectorControlClientId (null) + "', "
                        + count + ", "
                        + "document.getElementById ('" + List.ListMenu.ClientID + "'));";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
    }
  }
}