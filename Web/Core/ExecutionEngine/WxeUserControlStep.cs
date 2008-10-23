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
using System.Threading;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  [Serializable]
  public class WxeUserControlStep : WxeStep
  {
    private bool _isExecutionStarted;
    private bool _isPostBack;
    private readonly string _userControl;
    private WxePageStep _pageStep;
    private IUserControlExecutor _userControlExecutor = NullUserControlExecutor.Null;

    public WxeUserControlStep (string userControl)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("userControl", userControl);
      _userControl = userControl;
    }

    public override void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      
      if (!_isExecutionStarted)
      {
        _isExecutionStarted = true;
        _isPostBack = false;
      }
      else
      {
        _isPostBack = true;
      }

      PageStep.PageExecutor.ExecutePage (context, PageStep.Page, PageStep.IsPostBack);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ExecuteFunction (WxeUserControl2 userControl, WxeFunction subFunction, Control sender, bool usesEventTarget)
    {
      ArgumentUtility.CheckNotNull ("userControl", userControl);
      ArgumentUtility.CheckNotNull ("subFunction", subFunction);
      ArgumentUtility.CheckNotNull ("sender", sender);

      _userControlExecutor = new UserControlExecutor (this, userControl, subFunction, sender, usesEventTarget);
      Execute ();
    }

    public bool IsPostBack
    {
      get { return _isPostBack; }
    }

    public string UserControl
    {
      get { return _userControl; }
    }

    public IUserControlExecutor UserControlExecutor
    {
      get { return _userControlExecutor; }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void SetUserControlExecutor (IUserControlExecutor userControlExecutor)
    {
      ArgumentUtility.CheckNotNull ("userControlExecutor", userControlExecutor);
      _userControlExecutor = userControlExecutor;
    }

    public override string ToString ()
    {
      return "WxeUserControlStep: " + UserControl;
    }

    public WxePageStep PageStep
    {
      get
      {
        if (_pageStep == null)
          _pageStep = WxeStep.GetStepByType<WxePageStep> (this);
        Assertion.IsNotNull (_pageStep);
        return _pageStep;
      }
    }
  }
}