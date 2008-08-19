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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  [PersistChildren (true)]
  [ParseChildren (true, "RealControls")]
  public class LazyContainer : Control, INamingContainer
  {
    // types

    // static members and constants

    // member fields

    private bool _isEnsured;
    private EmptyControlCollection _emptyControlCollection;
    private PlaceHolder _placeHolder;
    private Dictionary<string, object> _childControlStatesBackUp;
    private bool _hasControlStateLoaded;
    private object _recursiveViewState;
    private bool _isSavingViewStateRecursive;
    private bool _isLoadingViewStateRecursive;
    private bool _isLazyLoadingEnabled = true;

    // construction and disposing

    public LazyContainer ()
    {
    }

    // methods and properties

    public void Ensure ()
    {
      if (_isEnsured)
        return;

      _isEnsured = true;

      if (_isLazyLoadingEnabled)
      {        
        if (!_hasControlStateLoaded && Page != null && Page.IsPostBack)
          throw new InvalidOperationException (string.Format ("Cannot ensure LazyContainer '{0}' before its state has been loaded.", ID));

        RestoreChildControlState ();
      }

      EnsurePlaceHolderCreated ();
      Controls.Add (_placeHolder);
    }

    public bool IsLazyLoadingEnabled
    {
      get { return _isLazyLoadingEnabled; }
      set { _isLazyLoadingEnabled = value; }
    }

    public override ControlCollection Controls
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
          if (_emptyControlCollection == null)
            _emptyControlCollection = new EmptyControlCollection (this);
          return _emptyControlCollection;
        }
      }
    }

    [Browsable (false)]
    public ControlCollection RealControls
    {
      get
      {
        EnsureChildControls ();

        EnsurePlaceHolderCreated ();
        return _placeHolder.Controls;
      }
    }

    private void EnsurePlaceHolderCreated ()
    {
      if (_placeHolder == null)
        _placeHolder = new PlaceHolder ();
    }

    protected override void CreateChildControls ()
    {
      if (! _isLazyLoadingEnabled || ControlHelper.IsDesignMode (this))
        Ensure ();
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      EnsureChildControls ();

      if (!ControlHelper.IsDesignMode (this))
      {
        Page.RegisterRequiresControlState (this);
      }
    }

    protected override void LoadViewState (object savedState)
    {
      if (_isLoadingViewStateRecursive)
        return;

      if (savedState != null)
      {
        Pair values = (Pair) savedState;
        base.LoadViewState (values.First);
        _recursiveViewState = values.Second;

        if (_isLazyLoadingEnabled)
        {
          _isLoadingViewStateRecursive = true;
          ControlHelper.LoadViewStateRecursive (this, _recursiveViewState);
          _isLoadingViewStateRecursive = false;
        }
      }
    }

    protected override object SaveViewState ()
    {
      if (_isSavingViewStateRecursive)
        return null;

      if (_isLazyLoadingEnabled && _isEnsured)
      {
        _isSavingViewStateRecursive = true;
        _recursiveViewState = ControlHelper.SaveViewStateRecursive (this);
        _isSavingViewStateRecursive = false;
      }

      Pair values = new Pair ();
      values.First = base.SaveViewState ();
      values.Second = _recursiveViewState;

      return values;
    }

    protected override void LoadControlState (object savedState)
    {
      Triplet values = ArgumentUtility.CheckNotNullAndType<Triplet> ("savedState", savedState);

      base.LoadControlState (savedState);
      bool hasChildControlStatesBackUp = (bool) values.Second;

      if (hasChildControlStatesBackUp)
        _childControlStatesBackUp = (Dictionary<string, object>) values.Third;
      else if (_isLazyLoadingEnabled)
        BackUpChildControlState ();

      _hasControlStateLoaded = true;
    }

    private void RestoreChildControlState ()
    {
      ControlHelper.SetChildControlState(this, _childControlStatesBackUp);

      _childControlStatesBackUp = null;
    }

    private void BackUpChildControlState ()
    {
      _childControlStatesBackUp = ControlHelper.GetChildControlState (this);
    }

    protected override object SaveControlState ()
    {
      bool hasChildControlStatesBackUp = _isLazyLoadingEnabled && !_isEnsured;

      Triplet values = new Triplet ();
      values.First = base.SaveControlState ();
      values.Second = hasChildControlStatesBackUp;
      if (hasChildControlStatesBackUp)
        values.Third = _childControlStatesBackUp;
      else
        values.Third = null;

      return values;
    }
  }

}
