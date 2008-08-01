/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   Can be used instead of <see cref="SmartLabel"/> controls 
///   (to label controls that do not implement ISmartControl).
/// </summary>
[ToolboxItemFilter("System.Web.UI")]
public class FormGridLabel: Label, ISmartControl
{
  private bool _required = false;
  private string _helpUrl = null;

  [Category("Behavior")]
  [DefaultValue (false)]
  [Description ("Specifies whether this row will be marked as 'required' in FormGrids.")]
  public bool Required
  {
    get { return _required; }
    set { _required = value; }
  }

  [Category("Behavior")]
  [DefaultValue (null)]
  [Description ("Specifies the relative URL to the row's help text.")]
  public string HelpUrl
  {
    get { return _helpUrl; }
    set { _helpUrl = StringUtility.NullToEmpty (value); }
  }

  [Browsable (false)]
  public bool IsRequired
  {
    get { return _required; }
  }

  HelpInfo ISmartControl.HelpInfo
  {
    get { return (_helpUrl != null) ? new HelpInfo (_helpUrl) : null; }
  }

  BaseValidator[] ISmartControl.CreateValidators()
  {
    return new BaseValidator[0];
  }

  Control ISmartControl.TargetControl
  {
    get { return this; }
  }

  bool ISmartControl.UseLabel
  {
    get { return true; }
  }

  string ISmartControl.DisplayName
  {
    get { return base.Text; }
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager (this, true);
    LoadResources (resourceManager);
  }

  protected virtual void LoadResources (IResourceManager resourceManager)
  {
    if (resourceManager == null)
      return;

    if (ControlHelper.IsDesignMode ((Control) this))
      return;

    string key = ResourceManagerUtility.GetGlobalResourceKey (Text);
    if (!StringUtility.IsNullOrEmpty (key))
      Text = resourceManager.GetString (key);
  }

  void ISmartControl.RegisterHtmlHeadContents (HttpContext context)
  {
  }

  protected override void AddAttributesToRender (HtmlTextWriter writer)
  {
    string associatedControlID = AssociatedControlID;
    if (associatedControlID.Length != 0)
    {
      Control control = this.FindControl (associatedControlID);
      if (control == null)
        throw new HttpException(string.Format("Unable to find the control with id '{0}' that is associated with the Label '{1}'.", associatedControlID, ID));
      writer.AddAttribute("for", control.ClientID);
    }
    AssociatedControlID = string.Empty;
    base.AddAttributesToRender(writer);
    AssociatedControlID = associatedControlID;
  }
 

}

}
