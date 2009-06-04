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

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering
{
  public abstract class SimpleControlRendererBase<TControl> : RendererBase<TControl>
      where TControl: IBocRenderableControl, IBusinessObjectBoundEditableWebControl
  {
    protected SimpleControlRendererBase (IHttpContext context, HtmlTextWriter writer, TControl control)
        : base (context, writer, control)
    {
    }

    protected void AddStandardAttributesToRender ()
    {
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Display, "inline-block");

      string cssClass = string.Empty;
      if (!string.IsNullOrEmpty (Control.CssClass))
        cssClass = Control.CssClass + " ";

      if (!string.IsNullOrEmpty (Control.ControlStyle.CssClass))
        cssClass += Control.ControlStyle.CssClass;

      if( !string.IsNullOrEmpty(cssClass))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);

      CssStyleCollection styles = Control.ControlStyle.GetStyleAttributes(Control);
      foreach (string style in styles.Keys)
      {
        Writer.AddStyleAttribute (style, styles[style]);
      }

      foreach (string attribute in Control.Attributes.Keys)
      {
        string value = Control.Attributes[attribute];
        if (!string.IsNullOrEmpty (value))
          Writer.AddAttribute (attribute, value);
      }
    }

    protected void AddAttributesToRender (bool overrideWidth)
    {
      Unit backUpWidth;
      string backUpStyleWidth;
      OverrideWidth(overrideWidth, "auto", out backUpWidth, out backUpStyleWidth);

      string backUpCssClass;
      string backUpAttributeCssClass;
      OverrideCssClass(out backUpCssClass, out backUpAttributeCssClass);

      AddStandardAttributesToRender ();

      RestoreClass(backUpCssClass, backUpAttributeCssClass);
      RestoreWidth(backUpStyleWidth, backUpWidth);

      AddAdditionalAttributes();
    }

    protected abstract void AddAdditionalAttributes();

    private void OverrideWidth (bool overrideWidth, string newWidth, out Unit backUpWidth, out string backUpStyleWidth)
    {
      backUpStyleWidth = Control.Style["width"];
      backUpWidth = Control.Width;
      if( !overrideWidth )
        return;

      Control.Style["width"] = newWidth;
      Control.Width = Unit.Empty;
    }

    private void OverrideCssClass (out string backUpCssClass, out string backUpAttributeCssClass)
    {
      backUpCssClass = Control.CssClass;
      bool hasCssClass = !string.IsNullOrEmpty (backUpCssClass);
      if (hasCssClass)
        Control.CssClass += GetAdditionalCssClass (Control.IsReadOnly, !Control.Enabled);

      backUpAttributeCssClass = Control.Attributes["class"];
      bool hasClassAttribute = !string.IsNullOrEmpty (backUpAttributeCssClass);
      if (hasClassAttribute)
        Control.Attributes["class"] += GetAdditionalCssClass (Control.IsReadOnly, !Control.Enabled);

      if (!hasCssClass && !hasClassAttribute)
        Control.CssClass = Control.CssClassBase + GetAdditionalCssClass (Control.IsReadOnly, !Control.Enabled);
    }

    private void RestoreWidth (string backUpStyleWidth, Unit backUpWidth)
    {
      Control.Style["width"] = backUpStyleWidth;
      Control.Width = backUpWidth;
    }

    private void RestoreClass (string backUpCssClass, string backUpAttributeCssClass)
    {
      Control.CssClass = backUpCssClass;
      Control.Attributes["class"] = backUpAttributeCssClass;
    }

    private string GetAdditionalCssClass (bool isReadOnly, bool isDisabled)
    {
      string additionalCssClass = string.Empty;
      if (isReadOnly)
        additionalCssClass = " " + Control.CssClassReadOnly;
      else if (isDisabled)
        additionalCssClass = " " + Control.CssClassDisabled;
      return additionalCssClass;
    }
  }
}