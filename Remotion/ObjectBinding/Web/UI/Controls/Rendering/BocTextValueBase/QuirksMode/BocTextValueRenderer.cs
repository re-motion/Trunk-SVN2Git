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

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocTextValueBase.QuirksMode
{
  public class BocTextValueRenderer : BocTextValueRendererBase<BocTextValue>, IBocTextValueBaseRenderer
  {
    private const string c_defaultTextBoxWidth = "150pt";

    public BocTextValueRenderer (IHttpContext context, HtmlTextWriter writer, BocTextValue control)
        : base (context, writer, control)
    {
    }

    public void Render ()
    {
      bool isControlHeightEmpty = Control.Height.IsEmpty && StringUtility.IsNullOrEmpty (Control.Style["height"]);

      string controlWidth = Control.Width.IsEmpty ? Control.Style["width"] : Control.Width.ToString();
      bool isControlWidthEmpty = string.IsNullOrEmpty (controlWidth);

      WebControl innerControl = Control.IsReadOnly ? (WebControl) Control.Label : Control.TextBox;
      bool isInnerControlHeightEmpty = innerControl.Height.IsEmpty && StringUtility.IsNullOrEmpty (innerControl.Style["height"]);
      bool isInnerControlWidthEmpty = innerControl.Width.IsEmpty && StringUtility.IsNullOrEmpty (innerControl.Style["width"]);

      if (!isControlHeightEmpty && isInnerControlHeightEmpty)
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");


      if (isInnerControlWidthEmpty)
      {
        if (isControlWidthEmpty)
        {
          bool needsColumnCount = Control.TextBoxStyle.TextMode != TextBoxMode.MultiLine || Control.TextBoxStyle.Columns == null;
          if (!Control.IsReadOnly && needsColumnCount)
            Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultTextBoxWidth);
        }
        else
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, controlWidth);
      }
      innerControl.RenderControl (Writer);
    }
  }
}