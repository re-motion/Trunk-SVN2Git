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
using Remotion.Utilities;
using System.Web;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the table (consisting of title and data rows) that shows the items contained in the <see cref="IBocList"/>.
  /// </summary>
  public class BocListTableBlockRenderer : IBocListTableBlockRenderer
  {
    private readonly BocListCssClassDefinition _cssClasses;
    private readonly IBocRowRenderer _rowRenderer;

    public BocListTableBlockRenderer (BocListCssClassDefinition cssClasses, IBocRowRenderer rowRenderer)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);
      ArgumentUtility.CheckNotNull ("rowRenderer", rowRenderer);

      _cssClasses = cssClasses;
      _rowRenderer = rowRenderer;
    }

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public IBocRowRenderer RowRenderer
    {
      get { return _rowRenderer; }
    }

    /// <summary>
    /// Renders the data contained in <see cref="IBocList"/> as a table.
    /// </summary>
    /// <remarks>
    /// The table consists of a title row showing the column titles, and a data row for each <see cref="IBusinessObject"/>
    /// in <see cref="IBocList"/>. If there is no data, the table will be completely hidden (only one cell containing only whitespace)
    /// if <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ShowEmptyListEditMode"/> is <see langword="false"/> and 
    /// <see cref="IBocList"/> is editable
    /// or if <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ShowEmptyListReadOnlyMode"/> is <see langword="false"/> and 
    /// <see cref="IBocList"/> is read-only.
    /// Exception: at design time, the title row will always be visible.
    /// </remarks>
    /// <seealso cref="RenderTableBlockColumnGroup"/>
    /// <seealso cref="RenderTableBody"/>
    public void Render (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      bool isDesignMode = ControlHelper.IsDesignMode (renderingContext.Control);
      bool isReadOnly = renderingContext.Control.IsReadOnly;
      bool showForEmptyList = isReadOnly && renderingContext.Control.ShowEmptyListReadOnlyMode
                              || !isReadOnly && renderingContext.Control.ShowEmptyListEditMode;

      if (renderingContext.Control.IsEmptyList && !showForEmptyList)
        RenderTable (renderingContext, isDesignMode, false);
      else
        RenderTable (renderingContext, true, true);

      RenderClientSelectionScript(renderingContext);
    }

    private void RenderTable (BocListRenderingContext renderingContext, bool tableHead, bool tableBody)
    {
      if (!tableHead && !tableBody)
      {
        RenderEmptyTable (renderingContext);
        return;
      }

      RenderTableOpeningTag (renderingContext);
      RenderTableBlockColumnGroup (renderingContext);

      if (tableHead)
        RenderTableHead (renderingContext);

      if (tableBody)
        RenderTableBody (renderingContext);

      RenderTableClosingTag (renderingContext);
    }

    private void RenderEmptyTable (BocListRenderingContext renderingContext)
    {
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Table);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);
      renderingContext.Writer.Write ("&nbsp;");
      renderingContext.Writer.RenderEndTag ();
      renderingContext.Writer.RenderEndTag ();
      renderingContext.Writer.RenderEndTag ();
    }

    /// <summary>
    /// Renders the data row of the <see cref="BocList"/>.
    /// </summary>
    /// <remarks>
    /// This method provides the outline of the table head, actual rendering of each row is delegated to
    /// <see cref="BocRowRenderer.RenderTitlesRow"/>.
    /// The rows are nested within a &lt;thead&gt; element.
    /// </remarks>
    /// <seealso cref="BocRowRenderer"/>
    protected virtual void RenderTableHead (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.TableHead);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Thead);
      RowRenderer.RenderTitlesRow (renderingContext);
      renderingContext.Writer.RenderEndTag();
    }

    /// <summary>
    /// Renders the data rows of the <see cref="BocList"/>.
    /// </summary>
    /// <remarks>
    /// This method provides the outline of the table body, actual rendering of each row is delegated to
    /// <see cref="BocRowRenderer.RenderDataRow"/>.
    /// The rows are nested within a &lt;tbody&gt; element.
    /// </remarks>
    /// <seealso cref="BocRowRenderer"/>
    protected virtual void RenderTableBody (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.TableBody);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tbody);

      if (renderingContext.Control.IsEmptyList && renderingContext.Control.ShowEmptyListMessage)
        RowRenderer.RenderEmptyListDataRow (renderingContext);
      else
      {
        int firstRow;
        BocListRow[] rows = renderingContext.Control.GetRowsToDisplay (out firstRow);

        for (int idxAbsoluteRows = firstRow, idxRelativeRows = 0;
             idxRelativeRows < rows.Length;
             idxAbsoluteRows++, idxRelativeRows++)
        {
          BocListRow row = rows[idxRelativeRows];
          int originalRowIndex = row.Index;
          RowRenderer.RenderDataRow (renderingContext, row.BusinessObject, idxRelativeRows, idxAbsoluteRows, originalRowIndex);
        }
      }

      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderClientSelectionScript (BocListRenderingContext renderingContext)
    {
      if (renderingContext.Control.HasClientScript)
      {
        //  Render the init script for the client side selection handling
        int count = 0;
        if (renderingContext.Control.IsPagingEnabled)
          count = renderingContext.Control.PageSize.Value;
        else if (renderingContext.Control.Value != null)
          count = renderingContext.Control.Value.Count;
        
        bool hasClickSensitiveRows = renderingContext.Control.IsSelectionEnabled && !renderingContext.Control.EditModeController.IsRowEditModeActive && renderingContext.Control.AreDataRowsClickSensitive();

        const string scriptTemplate = "BocList_InitializeList ( $('#{0}')[0], '{1}', {2}, {3}, {4}, $('#{5}')[0] );";
        string script = string.Format (
            scriptTemplate,
            renderingContext.Control.ClientID,
            renderingContext.Control.GetSelectorControlClientId(null),
            count,
            (int) renderingContext.Control.Selection,
            hasClickSensitiveRows ? "true" : "false",
            renderingContext.Control.ListMenu.ClientID);

        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (
            renderingContext.Control, typeof (BocListTableBlockRenderer), typeof (Controls.BocList).FullName + "_" + renderingContext.Control.ClientID + "_InitializeListScript", script);
      }
    }

    /// <summary> Renderes the opening tag of the table. </summary>
    private void RenderTableOpeningTag (BocListRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Table);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID + "_Table");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Table);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Table);
    }

    /// <summary> Renderes the closing tag of the table. </summary>
    private void RenderTableClosingTag (BocListRenderingContext renderingContext)
    {
      renderingContext.Writer.RenderEndTag(); // table

      RenderFakeTableHeadForScrolling (renderingContext);

      renderingContext.Writer.RenderEndTag(); // div
    }

    private void RenderFakeTableHeadForScrolling (BocListRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.FakeTableHead);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      renderingContext.Writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Table);
      RenderTableHead (renderingContext);
      renderingContext.Writer.RenderEndTag();

      renderingContext.Writer.RenderEndTag ();
    }

    /// <summary> Renders the column group, which provides the table's column layout. </summary>
    private void RenderTableBlockColumnGroup (BocListRenderingContext renderingContext)
    {
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

      bool isTextXml = false;

      if (!renderingContext.Control.IsDesignMode)
        isTextXml = ControlHelper.IsXmlConformResponseTextRequired (renderingContext.HttpContext);

      RenderIndexColumnDeclaration (renderingContext, isTextXml);
      RenderSelectorColumnDeclaration (renderingContext, isTextXml);

      //bool isFirstColumnUndefinedWidth = true;
      for (int i = 0; i < renderingContext.ColumnRenderers.Length; i++)
      {
        var renderer = renderingContext.ColumnRenderers[i];

        if (!renderingContext.Control.IsColumnVisible (renderer.ColumnDefinition))
          continue;

        RenderDataColumnDeclaration (renderingContext, isTextXml, renderer.ColumnDefinition);
      }

      //  Design-mode and empty table
      if (ControlHelper.IsDesignMode (renderingContext.Control) && renderingContext.ColumnRenderers.Length == 0)
      {
        for (int i = 0; i < BocRowRenderer.DesignModeDummyColumnCount; i++)
        {
          renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Col);
          renderingContext.Writer.RenderEndTag();
        }
      }

      renderingContext.Writer.RenderEndTag();
    }

    /// <summary>Renders a single col element for the given column.</summary>
    private void RenderDataColumnDeclaration (BocListRenderingContext renderingContext, bool isTextXml, BocColumnDefinition column)
    {
      renderingContext.Writer.WriteBeginTag ("col");
      if (!column.Width.IsEmpty)
      {
        renderingContext.Writer.Write (" style=\"");
        string width;
        BocValueColumnDefinition valueColumn = column as BocValueColumnDefinition;
        if (valueColumn != null && valueColumn.EnforceWidth && column.Width.Type != UnitType.Percentage)
          width = "2em";
        else
          width = column.Width.ToString();
        renderingContext.Writer.WriteStyleAttribute ("width", width);
        renderingContext.Writer.Write ("\"");
      }
      if (isTextXml)
        renderingContext.Writer.Write (" />");
      else
        renderingContext.Writer.Write (">");
    }

    /// <summary>Renders the col element for the selector column</summary>
    private void RenderSelectorColumnDeclaration (BocListRenderingContext renderingContext, bool isTextXml)
    {
      if (renderingContext.Control.IsSelectionEnabled)
      {
        renderingContext.Writer.WriteBeginTag ("col");
        renderingContext.Writer.Write (" style=\"");
        renderingContext.Writer.WriteStyleAttribute ("width", "1.6em");
        renderingContext.Writer.Write ("\"");
        if (isTextXml)
          renderingContext.Writer.Write (" />");
        else
          renderingContext.Writer.Write (">");
      }
    }

    /// <summary>Renders the col element for the index column</summary>
    private void RenderIndexColumnDeclaration (BocListRenderingContext renderingContext, bool isTextXml)
    {
      if (renderingContext.Control.IsIndexEnabled)
      {
        renderingContext.Writer.WriteBeginTag ("col");
        renderingContext.Writer.Write (" style=\"");
        renderingContext.Writer.WriteStyleAttribute ("width", "1.6em");
        renderingContext.Writer.Write ("\"");
        if (isTextXml)
          renderingContext.Writer.Write (" />");
        else
          renderingContext.Writer.Write (">");
      }
    }
  }
}