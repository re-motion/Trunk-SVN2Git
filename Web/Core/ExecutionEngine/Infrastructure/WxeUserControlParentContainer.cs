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
using System.Web.UI;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public sealed class WxeUserControlParentContainer : Control, INamingContainer
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
}