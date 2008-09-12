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
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Remotion.Web.Utilities;
using Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates;

namespace Remotion.Web.UI.Controls.ControlReplacing
{
  //TODO: Refactor to use SingleChildControlCollection
  public sealed class ControlReplacer : Control, INamingContainer
  {
    private readonly IInternalControlMemberCaller _memberCaller;
    //private bool _isControlStateLoaded;
    private bool _requiresClearChildControlState;

    public ControlReplacer (IInternalControlMemberCaller memberCaller)
    {
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      _memberCaller = memberCaller;
    }

    public IDictionary ControlStateBackup { get; private set; }

    public bool HasChildState { get; private set; }

    public IViewStateModificationState ViewStateModificationState { get; set; }
  
    public IControlStateModificationState ControlStateModificationState { get; set; }

    public Control WrappedControl
    {
      get { return Controls[0]; }
    }

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
        _memberCaller.SetChildControlState (this, controlStateBackup);
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

      ViewStateModificationState.LoadViewState (savedState);
    }

    protected override object SaveViewState ()
    {
      return "value";
    }

    public string SaveAllState ()
    {
      Pair state = new Pair (_memberCaller.SaveChildControlState (this), _memberCaller.SaveViewStateRecursive (this));
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
      string errorMessage = _memberCaller.SetCollectionReadOnly (parent.Controls, null);

      parent.Controls.RemoveAt (index);
      parent.Controls.AddAt (index, this);

      //Mark parent collection as readonly
      _memberCaller.SetCollectionReadOnly (parent.Controls, errorMessage);
      _memberCaller.InitRecursive (this, parent);

      ViewStateModificationState = new ViewStateLoadingState (this);

      if (savedState != null)
      {
        var formatter = new LosFormatter();
        var state = (Pair) formatter.Deserialize (savedState);

        HasChildState = true;
        ControlStateBackup = (IDictionary) state.First;
        ViewStateModificationState = new ViewStateReplacingState (this, _memberCaller, state.Second);
      }

      if (clearChildState)
      {
        if (ControlStateBackup != null)
          throw new InvalidOperationException ("Cannot clear child state if a state has been injected.");
        if (ViewStateModificationState is ViewStateReplacingState)
          throw new InvalidOperationException ("Cannot clear child state if a state has been injected.");
        _requiresClearChildControlState = true;
        ViewStateModificationState = new ViewStateClearingState (this);
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

    //    HasChildState = false;

    //    base.AddedControl (control, index);
    //  }
    //  else
    //  {
    //    base.AddedControl (control, index);
    //  }
    //}
  }
}