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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  /// <summary>
  /// Abstract base class for BocList renderers. Defines common constants, properties and utility methods.
  /// </summary>
  public abstract class BocListRendererBase : RenderableControlRendererBase<IBocList>
  {
    // constants
    // unused protected const string c_dataRowHiddenFieldIDSuffix = "_Boc_HiddenField_";
    /// <summary>Suffix for controls used for selecting or unselecting single rows.</summary>
    protected const string c_dataRowSelectorControlIDSuffix = "_Boc_SelectorControl_";

    /// <summary>Suffix for the control used to select all visible rows.</summary>
    protected const string c_titleRowSelectorControlIDSuffix = "_Boc_SelectorControl_SelectAll";

    // unused protected const string c_availableViewsListIDSuffix = "_Boc_AvailableViewsList";
    // unused protected const string c_optionsMenuIDSuffix = "_Boc_OptionsMenu";

    // <summary> Prefix applied to the post back argument of the event type column commands. </summary>
    // unused protected const int c_titleRowIndex = -1;protected const string c_eventListItemCommandPrefix = "ListCommand=";
    // <summary> Prefix applied to the post back argument of the event type menu commands. </summary>
    // unused protected const string c_eventMenuItemPrefix = "MenuItem=";
    // <summary> Prefix applied to the post back argument of the custom columns. </summary>
    // unused protected const string c_customCellEventPrefix = "CustomCell=";

    // unused protected const string c_rowEditModeRequiredFieldIcon = "RequiredField.gif";
    // unused protected const string c_rowEditModeValidationErrorIcon = "ValidationError.gif";

    // unused protected const string c_scriptFileUrl = "BocList.js";
    /// <summary>Name of the JavaScript function to call when a command control has been clicked.</summary>
    protected const string c_onCommandClickScript = "BocList_OnCommandClick();";

    // unused protected const string c_styleFileUrl = "BocList.css";

    /// <summary>Entity definition for whitespace separating controls, e.g. icons from following text</summary>
    protected const string c_whiteSpace = "&nbsp;";

    // <summary> The key identifying a fixed column resource entry. </summary>
    // unused protected const string c_resourceKeyFixedColumns = "FixedColumns";
    // <summary> The key identifying a options menu item resource entry. </summary>
    // unused protected const string c_resourceKeyOptionsMenuItems = "OptionsMenuItems";
    // <summary> The key identifying a List menu item resource entry. </summary>
    // unused protected const string c_resourceKeyListMenuItems = "ListMenuItems";

    /// <summary>Number of columns to show in design mode before actual columns have been defined.</summary>
    protected const int c_designModeDummyColumnCount = 3;

    /// <summary>
    /// Constructor initializing the renderer with the <see cref="BocList"/> rendering object and the
    /// <see cref="HtmlTextWriter"/> rendering target.
    /// </summary>
    /// <remarks>Each <see cref="BocList"/> renderer has to be bound to the list to render and 
    /// the <see cref="HtmlTextWriter"/> target to render it to. Therefore, these properties are <code>readonly</code>
    /// and must be set in the constructor.</remarks>
    /// <param name="context">The <see cref="IHttpContext"/> that contains the response for which to render the list.</param>
    /// <param name="list">The <see cref="BocList"/> to render.</param>
    /// <param name="writer">The <see cref="HtmlTextWriter"/> to render the list to.</param>
    protected BocListRendererBase (IHttpContext context, HtmlTextWriter writer, IBocList list)
        : base(context, writer, list)
    {
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("writer", writer);
    }

    /// <summary>Gets the <see cref="BocList"/> object that will be rendered.</summary>
    public IBocList List
    {
      get { return Control; }
    }

    /// <summary>
    /// Renders an <see cref="IconInfo"/> control with an alternate text.
    /// </summary>
    /// <remarks>If no alternate text is provided in the <code>icon</code> argument, the method will attempt to load
    /// the alternate text from the resources file, using <code>alternateTextID</code> as key.</remarks>
    /// <param name="icon">The icon to render. If it has an alternate text, that text will be used.</param>
    /// <param name="alternateTextID">The <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ResourceIdentifier"/> used to load 
    /// the alternate text from the resource file. Can be <see langword="null"/>, in which case no text will be loaded.</param>
    protected void RenderIcon (IconInfo icon, Controls.BocList.ResourceIdentifier? alternateTextID)
    {
      ArgumentUtility.CheckNotNull ("icon", icon);

      bool hasAlternateText = !StringUtility.IsNullOrEmpty (icon.AlternateText);
      if (!hasAlternateText)
      {
        if (alternateTextID.HasValue)
          icon.AlternateText = List.GetResourceManager().GetString (alternateTextID);
      }

      icon.Render (Writer);

      if (!hasAlternateText)
        icon.AlternateText = string.Empty;
    }

    /// <summary> Renders a <see cref="CheckBox"/> or <see cref="RadioButton"/> used for row selection. </summary>
    /// <param name="id"> The <see cref="string"/> rendered into the <c>id</c> and <c>name</c> attributes. </param>
    /// <param name="value"> The value of the <see cref="CheckBox"/> or <see cref="RadioButton"/>. </param>
    /// <param name="isChecked"> 
    ///   <see langword="true"/> if the <see cref="CheckBox"/> or <see cref="RadioButton"/> is checked. 
    /// </param>
    /// <param name="isSelectAllSelectorControl"> 
    ///   <see langword="true"/> if the rendered <see cref="CheckBox"/> or <see cref="RadioButton"/> is in the title row.
    /// </param>
    protected void RenderSelectorControl (
        string id,
        string value,
        bool isChecked,
        bool isSelectAllSelectorControl)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);

      if (List.Selection == RowSelection.SingleRadioButton)
        Writer.AddAttribute (HtmlTextWriterAttribute.Type, "radio");
      else
        Writer.AddAttribute (HtmlTextWriterAttribute.Type, "checkbox");
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, id);
      Writer.AddAttribute (HtmlTextWriterAttribute.Name, id);

      if (isChecked)
        Writer.AddAttribute (HtmlTextWriterAttribute.Checked, "checked");
      if (List.EditModeController.IsRowEditModeActive)
        Writer.AddAttribute (HtmlTextWriterAttribute.Disabled, "true");

      Writer.AddAttribute (HtmlTextWriterAttribute.Value, value);

      if (isSelectAllSelectorControl)
        AddSelectAllSelectorAttributes();
      else
        AddRowSelectorAttributes();

      Writer.RenderBeginTag (HtmlTextWriterTag.Input);
      Writer.RenderEndTag();
    }

    private void AddRowSelectorAttributes ()
    {
      string alternateText = List.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.SelectRowAlternateText);
      Writer.AddAttribute (HtmlTextWriterAttribute.Alt, alternateText);

      if (List.HasClientScript)
      {
        const string script = "BocList_OnSelectionSelectorControlClick();";
        Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
    }

    private void AddSelectAllSelectorAttributes ()
    {
      string alternateText = List.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.SelectAllRowsAlternateText);
      Writer.AddAttribute (HtmlTextWriterAttribute.Alt, alternateText);

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
                        + List.ClientID + c_dataRowSelectorControlIDSuffix + "', "
                        + count + ");";
        Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
    }

    protected string GetCssClassTableCell (bool isOddRow)
    {
      string cssClassTableCell;
      if (isOddRow)
        cssClassTableCell = List.CssClassDataCellOdd;
      else
        cssClassTableCell = List.CssClassDataCellEven;
      return cssClassTableCell;
    }

    protected override void AddAdditionalAttributes ()
    {
    }

    public override string CssClassBase
    {
      get { throw new NotImplementedException (); }
    }
  }
}