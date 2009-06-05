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
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocEnumValue.QuirksMode
{
  /// <summary>
  /// Responsible for rendering <see cref="BocEnumValue"/> controls.
  /// <seealso cref="IBocEnumValue"/>
  /// </summary>
  /// <include file='doc\include\UI\Controls\Rendering\QuirksMode\BocEnumValueRenderer.xml' path='BocEnumValueRenderer/Class'/>
  public class BocEnumValueRenderer : RenderableControlRendererBase<IBocEnumValue>, IBocEnumValueRenderer
  {
    /// <summary> The text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
    private const string c_designModeEmptyLabelContents = "##";

    private const string c_defaultListControlWidth = "150pt";

    public BocEnumValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocEnumValue control)
        : base (context, writer, control)
    {
    }

    /// <summary>
    /// Renders the concrete <see cref="ListControl"/> control as obtained from <see cref="IBocEnumValue.ListControlStyle"/>,
    /// wrapped in a &lt;div&gt;
    /// <seealso cref="ListControlType"/>
    /// </summary>
    /// <remarks>The <see cref="ISmartControl.IsRequired"/> attribute determines if a "null item" is inserted. In addition,
    /// as long as no value has been selected, <see cref="DropDownList"/> and <see cref="ListBox"/> have a "null item" inserted
    /// even when <see cref="ISmartControl.IsRequired"/> is <see langword="true"/>.
    /// </remarks>
    public void Render ()
    {
      AddAttributesToRender (false);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);
      bool isControlWidthEmpty = Control.Width.IsEmpty && string.IsNullOrEmpty (Control.Style["width"]);
      Label label = GetLabel();
      ListControl listControl = GetListControl();

      WebControl innerControl = Control.IsReadOnly ? (WebControl) label : listControl;

      bool isInnerControlHeightEmpty = innerControl.Height.IsEmpty && string.IsNullOrEmpty (innerControl.Style["height"]);
      if (!isControlHeightEmpty && isInnerControlHeightEmpty)
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isInnerControlWidthEmpty = innerControl.Width.IsEmpty && string.IsNullOrEmpty (innerControl.Style["width"]);

      if (isInnerControlWidthEmpty)
      {
        if (isControlWidthEmpty)
        {
          if (!Control.IsReadOnly)
            Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultListControlWidth);
        }
        else
        {
          if (Control.IsReadOnly)
          {
            if (!Control.Width.IsEmpty)
              Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Control.Width.ToString());
            else
              Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Control.Style["width"]);
          }
          else
            Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        }
      }

      innerControl.RenderControl (Writer);
      Writer.RenderEndTag();
    }

    private ListControl GetListControl ()
    {
      ListControl listControl = Control.ListControlStyle.Create (false);
      listControl.ID = Control.GetListControlClientID();
      listControl.Enabled = Control.Enabled;

      listControl.Width = Unit.Empty;
      listControl.Height = Unit.Empty;
      listControl.ApplyStyle (Control.CommonStyle);
      Control.ListControlStyle.ApplyStyle (listControl);

      bool needsNullValueItem = (Control.Value == null) && (Control.ListControlStyle.ControlType!=ListControlType.RadioButtonList);
      if (!Control.IsRequired || needsNullValueItem)
        listControl.Items.Add (CreateNullItem ());

      IEnumerationValueInfo[] valueInfos = Control.GetEnabledValues ();

      for (int i = 0; i < valueInfos.Length; i++)
      {
        IEnumerationValueInfo valueInfo = valueInfos[i];
        ListItem item = new ListItem (valueInfo.DisplayName, valueInfo.Identifier);
        if (valueInfo.Value.Equals(Control.Value))
          item.Selected = true;

        listControl.Items.Add (item);
      }

      return listControl;
    }

    /// <summary> Creates the <see cref="ListItem"/> symbolizing the undefined selection. </summary>
    /// <returns> A <see cref="ListItem"/>. </returns>
    private ListItem CreateNullItem ()
    {
      ListItem emptyItem = new ListItem (Control.GetNullItemText(), Control.NullIdentifier);
      if (Control.Value == null )
        emptyItem.Selected = true;

      return emptyItem;
    }

    private Label GetLabel ()
    {
      Label label = new Label { ID = Control.GetLabelClientID() };
      string text = null;
      if (Control.IsDesignMode && string.IsNullOrEmpty (label.Text))
      {
        text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }
      else if (!Control.IsDesignMode && Control.EnumerationValueInfo != null)
        text = Control.EnumerationValueInfo.DisplayName;

      if (string.IsNullOrEmpty (text))
        label.Text = "&nbsp;";
      else
        label.Text = text;

      label.Width = Unit.Empty;
      label.Height = Unit.Empty;
      label.ApplyStyle (Control.CommonStyle);
      label.ApplyStyle (Control.LabelStyle);
      return label;
    }

    protected override void AddAdditionalAttributes ()
    {
      Writer.AddStyleAttribute ("display", "inline");
    }
  }
}