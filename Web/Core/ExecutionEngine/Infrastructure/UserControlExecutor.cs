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
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  [Serializable]
  public class UserControlExecutor : IUserControlExecutor
  {
    private readonly WxeFunction _function;
    private readonly string _backedUpUserControlState;
    private readonly string _backedUpUserControl;
    private readonly string _userControlID;
    private bool _isReturningInnerFunction;
    private NameValueCollection _postBackCollection;
    private NameValueCollection _backedUpPostBackData;

    public UserControlExecutor (WxeStep parentStep, WxeUserControl2 userControl, WxeFunction subFunction, Control sender, bool usesEventTarget)
    {
      ArgumentUtility.CheckNotNull ("parentStep", parentStep);
      ArgumentUtility.CheckNotNull ("userControl", userControl);
      ArgumentUtility.CheckNotNull ("subFunction", subFunction);
      ArgumentUtility.CheckNotNull ("sender", sender);
      
      _backedUpUserControlState = userControl.SaveAllState ();
      _backedUpUserControl = userControl.AppRelativeVirtualPath;
      _userControlID = userControl.UniqueID;
      _function = subFunction;

      _function.SetParentStep (parentStep);

      _postBackCollection = userControl.WxePage.GetPostBackCollection ().Clone ();
      _backedUpPostBackData = new NameValueCollection();

      if (usesEventTarget)
      {
        _backedUpPostBackData.Add (ControlHelper.PostEventSourceID, _postBackCollection[ControlHelper.PostEventSourceID]);
        _backedUpPostBackData.Add (ControlHelper.PostEventArgumentID, _postBackCollection[ControlHelper.PostEventArgumentID]);
        _postBackCollection.Remove (ControlHelper.PostEventSourceID);
        _postBackCollection.Remove (ControlHelper.PostEventArgumentID);
      }
      else
      {
        _backedUpPostBackData.Add (sender.UniqueID, _postBackCollection[sender.UniqueID]);
        _postBackCollection.Remove (sender.UniqueID);
      }
    }

    public void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      if (!_function.IsExecutionStarted)
      {
        context.PostBackCollection = _postBackCollection;
        _postBackCollection = null;
      }

      bool isPostBackBackUp = context.IsPostBack;
      try
      {
        _function.Execute (context);
      }
      catch (WxeExecuteUserControlStepException)
      {
        context.SetIsPostBack (isPostBackBackUp);
      }
    }

    public void BeginReturn (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      NameValueCollection collection;
      if (StringUtility.AreEqual (context.HttpContext.Request.HttpMethod, "POST", false))
        collection = context.HttpContext.Request.Form;
      else
        collection = context.HttpContext.Request.QueryString;

      collection = collection.Clone();
      foreach (var key in _backedUpPostBackData.AllKeys)
        collection[key] = _backedUpPostBackData[key];
      context.PostBackCollection = collection;
      _backedUpPostBackData = null;

      context.SetIsReturningPostBack (true);
      _isReturningInnerFunction = true;
    }

    public void EndReturn (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      if (_function.ParentStep is WxePageStep)
        ((WxePageStep) _function.ParentStep).SetUserControlExecutor (NullUserControlExecutor.Null);
      else if (_function.ParentStep is WxeUserControlStep)
        ((WxeUserControlStep) _function.ParentStep).SetUserControlExecutor (NullUserControlExecutor.Null);
    }

    public WxeFunction Function
    {
      get { return _function; }
    }

    public string BackedUpUserControlState
    {
      get { return _backedUpUserControlState; }
    }

    public string BackedUpUserControl
    {
      get { return _backedUpUserControl; }
    }

    public string UserControlID
    {
      get { return _userControlID; }
    }

    public bool IsReturningInnerFunction
    {
      get { return _isReturningInnerFunction; }
    }

    public bool IsNull
    {
      get { return false; }
    }
  }
}