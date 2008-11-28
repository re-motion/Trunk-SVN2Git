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
  public partial class SecondControl : WxeUserControl
  {
    public static void Call (IWxePage page, WxeUserControl userControl, Control sender)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("userControl", userControl);
      ArgumentUtility.CheckNotNull ("sender", sender);

      ShowSecondUserControlFormFunction function;
      if ((page.IsReturningPostBack == false))
      {
        function = new ShowSecondUserControlFormFunction ();
        function.ExceptionHandler.SetCatchExceptionTypes (typeof (System.Exception));
        WxeUserControl actualUserControl = (WxeUserControl) page.FindControl (userControl.PermanentUniqueID);
        Assertion.IsNotNull (actualUserControl);
        actualUserControl.ExecuteFunction (function, sender, null);
        throw new System.Exception ("(Unreachable code)");
      }
      else
      {
        function = ((ShowSecondUserControlFormFunction) (page.ReturningFunction));
        if ((function.ExceptionHandler.Exception != null))
        {
          throw function.ExceptionHandler.Exception;
        }
      }
    }

    protected void ExecuteNextStep_Click (object sender, EventArgs e)
    {
      ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss");
      ExecuteNextStep ();
    }

    protected void ExecuteSecondUserControlButton_Click (object sender, EventArgs e)
    {
      throw new InvalidOperationException ("This event handler should never be called.");
    }

    protected void ExecuteThirdUserControlButton_Click (object sender, EventArgs e)
    {
      if (!WxePage.IsReturningPostBack)
      {
        ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Executed";
        ExecuteFunction (new ShowThirdUserControlFormFunction (), (Control) sender, null);
      }
      else
      {
        ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Returned";
      }
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
        Assertion.IsTrue (IsPostBack);
        Assertion.IsFalse (IsUserControlPostBack);
      }
      else
      {
        Assertion.IsTrue (IsPostBack);
        Assertion.IsTrue (IsUserControlPostBack);
      }
      Assertion.IsTrue (CurrentFunction is ShowSecondUserControlFormFunction);
      Assertion.IsTrue (WxePage.CurrentFunction is ShowUserControlFormFunction);
      Assertion.IsTrue (WxePage.Variables != this.Variables);

      ViewStateValue++;
      ViewStateLabel.Text = ViewStateValue.ToString();

      ControlStateValue++;
      ControlStateLabel.Text = ControlStateValue.ToString();

      if (!IsUserControlPostBack)
      {
        Assertion.IsNull (SubControl.ValueInViewState);
        SubControl.ValueInViewState = 1.ToString ();

        Assertion.IsNull (SubControl.ValueInControlState);
        SubControl.ValueInControlState = 1.ToString ();
      }
      else
      {
        Assertion.IsNotNull (SubControl.ValueInViewState);
        SubControl.ValueInViewState = (int.Parse (SubControl.ValueInViewState) + 1).ToString ();

        Assertion.IsNotNull (SubControl.ValueInControlState);
        SubControl.ValueInControlState = (int.Parse (SubControl.ValueInControlState) + 1).ToString ();
      }
    }

    protected override void LoadControlState (object savedState)
    {
      var controlState = (Tuple<object, int, Type>) savedState;
      base.LoadControlState (controlState.A);
      ControlStateValue = controlState.B;
      Assertion.IsTrue (controlState.C == typeof (SecondControl), "Expected ControlState from 'SecondControl' but was '{0}'.", controlState.C.Name);
    }

    protected override object SaveControlState ()
    {
      return new Tuple<object, int, Type> (base.SaveControlState (), ControlStateValue, typeof (SecondControl));
    }

    protected override void LoadViewState (object savedState)
    {
      Assertion.IsNotNull (savedState, "Missing ViewState.");

      var  statePair =  (Tuple<object, Type>) savedState;
      base.LoadViewState (statePair.A);

      Assertion.IsTrue (statePair.B == typeof (SecondControl), "Expected ViewState from 'SecondControl' but was '{0}'.", statePair.B.Name);
    }

    protected override object SaveViewState ()
    {
      return new Tuple<object, Type> (base.SaveViewState (), typeof (SecondControl));
    }

    private int ViewStateValue
    {
      get { return (int?) ViewState["Value"] ?? 0; }
      set { ViewState["Value"] = value; }
    }

    private int ControlStateValue { get; set; }
  }
}