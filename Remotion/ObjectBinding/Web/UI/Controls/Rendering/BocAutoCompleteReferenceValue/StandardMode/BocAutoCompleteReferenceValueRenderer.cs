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
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocAutoCompleteReferenceValue.StandardMode
{
  public class BocAutoCompleteReferenceValueRenderer
      : BocRendererBase<IBocAutoCompleteReferenceValue>, IBocAutoCompleteReferenceValueRenderer
  {
    public BocAutoCompleteReferenceValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocAutoCompleteReferenceValue control)
        : base (context, writer, control)
    {
    }

    public void Render ()
    {
      AddAttributesToRender (false);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (Control.IsReadOnly)
        RenderReadOnlyControl();
      else
        RenderEditableControl();

      Writer.RenderEndTag();
    }

    private void RenderReadOnlyControl ()
    {
      var label = new Label { ID = Control.TextBoxUniqueID, Text = Control.BusinessObjectDisplayName };
      label.ApplyStyle (Control.CommonStyle);
      label.ApplyStyle (Control.LabelStyle);

      Writer.Write (HttpUtility.HtmlEncode (Control.BusinessObjectDisplayName));
    }

    private void RenderEditableControl ()
    {
      RenderTextbox();

      if (Control.Enabled)
        RenderDropdownButton();

      RenderHiddenField();
      RenderDummy();
    }

    private void RenderDummy ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDummy);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      Writer.WriteLine ("&nbsp;");
      Writer.RenderEndTag();
    }

    private void RenderHiddenField ()
    {
      var hiddenField = new HiddenField
                        {
                            ID = Control.HiddenFieldUniqueID,
                            Page = Control.Page.WrappedInstance,
                            EnableViewState = true,
                            Value = Control.BusinessObjectUniqueIdentifier
                        };
      hiddenField.RenderControl (Writer);
    }

    private void RenderDropdownButton ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.DropDownButtonClientID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassButton);
      string imgUrl = ResourceUrlResolver.GetResourceUrl (
          Control, Context, typeof (IBocAutoCompleteReferenceValue), ResourceType.Image, "DropDownMenuArrow.gif");
      Writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundImage, string.Format ("url('{0}')", imgUrl));
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      IconInfo.Spacer.Render (Writer);
      Writer.RenderEndTag();
    }

    private void RenderTextbox ()
    {
      var textBox = new TextBox
                    {
                        ID = Control.TextBoxUniqueID,
                        CssClass = CssClassInput,
                        Text = Control.BusinessObjectDisplayName,
                        Enabled = Control.Enabled,
                        Page = Control.Page.WrappedInstance,
                        EnableViewState = false
                    };
      textBox.ApplyStyle (Control.CommonStyle);
      Control.TextBoxStyle.ApplyStyle (textBox);

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassInput);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      textBox.RenderControl (Writer);
      Writer.RenderEndTag();
    }

    public string CssClassDummy
    {
      get { return "bocAutoCompleteReferenceValueDummy"; }
    }

    public string CssClassButton
    {
      get { return "bocAutoCompleteReferenceValueButton"; }
    }

    protected override void AddAdditionalAttributes ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
    }

    public override string CssClassBase
    {
      get { return "bocAutoCompleteReferenceValue"; }
    }

    public string CssClassInput
    {
      get { return "bocAutoCompleteReferenceValueInput"; }
    }
  }
}