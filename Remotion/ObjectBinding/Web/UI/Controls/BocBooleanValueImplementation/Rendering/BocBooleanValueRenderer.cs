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
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocBooleanValue"/> controls.
  /// <seealso cref="IBocBooleanValue"/>
  /// </summary>
  public class BocBooleanValueRenderer : BocBooleanValueRendererBase<IBocBooleanValue>, IBocBooleanValueRenderer
  {
    private readonly IBocBooleanValueResourceSetFactory _resourceSetFactory;
    private const string c_nullString = "null";

    private static readonly string s_startUpScriptKeyPrefix = typeof (BocBooleanValueRenderer).FullName + "_Startup_";

    public BocBooleanValueRenderer (IResourceUrlFactory resourceUrlFactory, IBocBooleanValueResourceSetFactory resourceSetFactory)
      : base (resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("resourceSetFactory", resourceSetFactory);

      _resourceSetFactory = resourceSetFactory;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IBocBooleanValue control, HttpContextBase context)
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

      var resourceSet = _resourceSetFactory.CreateResourceSet (renderingContext.HttpContext, renderingContext.Control);

      AddAttributesToRender (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      Label labelControl = new Label { ID = renderingContext.Control.GetLabelClientID () };
      Image imageControl = new Image { ID = renderingContext.Control.GetImageClientID () };
      HiddenField hiddenFieldControl = new HiddenField { ID = renderingContext.Control.GetHiddenFieldUniqueID () };
      HyperLink linkControl = new HyperLink { ID = renderingContext.Control.GetHyperLinkUniqueID () };

      bool isClientScriptEnabled = DetermineClientScriptLevel (renderingContext);
      if (isClientScriptEnabled)
      {
        if (renderingContext.Control.Enabled)
          RegisterStarupScriptIfNeeded (renderingContext, resourceSet);

        string script = GetClickScript (renderingContext, resourceSet, imageControl, labelControl, hiddenFieldControl, renderingContext.Control.Enabled);
        labelControl.Attributes.Add ("onclick", script);
        linkControl.Attributes.Add ("onclick", script);
      }

      PrepareLinkControl (renderingContext, linkControl, isClientScriptEnabled);
      PrepareHiddenControl (renderingContext, hiddenFieldControl, renderingContext.Control.IsReadOnly);
      PrepareVisibleControls (renderingContext, resourceSet, imageControl, labelControl);

      hiddenFieldControl.RenderControl (renderingContext.Writer);
      linkControl.Controls.Add (imageControl);
      linkControl.RenderControl (renderingContext.Writer);
      labelControl.RenderControl (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag ();
    }

    private bool DetermineClientScriptLevel (BocBooleanValueRenderingContext renderingContext)
    {
      return !renderingContext.Control.IsDesignMode && !renderingContext.Control.IsReadOnly;
    }

    private void PrepareHiddenControl (BocBooleanValueRenderingContext renderingContext, HiddenField hiddenFieldControl, bool isReadOnly)
    {
      if (!isReadOnly)
        hiddenFieldControl.Value = renderingContext.Control.Value.HasValue ? renderingContext.Control.Value.ToString () : c_nullString;
      hiddenFieldControl.Visible = !isReadOnly;
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
        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (BocBooleanValueRenderer), startUpScriptKey, startupScript);
      }
    }

    private string GetClickScript (BocBooleanValueRenderingContext renderingContext, BocBooleanValueResourceSet resourceSet, Image imageControl, 
      Label labelControl, HiddenField hiddenFieldControl, bool isEnabled)
    {
      string script = "return false;";
      if (!isEnabled)
        return script;

      string requiredFlag = renderingContext.Control.IsRequired ? "true" : "false";
      string image = "document.getElementById ('" + imageControl.ClientID + "')";
      string label = renderingContext.Control.ShowDescription ? "document.getElementById ('" + labelControl.ClientID + "')" : "null";
      string hiddenField = "document.getElementById ('" + hiddenFieldControl.ClientID + "')";
      script = "BocBooleanValue_SelectNextCheckboxValue ("
               + "'" + resourceSet.ResourceKey + "', "
               + image + ", "
               + label + ", "
               + hiddenField + ", "
               + requiredFlag + ", "
               + (string.IsNullOrEmpty (renderingContext.Control.TrueDescription) ? "null" : "'" + ScriptUtility.EscapeClientScript (renderingContext.Control.TrueDescription) + "'")
               + ", "
               + (string.IsNullOrEmpty (renderingContext.Control.FalseDescription) ? "null" : "'" + ScriptUtility.EscapeClientScript (renderingContext.Control.FalseDescription) + "'")
               + ", "
               + (string.IsNullOrEmpty (renderingContext.Control.NullDescription) ? "null" : "'" + ScriptUtility.EscapeClientScript (renderingContext.Control.NullDescription) + "'")
               + ");";

      if (renderingContext.Control.IsAutoPostBackEnabled)
        script += renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, "") + ";";
      script += "return false;";
      return script;
    }

    private void PrepareVisibleControls (BocBooleanValueRenderingContext renderingContext, BocBooleanValueResourceSet resourceSet, Image imageControl, 
      Label labelControl)
    {
      string imageUrl;
      string description;

      if (!renderingContext.Control.Value.HasValue)
      {
        imageUrl = resourceSet.NullIconUrl;
        description = string.IsNullOrEmpty (renderingContext.Control.NullDescription) ? resourceSet.DefaultNullDescription : renderingContext.Control.NullDescription;
      }
      else if (renderingContext.Control.Value.Value)
      {
        imageUrl = resourceSet.TrueIconUrl;
        description = string.IsNullOrEmpty (renderingContext.Control.TrueDescription) ? resourceSet.DefaultTrueDescription : renderingContext.Control.TrueDescription;
      }
      else
      {
        imageUrl = resourceSet.FalseIconUrl;
        description = string.IsNullOrEmpty (renderingContext.Control.FalseDescription) ? resourceSet.DefaultFalseDescription : renderingContext.Control.FalseDescription;
      }

      imageControl.AlternateText = description;

      imageControl.ImageUrl = imageUrl;
      if (renderingContext.Control.ShowDescription)
        labelControl.Text = description;

      labelControl.Width = Unit.Empty;
      labelControl.Height = Unit.Empty;
      labelControl.ApplyStyle (renderingContext.Control.LabelStyle);
    }

    public override string GetCssClassBase(IBocBooleanValue control)
    {
      return "bocBooleanValue";
    }
  }
}