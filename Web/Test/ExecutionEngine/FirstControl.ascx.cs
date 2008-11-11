using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Test.ExecutionEngine
{
  public partial class FirstControl : WxeUserControl
  {
    protected void ExecuteSecondUserControlButton_Click (object sender, EventArgs e)
    {
      ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Executed";
      SecondControl.Call (WxePage, this, (Control) sender);
      ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Returned";

      //if (!WxePage.IsReturningPostBack)
      //{
      //  ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Executed";
      //  ExecuteFunction (new ShowSecondUserControlFormFunction(), (Control)sender, null);
      //}
      //else
      //{
      //  ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Returned";
      //}
    }

    protected void ExecuteNextStep_Click (object sender, EventArgs e)
    {
      ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss");
      ExecuteNextStep ();
    }

    protected override void OnInitComplete (EventArgs e)
    {
      base.OnInitComplete (e);
      Page.RegisterRequiresControlState (this);
      ViewStateLabel.Text = "#";
      ControlStateLabel.Text = "#";
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (ControlStateValue == 0)
      {
        Assertion.IsFalse (IsPostBack);
        Assertion.IsFalse (IsUserControlPostBack);
      }
      else
      {
        Assertion.IsTrue (IsPostBack);
        Assertion.IsTrue (IsUserControlPostBack);
      }

      if (CurrentFunction is ShowFirstUserControlFormFunction)
      {
        Assertion.IsTrue (WxePage.CurrentFunction is ShowUserControlFormFunction);
        Assertion.IsTrue (WxePage.Variables != this.Variables);
      }
      else
      {
        Assertion.IsTrue (WxePage.CurrentFunction == this.CurrentFunction);
        Assertion.IsTrue (CurrentFunction is ShowUserControlFormFunction);
        Assertion.IsTrue (WxePage.Variables == this.Variables);
      }

      ViewStateValue++;
      ViewStateLabel.Text = ViewStateValue.ToString();

      ControlStateValue++;
      ControlStateLabel.Text = ControlStateValue.ToString ();
    }

    protected override void LoadControlState (object savedState)
    {
      var controlState = (Tuple<object, int, Type, bool>) savedState;
      base.LoadControlState (controlState.A);
      ControlStateValue = controlState.B;
      Assertion.IsTrue (controlState.C == typeof (FirstControl), "Expected ControlState from 'FirstControl' but was '{0}'.", controlState.C.Name);
      HasLoaded = controlState.D;
    }

    protected override object SaveControlState ()
    {
      return new Tuple<object, int, Type, bool> (base.SaveControlState(), ControlStateValue, typeof (FirstControl), HasLoaded);
    }

    protected override void LoadViewState (object savedState)
    {
      Assertion.IsNotNull (savedState, "Missing ViewState.");

      var viewState = (Tuple<object, Type>) savedState;
      base.LoadViewState (viewState.A);

      Assertion.IsTrue (viewState.B == typeof (FirstControl), "Expected ViewState from 'FirstControl' but was '{0}'.", viewState.B.Name);
    }

    protected override object SaveViewState ()
    {
      return new Tuple<object, Type> (base.SaveViewState (), typeof (FirstControl));
    }

    private int ViewStateValue
    {
      get { return (int?) ViewState["Value"] ?? 0; }
      set { ViewState["Value"] = value; }
    }

    private int ControlStateValue { get; set; }
    private bool HasLoaded { get; set; }

  }
}