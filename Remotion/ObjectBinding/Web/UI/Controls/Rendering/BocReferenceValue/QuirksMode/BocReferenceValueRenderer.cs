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
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocReferenceValue.QuirksMode
{
  /// <summary>
  /// Responsible for rendering <see cref="BocReferenceValue"/> controls in Quirks Mode.
  /// </summary>
  /// <remarks>
  /// <para>During edit mode, the control is displayed using a <see cref="System.Web.UI.WebControls.DropDownList"/>.</para>
  /// <para>During read-only mode, the control's value is displayed using a <see cref="System.Web.UI.WebControls.Label"/>.</para>
  /// </remarks>
  public class BocReferenceValueRenderer : BocRendererBase<IBocReferenceValue>, IBocReferenceValueRenderer
  {
    private const string c_nullIdentifier = "==null==";
    private const string c_defaultControlWidth = "150pt";
    private readonly Func<DropDownList> _dropDownListFactoryMethod = () => new DropDownList();

    public BocReferenceValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocReferenceValue control) 
      : this (context, writer, control, null)
    {
    }

    public BocReferenceValueRenderer (
      IHttpContext context, HtmlTextWriter writer, IBocReferenceValue control, Func<DropDownList> dropDownListFactoryMethod)
      : base (context, writer, control)
    {
      if (dropDownListFactoryMethod != null)
        _dropDownListFactoryMethod = dropDownListFactoryMethod;
    }

    public void Render ()
    {
      AddAttributesToRender (false);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      DropDownList dropDownList = GetDropDownList();
      Label label = GetLabel();
      Image icon = GetIcon();

      if (Control.HasValueEmbeddedInsideOptionsMenu == true && Control.HasOptionsMenu
        || Control.HasValueEmbeddedInsideOptionsMenu == null && Control.IsReadOnly && Control.HasOptionsMenu)
      {
        RenderContentsWithIntegratedOptionsMenu(dropDownList, label);
      }
      else
      {
        RenderContentsWithSeparateOptionsMenu(dropDownList, label, icon);
      }
      Writer.RenderEndTag();
    }

    private DropDownList GetDropDownList ()
    {
      var dropDownList = _dropDownListFactoryMethod();
      dropDownList.ID = Control.DropDownListClientID;
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

    protected override void AddAdditionalAttributes ()
    {
      Writer.AddStyleAttribute ("display", "inline");
    }

    public override string CssClassBase
    {
      get { return "bocReferenceValue"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocReferenceValue"/>'s value. </summary>
    /// <remarks> Class: <c>bocReferenceValueContent</c> </remarks>
    public virtual string CssClassContent
    { get { return "bocReferenceValueContent"; } }

    private void RenderContentsWithSeparateOptionsMenu (DropDownList dropDownList, Label label, Image icon)
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
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }
      else
      {
        if (!isControlHeightEmpty && isDropDownListHeightEmpty)
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

        if (isDropDownListWidthEmpty)
        {
          if (isControlWidthEmpty)
            Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
          else
            Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        }
      }

      Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      Writer.AddStyleAttribute ("display", "inline");
      Writer.RenderBeginTag (HtmlTextWriterTag.Table);  // Begin table
      Writer.RenderBeginTag (HtmlTextWriterTag.Tr); //  Begin tr

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = "";
      if (!Control.IsDesignMode)
        postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = string.Empty;
      if (Control.InternalValue != c_nullIdentifier)
        objectID = Control.InternalValue;

      if (isReadOnly)
      {
        RenderReadOnlyValue (icon, label, isCommandEnabled, postBackEvent, string.Empty, objectID);
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (icon, isCommandEnabled, postBackEvent, string.Empty, objectID);
        RenderEditModeValue (dropDownList, isControlHeightEmpty, isDropDownListHeightEmpty, isDropDownListWidthEmpty);
      }

      bool hasOptionsMenu = Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        Writer.AddStyleAttribute ("padding-left", "0.3em");
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        //Writer.AddAttribute ("align", "right");
        Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
        Control.OptionsMenu.Width = Control.OptionsMenuWidth;
        Control.OptionsMenu.RenderControl (Writer);
        Writer.RenderEndTag ();  //  End td
      }

      //HACK: Opera has problems with inline tables and may collapse contents unless a cell with width 0% is present
      if (!Control.IsDesignMode && !isReadOnly && !hasOptionsMenu && !icon.Visible
          && Context.Request.Browser.Browser == "Opera")
      {
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
        Writer.Write ("&nbsp;");
        Writer.RenderEndTag (); // End td
      }

      Writer.RenderEndTag ();
      Writer.RenderEndTag ();
    }

    private void RenderContentsWithIntegratedOptionsMenu(DropDownList dropDownList, Label label)
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
      Control.OptionsMenu.RenderControl (Writer);
      Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (null);
    }

    public void RenderOptionsMenuTitle ()
    {
      DropDownList dropDownList = GetDropDownList();
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
      string objectID = string.Empty;
      if (Control.InternalValue != c_nullIdentifier)
        objectID = Control.InternalValue;


      if (isReadOnly)
      {
        RenderReadOnlyValue (icon, label, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        if (!isControlWidthEmpty)
        {
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%");
          Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
          Writer.RenderEndTag ();
        }
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        dropDownList.Attributes.Add ("onClick", DropDownMenu.OnHeadTitleClickScript);
        RenderEditModeValue (dropDownList, isControlHeightEmpty, isDropDownListHeightEmpty, isDropDownListWidthEmpty);
      }
    }

    private void RenderSeparateIcon (Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      Writer.AddStyleAttribute ("padding-right", "0.3em");
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (isCommandEnabled)
      {
        Control.Command.RenderBegin (Writer, postBackEvent, onClick, objectID, null);
        if (!string.IsNullOrEmpty (Control.Command.ToolTip))
          icon.ToolTip = Control.Command.ToolTip;
      }
      icon.RenderControl (Writer);
      if (isCommandEnabled)
        Control.Command.RenderEnd (Writer);

      Writer.RenderEndTag ();  //  End td
    }

    private void RenderReadOnlyValue (Image icon, Label label, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (isCommandEnabled)
        Control.Command.RenderBegin (Writer, postBackEvent, onClick, objectID, null);
      if (icon.Visible)
      {
        icon.RenderControl (Writer);
        Writer.Write ("&nbsp;");
      }
      label.RenderControl (Writer);
      if (isCommandEnabled)
        Control.Command.RenderEnd (Writer);

      Writer.RenderEndTag ();  //  End td
    }

    private void RenderEditModeValue (DropDownList dropDownList, bool isControlHeightEmpty, bool isDropDownListHeightEmpty, bool isDropDownListWidthEmpty)
    {
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (!isControlHeightEmpty && isDropDownListHeightEmpty)
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
      if (isDropDownListWidthEmpty)
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      dropDownList.RenderControl (Writer);

      Writer.RenderEndTag ();  //  End td

      RenderEditModeValueExtension();
    }

    /// <summary> Called after the edit mode value's cell is rendered. </summary>
    /// <remarks> Render a table cell: &lt;td style="width:0%"&gt;Your contents goes here&lt;/td&gt;</remarks>
    protected virtual void RenderEditModeValueExtension()
    {
    }
  }
}