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
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase
{
  public abstract class BocBooleanValueRendererBase<T>: RendererBase<T>, IRenderer
    where T : IBocBooleanValueBase
  {
    private const string c_defaultControlWidth = "100pt";

    protected BocBooleanValueRendererBase (IHttpContext context, HtmlTextWriter writer, T control)
        : base(context, writer, control)
    {
    }

    public abstract void Render ();

    protected void AddStandardAttributesToRender (HtmlTextWriter writer)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Display, "inline-block");

      string cssClass = string.Empty;
      if (!string.IsNullOrEmpty (Control.CssClass))
        cssClass = Control.CssClass + " ";

      if (!string.IsNullOrEmpty (Control.ControlStyle.CssClass))
        cssClass += Control.ControlStyle.CssClass;

      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);

      CssStyleCollection styles = Control.ControlStyle.GetStyleAttributes (Control);
      foreach (string style in styles.Keys)
        writer.AddStyleAttribute (style, styles[style]);

      foreach (string attribute in Control.Attributes.Keys)
        writer.AddAttribute (attribute, Control.Attributes[attribute]);
    }

    protected void AddAttributesToRender (HtmlTextWriter writer)
    {
      bool isReadOnly = Control.IsReadOnly;
      bool isDisabled = !Control.Enabled;

      string backUpCssClass = Control.CssClass; // base.CssClass and base.ControlStyle.CssClass
      bool hasCssClass = !string.IsNullOrEmpty (backUpCssClass);
      if (hasCssClass)
      {
        if (isReadOnly)
          Control.CssClass += " " + Control.CssClassReadOnly;
        else if (isDisabled)
          Control.CssClass += " " + Control.CssClassDisabled;
      }
      string backUpAttributeCssClass = Control.Attributes["class"];
      bool hasAttributeCssClass = !string.IsNullOrEmpty (backUpAttributeCssClass);
      if (hasAttributeCssClass)
      {
        if (isReadOnly)
          Control.Attributes["class"] += " " + Control.CssClassReadOnly;
        else if (isDisabled)
          Control.Attributes["class"] += " " + Control.CssClassDisabled;
      }

      AddStandardAttributesToRender (Writer);

      Control.CssClass = backUpCssClass;
      Control.Attributes["class"] = backUpAttributeCssClass;

      if (!hasCssClass && !hasAttributeCssClass)
      {
        string cssClass = Control.CssClassBase;
        if (isReadOnly)
          cssClass += " " + Control.CssClassReadOnly;
        else if (isDisabled)
          cssClass += " " + Control.CssClassDisabled;
        writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      }

      writer.AddStyleAttribute ("white-space", "nowrap");
      if (!isReadOnly)
      {
        bool isControlWidthEmpty = Control.Width.IsEmpty && StringUtility.IsNullOrEmpty (Control.Style["width"]);
        bool isLabelWidthEmpty = Control.LabelStyle.Width.IsEmpty;
        if (isLabelWidthEmpty && isControlWidthEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
      }
    }
  }
}