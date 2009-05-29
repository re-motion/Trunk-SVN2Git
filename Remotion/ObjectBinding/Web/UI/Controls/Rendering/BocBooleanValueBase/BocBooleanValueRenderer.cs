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
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocBooleanValue;
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase
{
  public class BocBooleanValueRenderer : BocBooleanValueRendererBase<IBocBooleanValue>
  {
    private const string c_nullString = "null";

    private static readonly string s_startUpScriptKeyPrefix = typeof (BocBooleanValue).FullName + "_Startup_";

    public BocBooleanValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocBooleanValue control)
        : base (context, writer, control)
    {
    }

    public override void Render ()
    {
      var resourceSet = Control.CreateResourceSet();

      AddAttributesToRender (Writer);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      bool isReadOnly = Control.IsReadOnly;

      Label labelControl = new Label { ID = Control.GetLabelKey() };
      Image imageControl = new Image { ID = Control.GetImageKey() };
      HiddenField hiddenFieldControl = new HiddenField { ID = Control.GetHiddenFieldKey() };
      HyperLink linkControl = new HyperLink { ID = Control.GetHyperLinkKey() };

      bool isClientScriptEnabled = Control.HasClientScript && !isReadOnly;
      if (isClientScriptEnabled)
      {
        if (Control.Enabled)
          RegisterStarupScriptIfNeeded (resourceSet);

        string script = GetClickScript (imageControl, labelControl, hiddenFieldControl, Control.Enabled, resourceSet);
        labelControl.Attributes.Add ("onclick", script);
        linkControl.Attributes.Add ("onclick", script);
      }

      PrepareLinkControl (linkControl, isClientScriptEnabled);
      PrepareHiddenControl (hiddenFieldControl, isReadOnly);
      PrepareVisibleControls (imageControl, labelControl, resourceSet);

      hiddenFieldControl.RenderControl (Writer);
      linkControl.Controls.Add (imageControl);
      linkControl.RenderControl (Writer);
      labelControl.RenderControl (Writer);

      Writer.RenderEndTag();
    }

    private void PrepareHiddenControl (HiddenField hiddenFieldControl, bool isReadOnly)
    {
      if (!isReadOnly)
        hiddenFieldControl.Value = Control.Value.HasValue ? Control.Value.ToString() : c_nullString;
      hiddenFieldControl.Visible = !isReadOnly;
    }

    private void PrepareLinkControl (HyperLink linkControl, bool isClientScriptEnabled)
    {
      if (!isClientScriptEnabled)
        return;

      linkControl.Attributes.Add ("onkeydown", "BocBooleanValue_OnKeyDown (this);");
      linkControl.Style["padding"] = "0px";
      linkControl.Style["border"] = "none";
      linkControl.Style["background-color"] = "transparent";
      linkControl.Attributes.Add ("href", "#");
      linkControl.Enabled = Control.Enabled;
    }

    private void RegisterStarupScriptIfNeeded (BocBooleanValueResourceSet resourceSet)
    {
      string startUpScriptKey = s_startUpScriptKeyPrefix + resourceSet.ResourceKey;
      if (!Control.Page.ClientScript.IsStartupScriptRegistered (startUpScriptKey))
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
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, startUpScriptKey, startupScript);
      }
    }

    private string GetClickScript (
        Image imageControl,
        Label labelControl,
        HiddenField hiddenFieldControl,
        bool isEnabled,
        BocBooleanValueResourceSet resourceSet)
    {
      string script = "return false;";
      if (!isEnabled)
        return script;

      string requiredFlag = Control.IsRequired ? "true" : "false";
      string image = "document.getElementById ('" + imageControl.ClientID + "')";
      string label = Control.ShowDescription ? "document.getElementById ('" + labelControl.ClientID + "')" : "null";
      string hiddenField = "document.getElementById ('" + hiddenFieldControl.ClientID + "')";
      script = "BocBooleanValue_SelectNextCheckboxValue ("
               + "'" + resourceSet.ResourceKey + "', "
               + image + ", "
               + label + ", "
               + hiddenField + ", "
               + requiredFlag + ", "
               + (string.IsNullOrEmpty (Control.TrueDescription) ? "null" : "'" + ScriptUtility.EscapeClientScript (Control.TrueDescription) + "'")
               + ", "
               + (string.IsNullOrEmpty (Control.FalseDescription) ? "null" : "'" + ScriptUtility.EscapeClientScript (Control.FalseDescription) + "'")
               + ", "
               + (string.IsNullOrEmpty (Control.NullDescription) ? "null" : "'" + ScriptUtility.EscapeClientScript (Control.NullDescription) + "'")
               + ");";

      if (Control.IsAutoPostBackEnabled)
        script += Control.Page.ClientScript.GetPostBackEventReference (Control, "") + ";";
      script += "return false;";
      return script;
    }

    private void PrepareVisibleControls (Image imageControl, Label labelControl, BocBooleanValueResourceSet resourceSet)
    {
      string imageUrl;
      string description;

      if (!Control.Value.HasValue)
      {
        imageUrl = resourceSet.NullIconUrl;
        description = string.IsNullOrEmpty (Control.NullDescription) ? resourceSet.DefaultNullDescription : Control.NullDescription;
      }
      else if (Control.Value.Value)
      {
        imageUrl = resourceSet.TrueIconUrl;
        description = string.IsNullOrEmpty (Control.TrueDescription) ? resourceSet.DefaultTrueDescription : Control.TrueDescription;
      }
      else
      {
        imageUrl = resourceSet.FalseIconUrl;
        description = string.IsNullOrEmpty (Control.FalseDescription) ? resourceSet.DefaultFalseDescription : Control.FalseDescription;
      }

      imageControl.AlternateText = description;
      imageControl.Style["vertical-align"] = "middle";

      imageControl.ImageUrl = imageUrl;
      if (Control.ShowDescription)
        labelControl.Text = description;

      labelControl.Width = Unit.Empty;
      labelControl.Height = Unit.Empty;
      labelControl.ApplyStyle (Control.LabelStyle);
    }
  }
}