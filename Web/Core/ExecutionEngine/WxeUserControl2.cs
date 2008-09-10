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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeUserControl2 : UserControl, IWxeTemplateControl, ILazyInitializedControl
  {
    private readonly WxeTemplateControlInfo _wxeInfo;
    private readonly LazyInitializationContainer _lazyContainer;
    private ControlReplacer _replacer;
    private bool _isInitComplete;
    private bool _isInOnInit;
    private bool _executeNextStep;

    public WxeUserControl2 ()
    {
      _wxeInfo = new WxeTemplateControlInfo (this);
      _lazyContainer = new LazyInitializationContainer ();
    }

    protected override sealed void OnInit (EventArgs e)
    {
      if (_isInOnInit)
        return;
      _isInOnInit = true;

      _wxeInfo.Initialize (Context);

      if (_replacer == null)
      {
        string savedState = CurrentPageStep.IsReturningInnerFunction ? CurrentPageStep.UserControlState : null;

        _replacer = new ControlReplacer (new InternalControlMemberCaller(), ID + "_Parent", savedState);
        _replacer.BeginWrapControlWithParentContainer (this);

        string uniqueID = _replacer.UniqueID + IdSeparator + ID;
        if (CurrentPageStep.UserControlID == uniqueID && !CurrentPageStep.IsReturningInnerFunction)
          AddReplacementUserControl();
        else
          CompleteInitialization (false);
      }
      else
        CompleteInitialization (false);

      _isInOnInit = false;
    }

    private void AddReplacementUserControl ()
    {
      Assertion.IsNotNull (_replacer, "The control has not been wrapped by the ControlReplacer during initialization or control replacement.");

      var control = (WxeUserControl2) _replacer.Page.LoadControl (CurrentUserControlStep.UserControl);
      control.ID = ID;
      control._replacer = _replacer;
      _replacer = null;

      control.CompleteInitialization (!CurrentUserControlStep.IsPostBack);
    }

    private void CompleteInitialization (bool clearChildState)
    {
      Assertion.IsNotNull (_replacer, "The control has not been wrapped by the ControlReplacer during initialization or control replacement.");

      if (Parent == null)
        _replacer.EndWrapControlWithParentContainer (this, clearChildState);

      _lazyContainer.Ensure(base.Controls);

      if (!_isInitComplete)
      {
        _isInitComplete = true;
        OnInitComplete (EventArgs.Empty);
      }
    }

    protected virtual void OnInitComplete (EventArgs e)
    {
      base.OnInit (e);
    }

    public override sealed ControlCollection Controls
    {
      get
      {
        EnsureChildControls ();

        return _lazyContainer.GetControls(base.Controls);
      }
    }

    protected override sealed void CreateChildControls ()
    {
      if (ControlHelper.IsDesignMode (this, Context))
        _lazyContainer.Ensure(base.Controls);
    }

    public void ExecuteFunction (WxeFunction function)
    {
      CurrentPageStep.ExecuteFunction (this, function);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public string SaveAllState ()
    {
      return _replacer.SaveAllState();
    }

    public WxePageStep CurrentPageStep
    {
      get { return _wxeInfo.CurrentPageStep; }
    }

    public WxeFunction CurrentFunction
    {
      get { return _wxeInfo.CurrentFunction; }
    }

    public NameObjectCollection Variables
    {
      get { return _wxeInfo.Variables; }
    }

    public IWxePage WxePage
    {
      get { return (IWxePage) base.Page; }
    }

    private WxeUserControlStep CurrentUserControlStep
    {
      get { return ((WxeUserControlStep) CurrentPageStep.InnerFunction.ExecutingStep); }
    }

    /// <summary> Implements <see cref="IWxePage.ExecuteNextStep">IWxePage.ExecuteNextStep</see>. </summary>
    public void ExecuteNextStep ()
    {
      _executeNextStep = true;
      Page.Visible = false; // suppress prerender and render events
    }

    public override void Dispose ()
    {
      base.Dispose();

      if (_executeNextStep)
      {
        if (Context != null)
          Context.Response.Clear(); // throw away page trace output
        throw new WxeExecuteUserControlNextStepException();
      }
    }

    bool ILazyInitializedControl.IsInitialized
    {
      get { return _lazyContainer.IsInitialized; }
    }
  }
}