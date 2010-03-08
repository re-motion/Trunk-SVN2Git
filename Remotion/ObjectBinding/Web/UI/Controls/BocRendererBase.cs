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
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering
{
  /// <summary>
  /// Base class for renderers of <see cref="IBocRenderableControl"/> objects.
  /// </summary>
  /// <typeparam name="TControl">The type of control that can be rendered.</typeparam>
  public abstract class BocRendererBase<TControl> : RendererBase<TControl>
      where TControl: IBocRenderableControl, IBusinessObjectBoundEditableWebControl
  {
    protected BocRendererBase (HttpContextBase context, TControl control)
        : base (context, control)
    {
    }

    protected void RegisterBrowserCompatibilityScript (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude (Control);

      string key = typeof (BocRendererBase<>).FullName + "_BrowserCompatibilityScript";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocRendererBase<>), ResourceType.Html, ResourceTheme, "BocBrowserCompatibility.js");
        htmlHeadAppender.RegisterJavaScriptInclude (key, scriptUrl);
      }
    }

    /// <summary>
    /// Adds class and style attributes found in the <see cref="RendererBase{TControl}.Control"/> 
    /// to the <paramref name="writer"/> so that they are rendered in the next begin tag.
    /// </summary>
    /// <param name="writer">The <see cref="HtmlTextWriter"/>.</param>
    /// <param name="overrideWidth">When <see langword="true"/>, the 'width' style attribute is rendered with a value of 'auto'
    /// without changing the contents of the actual style.</param>
    /// <remarks>This automatically adds the CSS classes found in <see cref="CssClassReadOnly"/>
    /// and <see cref="CssClassDisabled"/> if appropriate.</remarks>
    protected void AddAttributesToRender (HtmlTextWriter writer, bool overrideWidth)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      Unit backUpWidth;
      string backUpStyleWidth;
      OverrideWidth (overrideWidth, "auto", out backUpWidth, out backUpStyleWidth);

      string backUpCssClass;
      string backUpAttributeCssClass;
      OverrideCssClass (out backUpCssClass, out backUpAttributeCssClass);

      AddStandardAttributesToRender (writer);

      RestoreClass (backUpCssClass, backUpAttributeCssClass);
      RestoreWidth (backUpStyleWidth, backUpWidth);

      AddAdditionalAttributes (writer);
    }

    /// <summary>
    /// Called after all attributes have been added by <see cref="AddAttributesToRender"/>.
    /// Use this to render style attributes without putting them into the control's <see cref="IBocRenderableControl.Style"/> property.
    /// </summary>
    protected virtual void AddAdditionalAttributes (HtmlTextWriter writer)
    {
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>readOnly</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.readOnly</c> as a selector. </para>
    /// </remarks>
    public virtual string CssClassReadOnly
    {
      get { return "readOnly"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocTextValue</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    public abstract string CssClassBase { get; }

    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> when it is displayed disabled. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.disabled</c> as a selector.</para>
    /// </remarks>
    public virtual string CssClassDisabled
    {
      get { return "disabled"; }
    }

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
        Control.CssClass = CssClassBase + GetAdditionalCssClass (Control.IsReadOnly, !Control.Enabled);
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
        additionalCssClass = " " + CssClassReadOnly;
      else if (isDisabled)
        additionalCssClass = " " + CssClassDisabled;
      return additionalCssClass;
    }
  }
}
