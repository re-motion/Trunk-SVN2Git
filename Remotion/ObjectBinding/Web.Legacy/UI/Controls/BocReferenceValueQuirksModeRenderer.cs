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
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls
{
  /// <summary>
  /// Responsible for rendering <see cref="BocReferenceValue"/> controls in Quirks Mode.
  /// </summary>
  /// <remarks>
  /// <para>During edit mode, the control is displayed using a <see cref="System.Web.UI.WebControls.DropDownList"/>.</para>
  /// <para>During read-only mode, the control's value is displayed using a <see cref="System.Web.UI.WebControls.Label"/>.</para>
  /// </remarks>
  public class BocReferenceValueQuirksModeRenderer : BocRendererBase<IBocReferenceValue>
  {
    private const string c_defaultControlWidth = "150pt";
    private readonly Func<DropDownList> _dropDownListFactoryMethod;

    public BocReferenceValueQuirksModeRenderer (HttpContextBase context, IBocReferenceValue control)
        : this (context, control, () => new DropDownList())
    {
    }

    public BocReferenceValueQuirksModeRenderer (HttpContextBase context, IBocReferenceValue control, Func<DropDownList> dropDownListFactoryMethod)
        : base (context, control)
    {
      ArgumentUtility.CheckNotNull ("dropDownListFactoryMethod", dropDownListFactoryMethod);
      _dropDownListFactoryMethod = dropDownListFactoryMethod;
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude (Control);

      RegisterBrowserCompatibilityScript (htmlHeadAppender);

      string scriptFileKey = typeof (BocReferenceValueQuirksModeRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocReferenceValueQuirksModeRenderer), ResourceType.Html, ResourceTheme.Legacy, "BocReferenceValue.js");
        htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);
      }

      string styleFileKey = typeof (BocReferenceValueQuirksModeRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (styleFileKey))
      {
        string url = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocReferenceValueQuirksModeRenderer), ResourceType.Html, ResourceTheme.Legacy, "BocReferenceValue.css");

        htmlHeadAppender.RegisterStylesheetLink (styleFileKey, url, HtmlHeadAppender.Priority.Library);
      }
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      AddAttributesToRender (writer, false);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      DropDownList dropDownList = GetDropDownList();
      dropDownList.Page = Control.Page.WrappedInstance;
      Label label = GetLabel();
      Image icon = GetIcon();

      if (Control.HasValueEmbeddedInsideOptionsMenu == true && Control.HasOptionsMenu
          || Control.HasValueEmbeddedInsideOptionsMenu == null && Control.IsReadOnly && Control.HasOptionsMenu)
        RenderContentsWithIntegratedOptionsMenu (writer, dropDownList, label);
      else
        RenderContentsWithSeparateOptionsMenu (writer, dropDownList, label, icon);

      writer.RenderEndTag();
    }

    private DropDownList GetDropDownList ()
    {
      var dropDownList = _dropDownListFactoryMethod();
      dropDownList.ID = Control.DropDownListUniqueID;
      dropDownList.EnableViewState = false;
      Control.PopulateDropDownList (dropDownList);

      dropDownList.Enabled = Control.Enabled;
      dropDownList.Height = Unit.Empty;
      dropDownList.Width = Unit.Empty;
      dropDownList.ApplyStyle (Control.CommonStyle);
      Control.DropDownListStyle.ApplyStyle (dropDownList);

      return dropDownList;
    }

    private Label GetLabel ()
    {
      var label = new Label { ID = Control.LabelClientID, EnableViewState = false, Height = Unit.Empty, Width = Unit.Empty };
      label.ApplyStyle (Control.CommonStyle);
      label.ApplyStyle (Control.LabelStyle);
      label.Text = Control.GetLabelText();
      return label;
    }

    private Image GetIcon ()
    {
      var icon = new Image { EnableViewState = false, ID = Control.IconClientID, Visible = false };
      if (Control.EnableIcon && Control.Property != null)
      {
        IconInfo iconInfo = Control.GetIcon (Control.Value, Control.Property.ReferenceClass.BusinessObjectProvider);

        if (iconInfo != null)
        {
          icon.ImageUrl = iconInfo.Url;
          icon.Width = iconInfo.Width;
          icon.Height = iconInfo.Height;

          icon.Visible = true;
          icon.Style["vertical-align"] = "middle";
          icon.Style["border-style"] = "none";

          if (Control.IsCommandEnabled (Control.IsReadOnly))
          {
            if (string.IsNullOrEmpty (iconInfo.AlternateText))
            {
              if (Control.Value == null)
                icon.AlternateText = String.Empty;
              else
                icon.AlternateText = HttpUtility.HtmlEncode (Control.Value.DisplayNameSafe);
            }
            else
              icon.AlternateText = iconInfo.AlternateText;
          }
        }
      }
      return icon;
    }

    protected override void AddAdditionalAttributes (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      base.AddAdditionalAttributes (writer);
      writer.AddStyleAttribute ("display", "inline");
    }

    public override string CssClassBase
    {
      get { return "bocReferenceValue"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocReferenceValue"/>'s value. </summary>
    /// <remarks> Class: <c>bocReferenceValueContent</c> </remarks>
    public virtual string CssClassContent
    {
      get { return "bocReferenceValueContent"; }
    }

    private void RenderContentsWithSeparateOptionsMenu (HtmlTextWriter writer, DropDownList dropDownList, Label label, Image icon)
    {
      bool isReadOnly = Control.IsReadOnly;

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);
      bool isDropDownListHeightEmpty = dropDownList.Height.IsEmpty
                                       && string.IsNullOrEmpty (dropDownList.Style["height"]);
      bool isControlWidthEmpty = Control.Width.IsEmpty && string.IsNullOrEmpty (Control.Style["width"]);
      bool isLabelWidthEmpty = label.Width.IsEmpty
                               && string.IsNullOrEmpty (label.Style["width"]);
      bool isDropDownListWidthEmpty = dropDownList.Width.IsEmpty
                                      && string.IsNullOrEmpty (dropDownList.Style["width"]);
      if (isReadOnly)
      {
        if (isLabelWidthEmpty && !isControlWidthEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }
      else
      {
        if (!isControlHeightEmpty && isDropDownListHeightEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

        if (isDropDownListWidthEmpty)
        {
          if (isControlWidthEmpty)
            writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
          else
            writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        }
      }

      writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      writer.AddStyleAttribute ("display", "inline");
      writer.RenderBeginTag (HtmlTextWriterTag.Table); // Begin table
      writer.RenderBeginTag (HtmlTextWriterTag.Tr); //  Begin tr

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = "";
      if (!Control.IsDesignMode)
        postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
        RenderReadOnlyValue (writer, icon, label, isCommandEnabled, postBackEvent, string.Empty, objectID);
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (writer, icon, isCommandEnabled, postBackEvent, string.Empty, objectID);
        RenderEditModeValue (writer, dropDownList, isControlHeightEmpty, isDropDownListHeightEmpty, isDropDownListWidthEmpty);
      }

      bool hasOptionsMenu = Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        writer.AddStyleAttribute ("padding-left", "0.3em");
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        //writer.AddAttribute ("align", "right");
        writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
        Control.OptionsMenu.Width = Control.OptionsMenuWidth;
        Control.OptionsMenu.RenderControl (writer);
        writer.RenderEndTag(); //  End td
      }

      //HACK: Opera has problems with inline tables and may collapse contents unless a cell with width 0% is present
      if (!Control.IsDesignMode && !isReadOnly && !hasOptionsMenu && !icon.Visible
          && Context.Request.Browser.Browser == "Opera")
      {
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
        writer.Write ("&nbsp;");
        writer.RenderEndTag(); // End td
      }

      writer.RenderEndTag();
      writer.RenderEndTag();
    }

    private void RenderContentsWithIntegratedOptionsMenu (HtmlTextWriter writer, DropDownList dropDownList, Label label)
    {
      bool isReadOnly = Control.IsReadOnly;

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);
      bool isDropDownListHeightEmpty = string.IsNullOrEmpty (dropDownList.Style["height"]);
      bool isControlWidthEmpty = Control.Width.IsEmpty && string.IsNullOrEmpty (Control.Style["width"]);
      bool isLabelWidthEmpty = string.IsNullOrEmpty (label.Style["width"]);
      bool isDropDownListWidthEmpty = string.IsNullOrEmpty (dropDownList.Style["width"]);

      if (isReadOnly)
      {
        if (isLabelWidthEmpty && !isControlWidthEmpty)
          Control.OptionsMenu.Style["width"] = "100%";
        else
          Control.OptionsMenu.Style["width"] = "0%";
      }
      else
      {
        if (!isControlHeightEmpty && isDropDownListHeightEmpty)
          Control.OptionsMenu.Style["height"] = "100%";

        if (isDropDownListWidthEmpty)
        {
          if (isControlWidthEmpty)
            Control.OptionsMenu.Style["width"] = c_defaultControlWidth;
          else
            Control.OptionsMenu.Style["width"] = "100%";
        }
      }

      Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (RenderOptionsMenuTitle);
      Control.OptionsMenu.RenderControl (writer);
      Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (null);
    }

    public void RenderOptionsMenuTitle (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      DropDownList dropDownList = GetDropDownList();
      dropDownList.Page = Control.Page.WrappedInstance;
      Image icon = GetIcon();
      Label label = GetLabel();
      bool isReadOnly = Control.IsReadOnly;

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);
      bool isDropDownListHeightEmpty = string.IsNullOrEmpty (dropDownList.Style["height"]);
      bool isControlWidthEmpty = Control.Width.IsEmpty && string.IsNullOrEmpty (Control.Style["width"]);
      bool isDropDownListWidthEmpty = string.IsNullOrEmpty (dropDownList.Style["width"]);

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
      {
        RenderReadOnlyValue (writer, icon, label, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        if (!isControlWidthEmpty)
        {
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%");
          writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
          writer.RenderEndTag();
        }
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (writer, icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        dropDownList.Attributes.Add ("onclick", DropDownMenu.OnHeadTitleClickScript);
        RenderEditModeValue (writer, dropDownList, isControlHeightEmpty, isDropDownListHeightEmpty, isDropDownListWidthEmpty);
      }
    }

    private void RenderSeparateIcon (HtmlTextWriter writer, Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      writer.AddStyleAttribute ("padding-right", "0.3em");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (isCommandEnabled)
      {
        Control.Command.RenderBegin (writer, postBackEvent, onClick, objectID, null);
        if (!string.IsNullOrEmpty (Control.Command.ToolTip))
          icon.ToolTip = Control.Command.ToolTip;
      }
      icon.RenderControl (writer);
      if (isCommandEnabled)
        Control.Command.RenderEnd (writer);

      writer.RenderEndTag(); //  End td
    }

    private void RenderReadOnlyValue (
        HtmlTextWriter writer, Image icon, Label label, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (isCommandEnabled)
        Control.Command.RenderBegin (writer, postBackEvent, onClick, objectID, null);
      if (icon.Visible)
      {
        icon.RenderControl (writer);
        writer.Write ("&nbsp;");
      }
      label.RenderControl (writer);
      if (isCommandEnabled)
        Control.Command.RenderEnd (writer);

      writer.RenderEndTag(); //  End td
    }

    private void RenderEditModeValue (
        HtmlTextWriter writer, DropDownList dropDownList, bool isControlHeightEmpty, bool isDropDownListHeightEmpty, bool isDropDownListWidthEmpty)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (!isControlHeightEmpty && isDropDownListHeightEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
      if (isDropDownListWidthEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      dropDownList.RenderControl (writer);

      writer.RenderEndTag(); //  End td

      RenderEditModeValueExtension (writer);
    }

    /// <summary> Called after the edit mode value's cell is rendered. </summary>
    /// <remarks> Render a table cell: &lt;td style="width:0%"&gt;Your contents goes here&lt;/td&gt;</remarks>
    protected virtual void RenderEditModeValueExtension (HtmlTextWriter writer)
    {
    }
  }
}