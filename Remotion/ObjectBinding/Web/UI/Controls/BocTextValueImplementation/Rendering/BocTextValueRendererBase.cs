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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocTextValue"/> and <see cref="BocMultilineTextValue"/> controls, which is done
  /// by a template method for which deriving classes have to supply the <see cref="GetLabel()"/> method.
  /// <seealso cref="BocTextValueRenderer"/>
  /// <seealso cref="BocMultilineTextValueRenderer"/>
  /// </summary>
  /// <typeparam name="T">The concrete control or corresponding interface that will be rendered.</typeparam>
  public abstract class BocTextValueRendererBase<T> : BocRendererBase<T>
      where T: IBocTextValueBase
  {
    /// <summary> Text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
    protected const string c_designModeEmptyLabelContents = "##";

    protected BocTextValueRendererBase (HttpContextBase context, T control, IResourceUrlFactory resourceUrlFactory)
        : base(context, control, resourceUrlFactory)
    {
    }

    /// <summary>
    /// Renders a label when <see cref="IBusinessObjectBoundEditableControl.IsReadOnly"/> is <see langword="true"/>,
    /// a textbox in edit mode.
    /// </summary>
    public void Render (BocTextValueBaseRenderingContext<T> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (new RenderingContext<T> (renderingContext.HttpContext, renderingContext.Writer, renderingContext.Control));
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["height"]);

      WebControl innerControl = renderingContext.Control.IsReadOnly ? (WebControl) GetLabel () : GetTextBox ();
      innerControl.Page = renderingContext.Control.Page.WrappedInstance;

      bool isInnerControlHeightEmpty = innerControl.Height.IsEmpty && string.IsNullOrEmpty (innerControl.Style["height"]);

      if (!isControlHeightEmpty && isInnerControlHeightEmpty)
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      innerControl.RenderControl (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag ();
      renderingContext.Writer.RenderEndTag ();
    }

    /// <summary>
    /// Creates a <see cref="TextBox"/> control to use for rendering the <see cref="BocTextValueBase"/> control in edit mode.
    /// </summary>
    /// <returns>A <see cref="TextBox"/> control with the all relevant properties set and all appropriate styles applied to it.</returns>
    protected virtual TextBox GetTextBox ()
    {
      TextBox textBox = new TextBox { Text = Control.Text };
      textBox.ID = Control.TextBoxID;
      textBox.EnableViewState = false;
      textBox.Enabled = Control.Enabled;
      textBox.ReadOnly = !Control.Enabled;
      textBox.Width = Unit.Empty;
      textBox.Height = Unit.Empty;
      textBox.ApplyStyle (Control.CommonStyle);
      Control.TextBoxStyle.ApplyStyle (textBox);

      return textBox;
    }

    /// <summary>
    /// Creates a <see cref="Label"/> control to use for rendering the <see cref="BocTextValueBase"/> control in read-only mode.
    /// </summary>
    /// <returns>A <see cref="Label"/> control with all relevant properties set and all appropriate styles applied to it.</returns>
    protected abstract Label GetLabel ();

    private string CssClassContent
    {
      get { return "content"; }
    }
  }
}