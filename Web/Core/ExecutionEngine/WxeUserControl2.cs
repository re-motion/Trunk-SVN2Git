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
using System.Web.UI.WebControls;
using Remotion.Collections;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeUserControl2 : UserControl, IWxeTemplateControl
  {
    private readonly WxeTemplateControlInfo _wxeInfo;
    private bool _isEnsured;
    private PlaceHolder _placeHolder;

    public WxeUserControl2 ()
    {
      _wxeInfo = new WxeTemplateControlInfo (this);
    }

    protected override void OnInit (EventArgs e)
    {
      _wxeInfo.Initialize (Context);
      base.OnInit (e);
      Page.InitComplete += Page_InitComplete;
    }

    private void Page_InitComplete (object sender, EventArgs e)
    {
      Control parent = Parent;
      int index = parent.Controls.IndexOf (this);

      var lazyContainer = new LazyContainer();
      lazyContainer.ID = ID + "_LazyContainer";
      
      Page.InitComplete -= Page_InitComplete;
      parent.Controls.RemoveAt (index);
      parent.Controls.AddAt (index, lazyContainer);

      string uniqueID = lazyContainer.UniqueID + IdSeparator + ID;
      if ((string) CurrentFunction.Variables["UserControlID"] == uniqueID)
      {
        var control = (WxeUserControl2) lazyContainer.Page.LoadControl ((string) CurrentFunction.Variables["UserControlPath"]);
        control.ID = ID;
        CompleteInitialization(lazyContainer, control);
      }
      else
      {
        CompleteInitialization (lazyContainer, this);
      }
    }

    private void CompleteInitialization (LazyContainer lazyContainer, WxeUserControl2 control)
    {
      lazyContainer.RealControls.Add (control);
      lazyContainer.Page.RegisterRequiresControlState (control);
      lazyContainer.IsLazyLoadingEnabled = false;
      lazyContainer.Ensure();
      control.Ensure ();
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

    public void JumpToUserControl (WxeFunction function, string userControlPath)
    {
      function.Variables["UserControlID"] = UniqueID;
      function.Variables["UserControlPath"] = userControlPath;
      WxePage.ExecuteFunction (function, WxeCallArguments.Default);
    }

    public WxePageStep CurrentStep
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