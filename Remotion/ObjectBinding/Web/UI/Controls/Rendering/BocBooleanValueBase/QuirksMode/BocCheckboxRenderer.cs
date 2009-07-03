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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.Web;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase.QuirksMode
{
  /// <summary>
  /// Responsible for rendering <see cref="BocCheckBox"/> controls.
  /// <seealso cref="IBocCheckBox"/>
  /// </summary>
  /// <include file='doc\include\UI\Controls\Rendering\QuirksMode\BocCheckboxRenderer.xml' path='BocCheckboxRenderer/Class'/>
  public class BocCheckboxRenderer : BocBooleanValueRendererBase<IBocCheckBox>, IBocCheckboxRenderer
  {
    private const string c_trueIcon = "CheckBoxTrue.gif";
    private const string c_falseIcon = "CheckBoxFalse.gif";

    private static readonly string s_startUpScriptKey = typeof (BocCheckBox).FullName + "_Startup";

    public BocCheckboxRenderer (IHttpContext context, HtmlTextWriter writer, IBocCheckBox control)
        : base (context, writer, control)
    {
    }

    /// <summary>
    /// Renders an image and label in readonly mode, a checkbox and label in edit mode.
    /// </summary>
    public override void Render()
    {
      AddAttributesToRender (false);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      Label labelControl = new Label { ID = Control.GetLabelKey() };
      HtmlInputCheckBox checkBoxControl = new HtmlInputCheckBox { ID = Control.GetCheckboxUniqueID() };
      Image imageControl = new Image { ID = Control.GetImageUniqueID() };

      string description = GetDescription ();

      if (Control.IsReadOnly)
      {
        PrepareImage(imageControl, description);
        PrepareLabel (description, labelControl);

        imageControl.RenderControl (Writer);
        labelControl.RenderControl (Writer);
      }
      else
      {
        bool hasClientScript = DetermineClientScriptLevel();
        if (hasClientScript)
        {
          PrepareScripts(checkBoxControl, labelControl);
        }

        checkBoxControl.Checked = Control.Value.Value;
        checkBoxControl.Disabled = !Control.Enabled;

        PrepareLabel (description, labelControl);

        checkBoxControl.RenderControl (Writer);
        labelControl.RenderControl (Writer);
      }

      Writer.RenderEndTag();
    }

    private bool DetermineClientScriptLevel ()
    {
      return !Control.IsDesignMode;
    }
    
    private void PrepareScripts (HtmlInputCheckBox checkBoxControl, Label labelControl)
    {
      string checkBoxScript;
      string labelScript;

      if (Control.Enabled)
      {
        RegisterStartupScriptIfNeeded();

        string script = GetScriptParameters();
        checkBoxScript = "BocCheckBox_OnClick" + script;
        labelScript = "BocCheckBox_ToggleCheckboxValue" + script;
      }
      else
      {
        checkBoxScript = "return false;";
        labelScript = "return false;";
      }
      checkBoxControl.Attributes.Add ("onclick", checkBoxScript);
      labelControl.Attributes.Add ("onclick", labelScript);
    }

    private string GetScriptParameters ()
    {
      string label = Control.IsDescriptionEnabled ? "document.getElementById ('" + Control.GetLabelKey() + "')" : "null";
      string checkBox = "document.getElementById ('" + Control.GetCheckboxUniqueID() + "')";
      string script = " ("
                      + checkBox + ", "
                      + label + ", "
                      + (string.IsNullOrEmpty (Control.TrueDescription) ? "null" : "'" + Control.TrueDescription + "'") + ", "
                      + (string.IsNullOrEmpty (Control.FalseDescription) ? "null" : "'" + Control.FalseDescription + "'") + ");";

      if (Control.IsAutoPostBackEnabled)
        script += Control.Page.ClientScript.GetPostBackEventReference (Control, "") + ";";
      return script;
    }

    private void RegisterStartupScriptIfNeeded ()
    {
      if (Control.Page.ClientScript.IsStartupScriptRegistered (s_startUpScriptKey))
        return;

      string startupScript = string.Format (
          "BocCheckBox_InitializeGlobals ('{0}', '{1}');",
          Control.DefaultTrueDescription,
          Control.DefaultFalseDescription);
      Control.Page.ClientScript.RegisterStartupScriptBlock (Control, s_startUpScriptKey, startupScript);
    }

    private void PrepareImage (Image imageControl, string description)
    {
      string imageUrl = ResourceUrlResolver.GetResourceUrl (
          Control,
          Context,
          typeof (BocCheckBox),
          ResourceType.Image,
          Control.Value.Value ? c_trueIcon : c_falseIcon);

      imageControl.ImageUrl = imageUrl;
      imageControl.AlternateText = description;
      imageControl.Style["vertical-align"] = "middle";
    }

    private void PrepareLabel (string description, Label labelControl)
    {
      if (Control.IsDescriptionEnabled)
      {
        labelControl.Text = description;
        labelControl.Width = Unit.Empty;
        labelControl.Height = Unit.Empty;
        labelControl.ApplyStyle (Control.LabelStyle);
      }
    }

    private string GetDescription ()
    {
      string trueDescription = null;
      string falseDescription = null;
      if (Control.IsDescriptionEnabled)
      {
        string defaultTrueDescription = Control.DefaultTrueDescription;
        string defaultFalseDescription = Control.DefaultFalseDescription;

        trueDescription = (string.IsNullOrEmpty (Control.TrueDescription) ? defaultTrueDescription : Control.TrueDescription);
        falseDescription = (string.IsNullOrEmpty (Control.FalseDescription) ? defaultFalseDescription : Control.FalseDescription);
      }
      return Control.Value.Value ? trueDescription : falseDescription;
    }

    public override string CssClassBase
    {
      get { return "bocCheckBox"; }
    }
  }
}