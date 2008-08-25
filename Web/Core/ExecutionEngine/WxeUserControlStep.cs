using System;
using System.Web;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeUserControlStep : WxeUIStep
  {
    private bool _isExecutionStarted;
    private bool _isPostBack;
    private string _userControl;
    private string _userControlID;

    public WxeUserControlStep (string userControl)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("userControl", userControl);
      _userControl = userControl;
    }

    public override void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      if (Function == null)
      {
        //  This is the PageStep if it isn't executing a sub-function

        context.SetIsPostBack (true);
        context.SetIsReturningPostBack (false);
        context.PostBackCollection = PostBackCollection;
      }
      else
      {
        //  This is the PageStep currently executing a sub-function

        Function.Execute (context);
        //  This point is only reached after the sub-function has completed execution.

        //  This is the PageStep after the sub-function has completed execution

        ProcessExecutedFunction (context);
      }

      if (!_isExecutionStarted)
      {
        _isExecutionStarted = true;
        _isPostBack = false;
   //     throw new WxeExecuteUserControlStepException();
      }
      else
      {
        _isPostBack = true;
      }
        ExecutePage (context);
    }

    public bool IsPostBack
    {
      get { return _isPostBack; }
    }

    public string UserControl
    {
      get { return _userControl; }
    }

    public string UserControlID
    {
      get { return _userControlID; }
      set { _userControlID = value; }
    }

    public override string ToString ()
    {
      return "WxeUserControlStep: " + UserControl;
    }
  }
}