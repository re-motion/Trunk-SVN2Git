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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeUserControl2 : UserControl, IWxeTemplateControl, IReplaceableControl
  {
    private readonly WxeTemplateControlInfo _wxeInfo;
    private readonly LazyInitializationContainer _lazyContainer;
    private ControlReplacer _replacer;
    private bool _executeNextStep;
    private bool _isWxeInfoInitialized;
    private WxeUserControlStep _currentUserControlStep;

    public WxeUserControl2 ()
    {
      _wxeInfo = new WxeTemplateControlInfo (this);
      _lazyContainer = new LazyInitializationContainer();
    }

    protected override sealed void OnInit (EventArgs e)
    {
      if (!_isWxeInfoInitialized)
      {
        _wxeInfo.Initialize (Context);

        IUserControlExecutor userControlExecutor = _wxeInfo.CurrentPageStep.UserControlExecutor;
        while (userControlExecutor.Function.ExecutingStep is WxeUserControlStep
               && !((WxeUserControlStep) userControlExecutor.Function.ExecutingStep).UserControlExecutor.IsNull)
        {
          userControlExecutor = ((WxeUserControlStep) userControlExecutor.Function.ExecutingStep).UserControlExecutor;
        }

        _currentUserControlStep = (WxeUserControlStep) userControlExecutor.Function.ExecutingStep;
        
        _isWxeInfoInitialized = true;
      }

      if (_replacer == null)
      {
        var replacer = new ControlReplacer (new InternalControlMemberCaller());
        replacer.ID = ID + "_Parent";

        string uniqueID = UniqueID.Insert (UniqueID.Length - ID.Length, replacer.ID + IdSeparator);

        IUserControlExecutor userControlExecutor = GetUserControlExecutor();

        WxeUserControl2 control;
        IStateModificationStrategy stateModificationStrategy;
        if (userControlExecutor.UserControlID == uniqueID && !userControlExecutor.IsReturningInnerFunction)
        {
          var currentUserControlStep = (WxeUserControlStep) userControlExecutor.Function.ExecutingStep;
          control = (WxeUserControl2) Page.LoadControl (currentUserControlStep.UserControl);

          if (!currentUserControlStep.IsPostBack)
            stateModificationStrategy = new StateClearingStrategy();
          else
            stateModificationStrategy = new StateLoadingStrategy();
        }
        else
        {
          if (userControlExecutor.IsReturningInnerFunction)
          {
            control = (WxeUserControl2) Page.LoadControl (userControlExecutor.BackedUpUserControl);
            stateModificationStrategy = new StateReplacingStrategy (userControlExecutor.BackedUpUserControlState);
          }
          else
          {
            control = this;
            stateModificationStrategy = new StateLoadingStrategy();
          }
        }

        control.ID = ID;
        replacer.ReplaceAndWrap (this, control, stateModificationStrategy);
      }
      else
      {
        CompleteInitialization();
      }
    }

    private void CompleteInitialization ()
    {
      Assertion.IsNotNull (Parent, "The control has not been wrapped by the ControlReplacer during initialization or control replacement.");

      _lazyContainer.Ensure (base.Controls);

      OnInitComplete (EventArgs.Empty);
    }

    protected virtual void OnInitComplete (EventArgs e)
    {
      base.OnInit (e);
    }

    public override sealed ControlCollection Controls
    {
      get
      {
        EnsureChildControls();

        return _lazyContainer.GetControls (base.Controls);
      }
    }

    protected override sealed void CreateChildControls ()
    {
      if (ControlHelper.IsDesignMode (this, Context))
        _lazyContainer.Ensure (base.Controls);
    }

    public void ExecuteFunction (WxeFunction function, Control sender, bool? usesEventTarget)
    {
      if (CurrentPageStep.UserControlExecutor.IsNull)
        CurrentPageStep.ExecuteFunction (this, function, sender, usesEventTarget ?? UsesEventTarget);

      CurrentUserControlStep.ExecuteFunction (this, function, sender, usesEventTarget ?? UsesEventTarget);
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

    public WxeUserControlStep CurrentUserControlStep
    {
      get { return _currentUserControlStep; }
    }

    private IUserControlExecutor GetUserControlExecutor ()
    {
      IUserControlExecutor userControlExecutor = CurrentPageStep.UserControlExecutor;
      if (!userControlExecutor.IsNull && !CurrentUserControlStep.UserControlExecutor.IsNull)
        userControlExecutor = CurrentUserControlStep.UserControlExecutor;
      return userControlExecutor;
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
        throw new WxeExecuteUserControlNextStepException(GetUserControlExecutor());
      }
    }

    bool IReplaceableControl.IsInitialized
    {
      get { return _lazyContainer.IsInitialized; }
    }

    ControlReplacer IReplaceableControl.Replacer
    {
      get { return _replacer; }
      set { _replacer = value; }
    }

    //TODO: Code duplication with WxeExecutor.UsesEventTarget
    private bool UsesEventTarget
    {
      get
      {
        NameValueCollection postBackCollection = WxePage.GetPostBackCollection ();
        if (postBackCollection == null)
        {
          if (WxePage.IsPostBack)
            throw new InvalidOperationException ("The IWxePage has no PostBackCollection even though this is a post back.");
          return false;
        }
        return !StringUtility.IsNullOrEmpty (postBackCollection[ControlHelper.PostEventSourceID]);
      }
    }

  }
}