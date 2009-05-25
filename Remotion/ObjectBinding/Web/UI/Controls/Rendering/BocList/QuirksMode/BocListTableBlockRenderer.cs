// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Microsoft.Practices.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  public class BocListTableBlockRenderer : BocListRendererBase, IBocListTableBlockRenderer
  {
    private readonly IServiceLocator _serviceLocator;
    private readonly IBocRowRenderer _rowRenderer;

    public BocListTableBlockRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list, IServiceLocator serviceLocator)
        : base(context, writer, list)
    {
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      _serviceLocator = serviceLocator;
      _rowRenderer = _serviceLocator.GetInstance<IBocRowRendererFactory>().CreateRenderer (Context, Writer, List, _serviceLocator);
    }

    private IBocRowRenderer RowRenderer
    {
      get { return _rowRenderer; }
    }

      /// <summary>
    /// Renders the data contained in <see cref="BocListRendererBase.List"/> as a table.
    /// </summary>
    /// <remarks>
    /// The table consists of a title row showing the column titles, and a data row for each <see cref="IBusinessObject"/>
    /// in <see cref="BocListRendererBase.List"/>. If there is no data, the table will be completely hidden (only one cell containing only whitespace)
    /// if <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ShowEmptyListEditMode"/> is <see langword="false"/> and 
    /// <see cref="BocListRendererBase.List"/> is editable
    /// or if <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ShowEmptyListReadOnlyMode"/> is <see langword="false"/> and 
    /// <see cref="BocListRendererBase.List"/> is read-only.
    /// Exception: at design time, the title row will always be visible.
    /// </remarks>
    /// <seealso cref="RenderTableBlockColumnGroup"/>
    /// <seealso cref="RenderTableBody"/>
    public void Render ()
    {
      bool isDesignMode = ControlHelper.IsDesignMode (List);
      bool isReadOnly = List.IsReadOnly;
      bool showForEmptyList = isReadOnly && List.ShowEmptyListReadOnlyMode
                              || !isReadOnly && List.ShowEmptyListEditMode;

      if (List.IsEmptyList && !showForEmptyList)
        RenderTable (isDesignMode, false);
      else
        RenderTable (true, true);

      RenderClientSelectionScript ();
    }

    private void RenderTable (bool tableHead, bool tableBody)
    {
      if (!tableHead && !tableBody)
      {
        RenderEmptyTable ();
        return;
      }

      RenderTableOpeningTag ();
      RenderTableBlockColumnGroup ();
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, List.CssClassTableHead);

      if (tableHead)
        RenderTableHead ();

      if (tableBody)
        RenderTableBody ();

      RenderTableClosingTag ();
    }

    private void RenderEmptyTable ()
    {
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      Writer.RenderBeginTag (HtmlTextWriterTag.Table);
      Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      Writer.RenderBeginTag (HtmlTextWriterTag.Td);
      Writer.Write ("&nbsp;");
      Writer.RenderEndTag ();
      Writer.RenderEndTag ();
      Writer.RenderEndTag ();
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
    protected virtual void RenderTableHead ()
    {
      Writer.RenderBeginTag (HtmlTextWriterTag.Thead);
      RowRenderer.RenderTitlesRow ();
      Writer.RenderEndTag ();
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
    protected virtual void RenderTableBody ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, List.CssClassTableBody);
      Writer.RenderBeginTag (HtmlTextWriterTag.Tbody);

      int firstRow = 0;
      int totalRowCount = (List.Value != null) ? List.Value.Count : 0;
      int rowCountWithOffset = totalRowCount;

      if (List.IsPagingEnabled && !List.IsEmptyList)
      {
        firstRow = List.CurrentPage * List.PageSize.Value;
        rowCountWithOffset = firstRow + List.PageSize.Value;

        //  Check row count on last page
        if (List.Value != null)
          rowCountWithOffset = Math.Min (rowCountWithOffset, List.Value.Count);
      }

      if (List.IsEmptyList && List.ShowEmptyListMessage)
        RowRenderer.RenderEmptyListDataRow ();
      else
      {
        BocListRow[] rows = List.GetIndexedRows ();

        for (int idxAbsoluteRows = firstRow, idxRelativeRows = 0;
             idxAbsoluteRows < rowCountWithOffset;
             idxAbsoluteRows++, idxRelativeRows++)
        {
          BocListRow row = rows[idxAbsoluteRows];
          int originalRowIndex = row.Index;
          RowRenderer.RenderDataRow (row.BusinessObject, idxRelativeRows, idxAbsoluteRows, originalRowIndex);
        }
      }

      Writer.RenderEndTag ();
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

        string script =
            "BocList_InitializeList ("
            + "document.getElementById ('" + List.ClientID + "'), '"
            + List.ClientID + c_dataRowSelectorControlIDSuffix + "', "
            + count + ","
            + (int) List.Selection + ");";
        List.Page.ClientScript.RegisterStartupScriptBlock (
            List, typeof (Controls.BocList).FullName + "_" + List.ClientID + "_InitializeListScript", script);
      }
    }

    /// <summary> Renderes the opening tag of the table. </summary>
    private void RenderTableOpeningTag ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, List.CssClassTable);
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, List.ClientID + "_Table");
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, List.CssClassTable);
      Writer.RenderBeginTag (HtmlTextWriterTag.Table);
    }

    /// <summary> Renderes the closing tag of the table. </summary>
    private void RenderTableClosingTag ()
    {
      Writer.RenderEndTag (); // table
      Writer.RenderEndTag (); // div
    }

    /// <summary> Renders the column group, which provides the table's column layout. </summary>
    private void RenderTableBlockColumnGroup ()
    {
      BocColumnDefinition[] renderColumns = List.GetColumns ();

      Writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

      bool isTextXml = false;

      if (!List.IsDesignMode)
        isTextXml = ControlHelper.IsXmlConformResponseTextRequired (Context);

      RenderIndexColumnDeclaration (isTextXml);
      RenderSelectorColumnDeclaration (isTextXml);

      //bool isFirstColumnUndefinedWidth = true;
      for (int i = 0; i < renderColumns.Length; i++)
      {
        BocColumnDefinition column = renderColumns[i];

        if (!List.IsColumnVisible (column))
          continue;

        RenderDataColumnDeclaration (isTextXml, column);
      }

      //  Design-mode and empty table
      if (ControlHelper.IsDesignMode (List) && renderColumns.Length == 0)
      {
        for (int i = 0; i < c_designModeDummyColumnCount; i++)
        {
          Writer.RenderBeginTag (HtmlTextWriterTag.Col);
          Writer.RenderEndTag ();
        }
      }

      Writer.RenderEndTag ();
    }

    /// <summary>Renders a single col element for the given column.</summary>
    private void RenderDataColumnDeclaration (bool isTextXml, BocColumnDefinition column)
    {
      Writer.WriteBeginTag ("col");
      if (!column.Width.IsEmpty)
      {
        Writer.Write (" style=\"");
        string width;
        BocValueColumnDefinition valueColumn = column as BocValueColumnDefinition;
        if (valueColumn != null && valueColumn.EnforceWidth && column.Width.Type != UnitType.Percentage)
          width = "2em";
        else
          width = column.Width.ToString ();
        Writer.WriteStyleAttribute ("width", width);
        Writer.Write ("\"");
      }
      if (isTextXml)
        Writer.Write (" />");
      else
        Writer.Write (">");
    }

    /// <summary>Renders the col element for the selector column</summary>
    private void RenderSelectorColumnDeclaration (bool isTextXml)
    {
      if (List.IsSelectionEnabled)
      {
        Writer.WriteBeginTag ("col");
        Writer.Write (" style=\"");
        Writer.WriteStyleAttribute ("width", "1.6em");
        Writer.Write ("\"");
        if (isTextXml)
          Writer.Write (" />");
        else
          Writer.Write (">");
      }
    }

    /// <summary>Renders the col element for the index column</summary>
    private void RenderIndexColumnDeclaration (bool isTextXml)
    {
      if (List.IsIndexEnabled)
      {
        Writer.WriteBeginTag ("col");
        Writer.Write (" style=\"");
        Writer.WriteStyleAttribute ("width", "1.6em");
        Writer.Write ("\"");
        if (isTextXml)
          Writer.Write (" />");
        else
          Writer.Write (">");
      }
    }
  }
}