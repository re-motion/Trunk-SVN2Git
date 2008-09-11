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
    //private bool _isControlStateLoaded;
    //private bool _isViewStateLoaded;
    private bool _requiresClearChildControlState;
    private bool _requiresClearChildViewState;

    public ControlReplacer (IInternalControlMemberCaller memberCaller, string id)
    {
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      _memberCaller = memberCaller;
      ID = id;
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

      //_isControlStateLoaded = true;

      if (_requiresClearChildControlState)
      {
        Assertion.IsNull (ControlStateBackup);
        _requiresClearChildControlState = false;
        ClearChildControlState();
      }
      
      if (ControlStateBackup != null)
      {
        Assertion.IsFalse (_requiresClearChildControlState);
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

      //_isViewStateLoaded = true;

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


    public string SaveAllState ()
    {
      Pair state = new Pair (ControlHelper.SaveChildControlState (this), ControlHelper.SaveViewStateRecursive (this));
      LosFormatter formatter = new LosFormatter();
      StringWriter writer = new StringWriter();
      formatter.Serialize (writer, state);
      return writer.ToString();
    }

    public void ReplaceAndWrap<T> (T controlToReplace, T controlToWrap, bool clearChildState, string savedState)
      where T: Control, INamingContainer, IReplaceableControl
    {
      ArgumentUtility.CheckNotNull ("controlToReplace", controlToReplace);
      ArgumentUtility.CheckNotNull ("controlToWrap", controlToWrap);

      if (_memberCaller.GetControlState (controlToReplace) != ControlState.ChildrenInitialized)
        throw new InvalidOperationException ("Controls can only be wrapped during OnInit phase.");

      if (controlToReplace.IsInitialized)
        throw new InvalidOperationException ("Controls can only be wrapped before they are initialized.");

      controlToWrap.Replacer = this;

      Control parent = controlToReplace.Parent;
      int index = parent.Controls.IndexOf (controlToReplace);

      //Mark parent collection as modifiable
      string errorMessage = ControlHelper.SetCollectionReadOnly (parent.Controls, null);

      parent.Controls.RemoveAt (index);
      parent.Controls.AddAt (index, this);

      //Mark parent collection as readonly
      ControlHelper.SetCollectionReadOnly (parent.Controls, errorMessage);
      _memberCaller.InitRecursive (this, parent);

      if (savedState != null)
      {
        var formatter = new LosFormatter ();
        var state = (Pair) formatter.Deserialize (savedState);

        HasChildState = true;
        ControlStateBackup = (IDictionary) state.First;
        ViewStateBackup = state.Second;
      }

      if (clearChildState)
      {
        if (ViewStateBackup != null || ControlStateBackup != null)
        {
          throw new InvalidOperationException ("Cannot clear child state if a state has been injected.");
        }
        _requiresClearChildControlState = true;
        _requiresClearChildViewState = true;
      }

      Controls.Add (controlToWrap);
    }

    //protected override void AddedControl (Control control, int index)
    //{
    //  if (HasChildState)
    //  {
    //    if (_isControlStateLoaded)
    //    {
    //      if (_requiresClearChildControlState)
    //      {
    //        _requiresClearChildControlState = false;
    //        ClearChildControlState ();
    //      }
    //      IDictionary controlStateBackup = ControlStateBackup;
    //      ControlStateBackup = null;
    //      ControlHelper.SetChildControlState (this, controlStateBackup);
    //    }

    //    if (_isViewStateLoaded)
    //    {
    //      if (_requiresClearChildViewState)
    //      {
    //        _requiresClearChildViewState = false;
    //        ClearChildViewState ();
    //      }
    //      object viewStateBackup = ViewStateBackup;
    //      ViewStateBackup = null;
    //      ControlHelper.LoadViewStateRecursive (this, viewStateBackup);
    //    }

    //    HasChildState = false;

    //    if (_isViewStateLoaded)
    //    {
    //      bool enableViewStateBackup = control.EnableViewState;
    //      control.EnableViewState = false;
    //      Controls[0].Load += delegate { Controls[0].EnableViewState = enableViewStateBackup; };
    //    }

    //    base.AddedControl (control, index);
    //  }
    //  else
    //  {
    //    if (_isViewStateLoaded && _requiresClearChildViewState)
    //    {
    //      bool enableViewStateBackup = control.EnableViewState;
    //      control.EnableViewState = false;
    //      Controls[0].Load += delegate { Controls[0].EnableViewState = enableViewStateBackup; };
    //    }

    //    base.AddedControl (control, index);
    //  }
    //}
  }
}