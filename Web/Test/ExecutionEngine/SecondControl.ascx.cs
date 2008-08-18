using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Collections;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Test.ExecutionEngine
{
  public partial class SecondControl : WxeUserControl2
  {
    protected void ExecuteNextStep_Click (object sender, EventArgs e)
    {
      ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss");
      WxePage.ExecuteNextStep();
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

      ViewStateValue++;
      ViewStateLabel.Text = ViewStateValue.ToString ();

      ControlStateValue++;
      ControlStateLabel.Text = ControlStateValue.ToString ();
    }

    protected override void LoadControlState (object savedState)
    {
      var controlState = (Tuple<object, int>) savedState;
      base.LoadControlState (controlState.A);
      ControlStateValue = controlState.B;
    }

    protected override object SaveControlState ()
    {
      return new Tuple<object, int> (base.SaveControlState (), ControlStateValue);
    }

    private int ViewStateValue
    {
      get { return (int?) ViewState["Value"] ?? 0; }
      set { ViewState["Value"] = value; }
    }

    private int ControlStateValue { get; set; }
  }
}