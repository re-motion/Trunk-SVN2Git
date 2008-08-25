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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Collections;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeUserControl2 : UserControl, IWxeTemplateControl
  {
    private sealed class ParentContainer : Control, INamingContainer
    {
      private bool _isControlStateLoaded;
      private bool _isViewStateLoaded;
      private bool _requiresClearChildControlState;
      private bool _requiresClearChildViewState;
      public object ViewStateBackup { get; set; }

      public IDictionary ControlStateBackup { get; set; }

      public bool HasChildState {  get  ;  set  ; }

      protected override void OnInit (EventArgs e)
      {
        base.OnInit (e);
        Page.RegisterRequiresControlState (this);
      }

      protected override void LoadControlState (object savedState)
      {
        _isControlStateLoaded = true;
        
        if (_requiresClearChildControlState)
        {
          _requiresClearChildControlState = false;
          ClearChildControlState();
        }
        else if (ControlStateBackup != null)
        {
          IDictionary controlStateBackup = ControlStateBackup;
          ControlStateBackup = null;
          ControlHelper.SetChildControlState (this, controlStateBackup);
        }
      }

      protected override object SaveControlState ()
      {
        return "value";
      }

      protected override void LoadViewState (object savedState)
      {
        _isViewStateLoaded = true;
        
        if (_requiresClearChildViewState)
        {
          _requiresClearChildViewState = false;

          bool enableViewStateBackup = Controls[0].EnableViewState;
          Controls[0].EnableViewState = false;
          Controls[0].Load += delegate { Controls[0].EnableViewState = enableViewStateBackup; };
        }
        else if (ViewStateBackup != null)
        {
          object viewStateBackup = ViewStateBackup;
          ViewStateBackup = null;
          bool enableViewStateBackup = Controls[0].EnableViewState;
          Controls[0].EnableViewState = true;
          ControlHelper.LoadViewStateRecursive (this, viewStateBackup);
          Controls[0].EnableViewState = false;
          Controls[0].Load += delegate { Controls[0].EnableViewState = enableViewStateBackup; };
        }
      }

      protected override object SaveViewState ()
      {
        return "value";
      }

      public new void ClearChildState ()
      {
        _requiresClearChildControlState = true;
        _requiresClearChildViewState = true;
      }

      protected override void AddedControl (Control control, int index)
      {
        if (HasChildState)
        {
          if (_isControlStateLoaded)
          {
            if (_requiresClearChildControlState)
            {
              _requiresClearChildControlState = false;
              ClearChildControlState();
            }
            IDictionary controlStateBackup = ControlStateBackup;
            ControlStateBackup = null;
            ControlHelper.SetChildControlState (this, controlStateBackup);
          }

          if (_isViewStateLoaded)
          {
            if (_requiresClearChildViewState)
            {
              _requiresClearChildViewState = false;
              ClearChildViewState();
            }
            object viewStateBackup = ViewStateBackup;
            ViewStateBackup = null;
            ControlHelper.LoadViewStateRecursive (this, viewStateBackup);
          }

          HasChildState = false;

          if (_isViewStateLoaded)
          {
            bool enableViewStateBackup = control.EnableViewState;
            control.EnableViewState = false;
            Controls[0].Load += delegate { Controls[0].EnableViewState = enableViewStateBackup; };
          }

          base.AddedControl (control, index);
        }
        else
        {
          if (_isViewStateLoaded && _requiresClearChildViewState)
          {
            bool enableViewStateBackup = control.EnableViewState;
            control.EnableViewState = false;
            Controls[0].Load += delegate { Controls[0].EnableViewState = enableViewStateBackup; };
          }

          base.AddedControl (control, index);
        }
      }
    }

    private readonly WxeTemplateControlInfo _wxeInfo;
    private bool _isEnsured;
    private PlaceHolder _placeHolder;
    private ParentContainer _parentContainer;
    private bool _isInitComplete;
    private bool _isInOnInit;

    public WxeUserControl2 ()
    {
      _wxeInfo = new WxeTemplateControlInfo (this);
    }

    protected sealed override void OnInit (EventArgs e)
    {
      if (_isInOnInit)
        return;
      _isInOnInit = true;

      _wxeInfo.Initialize (Context);

      if (_parentContainer == null)
      {
        InitializeParentContainer ();
        WrapControlWithParentContainer ();

        string uniqueID = _parentContainer.UniqueID + IdSeparator + ID;
        var userControlStep = _wxeInfo.WxeHandler.RootFunction.ExecutingStep as WxeUserControlStep;
        if (userControlStep != null && (string)userControlStep.ParentFunction.Variables["UserControlID"] == uniqueID)
        {
          bool clearState = !userControlStep.IsPostBack;
          AddReplacementUserControl (clearState);
        }
        else
        {
          CompleteInitialization ();
        }
      }
      else
      {
        CompleteInitialization ();
      }

      _isInOnInit = false;
    }

    private void InitializeParentContainer ()
    {
      _parentContainer = new ParentContainer ();
      _parentContainer.ID = ID + "_Parent";
      string savedState = (string) CurrentFunction.Variables["UserControlState"];
      CurrentFunction.Variables["UserControlState"] = null;
      if (savedState != null)
      {
        var formatter = new LosFormatter ();
        var state = (Pair) formatter.Deserialize (savedState);

        _parentContainer.HasChildState = true;
        _parentContainer.ControlStateBackup = (IDictionary) state.First;
        _parentContainer.ViewStateBackup = state.Second;
      }
    }

    private void WrapControlWithParentContainer ()
    {
      Assertion.IsNotNull (_parentContainer, "The control has not been wrapped by a the parent container during initialization or control replacement.");
      
      Control parent = Parent;
      int index = parent.Controls.IndexOf (this);

      //Mark parent collection as writeable
      string errorMessage = MethodCaller.CallFunc<string> ("SetCollectionReadOnly", BindingFlags.Instance| BindingFlags.NonPublic).With (parent.Controls, (string) null);
      Assertion.IsNotNull (errorMessage, "The parent's collection is readonly during the initialization phase of a control");
        
      parent.Controls.RemoveAt (index);
      parent.Controls.AddAt (index, _parentContainer);

      //Mark parent collection as readonly
      MethodCaller.CallFunc<string> ("SetCollectionReadOnly", BindingFlags.Instance | BindingFlags.NonPublic).With (parent.Controls, errorMessage);
      MethodCaller.CallAction ("InitRecursive", BindingFlags.Instance | BindingFlags.NonPublic).With (_parentContainer, parent);
    }

    private void AddReplacementUserControl (bool clearState)
    {
      Assertion.IsNotNull (_parentContainer, "The control has not been wrapped by a the parent container during initialization or control replacement.");
      
      var control = (WxeUserControl2) _parentContainer.Page.LoadControl (((WxeUserControlStep) _wxeInfo.WxeHandler.RootFunction.ExecutingStep).UserControl);
      control.ID = ID;
      control._parentContainer = _parentContainer;
      _parentContainer = null;

      if (clearState)
        control._parentContainer.ClearChildState();
   
      control.CompleteInitialization ();
    }

    private void CompleteInitialization ()
    {
      Assertion.IsNotNull (_parentContainer, "The control has not been wrapped by a the parent container during initialization or control replacement.");

      if (Parent == null)
        _parentContainer.Controls.Add (this);

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

    public sealed override ControlCollection Controls
    {
      get
      {
        EnsureChildControls ();

        if (_isEnsured)
        {
          return base.Controls;
        }
        else
        {
          EnsurePlaceHolderCreated ();
          return _placeHolder.Controls;
        }
      }
    }

    private void Ensure ()
    {
      if (_isEnsured)
        return;

      _isEnsured = true;

      EnsurePlaceHolderCreated ();
      Controls.Add (_placeHolder);
    }

    private void EnsurePlaceHolderCreated ()
    {
      if (_placeHolder == null)
        _placeHolder = new PlaceHolder ();
    }

    protected sealed override void CreateChildControls ()
    {
      if (ControlHelper.IsDesignMode (this, Context))
        Ensure ();
    }

    public void ExecuteFunction (WxeFunction function)
    {
      try
      {
        Context.Handler = _wxeInfo.WxeHandler;
        CurrentFunction.Variables["UserControlState"] = SaveAllState ();
        function.Variables["UserControlID"] = UniqueID;

        CurrentStep.ExecuteFunction (this, function);
      }
      finally
      {
        Context.Handler = Page;
      }
    }

    private string SaveAllState ()
    {
      Pair state = new Pair (ControlHelper.SaveChildControlState (_parentContainer), ControlHelper.SaveViewStateRecursive (_parentContainer));
      LosFormatter formatter = new LosFormatter();
      StringWriter writer = new StringWriter();
      formatter.Serialize (writer, state);
      return writer.ToString();
    }

    public WxeUIStep CurrentStep
    {
      get { return _wxeInfo.CurrentStep; }
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
  }
}