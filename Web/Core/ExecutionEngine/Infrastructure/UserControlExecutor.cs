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
    private readonly WxeFunction _subFunction;
    private readonly string _userControlState;
    private readonly string _userControlID;
    private bool _isReturningInnerFunction;
    private NameValueCollection _postBackCollection;
    private NameValueCollection _backupPostBackData;

    public UserControlExecutor (WxeStep parentStep, WxeUserControl2 userControl, WxeFunction subFunction, Control sender, bool usesEventTarget)
    {
      ArgumentUtility.CheckNotNull ("parentStep", parentStep);
      ArgumentUtility.CheckNotNull ("userControl", userControl);
      ArgumentUtility.CheckNotNull ("subFunction", subFunction);
      ArgumentUtility.CheckNotNull ("sender", sender);
      
      _userControlState = userControl.SaveAllState ();
      _userControlID = userControl.UniqueID;
      _subFunction = subFunction;

      _subFunction.SetParentStep (parentStep);

      _postBackCollection = userControl.WxePage.GetPostBackCollection ().Clone ();
      _backupPostBackData = new NameValueCollection();

      if (usesEventTarget)
      {
        _backupPostBackData.Add (ControlHelper.PostEventSourceID, _postBackCollection[ControlHelper.PostEventSourceID]);
        _backupPostBackData.Add (ControlHelper.PostEventArgumentID, _postBackCollection[ControlHelper.PostEventArgumentID]);
        _postBackCollection.Remove (ControlHelper.PostEventSourceID);
        _postBackCollection.Remove (ControlHelper.PostEventArgumentID);
      }
      else
      {
        _backupPostBackData.Add (sender.UniqueID, _postBackCollection[sender.UniqueID]);
        _postBackCollection.Remove (sender.UniqueID);
      }
    }

    public void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      if (!_subFunction.IsExecutionStarted)
      {
        context.PostBackCollection = _postBackCollection;
        _postBackCollection = null;
      }

      bool isPostBackBackUp = context.IsPostBack;
      try
      {
        _subFunction.Execute (context);
      }
      catch (WxeExecuteUserControlStepException)
      {
        context.SetIsPostBack (isPostBackBackUp);
      }
    }

    public void Return (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      NameValueCollection collection;
      if (StringUtility.AreEqual (context.HttpContext.Request.HttpMethod, "POST", false))
        collection = context.HttpContext.Request.Form;
      else
        collection = context.HttpContext.Request.QueryString;

      collection = collection.Clone();
      foreach (var key in _backupPostBackData.AllKeys)
        collection[key] = _backupPostBackData[key];
      context.PostBackCollection = collection;
      _backupPostBackData = null;

      context.SetIsReturningPostBack (true);
      _isReturningInnerFunction = true;
    }

    public WxeFunction SubFunction
    {
      get { return _subFunction; }
    }

    public string UserControlState
    {
      get { return _userControlState; }
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