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
using Remotion.Web.UI.Controls.ControlReplacing;

namespace Remotion.Web.ExecutionEngine
{
  [Serializable]
  public class WxeUserControlStep : WxeStep
  {
    private bool _isExecutionStarted;
    private bool _isPostBack;
    private readonly ResourceObjectBase _userControl;
    private WxePageStep _pageStep;
    private IUserControlExecutor _userControlExecutor = NullUserControlExecutor.Null;

    [NonSerialized]
    private WxeHandler _wxeHandler;

    public WxeUserControlStep (string userControl)
      : this (new ResourceObject (null, ArgumentUtility.CheckNotNullOrEmpty ("userControl", userControl)))
    {
    }

    public WxeUserControlStep (WxeVariableReference pageref)
        : this (new ResourceObjectWithVarRef (null, pageref))
    {
    }

    public WxeUserControlStep (ResourceObjectBase userControl)
    {
      ArgumentUtility.CheckNotNull ("userControl", userControl);
      _userControl = userControl;
    }

    public override void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      if (_wxeHandler != null)
      {
        context.HttpContext.Handler = _wxeHandler;
        _wxeHandler = null;
      }

      if (!_isExecutionStarted)
      {
        _isExecutionStarted = true;
        _isPostBack = false;
      }
      else
      {
        _isPostBack = true;
      }

      _userControlExecutor.Execute (context);

      try
      {
        PageStep.PageExecutor.ExecutePage (context, PageStep.Page, PageStep.IsPostBack);
      }
      finally
      {
        if (_userControlExecutor.IsReturningPostBack)
          _userControlExecutor = NullUserControlExecutor.Null;
      }
    }

    //TODO: Remove CodeDuplication with WxePageStep
    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ExecuteFunction (WxeUserControl userControl, WxeFunction subFunction, Control sender, bool usesEventTarget)
    {
      ArgumentUtility.CheckNotNull ("userControl", userControl);
      ArgumentUtility.CheckNotNull ("subFunction", subFunction);
      ArgumentUtility.CheckNotNull ("sender", sender);

      IWxePage wxePage = userControl.WxePage;
      _wxeHandler = wxePage.WxeHandler;

      _userControlExecutor = new UserControlExecutor (this, userControl, subFunction, sender, usesEventTarget);

      IReplaceableControl replaceableControl = userControl;
      replaceableControl.Replacer.Controls.Clear();
      wxePage.SaveAllState();

      Execute();
    }

    public override WxeStep ExecutingStep
    {
      get { return _userControlExecutor.ExecutingStep ?? this; }
    }

    public bool IsPostBack
    {
      get { return _isPostBack; }
    }

    public string UserControl
    {
      get { return _userControl.GetResourcePath (Variables); }
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