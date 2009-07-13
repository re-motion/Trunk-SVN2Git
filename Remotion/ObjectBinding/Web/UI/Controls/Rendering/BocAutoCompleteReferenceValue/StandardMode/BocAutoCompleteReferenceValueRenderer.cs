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
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocAutoCompleteReferenceValue.StandardMode
{
  public class BocAutoCompleteReferenceValueRenderer
      : BocRendererBase<IBocAutoCompleteReferenceValue>, IBocRenderableControlRenderer<IBocAutoCompleteReferenceValue>
  {
    public BocAutoCompleteReferenceValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocAutoCompleteReferenceValue control)
        : base (context, writer, control)
    {
    }

    public void Render ()
    {
      AddAttributesToRender (true);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.TextBoxClientID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassInput);
      Writer.AddAttribute (HtmlTextWriterAttribute.Type, "text");
      Writer.RenderBeginTag (HtmlTextWriterTag.Input);
      Writer.RenderEndTag();
      Writer.RenderEndTag();

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassButton);
      string imgUrl = ResourceUrlResolver.GetResourceUrl (
          Control, Context, typeof (IBocAutoCompleteReferenceValue), ResourceType.Image, "DropDownMenuArrow.gif");
      Writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundImage, string.Format("url('{0}')", imgUrl));
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      IconInfo.Spacer.Render (Writer);
      Writer.RenderEndTag();

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.HiddenFieldClientID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Type, "hidden");
      Writer.RenderBeginTag (HtmlTextWriterTag.Input);
      Writer.RenderEndTag();

      Writer.RenderEndTag();
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
      get { return "ac_input"; }
    }
  }
}