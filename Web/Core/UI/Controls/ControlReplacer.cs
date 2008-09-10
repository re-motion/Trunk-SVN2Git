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
using System.IO;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  public sealed class ControlReplacer : Control, INamingContainer
  {
    private readonly IInternalControlMemberCaller _memberCaller;
    private bool _isControlStateLoaded;
    private bool _isViewStateLoaded;
    private bool _requiresClearChildControlState;
    private bool _requiresClearChildViewState;

    public ControlReplacer (IInternalControlMemberCaller memberCaller, string id, string savedState)
    {
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      _memberCaller = memberCaller;
      ID = id;
      if (savedState != null)
      {
        var formatter = new LosFormatter();
        var state = (Pair) formatter.Deserialize (savedState);

        HasChildState = true;
        ControlStateBackup = (IDictionary) state.First;
        ViewStateBackup = state.Second;
      }
    }

    public object ViewStateBackup { get; private set; }

    public IDictionary ControlStateBackup { get; private set; }

    public bool HasChildState { get; private set; }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      Page.RegisterRequiresControlState (this);
    }

    protected override void LoadControlState (object savedState)
    {
      if (_memberCaller.GetControlState (this) < ControlState.Initialized)
        throw new InvalidOperationException ("Controls can only load state after OnInit phase.");

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
      if (_memberCaller.GetControlState (this) < ControlState.Initialized)
        throw new InvalidOperationException ("Controls can only load state after OnInit phase.");

      _isViewStateLoaded = true;

      if (_requiresClearChildViewState)
      {
        Assertion.IsNull (ViewStateBackup);
        _requiresClearChildViewState = false;

        bool enableViewStateBackup = Controls[0].EnableViewState;
        Controls[0].EnableViewState = false;
        Controls[0].Load += delegate { Controls[0].EnableViewState = enableViewStateBackup; };
      }
      
      if (ViewStateBackup != null)
      {
        Assertion.IsFalse (_requiresClearChildViewState);
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

    public string SaveAllState ()
    {
      Pair state = new Pair (ControlHelper.SaveChildControlState (this), ControlHelper.SaveViewStateRecursive (this));
      LosFormatter formatter = new LosFormatter();
      StringWriter writer = new StringWriter();
      formatter.Serialize (writer, state);
      return writer.ToString();
    }

    public void BeginWrapControlWithParentContainer (Control control)
    {
      ArgumentUtility.CheckNotNullAndType<INamingContainer> ("control", control);
      ArgumentUtility.CheckNotNullAndType<ILazyInitializedControl> ("control", control);

      if (_memberCaller.GetControlState (control) != ControlState.ChildrenInitialized)
        throw new InvalidOperationException ("Controls can only be wrapped during OnInit phase.");

      if (((ILazyInitializedControl)control).IsInitialized)
        throw new InvalidOperationException ("Controls can only be wrapped before they are initialzied.");

      Control parent = control.Parent;
      int index = parent.Controls.IndexOf (control);

      //Mark parent collection as modifiable
      string errorMessage = ControlHelper.SetCollectionReadOnly (parent.Controls, null);

      parent.Controls.RemoveAt (index);
      parent.Controls.AddAt (index, this);

      //Mark parent collection as readonly
      ControlHelper.SetCollectionReadOnly (parent.Controls, errorMessage);
      _memberCaller.InitRecursive (this, parent);
    }

    public void EndWrapControlWithParentContainer (Control control, bool clearChildState)
    {
      if (clearChildState)
      {
        if (ViewStateBackup != null || ControlStateBackup != null)
        {
          throw new InvalidOperationException ("Cannot clear child state if a state has been injected.");
        }
        _requiresClearChildControlState = true;
        _requiresClearChildViewState = true;
      }

      Controls.Add (control);
    }
  }
}