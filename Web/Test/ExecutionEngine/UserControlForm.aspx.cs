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
using System.Text;
using System.Web.UI;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Test.ExecutionEngine
{
  public partial class UserControlForm : WxePage
  {
    protected void PageButton_Click (object sender, EventArgs e)
    {
      PageLabel.Text = DateTime.Now.ToString ("HH:mm:ss");
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      ViewStateLabel.Text = "#";
      ControlStateLabel.Text = "#";
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      //var control = Page.LoadControl ("FirstControl.ascx");
      //control.ID = "FirstControl";
      //FirstControlPlaceHoder.Controls.Add (control);

      //if (!IsPostBack && CurrentPageStep.UserControlExecutor.IsNull)
      //  FirstControl.ExecuteFunction (new ShowFirstUserControlFormFunction (), this, true);

      ViewStateValue++;
      ViewStateLabel.Text = ViewStateValue.ToString();

      ControlStateValue++;
      ControlStateLabel.Text = ControlStateValue.ToString();

      StringBuilder stringBuilder = new StringBuilder();
      for (WxeStep step = CurrentPageStep; step != null; step = step.ParentStep)
        stringBuilder.AppendFormat ("{0}<br>", step);
      StackLabel.Text = stringBuilder.ToString();

      if (!IsPostBack)
      {
        Assertion.IsNull (SubControlWithState.ValueInViewState);
        SubControlWithState.ValueInViewState = 1.ToString ();

        Assertion.IsNull (SubControlWithState.ValueInControlState);
        SubControlWithState.ValueInControlState = 1.ToString ();
      }
      else
      {
        Assertion.IsNotNull (SubControlWithState.ValueInViewState);
        SubControlWithState.ValueInViewState = (int.Parse (SubControlWithState.ValueInViewState) + 1).ToString ();

        Assertion.IsNotNull (SubControlWithState.ValueInControlState);
        SubControlWithState.ValueInControlState = (int.Parse (SubControlWithState.ValueInControlState) + 1).ToString ();
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (!IsPostBack)
      {
        Assertion.IsTrue (string.IsNullOrEmpty (SubControlWithFormElement.Text));
        SubControlWithFormElement.Text = 1.ToString ();
      }
      else
      {

        Assertion.IsFalse (string.IsNullOrEmpty (SubControlWithFormElement.Text));
        SubControlWithFormElement.Text = (int.Parse (SubControlWithFormElement.Text) + 1).ToString ();
      }
    }

    protected void ExecuteSecondUserControlButton_Click (object sender, EventArgs e)
    {
      SecondControl.Call (this, FirstControl, (Control) sender);
      //SecondControl.Call (this, (WxeUserControl) FirstControlPlaceHoder.Controls[0], (Control) sender);
    }

    protected override void LoadControlState (object savedState)
    {
      var controlState = (Tuple<object, int>) savedState;
      base.LoadControlState (controlState.A);
      ControlStateValue = controlState.B;
    }

    protected override object SaveControlState ()
    {
      return new Tuple<object, int> (base.SaveControlState(), ControlStateValue);
    }

    private int ViewStateValue
    {
      get { return (int?) ViewState["Value"] ?? 0; }
      set { ViewState["Value"] = value; }
    }

    private int ControlStateValue { get; set; }
  }
}