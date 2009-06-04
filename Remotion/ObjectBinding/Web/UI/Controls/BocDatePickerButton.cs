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
using Remotion.ObjectBinding.Web.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocDateTimeValue.QuirksMode;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public class BocDatePickerButton : WebControl, IBocDatePickerButton
  {
    private const string c_datePickerPopupForm = "DatePickerForm.aspx";
    private const string c_datePickerScriptFileName = "DatePicker.js";
    private const int c_defaultDatePickerLengthInPoints = 150;

    private static readonly string s_datePickerScriptFileKey = typeof (BocDateTimeValue).FullName + "_DatePickerScript";

    private readonly Style _datePickerButtonStyle;

    public BocDatePickerButton ()
    {
      EnableClientScript = true;
      AlternateText = string.Empty;
      DatePickerPopupWidth = Unit.Point (c_defaultDatePickerLengthInPoints);
      DatePickerPopupHeight = Unit.Point (c_defaultDatePickerLengthInPoints);
      _datePickerButtonStyle = new Style();
    }

    string IBocRenderableControl.CssClassBase
    {
      get { return null; }
    }

    string IBocRenderableControl.CssClassReadOnly
    {
      get { return "readOnly"; }
    }

    string IBocRenderableControl.CssClassDisabled
    {
      get { return "disabled"; }
    }

    public bool IsDesignMode { get; set; }

    public bool EnableClientScript { get; set; }

    public string AlternateText { get; set; }

    /// <summary> Gets or sets the width of the IFrame used to display the date picker. </summary>
    /// <value> The <see cref="Unit"/> value used for the width. The default value is <b>150pt</b>. </value>
    public Unit DatePickerPopupWidth { get; set; }

    /// <summary> Gets or sets the height of the IFrame used to display the date picker. </summary>
    /// <value> The <see cref="Unit"/> value used for the height. The default value is <b>150pt</b>. </value>
    public Unit DatePickerPopupHeight { get; set; }

    public Style DatePickerButtonStyle
    {
      get { return _datePickerButtonStyle; }
    }

    public string ContainerControlID{get;set;}
    public string TargetControlID { get; set; }

    string IBocDatePickerButton.GetDatePickerUrl ()
    {
      return ResourceUrlResolver.GetResourceUrl (Parent, Context, typeof (DatePickerPage), ResourceType.UI, c_datePickerPopupForm);
    }

    string IBocDatePickerButton.GetResolvedImageUrl ()
    {
      return ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocDateTimeValue), ResourceType.Image, ImageFileName);
    }

    public virtual bool HasClientScript { get; private set; }

    public string ScriptFileKey
    {
      get { return s_datePickerScriptFileKey; }
    }

    public string ScriptFileName
    {
      get { return c_datePickerScriptFileName; }
    }

    public virtual string ImageFileName
    {
      get { return "DatePicker.gif"; }
    }

    protected override void OnPreRender (EventArgs e)
    {
      DetermineClientScriptLevel();
    }

    public override void RenderControl (HtmlTextWriter writer)
    {
      var renderer = new BocDatePickerButtonRenderer (new HttpContextWrapper(Context), writer, this);
      renderer.Render();
    }

    public override void RenderBeginTag (HtmlTextWriter writer)
    {
    }

    public override void RenderEndTag (HtmlTextWriter writer)
    {
    }

    private void DetermineClientScriptLevel ()
    {
      HasClientScript = false;

      if (!IsDesignMode)
      {
        if (EnableClientScript)
        {
          bool isVersionGreaterOrEqual55 =
              Context.Request.Browser.MajorVersion >= 6
              || Context.Request.Browser.MajorVersion == 5
                 && Context.Request.Browser.MinorVersion >= 0.5;
          bool isInternetExplorer55AndHigher =
              Context.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual55;

          HasClientScript = isInternetExplorer55AndHigher;
        }
      }
    }
  }
}