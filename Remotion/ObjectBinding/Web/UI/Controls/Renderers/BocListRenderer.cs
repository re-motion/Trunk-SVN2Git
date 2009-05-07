using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Renderers
{
  /// <summary>
  /// Responsible for rendering a <see cref="BocList"/> object.
  /// </summary>
  /// <remarks>Renders the outline of a <see cref="BocList"/> object to an <see cref="HtmlTextWriter"/> and controls
  /// rendering of the various parts by delegating to specialized renderers.
  /// 
  /// This class should not be instantiated directly. Use a <see cref="BocListRendererFactory"/> to obtain an instance.</remarks>
  /// <seealso cref="BocListMenuBlockRenderer"/>
  /// <seealso cref="BocListNavigatorRenderer"/>
  /// <seealso cref="BocRowRenderer"/>
  public class BocListRenderer : BocListBaseRenderer
  {
    private const string c_defaultMenuBlockWidth = "70pt";
    private const string c_defaultMenuBlockOffset = "5pt";

    private delegate void RenderMethodDelegate ();

    private readonly BocListMenuBlockRenderer _menuBlockRenderer;
    private readonly BocListNavigatorRenderer _navigatorRenderer;
    private readonly BocRowRenderer _rowRenderer;

    /// <summary>
    /// Initializes the renderer with the <see cref="BocList"/> to render and the <see cref="HtmlTextWriter"/> to render it to,
    /// as well as a <see cref="BocListRendererFactory"/> used to create detail renderers.
    /// </summary>
    /// <param name="list">The <see cref="BocList"/> object to render.</param>
    /// <param name="writer">The target <see cref="HtmlTextWriter"/>.</param>
    /// <param name="factory">The <see cref="BocListRendererFactory"/> from which specialized renderers for the various parts
    /// can be obtained.</param>
    protected internal BocListRenderer (BocList list, HtmlTextWriter writer, BocListRendererFactory factory) : base(list, writer)
    {
      _menuBlockRenderer = factory.GetMenuBlockRenderer();
      _navigatorRenderer = factory.GetNavigatorRenderer();
      _rowRenderer = factory.GetRowRenderer();

      RenderTopLevelColumnGroup = RenderTopLevelColumnGroupForLegacyBrowser;

      if (!ControlHelper.IsDesignMode ((Control) List))
      {
        bool isXmlRequired = ControlHelper.IsXmlConformResponseTextRequired (HttpContext.Current);
        if (isXmlRequired)
          RenderTopLevelColumnGroup = RenderTopLevelColumnGroupForXmlBrowser;
      }
    }    

    private BocListMenuBlockRenderer MenuBlockRenderer
    {
      get { return _menuBlockRenderer; }
    }

    private BocListNavigatorRenderer NavigatorRenderer{
      get { return _navigatorRenderer; }
    }

    private BocRowRenderer RowRenderer
    {
      get { return _rowRenderer; }
    }

    private RenderMethodDelegate RenderTopLevelColumnGroup { get; set; }

    /// <summary>
    /// Renders the <see cref="BocList"/> in the <see cref="BocListBaseRenderer.List"/> property 
    /// to the <see cref="HtmlTextWriter"/> in the <see cref="BocListBaseRenderer.Writer"/> property.
    /// </summary>
    /// <remarks>
    /// This method provides the outline table of the <see cref="BocList"/>, creating three areas:
    /// <list type="bullet">
    /// <item><description>A table block displaying the title and data rows. See <see cref="RenderTableBlock"/>.</description></item>
    /// <item><description>A menu block containing the available commands. See <see cref="BocListMenuBlockRenderer.Render"/></description></item>
    /// <item><description>A navigation block to browse through pages of data rows. See <see cref="BocListNavigatorRenderer.Render"/>.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="BocListMenuBlockRenderer"/>
    /// <seealso cref="BocListNavigatorRenderer"/>
    public virtual void RenderContents ()
    {
      //  Render list block / menu block
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      Writer.RenderBeginTag (HtmlTextWriterTag.Table);

      RenderTopLevelColumnGroup();
      
      Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      //  List Block
      Writer.AddStyleAttribute ("vertical-align", "top");
      Writer.RenderBeginTag (HtmlTextWriterTag.Td);

      RenderTableBlock ();

      if (List.HasNavigator)
        NavigatorRenderer.Render ();

      Writer.RenderEndTag ();

      if (List.HasMenuBlock)
      {
        //  Menu Block
        Writer.AddStyleAttribute ("vertical-align", "top");
        Writer.RenderBeginTag (HtmlTextWriterTag.Td);
        MenuBlockRenderer.Render ();
        Writer.RenderEndTag ();
      }

      Writer.RenderEndTag ();  //  TR

      Writer.RenderEndTag ();  //  Table
    }

    private void RenderTopLevelColumnGroupForLegacyBrowser ()
    {
      Writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);
      
      //  Left: list block
      Writer.WriteBeginTag ("col"); //  Required because RenderBeginTag(); RenderEndTag();
                                    //  writes empty tags, which is not valid for col in HTML 4.01
      Writer.Write (">");           

      if (List.HasMenuBlock)
      {
        //  Right: menu block
        Writer.WriteBeginTag ("col");
        Writer.Write (" style=\"");

        string menuBlockWidth = c_defaultMenuBlockWidth;
        if (!List.MenuBlockWidth.IsEmpty)
          menuBlockWidth = List.MenuBlockWidth.ToString ();
        Writer.WriteStyleAttribute ("width", menuBlockWidth);

        string menuBlockOffset = c_defaultMenuBlockOffset;
        if (!List.MenuBlockOffset.IsEmpty)
          menuBlockOffset = List.MenuBlockOffset.ToString ();
        Writer.WriteStyleAttribute ("padding-left", menuBlockOffset);

        Writer.Write ("\">");
      }

      Writer.RenderEndTag ();

    }

    private void RenderTopLevelColumnGroupForXmlBrowser ()
    {
      Writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

      // Left: list block
      Writer.RenderBeginTag (HtmlTextWriterTag.Col);
      Writer.RenderEndTag();

      if (List.HasMenuBlock)
      {
        //  Right: menu block
        string menuBlockWidth = c_defaultMenuBlockWidth;
        if (!List.MenuBlockWidth.IsEmpty)
          menuBlockWidth = List.MenuBlockWidth.ToString ();

        string menuBlockOffset = c_defaultMenuBlockOffset;
        if (!List.MenuBlockOffset.IsEmpty)
          menuBlockOffset = List.MenuBlockOffset.ToString ();

        Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, menuBlockWidth);
        Writer.AddStyleAttribute (HtmlTextWriterStyle.PaddingLeft, menuBlockOffset);
        Writer.RenderBeginTag (HtmlTextWriterTag.Col);
        Writer.RenderEndTag ();
      }

      Writer.RenderEndTag ();

    }

    /// <summary>
    /// Renders the data contained in <see cref="BocListBaseRenderer.List"/> as a table.
    /// </summary>
    /// <remarks>
    /// The table consists of a title row showing the column titles, and a data row for each <see cref="IBusinessObject"/>
    /// in <see cref="BocListBaseRenderer.List"/>. If there is no data, the table will be completely hidden (only one cell containing only whitespace)
    /// if <see cref="BocList.ShowEmptyListEditMode"/> is <see langword="false"/> and <see cref="BocListBaseRenderer.List"/> is editable
    /// or if <see cref="BocList.ShowEmptyListReadOnlyMode"/> is <see langword="false"/> and <see cref="BocListBaseRenderer.List"/> is read-only.
    /// Exception: at design time, the title row will always be visible.
    /// </remarks>
    /// <seealso cref="RenderTableBlockColumnGroup"/>
    /// <seealso cref="RenderTableBody"/>
    protected virtual void RenderTableBlock ()
    {
      bool isDesignMode = ControlHelper.IsDesignMode ((Control) List);
      bool isReadOnly = List.IsReadOnly;
      bool showForEmptyList = isReadOnly && List.ShowEmptyListReadOnlyMode
                              || !isReadOnly && List.ShowEmptyListEditMode;

      if (List.IsEmptyList && !showForEmptyList)
      {
        RenderTable (isDesignMode, false);
      }
      else
      {
        RenderTable (true, true);
      }

      RenderClientSelectionScript ();
    }

    private void RenderTable (bool tableHead, bool tableBody)
    {
      if (!tableHead && !tableBody)
      {
        RenderEmptyTable();
        return;
      }

      RenderTableOpeningTag ();
      RenderTableBlockColumnGroup ();
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, List.CssClassTableHead);

      if (tableHead)
      {
        RenderTableHead();
      }

      if(tableBody)
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
      {
        RowRenderer.RenderEmptyListDataRow ();
      }
      else
      {
        bool isOddRow = true;
        BocListRow[] rows = List.EnsureGotIndexedRowsSorted ();

        for (int idxAbsoluteRows = firstRow, idxRelativeRows = 0;
             idxAbsoluteRows < rowCountWithOffset;
             idxAbsoluteRows++, idxRelativeRows++)
        {
          BocListRow row = rows[idxAbsoluteRows];
          int originalRowIndex = row.Index;
          RowRenderer.RenderDataRow (row.BusinessObject, idxRelativeRows, idxAbsoluteRows, originalRowIndex, isOddRow);
          isOddRow = !isOddRow;
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
        ScriptUtility.RegisterStartupScriptBlock (List, typeof (BocList).FullName + "_" + List.ClientID + "_InitializeListScript", script);
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
      BocColumnDefinition[] renderColumns = List.EnsureColumnsGot ();

      Writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

      bool isTextXml = false;

      if (!ControlHelper.IsDesignMode ((Control) List))
        isTextXml = ControlHelper.IsXmlConformResponseTextRequired (HttpContext.Current);

      RenderIndexColumnDeclaration(isTextXml);
      RenderSelectorColumnDeclaration(isTextXml);

      //bool isFirstColumnUndefinedWidth = true;
      for (int i = 0; i < renderColumns.Length; i++)
      {
        BocColumnDefinition column = renderColumns[i];

        if (!List.IsColumnVisible (column))
          continue;

        RenderDataColumnDeclaration(isTextXml, column);
      }

      //  Design-mode and empty table
      if (ControlHelper.IsDesignMode ((Control) List) && renderColumns.Length == 0)
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
        {
          width = "2em";
        }
        else
        {
          width = column.Width.ToString ();
        }
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
