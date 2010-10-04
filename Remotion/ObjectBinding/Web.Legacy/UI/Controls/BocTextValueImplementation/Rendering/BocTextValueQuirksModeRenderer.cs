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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocTextValueImplementation.Rendering
{
  /// <summary>
  /// Provides a label for rendering a <see cref="BocTextValue"/> control in read-only mode. 
  /// Rendering is done by the parent class.
  /// </summary>
  public class BocTextValueQuirksModeRenderer : BocTextValueQuirksModeRendererBase<IBocTextValue>, IBocTextValueRenderer
  {
    public BocTextValueQuirksModeRenderer (IResourceUrlFactory resourceUrlFactory) 
      : base(resourceUrlFactory)
    { 
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IBocTextValue control)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      control.TextBoxStyle.RegisterJavaScriptInclude (SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>(), htmlHeadAppender);
    }

    public void Render (BocTextValueRenderingContext renderingContext)
    {
      base.Render (renderingContext);
    }

    protected override Label GetLabel (BocTextValueBaseRenderingContext<IBocTextValue> renderingContext)
    {
      Label label = new Label { Text = renderingContext.Control.Text };
      label.ID = renderingContext.Control.GetTextBoxClientID ();
      label.EnableViewState = false;

      string text;
      if (renderingContext.Control.TextBoxStyle.TextMode == TextBoxMode.MultiLine
          && !StringUtility.IsNullOrEmpty (renderingContext.Control.Text))
      {
        //  Allows for an optional \r
        string temp = renderingContext.Control.Text.Replace ("\r", "");
        string[] lines = temp.Split ('\n');
        for (int i = 0; i < lines.Length; i++)
          lines[i] = HttpUtility.HtmlEncode (lines[i]);
        text = StringUtility.ConcatWithSeparator (lines, "<br />");
      }
      else
        text = HttpUtility.HtmlEncode (renderingContext.Control.Text);

      if (StringUtility.IsNullOrEmpty (text))
      {
        if (renderingContext.Control.IsDesignMode)
        {
          text = c_designModeEmptyLabelContents;
          //  Too long, can't resize in designer to less than the content's width
          //  Label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
        }
        else
          text = "&nbsp;";
      }
      label.Text = text;
      label.Width = Unit.Empty;
      label.Height = Unit.Empty;
      label.ApplyStyle (renderingContext.Control.CommonStyle);
      label.ApplyStyle (renderingContext.Control.LabelStyle);
      return label;
    }

    public override string CssClassBase
    {
      get { return "bocTextValue"; }
    }
  }
}