using System;
using System.Web.UI;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Obsolete;
using Remotion.Web.UI.Controls.PostBackTargets;

namespace Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest
{
  public partial class Main : MultiWindowTestPageBase
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      LoadFrameFunctionInFrame.Click += LoadFrameFunctionInFrameOnClick;
      LoadWindowFunctionInFrame.Click += LoadWindowFunctionInFrameOnClick;
      LoadMainAutoRefreshingFrameFunctionInFrame.Click += LoadMainAutoRefreshingFrameFunctionInFrameOnClick;
      LoadWindowFunctionInNewWindow.Click += LoadWindowFunctionInNewWindowOnClick;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      SetTestOutput (MainSmartLabel);
    }

    protected override void AddPostBackEventHandlerToPage (PostBackEventHandler postBackEventHandler)
    {
      UpdatePanel.ContentTemplateContainer.Controls.Add (postBackEventHandler);
    }

    private void LoadFrameFunctionInFrameOnClick (object sender, EventArgs e)
    {
      var function = new FrameFunction (false);
      LoadFunctionInFrame (function);
    }

    private void LoadWindowFunctionInFrameOnClick (object sender, EventArgs e)
    {
      var function = new WindowFunction();
      LoadFunctionInFrame (function);
    }

    private void LoadFunctionInFrame (WxeFunction function)
    {
      var variableKey = "WxeFunctionToOpen_" + Guid.NewGuid();
      Variables[variableKey] = function;
      ExecuteCommandOnClient_InFrame ("frame", ExecuteFunctionCommand, true, CurrentFunction.FunctionToken, variableKey);
    }

    private void LoadMainAutoRefreshingFrameFunctionInFrameOnClick (object sender, EventArgs e)
    {
      var function = new FrameFunction (true);
      LoadFunctionInFrame (function);
    }

    private void LoadWindowFunctionInNewWindowOnClick (object sender, EventArgs eventArgs)
    {
      if (IsReturningPostBack)
        return;

      this.ExecuteFunctionExternal (new WindowFunction(), "_blank", WindowOpenFeatures, (Control) sender, true, false, false);
    }
  }
}