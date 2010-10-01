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
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocReferenceValue"/> controls in Quirks Mode.
  /// </summary>
  /// <remarks>
  /// <para>During edit mode, the control is displayed using a <see cref="System.Web.UI.WebControls.DropDownList"/>.</para>
  /// <para>During read-only mode, the control's value is displayed using a <see cref="System.Web.UI.WebControls.Label"/>.</para>
  /// </remarks>
  public class BocReferenceValueQuirksModeRenderer : BocQuirksModeRendererBase<IBocReferenceValue>, IBocReferenceValueRenderer
  {
    private const string c_defaultControlWidth = "150pt";
    private readonly Func<DropDownList> _dropDownListFactoryMethod;

    public BocReferenceValueQuirksModeRenderer ()
        : this (() => new DropDownList())
    {
    }

    protected BocReferenceValueQuirksModeRenderer (Func<DropDownList> dropDownListFactoryMethod)
        : base (null, null)
    {
      ArgumentUtility.CheckNotNull ("dropDownListFactoryMethod", dropDownListFactoryMethod);
      _dropDownListFactoryMethod = dropDownListFactoryMethod;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IControl control, HttpContextBase context)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();

      string scriptFileKey = typeof (BocReferenceValueQuirksModeRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            control, context, typeof (BocReferenceValueQuirksModeRenderer), ResourceType.Html, "BocReferenceValue.js");
        htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);
      }

      string styleFileKey = typeof (BocReferenceValueQuirksModeRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (styleFileKey))
      {
        string url = ResourceUrlResolver.GetResourceUrl (
            control, context, typeof (BocReferenceValueQuirksModeRenderer), ResourceType.Html, "BocReferenceValue.css");

        htmlHeadAppender.RegisterStylesheetLink (styleFileKey, url, HtmlHeadAppender.Priority.Library);
      }
    }

    public void Render (BocReferenceValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (renderingContext, false);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      DropDownList dropDownList = GetDropDownList (renderingContext);
      dropDownList.Page = renderingContext.Control.Page.WrappedInstance;
      Label label = GetLabel (renderingContext);
      Image icon = GetIcon (renderingContext);

      if (IsEmbedInOptionsMenu(renderingContext))
        RenderContentsWithIntegratedOptionsMenu (renderingContext, dropDownList, label);
      else
        RenderContentsWithSeparateOptionsMenu (renderingContext, dropDownList, label, icon);

      renderingContext.Writer.RenderEndTag ();
    }

    private DropDownList GetDropDownList (BocReferenceValueRenderingContext renderingContext)
    {
      var dropDownList = _dropDownListFactoryMethod();
      dropDownList.ID = renderingContext.Control.DropDownListUniqueID;
      dropDownList.EnableViewState = false;
      renderingContext.Control.PopulateDropDownList (dropDownList);

      dropDownList.Enabled = renderingContext.Control.Enabled;
      dropDownList.Height = Unit.Empty;
      dropDownList.Width = Unit.Empty;
      dropDownList.ApplyStyle (renderingContext.Control.CommonStyle);
      renderingContext.Control.DropDownListStyle.ApplyStyle (dropDownList);

      return dropDownList;
    }

    private Label GetLabel (BocReferenceValueRenderingContext renderingContext)
    {
      var label = new Label { ID = renderingContext.Control.LabelClientID, EnableViewState = false, Height = Unit.Empty, Width = Unit.Empty };
      label.ApplyStyle (renderingContext.Control.CommonStyle);
      label.ApplyStyle (renderingContext.Control.LabelStyle);
      label.Text = HttpUtility.HtmlEncode (renderingContext.Control.GetLabelText ());
      return label;
    }

    private Image GetIcon (BocReferenceValueRenderingContext renderingContext)
    {
      var icon = new Image { EnableViewState = false, ID = renderingContext.Control.IconClientID, Visible = false };
      if (renderingContext.Control.EnableIcon && renderingContext.Control.Property != null)
      {
        IconInfo iconInfo = renderingContext.Control.GetIcon ();

        if (iconInfo != null)
        {
          icon.ImageUrl = iconInfo.Url;
          icon.Width = iconInfo.Width;
          icon.Height = iconInfo.Height;

          icon.Visible = true;
          icon.Style["vertical-align"] = "middle";
          icon.Style["border-style"] = "none";

          if (renderingContext.Control.IsCommandEnabled (renderingContext.Control.IsReadOnly))
          {
            if (string.IsNullOrEmpty (iconInfo.AlternateText))
            {
              if (renderingContext.Control.Value == null)
                icon.AlternateText = String.Empty;
              else
                icon.AlternateText = HttpUtility.HtmlEncode (renderingContext.Control.GetLabelText ());
            }
            else
              icon.AlternateText = iconInfo.AlternateText;
          }
        }
      }
      return icon;
    }

    protected override void AddAdditionalAttributes (RenderingContext<IBocReferenceValue> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      base.AddAdditionalAttributes (renderingContext);
      renderingContext.Writer.AddStyleAttribute ("display", "inline");
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

    private void RenderContentsWithSeparateOptionsMenu (BocReferenceValueRenderingContext renderingContext, DropDownList dropDownList, Label label, Image icon)
    {
      bool isReadOnly = renderingContext.Control.IsReadOnly;

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["height"]);
      bool isDropDownListHeightEmpty = dropDownList.Height.IsEmpty
                                       && string.IsNullOrEmpty (dropDownList.Style["height"]);
      bool isControlWidthEmpty = renderingContext.Control.Width.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["width"]);
      bool isLabelWidthEmpty = label.Width.IsEmpty
                               && string.IsNullOrEmpty (label.Style["width"]);
      bool isDropDownListWidthEmpty = dropDownList.Width.IsEmpty
                                      && string.IsNullOrEmpty (dropDownList.Style["width"]);
      if (isReadOnly)
      {
        if (isLabelWidthEmpty && !isControlWidthEmpty)
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }
      else
      {
        if (!isControlHeightEmpty && isDropDownListHeightEmpty)
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

        if (isDropDownListWidthEmpty)
        {
          if (isControlWidthEmpty)
            renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
          else
            renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        }
      }

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      renderingContext.Writer.AddStyleAttribute ("display", "inline");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Table); // Begin table
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr); //  Begin tr

      bool isCommandEnabled = renderingContext.Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = "";
      if (!renderingContext.Control.IsDesignMode)
        postBackEvent = renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (renderingContext.Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
        RenderReadOnlyValue (renderingContext, icon, label, isCommandEnabled, postBackEvent, string.Empty, objectID);
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (renderingContext, icon, isCommandEnabled, postBackEvent, string.Empty, objectID);
        RenderEditModeValue (renderingContext, dropDownList, isControlHeightEmpty, isDropDownListHeightEmpty, isDropDownListWidthEmpty);
      }

      bool hasOptionsMenu = renderingContext.Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        renderingContext.Writer.AddStyleAttribute ("padding-left", "0.3em");
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        //writer.AddAttribute ("align", "right");
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
        renderingContext.Control.OptionsMenu.Width = renderingContext.Control.OptionsMenuWidth;
        renderingContext.Control.OptionsMenu.RenderControl (renderingContext.Writer);
        renderingContext.Writer.RenderEndTag (); //  End td
      }

      //HACK: Opera has problems with inline tables and may collapse contents unless a cell with width 0% is present
      if (!renderingContext.Control.IsDesignMode && !isReadOnly && !hasOptionsMenu && !icon.Visible
          && renderingContext.HttpContext.Request.Browser.Browser == "Opera")
      {
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
        renderingContext.Writer.Write ("&nbsp;");
        renderingContext.Writer.RenderEndTag (); // End td
      }

      renderingContext.Writer.RenderEndTag ();
      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderContentsWithIntegratedOptionsMenu (BocReferenceValueRenderingContext renderingContext, DropDownList dropDownList, Label label)
    {
      bool isReadOnly = renderingContext.Control.IsReadOnly;

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["height"]);
      bool isDropDownListHeightEmpty = string.IsNullOrEmpty (dropDownList.Style["height"]);
      bool isControlWidthEmpty = renderingContext.Control.Width.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["width"]);
      bool isLabelWidthEmpty = string.IsNullOrEmpty (label.Style["width"]);
      bool isDropDownListWidthEmpty = string.IsNullOrEmpty (dropDownList.Style["width"]);

      if (isReadOnly)
      {
        if (isLabelWidthEmpty && !isControlWidthEmpty)
          renderingContext.Control.OptionsMenu.Style["width"] = "100%";
        else
          renderingContext.Control.OptionsMenu.Style["width"] = "0%";
      }
      else
      {
        if (!isControlHeightEmpty && isDropDownListHeightEmpty)
          renderingContext.Control.OptionsMenu.Style["height"] = "100%";

        if (isDropDownListWidthEmpty)
        {
          if (isControlWidthEmpty)
            renderingContext.Control.OptionsMenu.Style["width"] = c_defaultControlWidth;
          else
            renderingContext.Control.OptionsMenu.Style["width"] = "100%";
        }
      }

      renderingContext.Control.OptionsMenu.SetRenderHeadTitleMethodDelegate ((writer)=>RenderOptionsMenuTitle(renderingContext));
      renderingContext.Control.OptionsMenu.RenderControl (renderingContext.Writer);
      renderingContext.Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (null);
    }

    public void RenderOptionsMenuTitle (BocReferenceValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      DropDownList dropDownList = GetDropDownList (renderingContext);
      dropDownList.Page = renderingContext.Control.Page.WrappedInstance;
      Image icon = GetIcon (renderingContext);
      Label label = GetLabel (renderingContext);
      bool isReadOnly = renderingContext.Control.IsReadOnly;

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["height"]);
      bool isDropDownListHeightEmpty = string.IsNullOrEmpty (dropDownList.Style["height"]);
      bool isControlWidthEmpty = renderingContext.Control.Width.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["width"]);
      bool isDropDownListWidthEmpty = string.IsNullOrEmpty (dropDownList.Style["width"]);

      bool isCommandEnabled = renderingContext.Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (renderingContext.Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
      {
        RenderReadOnlyValue (renderingContext, icon, label, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        if (!isControlWidthEmpty)
        {
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%");
          renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
          renderingContext.Writer.RenderEndTag();
        }
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (renderingContext, icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        dropDownList.Attributes.Add ("onclick", DropDownMenu.OnHeadTitleClickScript);
        RenderEditModeValue (renderingContext, dropDownList, isControlHeightEmpty, isDropDownListHeightEmpty, isDropDownListWidthEmpty);
      }
    }

    private void RenderSeparateIcon (BocReferenceValueRenderingContext renderingContext, Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      renderingContext.Writer.AddStyleAttribute ("padding-right", "0.3em");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (isCommandEnabled)
      {
        renderingContext.Control.Command.RenderBegin (renderingContext.Writer, postBackEvent, onClick, objectID, null);
        if (!string.IsNullOrEmpty (renderingContext.Control.Command.ToolTip))
          icon.ToolTip = renderingContext.Control.Command.ToolTip;
      }
      icon.RenderControl (renderingContext.Writer);
      if (isCommandEnabled)
        renderingContext.Control.Command.RenderEnd (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag (); //  End td
    }

    private void RenderReadOnlyValue (
        BocReferenceValueRenderingContext renderingContext, Image icon, Label label, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (isCommandEnabled)
        renderingContext.Control.Command.RenderBegin (renderingContext.Writer, postBackEvent, onClick, objectID, null);
      if (icon.Visible)
      {
        icon.RenderControl (renderingContext.Writer);
        renderingContext.Writer.Write ("&nbsp;");
      }
      label.RenderControl (renderingContext.Writer);
      if (isCommandEnabled)
        renderingContext.Control.Command.RenderEnd (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag (); //  End td
    }

    private void RenderEditModeValue (
        BocReferenceValueRenderingContext renderingContext, DropDownList dropDownList, bool isControlHeightEmpty, bool isDropDownListHeightEmpty, bool isDropDownListWidthEmpty)
    {
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (!isControlHeightEmpty && isDropDownListHeightEmpty)
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
      if (isDropDownListWidthEmpty)
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      dropDownList.RenderControl (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag (); //  End td

      RenderEditModeValueExtension (renderingContext);
    }

    /// <summary> Called after the edit mode value's cell is rendered. </summary>
    /// <remarks> Render a table cell: &lt;td style="width:0%"&gt;Your contents goes here&lt;/td&gt;</remarks>
    protected virtual void RenderEditModeValueExtension (BocReferenceValueRenderingContext renderingContext)
    {
    }

    private bool IsEmbedInOptionsMenu(BocReferenceValueRenderingContext renderingContext)
    {
      return renderingContext.Control.HasValueEmbeddedInsideOptionsMenu == true && renderingContext.Control.HasOptionsMenu
               || renderingContext.Control.HasValueEmbeddedInsideOptionsMenu == null && renderingContext.Control.IsReadOnly && renderingContext.Control.HasOptionsMenu;
    }
  }
}