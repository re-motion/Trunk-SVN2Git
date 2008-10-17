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
using System.IO;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates;
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing
{
  //TODO: Refactor to use SingleChildControlCollection
  public sealed class ControlReplacer : Control, INamingContainer, IPostBackDataHandler
  {
    private readonly IInternalControlMemberCaller _memberCaller;
    private Control _controlToWrap;
    private IViewStateModificationState _viewStateModificationState = new ViewStateNullModificationState();
    private IControlStateModificationState _controlStateModificationState = new ControlStateNullModificationState();

    public ControlReplacer (IInternalControlMemberCaller memberCaller)
    {
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      _memberCaller = memberCaller;
    }

    public IViewStateModificationState ViewStateModificationState
    {
      get { return _viewStateModificationState; }
      set { _viewStateModificationState = ArgumentUtility.CheckNotNull ("value", value); }
    }

    public IControlStateModificationState ControlStateModificationState
    {
      get { return _controlStateModificationState; }
      set { _controlStateModificationState = ArgumentUtility.CheckNotNull ("value", value); }
    }

    public Control WrappedControl
    {
      get { return Controls.Count == 1 ? Controls[0] : null; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      Page.RegisterRequiresControlState (this);
      Page.RegisterRequiresPostBack (this);
    }

    protected override void LoadControlState (object savedState)
    {
      //ControlStateModificationState.LoadControlState (savedState);
    }

    protected override object SaveControlState ()
    {
      return "value";
    }

    protected override void LoadViewState (object savedState)
    {
      if (_memberCaller.GetControlState (this) < ControlState.Initialized)
        throw new InvalidOperationException ("Controls can only load state after OnInit phase.");

      //ViewStateModificationState.LoadViewState (savedState);
    }

    protected override object SaveViewState ()
    {
      return "value";
    }

    public bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      Assertion.IsNotNull (_controlToWrap);
      Control controlToWrap = _controlToWrap;
      _controlToWrap = null;

      if (ViewStateModificationState is ViewStateLoadingState)
      {
        //NOP
      }
      else if (ViewStateModificationState is ViewStateReplacingState)
      {
        _memberCaller.LoadViewStateRecursive (this, ((ViewStateReplacingState)ViewStateModificationState).ViewState);
      }
      else if (ViewStateModificationState is ViewStateClearingStateBase)
      {
        bool enableViewStateBackup = controlToWrap.EnableViewState;
        controlToWrap.EnableViewState = false;
        controlToWrap.Load += delegate { controlToWrap.EnableViewState = enableViewStateBackup; };
      }

      if (ControlStateModificationState is ControlStateLoadingState)
      {
        //NOP
      }
      else if (ControlStateModificationState is ControlStateReplacingState)
      {
        _memberCaller.SetChildControlState (this, ((ControlStateReplacingState)ControlStateModificationState).ControlState);
      }
      else if (ControlStateModificationState is ControlStateClearingStateBase)
      {
        _memberCaller.ClearChildControlState (this);
      }

      _memberCaller.SetControlState (controlToWrap, ControlState.Constructed);
      Controls.Add (controlToWrap);

      return false;
    }

    public void RaisePostDataChangedEvent ()
    {
      throw new NotSupportedException ();
    }

    //protected override void AddedControl (Control control, int index)
    //{
    //  Assertion.IsTrue (object.ReferenceEquals (WrappedControl, control), "The added control is not the same as the wrapped control.");

    //  Action<Control, int> baseCallControl = (controlArg, indexArg) => BaseAddedControl (controlArg, indexArg);
    //  Action<Control, int> baseCallControlState = (controlArg, indexArg) => ControlStateModificationState.AddedControl (controlArg, indexArg, baseCallControl);
    //  ViewStateModificationState.AddedControl (control, index, baseCallControlState);
    //}

    //private void BaseAddedControl (Control control, int index)
    //{
    //  base.AddedControl (control, index);
    //}

    public string SaveAllState ()
    {
      Pair state = new Pair (_memberCaller.SaveChildControlState (this), _memberCaller.SaveViewStateRecursive (this));
      LosFormatter formatter = new LosFormatter();
      StringWriter writer = new StringWriter();
      formatter.Serialize (writer, state);
      return writer.ToString();
    }

    public void ReplaceAndWrap<T> (T controlToReplace, T controlToWrap, IModificationStateSelectionStrategy modificationStateSelectionStrategy)
        where T: Control, IReplaceableControl
    {
      ArgumentUtility.CheckNotNull ("controlToReplace", controlToReplace);
      ArgumentUtility.CheckNotNull ("controlToWrap", controlToWrap);
      ArgumentUtility.CheckNotNull ("modificationStateSelectionStrategy", modificationStateSelectionStrategy);

      if (_memberCaller.GetControlState (controlToReplace) != ControlState.ChildrenInitialized)
        throw new InvalidOperationException ("Controls can only be wrapped during OnInit phase.");

      if (controlToReplace.IsInitialized)
        throw new InvalidOperationException ("Controls can only be wrapped before they are initialized.");

      controlToWrap.Replacer = this;

      ViewStateModificationState = modificationStateSelectionStrategy.CreateViewStateModificationState (this, _memberCaller);
      ControlStateModificationState = modificationStateSelectionStrategy.CreateControlStateModificationState (this, _memberCaller);

      Control parent = controlToReplace.Parent;
      int index = parent.Controls.IndexOf (controlToReplace);

      //Mark parent collection as modifiable
      string errorMessage = _memberCaller.SetCollectionReadOnly (parent.Controls, null);

      parent.Controls.RemoveAt (index);
      parent.Controls.AddAt (index, this);

      //Mark parent collection as readonly
      _memberCaller.SetCollectionReadOnly (parent.Controls, errorMessage);

      _memberCaller.InitRecursive (this, parent);
     
      if (!parent.Page.IsPostBack)
        Controls.Add (controlToWrap);
      else
        _controlToWrap = controlToWrap;
    }
  }
}