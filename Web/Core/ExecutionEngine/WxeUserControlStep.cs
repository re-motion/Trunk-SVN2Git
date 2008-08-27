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
using System.Web;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeUserControlStep : WxeStep
  {
    private bool _isExecutionStarted;
    private bool _isPostBack;
    private string _userControl;
    private WxePageStep _pageStep;

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

      throw new WxeExecuteUserControlStepException();
    }

    public bool IsPostBack
    {
      get { return _isPostBack; }
    }

    public string UserControl
    {
      get { return _userControl; }
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
        return _pageStep;
      }
    }
  }
}