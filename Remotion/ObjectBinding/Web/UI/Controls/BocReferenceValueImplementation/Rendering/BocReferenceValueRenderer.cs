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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocReferenceValue"/> controls in Standards Mode.
  /// </summary>
  /// <remarks>
  /// <para>During edit mode, the control is displayed using a <see cref="System.Web.UI.WebControls.DropDownList"/>.</para>
  /// <para>During read-only mode, the control's value is displayed using a <see cref="System.Web.UI.WebControls.Label"/>.</para>
  /// </remarks>
  public class BocReferenceValueRenderer : BocReferenceValueRendererBase<IBocReferenceValue>, IBocReferenceValueRenderer
  {
    private readonly Func<DropDownList> _dropDownListFactoryMethod;

    public BocReferenceValueRenderer (IResourceUrlFactory resourceUrlFactory)
      : this (resourceUrlFactory, () => new DropDownList ())
    {
    }

    protected BocReferenceValueRenderer (IResourceUrlFactory resourceUrlFactory, Func<DropDownList> dropDownListFactoryMethod)
      : base (null, null, resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("dropDownListFactoryMethod", dropDownListFactoryMethod);
      _dropDownListFactoryMethod = dropDownListFactoryMethod;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IControl control, HttpContextBase context)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();

      RegisterBrowserCompatibilityScript (htmlHeadAppender);
      RegisterJavaScriptFiles(htmlHeadAppender);
      RegisterStylesheets(htmlHeadAppender);
    }

    public void Render (BocReferenceValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      base.Render (renderingContext);
    }

    private void RegisterJavaScriptFiles (HtmlHeadAppender htmlHeadAppender)
    {
      string scriptFileKey = typeof (BocReferenceValueRenderer).FullName + "_Script";
      var scriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocReferenceValueRenderer), ResourceType.Html, "BocReferenceValue.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);
    }

    private void RegisterStylesheets (HtmlHeadAppender htmlHeadAppender)
    {
      string styleFileKey = typeof (BocReferenceValueRenderer).FullName + "_Style";
      var styleUrl = ResourceUrlFactory.CreateThemedResourceUrl (typeof (BocReferenceValueRenderer), ResourceType.Html, "BocReferenceValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
    }

    protected override sealed void RenderEditModeValueWithSeparateOptionsMenu (BocReferenceValueBaseRenderingContext<IBocReferenceValue> renderingContext)
    {
      DropDownList dropDownList = GetDropDownList (renderingContext);
      RenderEditModeValue (renderingContext, dropDownList);
    }

    protected override sealed void RenderEditModeValueWithIntegratedOptionsMenu (BocReferenceValueBaseRenderingContext<IBocReferenceValue> renderingContext)
    {
      DropDownList dropDownList = GetDropDownList (renderingContext);
      dropDownList.Attributes.Add ("onclick", DropDownMenu.OnHeadTitleClickScript);
      RenderEditModeValue (renderingContext, dropDownList);
    }

    private void RenderEditModeValue (BocReferenceValueBaseRenderingContext<IBocReferenceValue> renderingContext, DropDownList dropDownList)
    {
      dropDownList.RenderControl (renderingContext.Writer);
    }

    private DropDownList GetDropDownList (BocReferenceValueBaseRenderingContext<IBocReferenceValue> renderingContext)
    {
      var dropDownList = _dropDownListFactoryMethod ();
      dropDownList.ID = renderingContext.Control.DropDownListUniqueID;
      dropDownList.EnableViewState = false;
      dropDownList.Page = renderingContext.Control.Page.WrappedInstance;
      renderingContext.Control.PopulateDropDownList (dropDownList);

      dropDownList.Enabled = renderingContext.Control.Enabled;
      dropDownList.Height = Unit.Empty;
      dropDownList.Width = Unit.Empty;
      dropDownList.ApplyStyle (renderingContext.Control.CommonStyle);
      renderingContext.Control.DropDownListStyle.ApplyStyle (dropDownList);

      return dropDownList;
    }

    public override string GetCssClassBase(IBocReferenceValue control)
    {
      return "bocReferenceValue";
    }
  }
}