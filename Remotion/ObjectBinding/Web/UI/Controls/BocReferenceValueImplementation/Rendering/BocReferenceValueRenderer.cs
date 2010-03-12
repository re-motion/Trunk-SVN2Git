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
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocReferenceValue"/> controls in Quirks Mode.
  /// </summary>
  /// <remarks>
  /// <para>During edit mode, the control is displayed using a <see cref="System.Web.UI.WebControls.DropDownList"/>.</para>
  /// <para>During read-only mode, the control's value is displayed using a <see cref="System.Web.UI.WebControls.Label"/>.</para>
  /// </remarks>
  public class BocReferenceValueRenderer : BocRendererBase<IBocReferenceValue>
  {
    private readonly Func<DropDownList> _dropDownListFactoryMethod;

    public BocReferenceValueRenderer (HttpContextBase context, IBocReferenceValue control)
        : this (context, control, () => new DropDownList ())
    {
    }

    public BocReferenceValueRenderer (
        HttpContextBase context, IBocReferenceValue control, Func<DropDownList> dropDownListFactoryMethod)
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

      string scriptFileKey = typeof (BocReferenceValueRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocReferenceValueRenderer), ResourceType.Html, "BocReferenceValue.js");
        htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);
      }

      string styleFileKey = typeof (BocReferenceValueRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (styleFileKey))
      {
        string url = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocReferenceValueRenderer), ResourceType.Html, ResourceTheme, "BocReferenceValue.css");

        htmlHeadAppender.RegisterStylesheetLink (styleFileKey, url, HtmlHeadAppender.Priority.Library);
      }
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      RegisterAdjustPositionScript ();
      RegisterAdjustLayoutScript ();

      AddAttributesToRender (writer, false);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (EmbedInOptionsMenu)
        RenderContentsWithIntegratedOptionsMenu (writer);
      else
        RenderContentsWithSeparateOptionsMenu (writer);

      writer.RenderEndTag();
    }

    private void RegisterAdjustPositionScript ()
    {
      string key = Control.ClientID + "_AdjustPositionScript";
      Control.Page.ClientScript.RegisterStartupScriptBlock (
          Control,
          typeof (BocReferenceValueRenderer),
          key,
          string.Format (
              "BocReferenceValue_AdjustPosition(document.getElementById('{0}'), {1});",
              Control.ClientID,
              EmbedInOptionsMenu ? "true" : "false"));
    }

    private void RegisterAdjustLayoutScript ()
    {
      Control.Page.ClientScript.RegisterStartupScriptBlock (
          Control,
          typeof (BocReferenceValueRenderer),
          Guid.NewGuid ().ToString (),
          string.Format ("BocBrowserCompatibility.AdjustReferenceValueLayout ($('#{0}'));", Control.ClientID));
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
      var icon = new Image { EnableViewState = false, ID = Control.IconClientID, Visible = false, GenerateEmptyAlternateText = true };
      if (Control.EnableIcon && Control.Property != null)
      {
        IconInfo iconInfo = Control.GetIcon (Control.Value, Control.Property.ReferenceClass.BusinessObjectProvider);

        if (iconInfo != null)
        {
          icon.ImageUrl = iconInfo.Url;
          icon.Width = iconInfo.Width;
          icon.Height = iconInfo.Height;

          icon.Visible = true;
          icon.CssClass = CssClassContent;

          if (Control.IsCommandEnabled (Control.IsReadOnly))
          {
            if (string.IsNullOrEmpty (iconInfo.AlternateText))
            {
              if (Control.Value == null)
                icon.AlternateText = string.Empty;
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

    private void RenderContentsWithSeparateOptionsMenu (HtmlTextWriter writer)
    {
      DropDownList dropDownList = GetDropDownList ();
      dropDownList.Page = Control.Page.WrappedInstance;
      Label label = GetLabel ();
      Image icon = GetIcon ();

      bool isReadOnly = Control.IsReadOnly;

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = "";
      if (!Control.IsDesignMode)
        postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
      {
        RenderReadOnlyValue (writer, icon, label, isCommandEnabled, postBackEvent, string.Empty, objectID);
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (writer, icon, isCommandEnabled, postBackEvent, string.Empty, objectID);

        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassInnerContent);
        writer.RenderBeginTag (HtmlTextWriterTag.Span);

        RenderEditModeValue (writer, dropDownList);
       
        writer.RenderEndTag ();
      }

      bool hasOptionsMenu = Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassOptionsMenu);
        writer.RenderBeginTag (HtmlTextWriterTag.Span);

        Control.OptionsMenu.Width = Control.OptionsMenuWidth;
        Control.OptionsMenu.RenderControl (writer);

        writer.RenderEndTag ();
      }

      writer.RenderEndTag ();
    }

    private void RenderContentsWithIntegratedOptionsMenu(HtmlTextWriter writer)
    {
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
      label.CssClass = CssClassReadOnly;
      bool isReadOnly = Control.IsReadOnly;

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
      {
        RenderReadOnlyValue (writer, icon, label, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
      }
      else
      {
        if (icon.Visible)
        {
          RenderSeparateIcon (writer, icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        }
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassInnerContent);
        writer.RenderBeginTag (HtmlTextWriterTag.Span);

        dropDownList.Attributes.Add ("onclick", DropDownMenu.OnHeadTitleClickScript);
        RenderEditModeValue (writer, dropDownList);

        writer.RenderEndTag();
      }
    }

    private void RenderSeparateIcon (HtmlTextWriter writer, Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassCommand);
      if (isCommandEnabled)
      {
        Control.Command.RenderBegin (writer, postBackEvent, onClick, objectID, null);
        if (!string.IsNullOrEmpty (Control.Command.ToolTip))
          icon.ToolTip = Control.Command.ToolTip;
      }
      else
      {
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      icon.RenderControl (writer);

      if (isCommandEnabled)
        Control.Command.RenderEnd (writer);
      else
        writer.RenderEndTag();
    }

    private void RenderReadOnlyValue (HtmlTextWriter writer, Image icon, Label label, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassCommand);
      if (isCommandEnabled)
        Control.Command.RenderBegin (writer, postBackEvent, onClick, objectID, null);
      else
        writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (icon.Visible)
      {
        icon.RenderControl (writer);
      }
      label.RenderControl (writer);

      if (isCommandEnabled)
        Control.Command.RenderEnd (writer);
      else
        writer.RenderEndTag();
    }

    private void RenderEditModeValue (HtmlTextWriter writer, DropDownList dropDownList)
    {
      dropDownList.RenderControl (writer);

      RenderEditModeValueExtension (writer);
    }

    /// <summary> Called after the edit mode value's cell is rendered. </summary>
    /// <remarks> Render a table cell: &lt;td style="width:0%"&gt;Your contents goes here&lt;/td&gt;</remarks>
    protected virtual void RenderEditModeValueExtension (HtmlTextWriter writer)
    {
    }

    private bool EmbedInOptionsMenu
    {
      get
      {
        return Control.HasValueEmbeddedInsideOptionsMenu == true && Control.HasOptionsMenu
               || Control.HasValueEmbeddedInsideOptionsMenu == null && Control.IsReadOnly && Control.HasOptionsMenu;
      }
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

    private string CssClassInnerContent
    {
      get { return "content"; }
    }

    private string CssClassOptionsMenu
    {
      get { return "bocReferenceValueOptionsMenu"; }
    }

    private string CssClassCommand
    {
      get { return "bocReferenceValueCommand"; }
    }
  }
}