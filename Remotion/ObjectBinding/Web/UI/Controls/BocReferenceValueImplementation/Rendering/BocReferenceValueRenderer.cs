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
  public class BocReferenceValueRenderer : BocReferenceValueRendererBase<IBocReferenceValue>
  {
    private readonly Func<DropDownList> _dropDownListFactoryMethod;

    public BocReferenceValueRenderer (HttpContextBase context, IBocReferenceValue control, IResourceUrlFactory resourceUrlFactory)
      : this (context, control, resourceUrlFactory, () => new DropDownList ())
    {
    }

    public BocReferenceValueRenderer (
        HttpContextBase context, IBocReferenceValue control, IResourceUrlFactory resourceUrlFactory, Func<DropDownList> dropDownListFactoryMethod)
      : base (context, control, resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("dropDownListFactoryMethod", dropDownListFactoryMethod);
      _dropDownListFactoryMethod = dropDownListFactoryMethod;
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();

      RegisterBrowserCompatibilityScript (htmlHeadAppender);

      string scriptFileKey = typeof (BocReferenceValueRenderer).FullName + "_Script";
      var scriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocReferenceValueRenderer), ResourceType.Html, "BocReferenceValue.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);

      string styleFileKey = typeof (BocReferenceValueRenderer).FullName + "_Style";
      var styleUrl = ResourceUrlFactory.CreateThemedResourceUrl (typeof (BocReferenceValueRenderer), ResourceType.Html, "BocReferenceValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      RegisterAdjustLayoutScript ();

      AddAttributesToRender (writer, false);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (EmbedInOptionsMenu)
        RenderContentsWithIntegratedOptionsMenu (writer);
      else
        RenderContentsWithSeparateOptionsMenu (writer);

      writer.RenderEndTag();
    }

    private void RegisterAdjustLayoutScript ()
    {
      Control.Page.ClientScript.RegisterStartupScriptBlock (
          Control,
          typeof (BocReferenceValueRenderer),
          Guid.NewGuid ().ToString (),
          string.Format ("BocBrowserCompatibility.AdjustReferenceValueLayout ($('#{0}'));", Control.ClientID));
    }

    private DropDownList GetDropDownList ()
    {
      var dropDownList = _dropDownListFactoryMethod();
      dropDownList.ID = Control.DropDownListUniqueID;
      dropDownList.EnableViewState = false;
      Control.PopulateDropDownList (dropDownList);

      dropDownList.Enabled = Control.Enabled;
      dropDownList.Height = Unit.Empty;
      dropDownList.Width = Unit.Empty;
      dropDownList.ApplyStyle (Control.CommonStyle);
      Control.DropDownListStyle.ApplyStyle (dropDownList);

      return dropDownList;
    }

    protected override Label GetLabel ()
    {
      var label = new Label { ID = Control.LabelClientID, EnableViewState = false, Height = Unit.Empty, Width = Unit.Empty };
      label.ApplyStyle (Control.CommonStyle);
      label.ApplyStyle (Control.LabelStyle);
      label.Text = HttpUtility.HtmlEncode (Control.GetLabelText());
      return label;
    }

    protected override void RenderEditModeValueWithSeparateOptionsMenu (HtmlTextWriter writer)
    {
      DropDownList dropDownList = GetDropDownList();
      dropDownList.Page = Control.Page.WrappedInstance;
      RenderEditModeValue (writer, dropDownList);
    }

    protected override void RenderEditModeValueWithIntegratedOptionsMenu (HtmlTextWriter writer)
    {
      DropDownList dropDownList = GetDropDownList ();
      dropDownList.Page = Control.Page.WrappedInstance;
      dropDownList.Attributes.Add ("onclick", DropDownMenu.OnHeadTitleClickScript);
      RenderEditModeValue (writer, dropDownList);
    }

    private void RenderEditModeValue (HtmlTextWriter writer, DropDownList dropDownList)
    {
      dropDownList.RenderControl (writer);

      RenderEditModeValueExtension (writer);
    }

    protected virtual void RenderEditModeValueExtension (HtmlTextWriter writer)
    {
    }

    public override string CssClassBase
    {
      get { return "bocReferenceValue"; }
    }
  }
}