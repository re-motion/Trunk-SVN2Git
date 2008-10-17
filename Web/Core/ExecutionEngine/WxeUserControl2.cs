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
using System.Web.UI;
using Remotion.Collections;
using Remotion.Utilities;
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

    public WxeUserControl2 ()
    {
      _wxeInfo = new WxeTemplateControlInfo (this);
      _lazyContainer = new LazyInitializationContainer ();
    }

    protected override sealed void OnInit (EventArgs e)
    {
      if (!_isWxeInfoInitialized)
      {
        _wxeInfo.Initialize (Context);
        _isWxeInfoInitialized = true;
      }

      if (_replacer == null)
      {
        var replacer = new ControlReplacer (new InternalControlMemberCaller());
        replacer.ID = ID + "_Parent";

        string uniqueID = UniqueID.Insert (UniqueID.Length - ID.Length, replacer.ID + IdSeparator);


        if (CurrentPageStep.UserControlID == uniqueID && !CurrentPageStep.IsReturningInnerFunction)
        {
          var control = (WxeUserControl2) Page.LoadControl (CurrentUserControlStep.UserControl);
          control.ID = ID;

          IModificationStateSelectionStrategy selectionStrategy;
          if (!CurrentUserControlStep.IsPostBack)
            selectionStrategy = new ClearingStateSelectionStrategy();
          else
            selectionStrategy = new LoadingStateSelectionStrategy();

          replacer.ReplaceAndWrap (this, control, selectionStrategy);
        }
        else
        {
          IModificationStateSelectionStrategy selectionStrategy;
          if (CurrentPageStep.IsReturningInnerFunction)
            selectionStrategy = new ReplacingStateSelectionStrategy (CurrentPageStep.UserControlState);
          else
            selectionStrategy = new LoadingStateSelectionStrategy();

          replacer.ReplaceAndWrap (this, this, selectionStrategy);
        }
      }
      else
      {
        CompleteInitialization ();
      }
    }

    private void CompleteInitialization ()
    {
      Assertion.IsNotNull (Parent, "The control has not been wrapped by the ControlReplacer during initialization or control replacement.");

      _lazyContainer.Ensure(base.Controls);

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

    bool IReplaceableControl.IsInitialized
    {
      get { return _lazyContainer.IsInitialized; }
    }

    ControlReplacer IReplaceableControl.Replacer
    {
      get { return _replacer; }
      set { _replacer = value; }
    }
  }
}