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
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase
{
  public class BocBooleanValueRenderer : RendererBase<IBocBooleanValue>, IRenderer
  {
    private const string c_nullString = "null";
    private const string c_defaultControlWidth = "100pt";

    private static readonly string s_startUpScriptKeyPrefix = typeof (BocBooleanValue).FullName + "_Startup_";

    public BocBooleanValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocBooleanValue control)
        : base (context, writer, control)
    {
    }

    public void Render ()
    {
      AddAttributesToRender (Writer);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      bool isReadOnly = Control.IsReadOnly;

      Label labelControl = new Label { ID = Control.GetLabelKey () };
      Image imageControl = new Image { ID = Control.GetImageKey () };
      HiddenField hiddenFieldControl = new HiddenField { ID = Control.GetHiddenFieldKey () };
      HyperLink linkControl = new HyperLink { ID = Control.GetHyperLinkKey () };

      bool isClientScriptEnabled = Control.HasClientScript && !isReadOnly;
      if (isClientScriptEnabled)
      {
        if (Control.Enabled)
          RegisterStarupScriptIfNeeded();
        
        string script = GetClickScript (imageControl, labelControl, hiddenFieldControl, Control.Enabled);
        labelControl.Attributes.Add ("onclick", script);
        linkControl.Attributes.Add ("onclick", script);
      }

      PrepareLinkControl (linkControl, isClientScriptEnabled);
      PrepareHiddenControl(hiddenFieldControl, isReadOnly);
      PrepareVisibleControls(imageControl, labelControl);

      hiddenFieldControl.RenderControl (Writer);
      linkControl.Controls.Add (imageControl);
      linkControl.RenderControl (Writer);
      labelControl.RenderControl (Writer);

      Writer.RenderEndTag();
    }

    private void PrepareHiddenControl (HiddenField hiddenFieldControl, bool isReadOnly)
    {
      if (!isReadOnly)
        hiddenFieldControl.Value = Control.Value.HasValue ? Control.Value.ToString () : c_nullString;
      hiddenFieldControl.Visible = !isReadOnly;
    }

    private void PrepareLinkControl (HyperLink linkControl, bool isClientScriptEnabled)
    {
      if (!isClientScriptEnabled )
        return;

      linkControl.Attributes.Add ("onkeydown", "BocBooleanValue_OnKeyDown (this);");
      linkControl.Style["padding"] = "0px";
      linkControl.Style["border"] = "none";
      linkControl.Style["background-color"] = "transparent";
      linkControl.Attributes.Add ("href", "#");
      linkControl.Enabled = Control.Enabled;
    }

    private void RegisterStarupScriptIfNeeded ()
    {
      string startUpScriptKey = s_startUpScriptKeyPrefix + Control.ResourceKey;
      if (!Control.Page.ClientScript.IsStartupScriptRegistered (startUpScriptKey))
      {
        string trueValue = true.ToString();
        string falseValue = false.ToString();
        string nullValue = c_nullString;

        string startupScript = string.Format (
            "BocBooleanValue_InitializeGlobals ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');",
            Control.ResourceKey,
            trueValue,
            falseValue,
            nullValue,
            ScriptUtility.EscapeClientScript (Control.DefaultTrueDescription),
            ScriptUtility.EscapeClientScript (Control.DefaultFalseDescription),
            ScriptUtility.EscapeClientScript (Control.DefaultNullDescription),
            Control.TrueIconUrl,
            Control.FalseIconUrl,
            Control.NullIconUrl);
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, startUpScriptKey, startupScript);
      }
    }

    private string GetClickScript (Image imageControl, Label labelControl, HiddenField hiddenFieldControl, bool isEnabled)
    {
      string script = "return false;";
      if (!isEnabled)
        return script;

      string requiredFlag = Control.IsRequired ? "true" : "false";
      string image = "document.getElementById ('" + imageControl.ClientID + "')";
      string label = Control.ShowDescription ? "document.getElementById ('" + labelControl.ClientID + "')" : "null";
      string hiddenField = "document.getElementById ('" + hiddenFieldControl.ClientID + "')";
      script = "BocBooleanValue_SelectNextCheckboxValue ("
               + "'" + Control.ResourceKey + "', "
               + image + ", "
               + label + ", "
               + hiddenField + ", "
               + requiredFlag + ", "
               +
               (StringUtility.IsNullOrEmpty (Control.TrueDescription)
                    ? "null"
                    : "'" + ScriptUtility.EscapeClientScript (Control.TrueDescription) + "'") + ", "
               +
               (StringUtility.IsNullOrEmpty (Control.FalseDescription)
                    ? "null"
                    : "'" + ScriptUtility.EscapeClientScript (Control.FalseDescription) + "'") + ", "
               +
               (StringUtility.IsNullOrEmpty (Control.NullDescription)
                    ? "null"
                    : "'" + ScriptUtility.EscapeClientScript (Control.NullDescription) + "'") + ");";

      if (Control.IsAutoPostBackEnabled)
        script += Control.Page.ClientScript.GetPostBackEventReference (Control, "") + ";";
      script += "return false;";
      return script;
    }

    private void PrepareVisibleControls (Image imageControl, Label labelControl)
    {
      string imageUrl;
      string description;

      if (!Control.Value.HasValue)
      {
        imageUrl = Control.NullIconUrl;
        description = string.IsNullOrEmpty (Control.NullDescription) ? Control.DefaultNullDescription : Control.NullDescription;
      }
      else if (Control.Value.Value)
      {
        imageUrl = Control.TrueIconUrl;
        description = string.IsNullOrEmpty (Control.TrueDescription) ? Control.DefaultTrueDescription : Control.TrueDescription;
      }
      else
      {
        imageUrl = Control.FalseIconUrl;
        description = string.IsNullOrEmpty (Control.FalseDescription) ? Control.DefaultFalseDescription : Control.FalseDescription;
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

    protected void AddAttributesToRender (HtmlTextWriter writer)
    {
      bool isReadOnly = Control.IsReadOnly;
      bool isDisabled = !Control.Enabled;

      string backUpCssClass = Control.CssClass; // base.CssClass and base.ControlStyle.CssClass
      bool hasCssClass = !StringUtility.IsNullOrEmpty (backUpCssClass);
      if (hasCssClass)
      {
        if (isReadOnly)
          Control.CssClass += " " + Control.CssClassReadOnly;
        else if (isDisabled)
          Control.CssClass += " " + Control.CssClassDisabled;
      }
      string backUpAttributeCssClass = Control.Attributes["class"];
      bool hasAttributeCssClass = !StringUtility.IsNullOrEmpty (backUpAttributeCssClass);
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
      {
        writer.AddStyleAttribute (style, styles[style]);
      }

      foreach (string attribute in Control.Attributes.Keys)
      {
        writer.AddAttribute (attribute, Control.Attributes[attribute]);
      }
    }
  }
}