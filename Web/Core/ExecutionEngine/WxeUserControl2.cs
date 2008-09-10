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
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeUserControl2 : UserControl, IWxeTemplateControl
  {
    private readonly WxeTemplateControlInfo _wxeInfo;
    private bool _isEnsured;
    private PlaceHolder _placeHolder;
    private WxeUserControlParentContainer _parentContainer;
    private bool _isInitComplete;
    private bool _isInOnInit;
    private bool _executeNextStep;

    public WxeUserControl2 ()
    {
      _wxeInfo = new WxeTemplateControlInfo (this);
    }

    protected override sealed void OnInit (EventArgs e)
    {
      if (_isInOnInit)
        return;
      _isInOnInit = true;

      _wxeInfo.Initialize (Context);

      if (_parentContainer == null)
      {
        string savedState = CurrentPageStep.IsReturningInnerFunction ? CurrentPageStep.UserControlState : null;

        _parentContainer = new WxeUserControlParentContainer (new InternalControlMemberCaller(), ID + "_Parent", savedState);
        _parentContainer.BeginWrapControlWithParentContainer (this);

        string uniqueID = _parentContainer.UniqueID + IdSeparator + ID;
        if (CurrentPageStep.UserControlID == uniqueID && !CurrentPageStep.IsReturningInnerFunction)
          AddReplacementUserControl();
        else
          CompleteInitialization();
      }
      else
        CompleteInitialization();

      _isInOnInit = false;
    }

    private void AddReplacementUserControl ()
    {
      Assertion.IsNotNull (
          _parentContainer, "The control has not been wrapped by a the parent container during initialization or control replacement.");

      var control = (WxeUserControl2) _parentContainer.Page.LoadControl (CurrentUserControlStep.UserControl);
      control.ID = ID;
      control._parentContainer = _parentContainer;
      _parentContainer = null;

      if (!CurrentUserControlStep.IsPostBack)
        control._parentContainer.ClearChildState();

      control.CompleteInitialization();
    }

    private void CompleteInitialization ()
    {
      Assertion.IsNotNull (
          _parentContainer, "The control has not been wrapped by a the parent container during initialization or control replacement.");

      if (Parent == null)
        _parentContainer.EndWrapControlWithParentContainer(this);

      Ensure();

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
        EnsureChildControls();

        if (_isEnsured)
          return base.Controls;
        else
        {
          EnsurePlaceHolderCreated();
          return _placeHolder.Controls;
        }
      }
    }

    private void Ensure ()
    {
      if (_isEnsured)
        return;

      _isEnsured = true;

      EnsurePlaceHolderCreated();
      List<Control> controls = new List<Control> (_placeHolder.Controls.Cast<Control>());
      foreach (Control control in controls)
        base.Controls.Add (control);
    }

    private void EnsurePlaceHolderCreated ()
    {
      if (_placeHolder == null)
        _placeHolder = new PlaceHolder();
    }

    protected override sealed void CreateChildControls ()
    {
      if (ControlHelper.IsDesignMode (this, Context))
        Ensure();
    }

    public void ExecuteFunction (WxeFunction function)
    {
      CurrentPageStep.ExecuteFunction (this, function);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public string SaveAllState ()
    {
      return _parentContainer.SaveAllState();
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
  }
}