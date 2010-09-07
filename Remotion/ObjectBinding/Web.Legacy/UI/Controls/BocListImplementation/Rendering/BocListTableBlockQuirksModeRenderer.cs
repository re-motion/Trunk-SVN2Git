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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the table (consisting of title and data rows) that shows the items contained in the <see cref="IBocList"/>.
  /// </summary>
  public class BocListTableBlockQuirksModeRenderer : IBocListTableBlockRenderer
  {
    private readonly HttpContextBase _context;
    private readonly IBocList _list;
    private readonly BocListQuirksModeCssClassDefinition _cssClasses;
    private readonly IBocRowRenderer _rowRenderer;
    private readonly IBocColumnRenderer[] _columnRenderers;

    public BocListTableBlockQuirksModeRenderer (
        HttpContextBase context, IBocList list, BocListQuirksModeCssClassDefinition cssClasses, IBocRowRenderer rowRenderer, IBocColumnRenderer[] columnRenderers)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);
      ArgumentUtility.CheckNotNull ("rowRenderer", rowRenderer);

      _context = context;
      _list = list;
      _cssClasses = cssClasses;
      _rowRenderer = rowRenderer;
      _columnRenderers = columnRenderers;
    }

    public HttpContextBase Context
    {
      get { return _context; }
    }

    public IBocList List
    {
      get { return _list; }
    }

    public BocListQuirksModeCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public IBocRowRenderer RowRenderer
    {
      get { return _rowRenderer; }
    }

    /// <summary>
    /// Renders the data contained in <see cref="Web.UI.Controls.BocListImplementation.Rendering.BocListRenderer.List"/> as a table.
    /// </summary>
    /// <remarks>
    /// The table consists of a title row showing the column titles, and a data row for each <see cref="IBusinessObject"/>
    /// in <see cref="Web.UI.Controls.BocListImplementation.Rendering.BocListRenderer.List"/>. If there is no data, the table will be completely hidden (only one cell containing only whitespace)
    /// if <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ShowEmptyListEditMode"/> is <see langword="false"/> and 
    /// <see cref="Web.UI.Controls.BocListImplementation.Rendering.BocListRenderer.List"/> is editable
    /// or if <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ShowEmptyListReadOnlyMode"/> is <see langword="false"/> and 
    /// <see cref="Web.UI.Controls.BocListImplementation.Rendering.BocListRenderer.List"/> is read-only.
    /// Exception: at design time, the title row will always be visible.
    /// </remarks>
    /// <seealso cref="RenderTableBlockColumnGroup"/>
    /// <seealso cref="RenderTableBody"/>
    public void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      bool isDesignMode = ControlHelper.IsDesignMode (List);
      bool isReadOnly = List.IsReadOnly;
      bool showForEmptyList = isReadOnly && List.ShowEmptyListReadOnlyMode
                              || !isReadOnly && List.ShowEmptyListEditMode;

      if (List.IsEmptyList && !showForEmptyList)
        RenderTable (writer, isDesignMode, false);
      else
        RenderTable (writer, true, true);

      RenderClientSelectionScript();
    }

    private void RenderTable (HtmlTextWriter writer, bool tableHead, bool tableBody)
    {
      if (!tableHead && !tableBody)
      {
        RenderEmptyTable (writer);
        return;
      }

      RenderTableOpeningTag (writer);
      RenderTableBlockColumnGroup (writer);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.TableHead);

      if (tableHead)
        RenderTableHead (writer);

      if (tableBody)
        RenderTableBody (writer);

      RenderTableClosingTag (writer);
    }

    private void RenderEmptyTable (HtmlTextWriter writer)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      writer.RenderBeginTag (HtmlTextWriterTag.Table);
      writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      writer.Write ("&nbsp;");
      writer.RenderEndTag();
      writer.RenderEndTag();
      writer.RenderEndTag();
    }

    /// <summary>
    /// Renders the data row of the <see cref="BocList"/>.
    /// </summary>
    /// <remarks>
    /// This method provides the outline of the table head, actual rendering of each row is delegated to
    /// <see cref="Web.UI.Controls.BocListImplementation.Rendering.BocRowRenderer.RenderTitlesRow"/>.
    /// The rows are nested within a &lt;thead&gt; element.
    /// </remarks>
    /// <seealso cref="Web.UI.Controls.BocListImplementation.Rendering.BocRowRenderer"/>
    protected virtual void RenderTableHead (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      writer.RenderBeginTag (HtmlTextWriterTag.Thead);
      RowRenderer.RenderTitlesRow (writer);
      writer.RenderEndTag();
    }

    /// <summary>
    /// Renders the data rows of the <see cref="BocList"/>.
    /// </summary>
    /// <remarks>
    /// This method provides the outline of the table body, actual rendering of each row is delegated to
    /// <see cref="Web.UI.Controls.BocListImplementation.Rendering.BocRowRenderer.RenderDataRow"/>.
    /// The rows are nested within a &lt;tbody&gt; element.
    /// </remarks>
    /// <seealso cref="Web.UI.Controls.BocListImplementation.Rendering.BocRowRenderer"/>
    protected virtual void RenderTableBody (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.TableBody);
      writer.RenderBeginTag (HtmlTextWriterTag.Tbody);

      if (List.IsEmptyList && List.ShowEmptyListMessage)
        RowRenderer.RenderEmptyListDataRow (writer);
      else
      {
        int firstRow;
        BocListRow[] rows = List.GetRowsToDisplay (out firstRow);

        for (int idxAbsoluteRows = firstRow, idxRelativeRows = 0;
             idxRelativeRows < rows.Length;
             idxAbsoluteRows++, idxRelativeRows++)
        {
          BocListRow row = rows[idxRelativeRows];
          int originalRowIndex = row.Index;
          RowRenderer.RenderDataRow (writer, row.BusinessObject, idxRelativeRows, idxAbsoluteRows, originalRowIndex);
        }
      }

      writer.RenderEndTag();
    }

    private void RenderClientSelectionScript ()
    {
      if (List.HasClientScript && List.IsSelectionEnabled)
      {
        //  Render the init script for the client side selection handling
        int count = 0;
        if (List.IsPagingEnabled)
          count = List.PageSize.Value;
        else if (List.Value != null)
          count = List.Value.Count;

        bool hasClickSensitiveRows = List.IsSelectionEnabled && !List.EditModeController.IsRowEditModeActive && List.AreDataRowsClickSensitive();

        const string scriptTemplate = "BocList_InitializeList ( $('#{0}')[0], '{1}', {2}, {3}, {4}, $('#{5}')[0] );";
        string script = string.Format (
            scriptTemplate,
            List.ClientID,
            List.GetSelectorControlClientId (null),
            count,
            (int) List.Selection,
            hasClickSensitiveRows ? "true" : "false",
            List.ListMenu.ClientID);

        List.Page.ClientScript.RegisterStartupScriptBlock (
            List, typeof (BocListTableBlockQuirksModeRenderer), typeof (BocList).FullName + "_" + List.ClientID + "_InitializeListScript", script);
      }
    }

    /// <summary> Renderes the opening tag of the table. </summary>
    private void RenderTableOpeningTag (HtmlTextWriter writer)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Table);
      writer.AddAttribute (HtmlTextWriterAttribute.Id, List.ClientID + "_Table");
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Table);
      writer.RenderBeginTag (HtmlTextWriterTag.Table);
    }

    /// <summary> Renderes the closing tag of the table. </summary>
    private void RenderTableClosingTag (HtmlTextWriter writer)
    {
      writer.RenderEndTag(); // table
      writer.RenderEndTag(); // div
    }

    /// <summary> Renders the column group, which provides the table's column layout. </summary>
    private void RenderTableBlockColumnGroup (HtmlTextWriter writer)
    {
      IBocColumnRenderer[] columnRenderers = _columnRenderers;

      writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

      bool isTextXml = false;

      if (!List.IsDesignMode)
        isTextXml = ControlHelper.IsXmlConformResponseTextRequired (Context);

      RenderIndexColumnDeclaration (writer, isTextXml);
      RenderSelectorColumnDeclaration (writer, isTextXml);

      //bool isFirstColumnUndefinedWidth = true;
      for (int i = 0; i < columnRenderers.Length; i++)
      {
        BocColumnDefinition column = columnRenderers[i].Column;

        if (!List.IsColumnVisible (column))
          continue;

        RenderDataColumnDeclaration (writer, isTextXml, column);
      }

      //  Design-mode and empty table
      if (ControlHelper.IsDesignMode (List) && columnRenderers.Length == 0)
      {
        for (int i = 0; i < BocRowQuirksModeRenderer.DesignModeDummyColumnCount; i++)
        {
          writer.RenderBeginTag (HtmlTextWriterTag.Col);
          writer.RenderEndTag();
        }
      }

      writer.RenderEndTag();
    }

    /// <summary>Renders a single col element for the given column.</summary>
    private void RenderDataColumnDeclaration (HtmlTextWriter writer, bool isTextXml, BocColumnDefinition column)
    {
      writer.WriteBeginTag ("col");
      if (!column.Width.IsEmpty)
      {
        writer.Write (" style=\"");
        string width;
        BocValueColumnDefinition valueColumn = column as BocValueColumnDefinition;
        if (valueColumn != null && valueColumn.EnforceWidth && column.Width.Type != UnitType.Percentage)
          width = "2em";
        else
          width = column.Width.ToString();
        writer.WriteStyleAttribute ("width", width);
        writer.Write ("\"");
      }
      if (isTextXml)
        writer.Write (" />");
      else
        writer.Write (">");
    }

    /// <summary>Renders the col element for the selector column</summary>
    private void RenderSelectorColumnDeclaration (HtmlTextWriter writer, bool isTextXml)
    {
      if (List.IsSelectionEnabled)
      {
        writer.WriteBeginTag ("col");
        writer.Write (" style=\"");
        writer.WriteStyleAttribute ("width", "1.6em");
        writer.Write ("\"");
        if (isTextXml)
          writer.Write (" />");
        else
          writer.Write (">");
      }
    }

    /// <summary>Renders the col element for the index column</summary>
    private void RenderIndexColumnDeclaration (HtmlTextWriter writer, bool isTextXml)
    {
      if (List.IsIndexEnabled)
      {
        writer.WriteBeginTag ("col");
        writer.Write (" style=\"");
        writer.WriteStyleAttribute ("width", "1.6em");
        writer.Write ("\"");
        if (isTextXml)
          writer.Write (" />");
        else
          writer.Write (">");
      }
    }
  }
}