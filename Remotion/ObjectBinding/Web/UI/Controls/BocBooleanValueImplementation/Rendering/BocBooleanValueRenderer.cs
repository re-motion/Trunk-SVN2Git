// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocBooleanValue"/> controls.
  /// <seealso cref="IBocBooleanValue"/>
  /// </summary>
  [ImplementationFor (typeof (IBocBooleanValueRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocBooleanValueRenderer : BocBooleanValueRendererBase<IBocBooleanValue>, IBocBooleanValueRenderer
  {
    private readonly IBocBooleanValueResourceSetFactory _resourceSetFactory;
    private const string c_nullString = "null";

    private static readonly string s_startUpScriptKeyPrefix = typeof (BocBooleanValueRenderer).FullName + "_Startup_";

    public BocBooleanValueRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IBocBooleanValueResourceSetFactory resourceSetFactory)
        : base (resourceUrlFactory, globalizationService)
    {
      ArgumentUtility.CheckNotNull ("resourceSetFactory", resourceSetFactory);

      _resourceSetFactory = resourceSetFactory;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string scriptFileKey = typeof (BocBooleanValueRenderer).FullName + "_Script";
      var scriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocBooleanValueRenderer), ResourceType.Html, "BocBooleanValue.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);

      string styleFileKey = typeof (BocBooleanValueRenderer).FullName + "_Style";
      var styleUrl = ResourceUrlFactory.CreateThemedResourceUrl (typeof (BocBooleanValueRenderer), ResourceType.Html, "BocBooleanValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
    }

    /// <summary>
    /// Renders an image and a label. In edit mode, the image is wrapped in a hyperlink that is
    /// scripted to respond to clicks and change the "checkbox" state accordingly; 
    /// in addition, the state is put into an additional hidden field.
    /// </summary>
    public void Render (BocBooleanValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      var resourceSet = _resourceSetFactory.CreateResourceSet (renderingContext.Control);

      AddAttributesToRender (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      var labelControl = new Label { ID = GetLabelName (renderingContext), ClientIDMode = ClientIDMode.Static };
      var imageControl = new Image { ID = GetImageName (renderingContext), ClientIDMode = ClientIDMode.Static };
      var hiddenFieldControl = new HiddenField { ID = renderingContext.Control.GetKeyValueName(), ClientIDMode = ClientIDMode.Static };
      var dataValueReadOnlyControl = new Label { ID = renderingContext.Control.GetKeyValueName(), ClientIDMode = ClientIDMode.Static };
      var linkControl = new HyperLink { ID = renderingContext.Control.GetTextValueName(), ClientIDMode = ClientIDMode.Static };

      bool isClientScriptEnabled = DetermineClientScriptLevel (renderingContext);
      if (isClientScriptEnabled)
      {
        if (renderingContext.Control.Enabled)
          RegisterStarupScriptIfNeeded (renderingContext, resourceSet);

        var script = GetClickScript (
            renderingContext,
            resourceSet,
            imageControl,
            labelControl,
            renderingContext.Control.IsReadOnly ? (Control) dataValueReadOnlyControl : hiddenFieldControl,
            renderingContext.Control.Enabled);
        labelControl.Attributes.Add ("onclick", script);
        linkControl.Attributes.Add ("onclick", script);
      }

      PrepareLinkControl (renderingContext, linkControl, isClientScriptEnabled);
      PrepareVisibleControls (renderingContext, resourceSet, imageControl, labelControl);

      if (!renderingContext.Control.IsReadOnly)
      {
        hiddenFieldControl.Value = renderingContext.Control.Value.HasValue ? renderingContext.Control.Value.ToString() : c_nullString;
        hiddenFieldControl.Visible = true;
        hiddenFieldControl.RenderControl (renderingContext.Writer);
      }
      else
      {
        if (renderingContext.Control.Value.HasValue)
          dataValueReadOnlyControl.Attributes.Add ("data-value", renderingContext.Control.Value.Value.ToString());
        dataValueReadOnlyControl.RenderControl (renderingContext.Writer);
      }
      linkControl.Controls.Add (imageControl);
      linkControl.RenderControl (renderingContext.Writer);
      labelControl.RenderControl (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag();
    }

    private bool DetermineClientScriptLevel (BocBooleanValueRenderingContext renderingContext)
    {
      return !renderingContext.Control.IsDesignMode && !renderingContext.Control.IsReadOnly;
    }

    private void PrepareLinkControl (BocBooleanValueRenderingContext renderingContext, HyperLink linkControl, bool isClientScriptEnabled)
    {
      if (!isClientScriptEnabled)
        return;

      linkControl.Attributes.Add ("onkeydown", "BocBooleanValue_OnKeyDown (this);");
      linkControl.Attributes.Add ("href", "#");
      linkControl.Enabled = renderingContext.Control.Enabled;
    }

    private void RegisterStarupScriptIfNeeded (BocBooleanValueRenderingContext renderingContext, BocBooleanValueResourceSet resourceSet)
    {
      string startUpScriptKey = s_startUpScriptKeyPrefix + resourceSet.ResourceKey;
      if (!renderingContext.Control.Page.ClientScript.IsStartupScriptRegistered (typeof (BocBooleanValueRenderer), startUpScriptKey))
      {
        string trueValue = true.ToString();
        string falseValue = false.ToString();
        string nullValue = c_nullString;

        string startupScript = string.Format (
            "BocBooleanValue_InitializeGlobals ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');",
            resourceSet.ResourceKey,
            trueValue,
            falseValue,
            nullValue,
            ScriptUtility.EscapeClientScript (resourceSet.DefaultTrueDescription),
            ScriptUtility.EscapeClientScript (resourceSet.DefaultFalseDescription),
            ScriptUtility.EscapeClientScript (resourceSet.DefaultNullDescription),
            resourceSet.TrueIconUrl,
            resourceSet.FalseIconUrl,
            resourceSet.NullIconUrl);
        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (
            renderingContext.Control,
            typeof (BocBooleanValueRenderer),
            startUpScriptKey,
            startupScript);
      }
    }

    private string GetClickScript (
        BocBooleanValueRenderingContext renderingContext,
        BocBooleanValueResourceSet resourceSet,
        Image imageControl,
        Label labelControl,
        Control dataValueControl,
        bool isEnabled)
    {
      string script = "return false;";
      if (!isEnabled)
        return script;

      string requiredFlag = renderingContext.Control.IsRequired ? "true" : "false";

      var scriptBuilder = new StringBuilder (500);
      scriptBuilder.Append ("BocBooleanValue_SelectNextCheckboxValue (");
      scriptBuilder.Append ("'").Append (resourceSet.ResourceKey).Append ("'");
      scriptBuilder.Append (", ");
      scriptBuilder.Append ("document.getElementById ('").Append (imageControl.ClientID).Append ("')");
      scriptBuilder.Append (", ");
      if (renderingContext.Control.ShowDescription)
        scriptBuilder.Append ("document.getElementById ('").Append (labelControl.ClientID).Append ("')");
      else
        scriptBuilder.Append ("null");
      scriptBuilder.Append (", ");
      scriptBuilder.Append ("document.getElementById ('").Append (dataValueControl.ClientID).Append ("')");
      scriptBuilder.Append (", ");
      scriptBuilder.Append (requiredFlag);
      scriptBuilder.Append (", ");
      AppendStringValueOrNullToScript (scriptBuilder, renderingContext.Control.TrueDescription);
      scriptBuilder.Append (", ");
      AppendStringValueOrNullToScript (scriptBuilder, renderingContext.Control.FalseDescription);
      scriptBuilder.Append (", ");
      AppendStringValueOrNullToScript (scriptBuilder, renderingContext.Control.NullDescription);
      scriptBuilder.Append (");");

      script = scriptBuilder.ToString();

      if (renderingContext.Control.IsAutoPostBackEnabled)
        script += renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, "") + ";";
      script += "return false;";
      return script;
    }

    private void PrepareVisibleControls (
        BocBooleanValueRenderingContext renderingContext,
        BocBooleanValueResourceSet resourceSet,
        Image imageControl,
        Label labelControl)
    {
      string imageUrl;
      string description;

      if (!renderingContext.Control.Value.HasValue)
      {
        imageUrl = resourceSet.NullIconUrl;
        description = string.IsNullOrEmpty (renderingContext.Control.NullDescription)
            ? resourceSet.DefaultNullDescription
            : renderingContext.Control.NullDescription;
      }
      else if (renderingContext.Control.Value.Value)
      {
        imageUrl = resourceSet.TrueIconUrl;
        description = string.IsNullOrEmpty (renderingContext.Control.TrueDescription)
            ? resourceSet.DefaultTrueDescription
            : renderingContext.Control.TrueDescription;
      }
      else
      {
        imageUrl = resourceSet.FalseIconUrl;
        description = string.IsNullOrEmpty (renderingContext.Control.FalseDescription)
            ? resourceSet.DefaultFalseDescription
            : renderingContext.Control.FalseDescription;
      }

      imageControl.AlternateText = description;

      imageControl.ImageUrl = imageUrl;
      if (renderingContext.Control.ShowDescription)
        labelControl.Text = description;

      labelControl.Width = Unit.Empty;
      labelControl.Height = Unit.Empty;
      labelControl.ApplyStyle (renderingContext.Control.LabelStyle);
    }

    private string GetLabelName (BocBooleanValueRenderingContext renderingContext)
    {
      return renderingContext.Control.ClientID + "_LabelValue";
    }

    private string GetImageName (BocBooleanValueRenderingContext renderingContext)
    {
      return renderingContext.Control.ClientID + "_Image";
    }

    public override string GetCssClassBase (IBocBooleanValue control)
    {
      return "bocBooleanValue";
    }
  }
}