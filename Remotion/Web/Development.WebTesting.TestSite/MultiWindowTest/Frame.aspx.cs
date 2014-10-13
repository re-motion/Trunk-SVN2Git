using System;
using System.Web.UI;
using Remotion.Web.ExecutionEngine.Obsolete;
using Remotion.Web.UI.Controls.PostBackTargets;

namespace Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest
{
  public partial class Frame : MultiWindowTestPageBase
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      SimplePostBack.Click += SimplePostBackOnClick;
      NextStep.Click += NextStepOnClick;
      LoadWindowFunctionInNewWindow.Click += LoadWindowFunctionInNewWindowOnClick;
      RefreshMainUpdatePanel.Click += RefreshMainUpdatePanelOnClick;

      RegisterControlForDirtyStateTracking (MyTextBox);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      SetTestOutput (FrameLabel);
    }

    protected override void AddPostBackEventHandlerToPage (PostBackEventHandler postBackEventHandler)
    {
      UpdatePanel.TemplateControl.Controls.Add (postBackEventHandler);
    }

    private void SimplePostBackOnClick (object sender, EventArgs eventArgs)
    {
      var frameFunction = ((FrameFunction) CurrentFunction);
      if (frameFunction.AlwaysRefreshMain)
        RefreshMainFrame();
    }

    private void NextStepOnClick (object sender, EventArgs eventArgs)
    {
      ExecuteNextStep();
    }

    private void LoadWindowFunctionInNewWindowOnClick (object sender, EventArgs eventArgs)
    {
      if (!IsReturningPostBack)
        this.ExecuteFunctionExternal (new WindowFunction(), "_blank", WindowOpenFeatures, (Control) sender, true, false, false);
      else if (ReturningFunction.Variables["Refresh"] != null)
        RefreshMainFrame();
    }

    private void RefreshMainUpdatePanelOnClick (object sender, EventArgs eventArgs)
    {
      RefreshMainFrame();
    }

    private void RefreshMainFrame ()
    {
      ExecuteCommandOnClient_InParent (RefreshCommand, false);
    }
  }
}