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

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocReferenceValue.StandardMode
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
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
      AddAttributesToRender (false);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      DropDownList dropDownList = GetDropDownList();
      dropDownList.Page = Control.Page.WrappedInstance;
      Label label = GetLabel();
      Image icon = GetIcon();

      if (Control.EmbedInOptionsMenu)
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
      dropDownList.ID = Control.DropDownListUniqueID;
      dropDownList.EnableViewState = false;
      Control.PopulateDropDownList (dropDownList);

      dropDownList.Enabled = Control.Enabled;
      dropDownList.Height = Unit.Empty;
      dropDownList.Width = Unit.Empty;
      dropDownList.ApplyStyle (Control.CommonStyle);
      Control.DropDownListStyle.ApplyStyle (dropDownList);
      dropDownList.Style[HtmlTextWriterStyle.Margin] = "0";

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
          icon.Style["vertical-align"] = "middle";
          icon.Style["border-style"] = "none";
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

    protected override void AddAdditionalAttributes ()
    {
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

    public virtual string CssClassInnerContent
    {
      get { return "content"; }
    }

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

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);

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

        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassInnerContent);
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);

        RenderEditModeValue (dropDownList, isControlHeightEmpty, isDropDownListHeightEmpty);
       
        Writer.RenderEndTag ();
      }

      bool hasOptionsMenu = Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassOptionsMenu);
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);

        Control.OptionsMenu.Width = Control.OptionsMenuWidth;
        Control.OptionsMenu.RenderControl (Writer);

        Writer.RenderEndTag ();
      }

      Writer.RenderEndTag ();
    }

    protected string CssClassOptionsMenu
    {
      get { return "bocReferenceValueOptionsMenu"; }
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
      dropDownList.Page = Control.Page.WrappedInstance;

      Image icon = GetIcon();
      Label label = GetLabel();
      label.CssClass = CssClassReadOnly;
      bool isReadOnly = Control.IsReadOnly;

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);
      bool isDropDownListHeightEmpty = string.IsNullOrEmpty (dropDownList.Style["height"]);

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = string.Empty;
      if (Control.InternalValue != c_nullIdentifier)
        objectID = Control.InternalValue;


      if (isReadOnly)
      {
        RenderReadOnlyValue (icon, label, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
      }
      else
      {
        if (icon.Visible)
        {
          RenderSeparateIcon (icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        }
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassInnerContent);
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);

        dropDownList.Attributes.Add ("onclick", DropDownMenu.OnHeadTitleClickScript);
        RenderEditModeValue (dropDownList, isControlHeightEmpty, isDropDownListHeightEmpty);

        Writer.RenderEndTag();
      }
    }

    private void RenderSeparateIcon (Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassCommand);
      if (isCommandEnabled)
      {
        Control.Command.RenderBegin (Writer, postBackEvent, onClick, objectID, null);
        if (!string.IsNullOrEmpty (Control.Command.ToolTip))
          icon.ToolTip = Control.Command.ToolTip;
      }
      else
      {
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      icon.RenderControl (Writer);

      if (isCommandEnabled)
        Control.Command.RenderEnd (Writer);
      else
        Writer.RenderEndTag();
    }

    private void RenderReadOnlyValue (Image icon, Label label, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassCommand);
      if (isCommandEnabled)
        Control.Command.RenderBegin (Writer, postBackEvent, onClick, objectID, null);
      else
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (icon.Visible)
      {
        icon.RenderControl (Writer);
      }
      label.RenderControl (Writer);

      if (isCommandEnabled)
        Control.Command.RenderEnd (Writer);
      else
        Writer.RenderEndTag();
    }

    protected string CssClassCommand
    {
      get { return "bocReferenceValueCommand"; }
    }

    private void RenderEditModeValue (DropDownList dropDownList, bool isControlHeightEmpty, bool isDropDownListHeightEmpty)
    {
      if (!isControlHeightEmpty && isDropDownListHeightEmpty)
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      dropDownList.RenderControl (Writer);

      RenderEditModeValueExtension();
    }

    /// <summary> Called after the edit mode value's cell is rendered. </summary>
    /// <remarks> Render a table cell: &lt;td style="width:0%"&gt;Your contents goes here&lt;/td&gt;</remarks>
    protected virtual void RenderEditModeValueExtension()
    {
    }
  }
}